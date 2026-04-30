import os
import uuid
from pathlib import Path

from fastapi import FastAPI, HTTPException
from minio import Minio
from minio.error import S3Error

from models import ConversionJob
from converter import convert


app = FastAPI()

ENDPOINT = "minio-storage:9000"
ACCESS_KEY = "admin"
ACCESS_PASSWORD = "Password123!"

minio_client = Minio(
    ENDPOINT, 
    access_key=ACCESS_KEY, 
    secret_key=ACCESS_PASSWORD, 
    secure=False
)

BUCKET_NAME = "user-files"


def _extension_from_key(storage_key: str) -> str:
    return Path(storage_key).suffix.lstrip(".").lower()


def _ensure_bucket_online() -> None:
    try:
        if not minio_client.bucket_exists(bucket_name=BUCKET_NAME):
            minio_client.make_bucket(bucket_name=BUCKET_NAME)
    except S3Error as err:
        print(err)
        raise err


@app.on_event("startup")
async def startup() -> None:
    _ensure_bucket_online()

@app.post("/convert", status_code=200)
async def convert_file(job: ConversionJob):
    conversion_id = uuid.uuid4()
    source_ext = _extension_from_key(job.input_file_storage_key)
    target_ext = job.target_format.lstrip(".").lower()

    tmp_input = f"/tmp/{conversion_id}_input.{source_ext}"
    tmp_output = f"/tmp/{conversion_id}_output.{target_ext}"

    try:
        minio_client.fget_object(BUCKET_NAME, job.input_file_storage_key, tmp_input)

        convert(tmp_input, tmp_output)

        minio_client.fput_object(
            BUCKET_NAME, 
            job.output_file_storage_key, 
            tmp_output)
        print(f"[CONVERSION SUCCESSFUL] {source_ext} -> {target_ext}")

    except (ValueError, RuntimeError) as err:
        print(f"[CONVERSION ERROR] {job.input_file_storage_key} => {err}")
        raise HTTPException(status_code=422, detail=str(err))

    except S3Error as err:
        print(f"[MinIO ERROR] {err}")
        raise HTTPException(status_code=500, detail=str(err))

    except Exception as err:
        print(f"[UNEXPECTED ERROR] {err}")
        raise HTTPException(status_code=500, detail=str(err))

    finally:
        for path in (tmp_input, tmp_output):
            if os.path.exists(path):
                os.remove(path)
    return;
import os
import uuid
from pathlib import Path

from fastapi import FastAPI, HTTPException
from minio import Minio
from minio.error import S3Error

from conversion_model import ConversionModel
from converter import convert


app = FastAPI(title="Inner Converter API")

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


@app.post("/convert")
async def convert_file(task: ConversionModel):
    conversion_id = uuid.uuid4()
    source_ext = _extension_from_key(task.input_file_key)
    target_ext = task.target_format.lstrip(".").lower()

    tmp_input = f"/tmp/{conversion_id}_input.{source_ext}"
    tmp_output = f"/tmp/{conversion_id}_output.{target_ext}"

    try:
        minio_client.fget_object(
            BUCKET_NAME, 
            task.input_file_key, 
            tmp_input)

        convert(tmp_input, tmp_output)

        minio_client.fput_object(
            BUCKET_NAME, 
            task.output_file_key, 
            tmp_output)

    except (ValueError, RuntimeError) as err:
        print(f"[CONVERSION ERROR] {task.input_file_key} => {err}")
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

    return {
        "status": "success", 
        "output_key": task.output_file_key
    }
import os
import uuid
from pathlib import Path

from fastapi import FastAPI, HTTPException
from minio import Minio
from minio.error import S3Error

from converter import convert
from models import ConversionJob, ConversionResult


app = FastAPI()

ENDPOINT = os.getenv(
    "MINIO_ENDPOINT",
    "minio-storage:9000",
)

ACCESS_KEY = os.getenv(
    "MINIO_ACCESS_KEY",
    "admin",
)

ACCESS_PASSWORD = os.getenv(
    "MINIO_SECRET_KEY",
    "Password123!",
)

BUCKET_NAME = os.getenv(
    "MINIO_BUCKET",
    "user-files",
)


CONTENT_TYPES = {
    "jpg": "image/jpeg",
    "jpeg": "image/jpeg",
    "png": "image/png",
    "webp": "image/webp",
    "ppm": "image/x-portable-pixmap",
    "bmp": "image/bmp",
}

minio_client = Minio(
    ENDPOINT,
    access_key=ACCESS_KEY,
    secret_key=ACCESS_PASSWORD,
    secure=False,
)

def extension_from_key(storage_key: str) -> str:
    extension = (
        Path(storage_key)
        .suffix
        .lstrip(".")
        .lower()
    )

    if not extension:
        raise ValueError(
            "Storage key has no extension"
        )

    return extension


def ensure_bucket_online() -> None:
    if not minio_client.bucket_exists(
        bucket_name=BUCKET_NAME
    ):
        minio_client.make_bucket(
            bucket_name=BUCKET_NAME
        )


@app.on_event("startup")
async def startup() -> None:
    ensure_bucket_online()


@app.post(
    "/convert",
    response_model=ConversionResult,
)
def convert_file(
    job: ConversionJob,
) -> ConversionResult:
    temporary_id = uuid.uuid4()

    source_extension = extension_from_key(
        job.input_file_storage_key
    )

    target_extension = (
        job.target_extension
        .strip()
        .lstrip(".")
        .lower()
    )

    output_key_extension = extension_from_key(
        job.output_file_storage_key
    )

    if output_key_extension != target_extension:
        raise HTTPException(
            status_code=422,
            detail=(
                "Output storage-key extension does "
                "not match target extension"
            ),
        )

    input_path = (
        f"/tmp/{temporary_id}_input."
        f"{source_extension}"
    )

    output_path = (
        f"/tmp/{temporary_id}_output."
        f"{target_extension}"
    )

    try:
        minio_client.fget_object(
            BUCKET_NAME,
            job.input_file_storage_key,
            input_path,
        )

        convert(
            input_path,
            output_path,
        )

        content_type = CONTENT_TYPES.get(
            target_extension,
            "application/octet-stream",
        )

        size_bytes = os.path.getsize(
            output_path
        )

        minio_client.fput_object(
            BUCKET_NAME,
            job.output_file_storage_key,
            output_path,
            content_type=content_type,
        )

        return ConversionResult(
            outputFileStorageKey=(
                job.output_file_storage_key
            ),
            extension=target_extension,
            contentType=content_type,
            sizeBytes=size_bytes,
        )

    except (ValueError, RuntimeError) as error:
        raise HTTPException(
            status_code=422,
            detail=str(error),
        ) from error

    except S3Error as error:
        raise HTTPException(
            status_code=500,
            detail=str(error),
        ) from error

    except Exception as error:
        raise HTTPException(
            status_code=500,
            detail=str(error),
        ) from error

    finally:
        for path in (input_path, output_path):
            if os.path.exists(path):
                os.remove(path)
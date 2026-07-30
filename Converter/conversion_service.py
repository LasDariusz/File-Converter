import os
import tempfile
from pathlib import Path

from minio import Minio

from converter import convert
from models import ConversionJob, ConversionResult


ENDPOINT = os.getenv("MINIO_ENDPOINT", "storage:9000")
ACCESS_KEY = os.getenv("MINIO_ACCESS_KEY", "fileconverter")
SECRET_KEY = os.getenv("MINIO_SECRET_KEY", "DevPassword123!")
BUCKET_NAME = os.getenv("MINIO_BUCKET", "user-files")

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
    secret_key=SECRET_KEY,
    secure=False,
)


def extension_from_key(storage_key: str) -> str:
    extension = Path(storage_key).suffix.lstrip(".").lower()

    if not extension:
        raise ValueError("Storage key has no extension")

    return extension


def ensure_bucket_online() -> None:
    if not minio_client.bucket_exists(BUCKET_NAME):
        minio_client.make_bucket(BUCKET_NAME)


def execute_conversion(job: ConversionJob) -> ConversionResult:
    source_extension = extension_from_key(
        job.input_file_storage_key
    )
    target_extension = (
        job.target_extension.strip().lstrip(".").lower()
    )

    output_extension = extension_from_key(
        job.output_file_storage_key
    )

    if output_extension != target_extension:
        raise ValueError(
            "Output storage-key extension does not match target extension"
        )

    with tempfile.TemporaryDirectory() as directory:
        input_path = os.path.join(
            directory,
            f"input.{source_extension}",
        )
        output_path = os.path.join(
            directory,
            f"output.{target_extension}",
        )

        minio_client.fget_object(
            BUCKET_NAME,
            job.input_file_storage_key,
            input_path,
        )

        convert(input_path, output_path)

        content_type = CONTENT_TYPES.get(
            target_extension,
            "application/octet-stream",
        )
        size_bytes = os.path.getsize(output_path)

        minio_client.fput_object(
            BUCKET_NAME,
            job.output_file_storage_key,
            output_path,
            content_type=content_type,
        )

        return ConversionResult(
            outputStorageKey=job.output_file_storage_key,
            extension=target_extension,
            contentType=content_type,
            sizeBytes=size_bytes,
        )

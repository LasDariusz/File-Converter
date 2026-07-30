from fastapi import FastAPI, HTTPException
from minio.error import S3Error

from conversion_service import ensure_bucket_online, execute_conversion
from models import ConversionJob, ConversionResult


app = FastAPI()


@app.on_event("startup")
async def startup() -> None:
    ensure_bucket_online()


@app.post("/convert", response_model=ConversionResult)
def convert_file(job: ConversionJob) -> ConversionResult:
    try:
        return execute_conversion(job)
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

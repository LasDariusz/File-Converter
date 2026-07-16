from datetime import datetime, timezone
from uuid import UUID, uuid4

from pydantic import BaseModel, ConfigDict, Field


def utc_now() -> datetime:
    return datetime.now(timezone.utc)


class ConversionRequested:
    model_config = ConfigDict 

class ConversionJob(BaseModel):

    input_file_storage_key: str = Field(alias="originalFileStorageKey")

    output_file_storage_key: str = Field(alias="outputFileStorageKey")

    target_extension: str = Field(alias="targetExtension")
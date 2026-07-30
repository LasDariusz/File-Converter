from datetime import datetime
from uuid import UUID

from pydantic import BaseModel, Field


class ConversionJob(BaseModel):
    input_file_storage_key: str = Field(
        alias="originalFileStorageKey"
    )
    output_file_storage_key: str = Field(
        alias="outputFileStorageKey"
    )
    target_extension: str = Field(
        alias="targetExtension"
    )

    model_config = {
        "populate_by_name": True
    }


class ConversionRequestedMessage(ConversionJob):
    message_id: UUID = Field(alias="messageId")
    conversion_id: UUID = Field(alias="conversionId")
    requested_at_utc: datetime = Field(alias="requestedAtUtc")


class ConversionResult(BaseModel):
    output_file_storage_key: str = Field(
        alias="outputStorageKey"
    )
    extension: str
    content_type: str = Field(alias="contentType")
    size_bytes: int = Field(alias="sizeBytes")

    model_config = {
        "populate_by_name": True
    }

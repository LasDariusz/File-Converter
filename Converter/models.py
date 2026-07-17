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


class ConversionResult(BaseModel):
    output_file_storage_key: str = Field(
        alias="outputFileStorageKey"
    )

    extension: str
    content_type: str = Field(
        alias="contentType"
    )

    size_bytes: int = Field(
        alias="sizeBytes"
    )

    model_config = {
        "populate_by_name": True
    }
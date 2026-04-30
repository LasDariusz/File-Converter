from pydantic import BaseModel, Field, ConfigDict


class ConversionJob(BaseModel):

    input_file_storage_key: str = Field(alias="originalFileStorageKey")

    output_file_storage_key: str = Field(alias="outputFileStorageKey")

    target_extension: str = Field(alias="targetExtension")
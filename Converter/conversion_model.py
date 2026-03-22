from pydantic import BaseModel, Field, ConfigDict


class ConversionModel(BaseModel):
    model_config = ConfigDict(populate_by_name=True)

    input_file_key: str = Field(alias="inputFileStorageKey")
    output_file_key: str = Field(alias="outputFileStorageKey")
    target_format: str = Field(alias="targetFormat")
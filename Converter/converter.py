import shutil
from pathlib import Path

from PIL import Image, ImageOps


IMAGE_FORMATS = {
    "jpeg", 
    "jpg", 
    "png", 
    "webp", 
    "ppm", 
    "bmp"
}

PILLOW_FORMAT_MAP: dict[str, str] = {
    "jpg": "JPEG",
    "jpeg": "JPEG",
    "png": "PNG",
    "webp": "WEBP",
    "ppm": "PPM",
    "bmp": "BMP",
}


def extension_from_path(path: str) -> str:
    return (
        Path(path)
        .suffix
        .lstrip(".")
        .lower()
    )

def convert_image(
    source_path: str,
    target_path: str,
    target_extension: str
) -> None:

    pillow_format = PILLOW_FORMAT_MAP.get(target_extension)

    if pillow_format is None:
        raise ValueError(
            f"Unsupported target format [{target_extension}]")

    with Image.open(source_path) as source_image:
        image = ImageOps.exif_transponse(source_image)

        if pillow_format == "JPEG" and source_image.mode in ("RGBA", "LA", "P"):
            rgba_image = image.convert("RGBA")

            background = Image.new(
                mode="RGB",
                size=rgba_image.size,
                color=(255, 255, 255)
            )

            background.paste(
                rgba_image, 
                mask=rgba_image.getchannel("A")
            )

            background.save(
                target_path, 
                format=pillow_format, 
                quality=92
            )

            return

        source_image.save(
            target_path, 
            format=pillow_format)

def convert(
    source_path: str,
   target_path: str
) -> None:

    source_extension = extension_from_path(source_path)
    target_extension = extension_from_path(target_path)

    if source_extension == target_extension:
        shutil.copyfile(source_path, target_path)
        return

    if source_extension not in IMAGE_FORMATS:
        raise ValueError(f"Unsupported source format {source_extension}")

    if target_extension not in IMAGE_FORMATS:
        raise ValueError(f"Unsupported target format {target_extension}")

    convert_image(source_path, target_path, target_extension)
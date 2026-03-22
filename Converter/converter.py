import os
import io
from pathlib import Path
from PIL import Image


IMAGE_FORMATS = {"jpeg", "jpg", "png", "webp", "ppm", "bmp"}

PILLOW_FORMAT_MAP: dict[str, str] = {
    "jpg": "JPEG",
    "jpeg": "JPEG",
    "png": "PNG",
    "webp": "WEBP",
    "ppm": "PPM",
    "bmp": "BMP",
}


def _extension(path: str) -> str:
    return Path(path).suffix.lstrip(".").lower()


def _convert_image(source_path: str, 
                   target_path: str, 
                   target_extension: str) -> None:

    pillow_format = PILLOW_FORMAT_MAP.get(target_extension)

    if pillow_format is None:
        raise ValueError(f"Unsupported file format [{target_extension}]")

    with Image.open(source_path) as img:
        if pillow_format == "JPEG" and img.mode in ("RGBA", "LA", "P"):
            bg = Image.new("RGB", img.size, (255, 255, 255))
            bg.paste(img.convert("RGBA"), mask=img.convert("RGBA").split()[3])
            bg.save(target_path, format=pillow_format, quality=92)

        else:
            img.save(target_path, format=pillow_format)


def convert(source_path: str, target_path: str) -> None:
    source_extension = _extension(source_path)
    target_extension = _extension(target_path)

    if source_extension == target_extension:
        import shutil
        shutil.copyfile(source_path, target_path)
        return

    if source_extension not in IMAGE_FORMATS or target_extension not in IMAGE_FORMATS:
        raise ValueError(f"Unsupported conversion {source_extension} => {target_extension}")

    _convert_image(source_path, target_path, target_extension)
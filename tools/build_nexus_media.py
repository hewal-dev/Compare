#!/usr/bin/env python3
"""Nexus-Promo-Bilder für Compare erzeugen (Autorenwerkzeug, optional).

Aufruf:
    python3 tools/build_nexus_media.py screenshot1.png screenshot2.png [...]
    NEXUS_SOURCES="a.png:b.png" python3 tools/build_nexus_media.py
    # oder Bilder nach media/sources/ legen und ohne Argumente starten

Schreibt Thumbnail, Header, Galerie und Kontaktbogen nach media/nexus/.
Benötigt Pillow sowie DejaVu-Schriften (überschreibbar per NEXUS_FONT_BOLD / NEXUS_FONT_REGULAR).
"""
import os
import sys
from pathlib import Path
from PIL import Image, ImageDraw, ImageFilter, ImageFont

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "media" / "nexus"
OUT.mkdir(parents=True, exist_ok=True)


def resolve_sources() -> list[Path]:
    """Bildquellen aus Argumenten, NEXUS_SOURCES oder media/sources/."""
    raw = list(sys.argv[1:])
    if not raw and os.environ.get("NEXUS_SOURCES"):
        raw = [p for p in os.environ["NEXUS_SOURCES"].split(os.pathsep) if p]
    if not raw:
        default_dir = ROOT / "media" / "sources"
        if default_dir.is_dir():
            raw = sorted(str(p) for p in default_dir.glob("*.png"))
    if not raw:
        raise SystemExit(
            "FEHLER: Keine Bildquellen angegeben.\n"
            "  python3 tools/build_nexus_media.py <screenshot1.png> [<screenshot2.png> ...]\n"
            'oder NEXUS_SOURCES="a.png:b.png" setzen oder Bilder in media/sources/ legen.'
        )
    paths = [Path(p).expanduser() for p in raw]
    missing = [str(p) for p in paths if not p.is_file()]
    if missing:
        raise SystemExit("FEHLER: Diese Bildquellen fehlen:\n  " + "\n  ".join(missing))
    return paths


SOURCES: list[Path] = []

FONT_BOLD = os.environ.get("NEXUS_FONT_BOLD", "/usr/share/fonts/TTF/DejaVuSansCondensed-Bold.ttf")
FONT_REGULAR = os.environ.get("NEXUS_FONT_REGULAR", "/usr/share/fonts/TTF/DejaVuSans.ttf")
WHITE = (244, 246, 244, 255)
MUTED = (177, 184, 180, 255)
ORANGE = (232, 137, 42, 255)
GREEN = (36, 225, 67, 255)
RED = (244, 48, 45, 255)
DARK = (10, 14, 14, 255)


def font(size: int, bold: bool = False) -> ImageFont.FreeTypeFont:
    path = FONT_BOLD if bold else FONT_REGULAR
    if not Path(path).is_file():
        candidates = [
            "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf" if bold
            else "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
            "/usr/share/fonts/TTF/DejaVuSans.ttf",
        ]
        path = next((p for p in candidates if Path(p).is_file()), "")
        if not path:
            raise SystemExit(
                f"FEHLER: Schriftart nicht gefunden.\n"
                f"Per NEXUS_FONT_BOLD / NEXUS_FONT_REGULAR übergeben oder DejaVu installieren."
            )
    return ImageFont.truetype(path, size)


def cover(img: Image.Image, size: tuple[int, int]) -> Image.Image:
    iw, ih = img.size
    tw, th = size
    scale = max(tw / iw, th / ih)
    resized = img.resize((round(iw * scale), round(ih * scale)), Image.Resampling.LANCZOS)
    left = (resized.width - tw) // 2
    top = (resized.height - th) // 2
    return resized.crop((left, top, left + tw, top + th))


def contain(img: Image.Image, size: tuple[int, int]) -> Image.Image:
    iw, ih = img.size
    tw, th = size
    scale = min(tw / iw, th / ih)
    return img.resize((max(1, round(iw * scale)), max(1, round(ih * scale))), Image.Resampling.LANCZOS)


def background(img: Image.Image, size: tuple[int, int]) -> Image.Image:
    bg = cover(img, size).filter(ImageFilter.GaussianBlur(30)).convert("RGBA")
    bg.alpha_composite(Image.new("RGBA", size, (4, 8, 8, 218)))
    draw = ImageDraw.Draw(bg, "RGBA")
    for y in range(size[1]):
        alpha = round(40 + 90 * y / max(size[1] - 1, 1))
        draw.line((0, y, size[0], y), fill=(0, 0, 0, alpha))
    draw.polygon([(0, 0), (size[0] * 0.68, 0), (size[0] * 0.40, size[1]), (0, size[1])], fill=(5, 9, 9, 90))
    return bg


def paste_card(canvas: Image.Image, img: Image.Image, xy: tuple[int, int], max_size: tuple[int, int],
               accent=ORANGE, border: int = 3, shadow: int = 22) -> tuple[int, int, int, int]:
    card = contain(img.convert("RGBA"), max_size)
    x, y = xy
    shadow_img = Image.new("RGBA", (card.width + shadow * 2, card.height + shadow * 2), (0, 0, 0, 0))
    sd = ImageDraw.Draw(shadow_img, "RGBA")
    sd.rounded_rectangle((shadow, shadow, shadow + card.width, shadow + card.height), radius=8, fill=(0, 0, 0, 205))
    shadow_img = shadow_img.filter(ImageFilter.GaussianBlur(shadow // 2))
    canvas.alpha_composite(shadow_img, (x - shadow, y - shadow))
    frame = Image.new("RGBA", (card.width + border * 2, card.height + border * 2), accent)
    frame.alpha_composite(card, (border, border))
    canvas.alpha_composite(frame, (x - border, y - border))
    return x, y, card.width, card.height


def add_brand(draw: ImageDraw.ImageDraw, title: str, subtitle: str, width: int) -> None:
    draw.text((104, 72), title, font=font(132, True), fill=WHITE, stroke_width=2, stroke_fill=(0, 0, 0, 150))
    title_width = draw.textbbox((0, 0), title, font=font(132, True))[2]
    draw.rounded_rectangle((108 + title_width, 99, min(width - 100, 108 + title_width + 114), 124), radius=11, fill=ORANGE)
    draw.text((110, 230), subtitle, font=font(42, True), fill=ORANGE)
    draw.text((110, 286), "See the crafted result before spending resources", font=font(28), fill=MUTED)


def build_thumbnail(images: list[Image.Image]) -> None:
    canvas = background(images[2], (1920, 1080))
    draw = ImageDraw.Draw(canvas, "RGBA")
    add_brand(draw, "COMPARE", "CRAFTING STAT PREVIEW", canvas.width)

    # Two broad tool comparisons form the visual foundation.
    paste_card(canvas, images[0], (72, 410), (850, 340), ORANGE)
    paste_card(canvas, images[2], (998, 410), (850, 330), ORANGE)

    # Weapon comparison in the foreground proves ranged-weapon support.
    x, y, w, h = paste_card(canvas, images[1], (665, 648), (590, 388), (236, 158, 50, 255), border=4, shadow=28)
    draw.rounded_rectangle((x + 28, y - 28, x + 286, y + 13), radius=16, fill=(8, 12, 12, 235), outline=ORANGE, width=2)
    draw.text((x + 49, y - 21), "TOOLS + WEAPONS", font=font(21, True), fill=WHITE)

    # Feature chips remain readable when Nexus scales the image down.
    chips = [("REAL VALUES", GREEN), ("CLEAR %", ORANGE), ("BETTER / WORSE", RED)]
    cx = 1254
    cy = 275
    for text, color in chips:
        tw = draw.textbbox((0, 0), text, font=font(20, True))[2]
        draw.rounded_rectangle((cx, cy, cx + tw + 42, cy + 43), radius=18, fill=(15, 20, 20, 235), outline=color, width=2)
        draw.text((cx + 21, cy + 9), text, font=font(20, True), fill=WHITE)
        cx += tw + 58

    canvas.convert("RGB").save(OUT / "compare-thumbnail-1920x1080.jpg", quality=94, optimize=True, progressive=True)


def build_header(images: list[Image.Image]) -> None:
    canvas = background(images[0], (1300, 372))
    draw = ImageDraw.Draw(canvas, "RGBA")

    # Keep the left side deliberately quiet for Nexus' overlaid page title and buttons.
    draw.rectangle((0, 0, 445, 372), fill=(5, 9, 9, 225))
    draw.rectangle((434, 28, 440, 344), fill=ORANGE)
    for x in (72, 126, 180, 234):
        draw.line((x, 72, x + 110, 300), fill=(232, 137, 42, 22), width=3)

    # The compare panel on the right communicates the feature without extra copy.
    crop = images[0].crop((386, 0, images[0].width, images[0].height))
    crop = cover(crop, (836, 334))
    shadow = Image.new("RGBA", (872, 370), (0, 0, 0, 0))
    ImageDraw.Draw(shadow, "RGBA").rounded_rectangle((20, 18, 852, 352), radius=8, fill=(0, 0, 0, 210))
    shadow = shadow.filter(ImageFilter.GaussianBlur(12))
    canvas.alpha_composite(shadow, (428, 1))
    frame = Image.new("RGBA", (842, 340), ORANGE)
    frame.alpha_composite(crop, (3, 3))
    canvas.alpha_composite(frame, (458, 16))

    # A small visual cue remains outside the likely title overlay area.
    draw.rounded_rectangle((1034, 305, 1268, 346), radius=17, fill=(8, 12, 12, 235), outline=GREEN, width=2)
    draw.text((1060, 313), "LIVE STAT PREVIEW", font=font(18, True), fill=WHITE)

    canvas.convert("RGB").save(OUT / "compare-header-1300x372.jpg", quality=94, optimize=True, progressive=True)


def build_gallery(images: list[Image.Image]) -> None:
    specs = [
        (images[0], "TOOL COMPARISON", "Steel pickaxe: crafted values versus the equipped item", "compare-gallery-01-pickaxe-1920x1080.jpg"),
        (images[1], "WEAPON COMPARISON", "Damage, magazine size, fire rate, range and durability", "compare-gallery-02-ak47-1920x1080.jpg"),
        (images[2], "BETTER OR WORSE — AT A GLANCE", "Green and red percentages show the real change", "compare-gallery-03-fireaxe-1920x1080.jpg"),
    ]
    for source, title, subtitle, filename in specs:
        canvas = background(source, (1920, 1080))
        draw = ImageDraw.Draw(canvas, "RGBA")
        draw.text((110, 78), title, font=font(72, True), fill=WHITE)
        draw.rectangle((112, 172, 560, 181), fill=ORANGE)
        draw.text((112, 208), subtitle, font=font(30), fill=MUTED)
        card = contain(source.convert("RGBA"), (1660, 720))
        x = (1920 - card.width) // 2
        y = 320 + (700 - card.height) // 2
        paste_card(canvas, source, (x, y), (1660, 700), ORANGE, border=4, shadow=30)
        canvas.convert("RGB").save(OUT / filename, quality=94, optimize=True, progressive=True)


def build_contact_sheet() -> None:
    files = [
        OUT / "compare-thumbnail-1920x1080.jpg",
        OUT / "compare-header-1300x372.jpg",
        OUT / "compare-gallery-01-pickaxe-1920x1080.jpg",
        OUT / "compare-gallery-02-ak47-1920x1080.jpg",
        OUT / "compare-gallery-03-fireaxe-1920x1080.jpg",
    ]
    thumbs = []
    for path in files:
        img = Image.open(path).convert("RGB")
        thumb = contain(img, (720, 405))
        thumbs.append((path.name, thumb))
    sheet = Image.new("RGB", (1540, 1400), (18, 22, 22))
    draw = ImageDraw.Draw(sheet)
    positions = [(30, 55), (790, 55), (30, 500), (790, 500), (410, 945)]
    for (name, img), (x, y) in zip(thumbs, positions):
        sheet.paste(img, (x, y + 34))
        draw.text((x, y), name, font=font(18, True), fill=(235, 238, 236))
    sheet.save(OUT / "compare-nexus-media-preview.jpg", quality=91, optimize=True)


def main() -> None:
    global SOURCES
    SOURCES = resolve_sources()
    images = [Image.open(path).convert("RGBA") for path in SOURCES]
    build_thumbnail(images)
    build_header(images)
    build_gallery(images)
    build_contact_sheet()
    print(f"Created Nexus media in {OUT}")


if __name__ == "__main__":
    main()

import os
import glob
import math
from PIL import Image, ImageDraw, ImageFilter, ImageOps

brain_dir = r"C:\Users\jisau\.gemini\antigravity-ide\brain\e97d23ae-5b80-4ddd-8719-e341e9e126ff"
out_dirs = [
    r"Assets\Art\Sprites",
    r"Assets\Resources\Art\Sprites"
]

for d in out_dirs:
    os.makedirs(d, exist_ok=True)

def process_sprite_with_black_outline(img_path, outline_thickness=4, tolerance=232):
    img = Image.open(img_path).convert("RGBA")
    w, h = img.size
    
    # 1. Detect background from border
    mask = Image.new("L", (w, h), 0)
    img_data = img.getdata()
    mask_pixels = []
    
    for item in img_data:
        r, g, b, a = item
        # If very close to white background
        if r >= tolerance and g >= tolerance and b >= tolerance:
            mask_pixels.append(255)
        else:
            mask_pixels.append(0)
    mask.putdata(mask_pixels)
    
    # Floodfill connected background from borders
    for x in range(0, w, 4):
        if mask.getpixel((x, 0)) == 255: ImageDraw.floodfill(mask, (x, 0), 128)
        if mask.getpixel((x, h - 1)) == 255: ImageDraw.floodfill(mask, (x, h - 1), 128)
    for y in range(0, h, 4):
        if mask.getpixel((0, y)) == 255: ImageDraw.floodfill(mask, (0, y), 128)
        if mask.getpixel((w - 1, y)) == 255: ImageDraw.floodfill(mask, (w - 1, y), 128)
        
    # Alpha mask: foreground = 255, background = 0
    fg_mask = Image.new("L", (w, h), 0)
    fg_pixels = []
    for val in mask.getdata():
        if val == 128:
            fg_pixels.append(0)
        else:
            fg_pixels.append(255)
    fg_mask.putdata(fg_pixels)
    
    # 2. Erode foreground mask by 2 pixels to completely eliminate white fringe
    # MinFilter with size 3 or 5 erodes white regions
    eroded_fg = fg_mask.filter(ImageFilter.MinFilter(size=3))
    
    # 3. Create dilated mask for black outline
    # MaxFilter dilates
    dilated_mask = eroded_fg.filter(ImageFilter.MaxFilter(size=outline_thickness * 2 + 1))
    
    # 4. Composite:
    # Base: Black silhouette of dilated mask
    result = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    black_layer = Image.new("RGBA", (w, h), (25, 25, 28, 255))
    result.paste(black_layer, (0, 0), dilated_mask)
    
    # Foreground: original image masked by eroded_fg
    fg_img = img.copy()
    fg_img.putalpha(eroded_fg)
    result.alpha_composite(fg_img)
    
    return result

mapping = {
    "yellow_submarine": "yellow_submarine_*.jpg",
    "captain_finn_idle": "captain_finn_idle_*.jpg",
    "captain_finn_cheer": "captain_finn_cheer_*.jpg",
    "pink_octopus": "pink_octopus_*.jpg",
    "tropical_fish": "tropical_fish_*.jpg",
    "toy_red_car": "toy_red_car_*.jpg",
    "toy_blue_boat": "toy_blue_boat_*.jpg",
    "toy_green_plane": "toy_green_plane_*.jpg",
    "starfish_button": "starfish_button_*.jpg",
    "ship_wheel": "ship_wheel_*.jpg",
    "gold_star_badge": "gold_star_badge_*.jpg",
    "rock_tablet": "rock_tablet_*.jpg",
}

for name, pattern in mapping.items():
    matches = glob.glob(os.path.join(brain_dir, pattern))
    if matches:
        print(f"Processing {name} with black outline...")
        res = process_sprite_with_black_outline(matches[0], outline_thickness=5, tolerance=232)
        
        # Center ship wheel
        if name == "ship_wheel":
            bbox = res.getbbox()
            if bbox:
                cr = res.crop(bbox)
                side = max(cr.size) + 16
                sq = Image.new("RGBA", (side, side), (0, 0, 0, 0))
                sq.paste(cr, ((side - cr.width) // 2, (side - cr.height) // 2))
                res = sq
                
        for out_dir in out_dirs:
            res.save(os.path.join(out_dir, f"{name}.png"))
        print(f"Saved {name}.png with black outline!")

print("All sprites reprocessed with clean black outlines!")

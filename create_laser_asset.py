import os
import math
from PIL import Image, ImageDraw, ImageFilter

def draw_laser_icon():
    # Render at 1024x1024, downsample to 256x256 for supreme antialiasing
    S = 1024
    img = Image.new("RGBA", (S, S), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # Center coords and scaling
    # We want a cartoon laser blaster facing right, with handle on bottom-left, barrel on right
    
    # Let's draw shadows and outer glow first on a separate layer
    glow_layer = Image.new("RGBA", (S, S), (0, 0, 0, 0))
    gdraw = ImageDraw.Draw(glow_layer)

    # Laser muzzle glow burst center around (760, 430)
    muzzle_x, muzzle_y = 750, 430

    # Draw laser spark rays coming from muzzle
    def draw_spark(d, cx, cy, radius, width, col):
        for ang in [0, 45, 90, 135, 180, 225, 270, 315]:
            rad = math.radians(ang)
            r2 = radius * (1.4 if ang % 90 == 0 else 0.7)
            x1 = cx + math.cos(rad) * (radius * 0.2)
            y1 = cy + math.sin(rad) * (radius * 0.2)
            x2 = cx + math.cos(rad) * r2
            y2 = cy + math.sin(rad) * r2
            d.line([(x1, y1), (x2, y2)], fill=col, width=width)

    # Soft ambient glow behind muzzle
    for r in range(160, 20, -10):
        alpha = int(70 * (1.0 - r / 160.0))
        gdraw.ellipse([muzzle_x - r, muzzle_y - r, muzzle_x + r, muzzle_y + r], fill=(255, 50, 80, alpha))

    # Big 4-point star burst at muzzle
    def draw_laser_star(d, cx, cy, r_outer, r_inner, fill_c):
        pts = []
        for i in range(8):
            ang = i * (math.pi / 4)
            r = r_outer if i % 2 == 0 else r_inner
            pts.append((cx + r * math.cos(ang), cy + r * math.sin(ang)))
        d.polygon(pts, fill=fill_c)

    draw_laser_star(gdraw, muzzle_x + 30, muzzle_y, 140, 35, (255, 60, 100, 190))
    draw_laser_star(gdraw, muzzle_x + 30, muzzle_y, 90, 22, (255, 220, 100, 240))
    draw_laser_star(gdraw, muzzle_x + 30, muzzle_y, 45, 12, (255, 255, 255, 255))

    # Outer outline layer (thick dark border)
    body_layer = Image.new("RGBA", (S, S), (0, 0, 0, 0))
    bdraw = ImageDraw.Draw(body_layer)

    OUTLINE_COL = (24, 20, 38, 255)
    OUTLINE_W = 22

    # --- 1. HANDLE / GRIP ---
    # Slanted grip from (260, 520) down to (200, 780)
    grip_pts = [(280, 520), (370, 520), (320, 770), (220, 760), (200, 720)]
    # Rounded grip end
    bdraw.polygon([(260, 520), (380, 520), (325, 780), (205, 760)], fill=(35, 45, 75, 255))
    # Grip rounded bottom cap
    bdraw.ellipse([190, 720, 335, 800], fill=(25, 32, 55, 255))
    # Grip tactile ribs (orange/gold rubber accents)
    for gy in [580, 630, 680]:
        t = (gy - 520) / 260.0
        gx = int(270 - t * 65)
        gw = 95
        bdraw.rounded_rectangle([gx, gy, gx + gw, gy + 26], radius=12, fill=(255, 160, 30, 255))
        # Highlight on rib
        bdraw.rounded_rectangle([gx + 8, gy + 4, gx + gw - 8, gy + 12], radius=6, fill=(255, 225, 120, 255))

    # Trigger guard & trigger
    # Guard loop
    bdraw.arc([290, 530, 440, 680], start=40, end=190, fill=(35, 45, 75, 255), width=24)
    # Trigger
    bdraw.polygon([(360, 550), (385, 590), (365, 620), (345, 570)], fill=(255, 200, 50, 255))

    # --- 2. MAIN BLASTER CHASSIS ---
    # Rounded sleek body: from X: 220 to 640, Y: 330 to 530
    chassis_box = [220, 330, 640, 530]
    # Gradient red body: Crimson red to deep ruby red
    for cy in range(330, 530):
        t = (cy - 330) / 200.0
        r = int(255 - t * 75)
        g = int(55 - t * 35)
        b = int(95 - t * 55)
        bdraw.rounded_rectangle([220, cy, 640, cy + 2], radius=55, fill=(r, g, b, 255))

    # Top sleek fin / scope
    bdraw.polygon([(290, 330), (330, 270), (510, 270), (550, 330)], fill=(225, 30, 65, 255))
    bdraw.rounded_rectangle([320, 260, 520, 290], radius=14, fill=(255, 195, 45, 255))
    # Scope lens highlight
    bdraw.ellipse([495, 263, 522, 287], fill=(120, 240, 255, 255))

    # Rear cap / battery hatch (Navy / Gold)
    bdraw.rounded_rectangle([190, 350, 240, 510], radius=24, fill=(40, 52, 85, 255))
    bdraw.rounded_rectangle([180, 380, 205, 480], radius=12, fill=(255, 185, 35, 255))

    # --- 3. ENERGY CORE (Glowing cyan window in center) ---
    core_box = [340, 380, 520, 480]
    # Dark casing inset
    bdraw.rounded_rectangle(core_box, radius=26, fill=(15, 25, 48, 255), outline=(255, 205, 50, 255), width=10)
    # Glowing plasma tube
    bdraw.rounded_rectangle([356, 396, 504, 464], radius=18, fill=(0, 210, 255, 255))
    # Inner bright plasma beam
    bdraw.rounded_rectangle([370, 412, 490, 448], radius=12, fill=(200, 250, 255, 255))
    # Glass reflection slant across core
    bdraw.polygon([(380, 396), (410, 396), (365, 464), (356, 464)], fill=(255, 255, 255, 130))

    # --- 4. BARREL & MUZZLE ---
    # Barrel collar / coupler (Gold rings)
    bdraw.rounded_rectangle([620, 360, 660, 500], radius=18, fill=(255, 195, 40, 255))
    bdraw.rounded_rectangle([630, 370, 650, 490], radius=10, fill=(255, 235, 130, 255))

    # Nozzle cone (Titanium steel with red accent)
    bdraw.polygon([(655, 375), (745, 395), (745, 465), (655, 485)], fill=(75, 90, 125, 255))
    # Nozzle ring highlight
    bdraw.polygon([(665, 377), (700, 385), (700, 475), (665, 483)], fill=(120, 140, 180, 255))

    # Laser Emitter Tip (Ruby lens)
    bdraw.rounded_rectangle([735, 390, 775, 470], radius=18, fill=(255, 30, 70, 255))
    bdraw.rounded_rectangle([750, 402, 780, 458], radius=14, fill=(255, 120, 160, 255))
    bdraw.ellipse([758, 415, 782, 445], fill=(255, 255, 255, 255))

    # --- 5. GLOSS & SPECULAR HIGHLIGHTS (Juicy 3D look) ---
    # Gloss highlight on upper body
    gloss_pts = [(260, 350), (600, 350), (580, 375), (280, 375)]
    bdraw.rounded_rectangle([250, 345, 600, 372], radius=14, fill=(255, 255, 255, 160))
    bdraw.rounded_rectangle([270, 350, 520, 362], radius=7, fill=(255, 255, 255, 230))

    # Lower rim ambient light (Soft cyan reflection from ocean)
    bdraw.rounded_rectangle([250, 500, 590, 516], radius=8, fill=(80, 220, 255, 120))

    # Combine: Stroke outline around body
    # We create outline by taking alpha mask, dilating it with max filter, and filling with outline color
    alpha_mask = body_layer.split()[3]
    # Dilate mask for dark outline
    dilated_mask = alpha_mask.filter(ImageFilter.MaxFilter(25))
    outline_img = Image.new("RGBA", (S, S), OUTLINE_COL)
    
    # Final composite
    final_canvas = Image.new("RGBA", (S, S), (0, 0, 0, 0))
    # 1. Glow layer
    final_canvas.paste(glow_layer, (0, 0), glow_layer)
    # 2. Outline
    final_canvas.paste(outline_img, (0, 0), dilated_mask)
    # 3. Inner body
    final_canvas.paste(body_layer, (0, 0), body_layer)

    # Downsample with Lanczos for smooth crisp icon
    icon_final = final_canvas.resize((384, 384), Image.Resampling.LANCZOS)
    
    # Auto-crop to content bounding box with slight padding
    bbox = icon_final.getbbox()
    if bbox:
        pad = 8
        crop_box = (
            max(0, bbox[0] - pad),
            max(0, bbox[1] - pad),
            min(icon_final.width, bbox[2] + pad),
            min(icon_final.height, bbox[3] + pad)
        )
        icon_final = icon_final.crop(crop_box)
    
    out_path = "Assets/Resources/Art/Sprites/icon_laser_tool.png"
    icon_final.save(out_path, "PNG")
    print(f"Saved {out_path} ({icon_final.size})")

if __name__ == "__main__":
    draw_laser_icon()

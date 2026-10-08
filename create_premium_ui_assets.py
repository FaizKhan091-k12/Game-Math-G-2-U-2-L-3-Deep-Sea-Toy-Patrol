import os
import math
from PIL import Image, ImageDraw, ImageFont, ImageFilter, ImageOps

FONT_COOPER = "C:/Windows/Fonts/COOPBL.TTF"
FONT_ARIAL_ROUND = "C:/Windows/Fonts/ARLRDBD.TTF"
if not os.path.exists(FONT_COOPER):
    FONT_COOPER = FONT_ARIAL_ROUND

def create_gradient_fill(width, height, top_color, bottom_color):
    base = Image.new("RGBA", (width, height))
    for y in range(height):
        t = y / max(1, height - 1)
        r = int(top_color[0] + (bottom_color[0] - top_color[0]) * t)
        g = int(top_color[1] + (bottom_color[1] - top_color[1]) * t)
        b = int(top_color[2] + (bottom_color[2] - top_color[2]) * t)
        a = int(top_color[3] + (bottom_color[3] - top_color[3]) * t) if len(top_color) > 3 else 255
        for x in range(width):
            base.putpixel((x, y), (r, g, b, a))
    return base

def draw_star(draw, cx, cy, r_out, r_in, fill_col, outline_col=None, outline_w=0):
    points = []
    for i in range(10):
        angle = -math.pi / 2 + i * (math.pi / 5)
        r = r_out if i % 2 == 0 else r_in
        x = cx + r * math.cos(angle)
        y = cy + r * math.sin(angle)
        points.append((x, y))
    
    if outline_col and outline_w > 0:
        draw.polygon(points, fill=outline_col)
        # Inner slightly smaller
        inner_points = []
        for i in range(10):
            angle = -math.pi / 2 + i * (math.pi / 5)
            r = (r_out - outline_w) if i % 2 == 0 else max(1, r_in - outline_w)
            x = cx + r * math.cos(angle)
            y = cy + r * math.sin(angle)
            inner_points.append((x, y))
        draw.polygon(inner_points, fill=fill_col)
    else:
        draw.polygon(points, fill=fill_col)

def draw_bubble(draw, cx, cy, radius):
    # Outer dark cyan circle
    draw.ellipse([cx - radius, cy - radius, cx + radius, cy + radius], outline=(0, 40, 80, 220), width=4)
    # Inner light cyan ring
    draw.ellipse([cx - radius + 3, cy - radius + 3, cx + radius - 3, cy + radius - 3], fill=(160, 235, 255, 60), outline=(220, 250, 255, 220), width=3)
    # Specular highlight
    hr = radius * 0.35
    hx = cx - radius * 0.35
    hy = cy - radius * 0.35
    draw.ellipse([hx - hr, hy - hr * 0.6, hx + hr, hy + hr * 0.6], fill=(255, 255, 255, 240))
    # Small secondary highlight
    sr = radius * 0.15
    sx = cx + radius * 0.4
    sy = cy + radius * 0.4
    draw.ellipse([sx - sr, sy - sr, sx + sr, sy + sr], fill=(255, 255, 255, 180))

def create_title_logo():
    # Supersampled canvas (2x)
    SW, SH = 1920, 700
    canvas = Image.new("RGBA", (SW, SH), (0, 0, 0, 0))
    
    font_deep = ImageFont.truetype(FONT_COOPER, 140)
    font_toy = ImageFont.truetype(FONT_COOPER, 180)
    
    text_deep = "DEEP SEA"
    text_toy = "TOY PATROL"
    
    # Calculate bounding boxes
    dummy_draw = ImageDraw.Draw(canvas)
    bbox_deep = dummy_draw.textbbox((0, 0), text_deep, font=font_deep)
    w_deep = bbox_deep[2] - bbox_deep[0]
    h_deep = bbox_deep[3] - bbox_deep[1]
    
    bbox_toy = dummy_draw.textbbox((0, 0), text_toy, font=font_toy)
    w_toy = bbox_toy[2] - bbox_toy[0]
    h_toy = bbox_toy[3] - bbox_toy[1]
    
    x_deep = (SW - w_deep) // 2
    y_deep = 110
    
    x_toy = (SW - w_toy) // 2
    y_toy = y_deep + h_deep + 55
    
    # ---------------------------------------------
    # Render "TOY PATROL" (Bottom word)
    # ---------------------------------------------
    extrude_toy = 26
    stroke_toy = 22
    
    # 1. 3D Bottom Extrusion Shadow (Dark chocolate/amber)
    for dy in range(extrude_toy, 0, -2):
        for dx in range(-stroke_toy, stroke_toy + 1, 3):
            for ddy in range(-stroke_toy, stroke_toy + 1, 3):
                dummy_draw.text((x_toy + dx, y_toy + dy + ddy), text_toy, font=font_toy, fill=(0, 0, 0, 255))
    
    # Extrusion core
    for dy in range(1, extrude_toy):
        dummy_draw.text((x_toy, y_toy + dy), text_toy, font=font_toy, fill=(120, 50, 0, 255))
        
    # 2. Main Outer Black Stroke
    for dx in range(-stroke_toy, stroke_toy + 1, 2):
        for dy in range(-stroke_toy, stroke_toy + 1, 2):
            if dx*dx + dy*dy <= stroke_toy*stroke_toy:
                dummy_draw.text((x_toy + dx, y_toy + dy), text_toy, font=font_toy, fill=(0, 0, 0, 255))
                
    # 3. Inner Cream/Gold Bevel Stroke (thickness 7)
    inner_stroke = 7
    for dx in range(-inner_stroke, inner_stroke + 1, 2):
        for dy in range(-inner_stroke, inner_stroke + 1, 2):
            dummy_draw.text((x_toy + dx, y_toy + dy), text_toy, font=font_toy, fill=(255, 245, 180, 255))
            
    # 4. Main Text Mask & Gradient Fill
    mask_toy = Image.new("L", (SW, SH), 0)
    m_draw = ImageDraw.Draw(mask_toy)
    m_draw.text((x_toy, y_toy), text_toy, font=font_toy, fill=255)
    
    grad_toy = create_gradient_fill(SW, SH, (255, 235, 50, 255), (255, 100, 0, 255))
    canvas.paste(grad_toy, (0, 0), mask_toy)
    
    # 5. Top Gloss Highlight on TOY PATROL
    gloss_mask_toy = Image.new("L", (SW, SH), 0)
    g_draw = ImageDraw.Draw(gloss_mask_toy)
    g_draw.text((x_toy, y_toy), text_toy, font=font_toy, fill=255)
    # Clip to top 42%
    g_clip = Image.new("L", (SW, SH), 0)
    g_clip_draw = ImageDraw.Draw(g_clip)
    g_clip_draw.rectangle([x_toy - 20, y_toy - 20, x_toy + w_toy + 20, y_toy + int(h_toy * 0.44)], fill=255)
    gloss_final_toy = ImageOps.invert(ImageOps.invert(gloss_mask_toy)) # copy
    # Combined mask
    comb_toy = Image.new("L", (SW, SH), 0)
    for px in range(x_toy - 10, x_toy + w_toy + 10, 4):
        for py in range(y_toy, y_toy + int(h_toy * 0.44), 4):
            if gloss_mask_toy.getpixel((px, py)) > 0:
                comb_toy.putpixel((px, py), 160)
    white_gloss = Image.new("RGBA", (SW, SH), (255, 255, 255, 180))
    canvas.paste(white_gloss, (0, 0), comb_toy)

    # ---------------------------------------------
    # Render "DEEP SEA" (Top word)
    # ---------------------------------------------
    extrude_deep = 22
    stroke_deep = 18
    
    # 1. 3D Bottom Extrusion Shadow (Navy/Black)
    for dy in range(extrude_deep, 0, -2):
        for dx in range(-stroke_deep, stroke_deep + 1, 3):
            for ddy in range(-stroke_deep, stroke_deep + 1, 3):
                dummy_draw.text((x_deep + dx, y_deep + dy + ddy), text_deep, font=font_deep, fill=(0, 0, 0, 255))
                
    for dy in range(1, extrude_deep):
        dummy_draw.text((x_deep, y_deep + dy), text_deep, font=font_deep, fill=(5, 40, 85, 255))

    # 2. Main Outer Black Stroke
    for dx in range(-stroke_deep, stroke_deep + 1, 2):
        for dy in range(-stroke_deep, stroke_deep + 1, 2):
            if dx*dx + dy*dy <= stroke_deep*stroke_deep:
                dummy_draw.text((x_deep + dx, y_deep + dy), text_deep, font=font_deep, fill=(0, 0, 0, 255))

    # 3. Inner White Bevel Rim
    for dx in range(-6, 7, 2):
        for dy in range(-6, 7, 2):
            dummy_draw.text((x_deep + dx, y_deep + dy), text_deep, font=font_deep, fill=(210, 245, 255, 255))

    # 4. Main Text Mask & Gradient Fill (Cyan -> Rich Blue)
    mask_deep = Image.new("L", (SW, SH), 0)
    m_draw_deep = ImageDraw.Draw(mask_deep)
    m_draw_deep.text((x_deep, y_deep), text_deep, font=font_deep, fill=255)
    
    grad_deep = create_gradient_fill(SW, SH, (90, 240, 255, 255), (10, 110, 230, 255))
    canvas.paste(grad_deep, (0, 0), mask_deep)
    
    # 5. Top Gloss Highlight on DEEP SEA
    comb_deep = Image.new("L", (SW, SH), 0)
    for px in range(x_deep - 10, x_deep + w_deep + 10, 4):
        for py in range(y_deep, y_deep + int(h_deep * 0.42), 4):
            if mask_deep.getpixel((px, py)) > 0:
                comb_deep.putpixel((px, py), 170)
    canvas.paste(white_gloss, (0, 0), comb_deep)

    # ---------------------------------------------
    # Add Decorative Sea Elements (Stars & Bubbles)
    # ---------------------------------------------
    decor_draw = ImageDraw.Draw(canvas)
    
    # Left Golden Star
    draw_star(decor_draw, x_deep - 90, y_deep + 80, 52, 24, (255, 215, 20, 255), outline_col=(0, 0, 0, 255), outline_w=8)
    draw_star(decor_draw, x_deep - 90, y_deep + 80, 40, 18, (255, 245, 120, 255))
    # Right Golden Star
    draw_star(decor_draw, x_deep + w_deep + 90, y_deep + 80, 52, 24, (255, 215, 20, 255), outline_col=(0, 0, 0, 255), outline_w=8)
    draw_star(decor_draw, x_deep + w_deep + 90, y_deep + 80, 40, 18, (255, 245, 120, 255))
    
    # Sparkle stars
    draw_star(decor_draw, x_toy - 60, y_toy + 130, 36, 14, (255, 235, 80, 255), outline_col=(0, 0, 0, 255), outline_w=6)
    draw_star(decor_draw, x_toy + w_toy + 60, y_toy + 130, 36, 14, (255, 235, 80, 255), outline_col=(0, 0, 0, 255), outline_w=6)

    # Glossy Bubbles
    bubble_coords = [
        (x_deep - 160, y_deep + 20, 34),
        (x_deep - 190, y_deep + 110, 22),
        (x_deep - 130, y_deep + 150, 18),
        (x_deep + w_deep + 160, y_deep + 20, 34),
        (x_deep + w_deep + 190, y_deep + 110, 22),
        (x_deep + w_deep + 135, y_deep + 150, 18),
        (x_toy - 120, y_toy + 60, 28),
        (x_toy + w_toy + 120, y_toy + 60, 28),
    ]
    for bx, by, br in bubble_coords:
        draw_bubble(decor_draw, bx, by, br)

    # Downscale with Lanczos for smooth anti-aliased cartoon edges
    final_img = canvas.resize((SW // 2, SH // 2), Image.Resampling.LANCZOS)
    return final_img

def create_play_button():
    # Supersampled canvas (2x)
    BW, BH = 1000, 460
    canvas = Image.new("RGBA", (BW, BH), (0, 0, 0, 0))
    draw = ImageDraw.Draw(canvas)
    
    # Button bounding box
    pad_x, pad_y = 80, 60
    w_btn = BW - pad_x * 2
    h_btn = BH - pad_y * 2 - 25
    radius = 75
    
    bx0, by0 = pad_x, pad_y
    bx1, by1 = pad_x + w_btn, pad_y + h_btn
    
    # 1. 3D Bottom Depth Shadow (Deep Pine / Emerald shade + Black cartoon stroke)
    depth_offset = 28
    stroke_w = 16
    
    # Outer black stroke
    draw.rounded_rectangle([bx0 - stroke_w, by0 - stroke_w + depth_offset, bx1 + stroke_w, by1 + stroke_w + depth_offset], radius=radius + stroke_w, fill=(0, 0, 0, 255))
    draw.rounded_rectangle([bx0 - stroke_w, by0 - stroke_w, bx1 + stroke_w, by1 + stroke_w], radius=radius + stroke_w, fill=(0, 0, 0, 255))
    
    # 3D Lower rim (Rich dark forest emerald)
    draw.rounded_rectangle([bx0, by0 + depth_offset, bx1, by1 + depth_offset], radius=radius, fill=(12, 95, 38, 255))
    # Bevel lines on lower rim
    draw.rounded_rectangle([bx0 + 6, by0 + depth_offset + 6, bx1 - 6, by1 + depth_offset - 4], radius=radius - 4, fill=(8, 75, 30, 255))
    
    # 2. Main Button Face (Golden/Yellow border + Emerald gradient body)
    # Golden border frame (8px)
    draw.rounded_rectangle([bx0, by0, bx1, by1], radius=radius, fill=(255, 225, 45, 255))
    draw.rounded_rectangle([bx0 + 9, by0 + 9, bx1 - 9, by1 - 9], radius=radius - 7, fill=(20, 115, 45, 255))
    
    # Face Gradient (Spring Lime -> Radiant Emerald)
    face_mask = Image.new("L", (BW, BH), 0)
    fm_draw = ImageDraw.Draw(face_mask)
    fm_draw.rounded_rectangle([bx0 + 14, by0 + 14, bx1 - 14, by1 - 14], radius=radius - 11, fill=255)
    
    face_grad = create_gradient_fill(BW, BH, (85, 235, 75, 255), (28, 165, 52, 255))
    canvas.paste(face_grad, (0, 0), face_mask)
    
    # 3. Top Curved Glossy Reflection
    gloss_mask = Image.new("L", (BW, BH), 0)
    gm_draw = ImageDraw.Draw(gloss_mask)
    gm_draw.rounded_rectangle([bx0 + 18, by0 + 16, bx1 - 18, by0 + int(h_btn * 0.44)], radius=radius - 15, fill=160)
    white_gloss = Image.new("RGBA", (BW, BH), (255, 255, 255, 175))
    canvas.paste(white_gloss, (0, 0), gloss_mask)
    
    # 4. Perfectly Centered Play Triangle & "PLAY" Text Lockup
    font_play = ImageFont.truetype(FONT_COOPER, 145)
    text_play = "PLAY"
    
    t_bbox = draw.textbbox((0, 0), text_play, font=font_play)
    tw = t_bbox[2] - t_bbox[0]
    th = t_bbox[3] - t_bbox[1]
    
    tri_w = 90
    gap = 45
    total_lockup_w = tri_w + gap + tw
    
    start_x = bx0 + (w_btn - total_lockup_w) // 2
    
    # Triangle center
    tri_x = start_x + 35
    tri_y = by0 + h_btn // 2 - 4
    tri_size = 54
    
    tri_pts = [
        (tri_x - tri_size * 0.7, tri_y - tri_size),
        (tri_x - tri_size * 0.7, tri_y + tri_size),
        (tri_x + tri_size * 1.1, tri_y)
    ]
    
    # Triangle 3D Shadow
    t_shadow_offset = 8
    draw.polygon([(x, y + t_shadow_offset) for x, y in tri_pts], fill=(0, 0, 0, 255))
    
    # Triangle Black Outline
    for dx in range(-10, 11, 2):
        for dy in range(-10, 11, 2):
            if dx*dx + dy*dy <= 100:
                draw.polygon([(x + dx, y + dy) for x, y in tri_pts], fill=(0, 0, 0, 255))
                
    # Triangle Golden Rim
    for dx in range(-4, 5, 2):
        for dy in range(-4, 5, 2):
            draw.polygon([(x + dx, y + dy) for x, y in tri_pts], fill=(255, 230, 80, 255))
            
    # Triangle Pure White Face
    draw.polygon(tri_pts, fill=(255, 255, 255, 255))
    # Triangle top shine
    tri_shine = [
        (tri_x - tri_size * 0.65, tri_y - tri_size * 0.9),
        (tri_x - tri_size * 0.65, tri_y),
        (tri_x + tri_size * 0.6, tri_y - tri_size * 0.2)
    ]
    draw.polygon(tri_shine, fill=(240, 255, 250, 255))
    
    # "PLAY" Text
    tx = start_x + tri_w + gap
    ty = by0 + (h_btn - th) // 2 - 20
    
    # 3D Shadow for "PLAY"
    play_shadow = 10
    for dy in range(1, play_shadow + 1):
        draw.text((tx, ty + dy), text_play, font=font_play, fill=(10, 60, 25, 255))
        
    # Black Outline for "PLAY"
    p_stroke = 14
    for dx in range(-p_stroke, p_stroke + 1, 2):
        for dy in range(-p_stroke, p_stroke + 1, 2):
            if dx*dx + dy*dy <= p_stroke*p_stroke:
                draw.text((tx + dx, ty + dy), text_play, font=font_play, fill=(0, 0, 0, 255))
                
    # Golden Inner Rim
    for dx in range(-4, 5, 2):
        for dy in range(-4, 5, 2):
            draw.text((tx + dx, ty + dy), text_play, font=font_play, fill=(255, 240, 150, 255))
            
    # White-to-Cream Face Fill
    p_mask = Image.new("L", (BW, BH), 0)
    pm_draw = ImageDraw.Draw(p_mask)
    pm_draw.text((tx, ty), text_play, font=font_play, fill=255)
    
    p_grad = create_gradient_fill(BW, BH, (255, 255, 255, 255), (230, 255, 235, 255))
    canvas.paste(p_grad, (0, 0), p_mask)
    
    # Downscale with Lanczos for smooth edges
    final_img = canvas.resize((BW // 2, BH // 2), Image.Resampling.LANCZOS)
    return final_img

# Generate and save both images
title_img = create_title_logo()
play_img = create_play_button()

# Save to Assets/Resources/Art/Sprites and Assets/Art/Sprites
paths = [
    ("Assets/Resources/Art/Sprites/title_logo.png", title_img),
    ("Assets/Art/Sprites/title_logo.png", title_img),
    ("Assets/Resources/Art/Sprites/btn_big_play.png", play_img),
    ("Assets/Art/Sprites/btn_big_play.png", play_img)
]

for p, img in paths:
    os.makedirs(os.path.dirname(p), exist_ok=True)
    img.save(p, "PNG")
    print(f"Saved: {p} ({img.size})")

print("PREMIUM_UI_ASSETS_CREATED_SUCCESSFULLY")

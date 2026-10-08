import os
import math
from PIL import Image, ImageDraw, ImageFont, ImageFilter, ImageOps

FONT_COOPER = "C:/Windows/Fonts/COOPBL.TTF"
if not os.path.exists(FONT_COOPER):
    FONT_COOPER = "C:/Windows/Fonts/ARLRDBD.TTF"

def get_gradient_color(c1, c2, t):
    return (
        int(c1[0] + (c2[0] - c1[0]) * t),
        int(c1[1] + (c2[1] - c1[1]) * t),
        int(c1[2] + (c2[2] - c1[2]) * t),
        int(c1[3] + (c2[3] - c1[3]) * t) if len(c1) > 3 else 255
    )

def create_letter_image(char, font, fill_top, fill_bot, inner_rim_color=None, extrusion_color=(20, 20, 40, 255), extrude_depth=16):
    # Render a single high-quality 3D cartoon letter
    # 1. Measure character
    dummy = Image.new("RGBA", (10, 10))
    d_draw = ImageDraw.Draw(dummy)
    bbox = d_draw.textbbox((0, 0), char, font=font)
    bw = bbox[2] - bbox[0]
    bh = bbox[3] - bbox[1]
    
    pad = 40
    lw = bw + pad * 2
    lh = bh + pad * 2 + extrude_depth
    
    # Coordinates of origin for char
    ox = pad - bbox[0]
    oy = pad - bbox[1]
    
    layer = Image.new("RGBA", (lw, lh), (0, 0, 0, 0))
    draw = ImageDraw.Draw(layer)
    
    # 1. 3D Extrusion
    for d in range(extrude_depth, 0, -1):
        ratio = d / extrude_depth
        ext_col = get_gradient_color(extrusion_color, (0, 0, 0, 255), ratio * 0.4)
        draw.text((ox, oy + d), char, font=font, fill=ext_col)
        
    # 2. Outer stroke for the face
    stroke_w = 6
    for dx in range(-stroke_w, stroke_w + 1, 2):
        for dy in range(-stroke_w, stroke_w + 1, 2):
            if dx*dx + dy*dy <= stroke_w * stroke_w:
                draw.text((ox + dx, oy + dy), char, font=font, fill=extrusion_color)
                
    # 3. Inner bevel rim
    if inner_rim_color:
        rim_w = 3
        for dx in range(-rim_w, rim_w + 1):
            for dy in range(-rim_w, rim_w + 1):
                draw.text((ox + dx, oy + dy), char, font=font, fill=inner_rim_color)
                
    # 4. Face gradient mask
    mask = Image.new("L", (lw, lh), 0)
    m_draw = ImageDraw.Draw(mask)
    m_draw.text((ox, oy), char, font=font, fill=255)
    
    # Gradient for face
    face_grad = Image.new("RGBA", (lw, lh), (0, 0, 0, 0))
    for y in range(lh):
        t = max(0.0, min(1.0, (y - pad) / max(1, bh)))
        col = get_gradient_color(fill_top, fill_bot, t)
        for x in range(lw):
            face_grad.putpixel((x, y), col)
            
    layer.paste(face_grad, (0, 0), mask)
    
    # 5. Top Specular Gloss Highlight
    gloss_mask = Image.new("L", (lw, lh), 0)
    g_draw = ImageDraw.Draw(gloss_mask)
    # Top curved ellipse mask intersecting with character
    g_draw.ellipse([ox - 10, oy - 15, ox + bw + 10, oy + int(bh * 0.48)], fill=180)
    # Intersect with text mask
    final_gloss_mask = Image.new("L", (lw, lh), 0)
    for y in range(oy, oy + int(bh * 0.52)):
        for x in range(ox, ox + bw):
            if mask.getpixel((x, y)) > 150 and gloss_mask.getpixel((x, y)) > 0:
                final_gloss_mask.putpixel((x, y), 150)
                
    gloss_img = Image.new("RGBA", (lw, lh), (255, 255, 255, 200))
    layer.paste(gloss_img, (0, 0), final_gloss_mask)
    
    return layer, bw, bh, ox, oy

def draw_star(draw, cx, cy, r_out, r_in, fill_col, outline_col=(0, 0, 0, 255), outline_w=6):
    points = []
    for i in range(10):
        angle = -math.pi / 2 + i * (math.pi / 5)
        r = r_out if i % 2 == 0 else r_in
        points.append((cx + r * math.cos(angle), cy + r * math.sin(angle)))
    if outline_w > 0:
        draw.polygon(points, fill=outline_col)
        inner = []
        for i in range(10):
            angle = -math.pi / 2 + i * (math.pi / 5)
            r = (r_out - outline_w) if i % 2 == 0 else max(1, r_in - outline_w)
            inner.append((cx + r * math.cos(angle), cy + r * math.sin(angle)))
        draw.polygon(inner, fill=fill_col)
    else:
        draw.polygon(points, fill=fill_col)

def draw_bubble(draw, cx, cy, radius):
    draw.ellipse([cx - radius, cy - radius, cx + radius, cy + radius], outline=(5, 35, 75, 230), width=4)
    draw.ellipse([cx - radius + 2, cy - radius + 2, cx + radius - 2, cy + radius - 2], fill=(160, 240, 255, 80), outline=(230, 255, 255, 230), width=3)
    hr = radius * 0.35
    hx = cx - radius * 0.35
    hy = cy - radius * 0.35
    draw.ellipse([hx - hr, hy - hr * 0.6, hx + hr, hy + hr * 0.6], fill=(255, 255, 255, 230))
    sr = radius * 0.15
    sx = cx + radius * 0.4
    sy = cy + radius * 0.4
    draw.ellipse([sx - sr, sy - sr, sx + sr, sy + sr], fill=(255, 255, 255, 170))

def build_deluxe_title():
    CW, CH = 2200, 850
    canvas = Image.new("RGBA", (CW, CH), (0, 0, 0, 0))
    
    font_deep = ImageFont.truetype(FONT_COOPER, 145)
    font_toy = ImageFont.truetype(FONT_COOPER, 190)
    
    text_deep = "DEEP SEA"
    text_toy = "TOY PATROL"
    
    # ----------------------------------------------------
    # Line 1: "DEEP SEA" (Arched tropical cyan / aquamarine)
    # ----------------------------------------------------
    deep_letters = []
    deep_total_w = 0
    for ch in text_deep:
        if ch == " ":
            deep_letters.append((None, 45, 0, None))
            deep_total_w += 45
        else:
            img, bw, bh, ox, oy = create_letter_image(
                ch, font_deep,
                fill_top=(140, 250, 255, 255),
                fill_bot=(0, 145, 240, 255),
                inner_rim_color=(230, 255, 255, 255),
                extrusion_color=(4, 45, 95, 255),
                extrude_depth=20
            )
            deep_letters.append((img, bw, bh, (ox, oy)))
            deep_total_w += bw - 8  # slight kerning overlap
            
    # Position DEEP SEA with gentle arc
    start_x_deep = (CW - deep_total_w) // 2
    cur_x = start_x_deep
    base_y_deep = 110
    
    deep_positions = []
    n_deep = len(text_deep)
    for i, (l_img, bw, bh, orig) in enumerate(deep_letters):
        # Arc offset: peak at center
        rel_pos = (i - n_deep / 2.0) / (n_deep / 2.0)
        arc_y = int((rel_pos * rel_pos) * 22)  # curves upward in the middle
        deep_positions.append((cur_x, base_y_deep + arc_y, l_img, orig))
        cur_x += (bw - 8) if l_img else bw

    # ----------------------------------------------------
    # Line 2: "TOY PATROL" (Big Bold Radiant Golden Sunburst)
    # ----------------------------------------------------
    toy_letters = []
    toy_total_w = 0
    for ch in text_toy:
        if ch == " ":
            toy_letters.append((None, 55, 0, None))
            toy_total_w += 55
        else:
            img, bw, bh, ox, oy = create_letter_image(
                ch, font_toy,
                fill_top=(255, 248, 80, 255),
                fill_bot=(255, 115, 0, 255),
                inner_rim_color=(255, 255, 210, 255),
                extrusion_color=(135, 45, 0, 255),
                extrude_depth=26
            )
            toy_letters.append((img, bw, bh, (ox, oy)))
            toy_total_w += bw - 10 # playful overlap
            
    start_x_toy = (CW - toy_total_w) // 2
    cur_x = start_x_toy
    base_y_toy = 300
    
    toy_positions = []
    n_toy = len(text_toy)
    for i, (l_img, bw, bh, orig) in enumerate(toy_letters):
        rel_pos = (i - n_toy / 2.0) / (n_toy / 2.0)
        arc_y = int((rel_pos * rel_pos) * 16)
        toy_positions.append((cur_x, base_y_toy + arc_y, l_img, orig))
        cur_x += (bw - 10) if l_img else bw

    # ----------------------------------------------------
    # Master Silhouette Outline Mask
    # Create unified solid cartoon outline around ALL letters
    # ----------------------------------------------------
    solid_mask = Image.new("L", (CW, CH), 0)
    for x, y, img, orig in deep_positions + toy_positions:
        if img:
            ox, oy = orig
            # Paste alpha channel of each letter image
            a_chan = img.split()[3]
            solid_mask.paste(a_chan, (x - ox, y - oy), a_chan)
            
    # Expand silhouette to create ultra-smooth thick comic boundary
    outline_img = Image.new("RGBA", (CW, CH), (0, 0, 0, 0))
    outline_draw = ImageDraw.Draw(outline_img)
    
    # 1. Soft Drop Shadow below the whole logo
    shadow_mask = solid_mask.filter(ImageFilter.GaussianBlur(14))
    shadow_layer = Image.new("RGBA", (CW, CH), (3, 20, 50, 140))
    canvas.paste(shadow_layer, (0, 22), shadow_mask)
    
    # 2. Bold Black Comic Contour (radius 18)
    contour_mask = solid_mask.filter(ImageFilter.MaxFilter(27))
    contour_layer = Image.new("RGBA", (CW, CH), (0, 0, 0, 255))
    canvas.paste(contour_layer, (0, 0), contour_mask)

    # 3. Inner Oceanic Cyan / Gold Accent Glow Outline (radius 14)
    accent_mask = solid_mask.filter(ImageFilter.MaxFilter(15))
    # Gradient accent for outline (Cyan on top, Gold on bottom)
    grad_contour = Image.new("RGBA", (CW, CH), (0, 0, 0, 0))
    for y in range(CH):
        t = y / CH
        col = get_gradient_color((180, 245, 255, 255), (255, 220, 100, 255), t)
        for x in range(CW):
            grad_contour.putpixel((x, y), col)
    canvas.paste(grad_contour, (0, 0), accent_mask)
    
    # Re-apply tight black outline directly under letters (radius 8)
    tight_black = solid_mask.filter(ImageFilter.MaxFilter(9))
    canvas.paste(contour_layer, (0, 0), tight_black)

    # 4. Paste All 3D Letters
    for x, y, img, orig in deep_positions:
        if img:
            ox, oy = orig
            canvas.paste(img, (x - ox, y - oy), img)
            
    for x, y, img, orig in toy_positions:
        if img:
            ox, oy = orig
            canvas.paste(img, (x - ox, y - oy), img)

    # ----------------------------------------------------
    # 5. Playful Deep Sea Accents: Starfish, Stars, Bubbles
    # ----------------------------------------------------
    decor_draw = ImageDraw.Draw(canvas)
    
    # Big Golden Captain Star at the very top center crown
    star_top_cx = CW // 2
    star_top_cy = base_y_deep - 25
    draw_star(decor_draw, star_top_cx, star_top_cy, 68, 30, (255, 225, 30, 255), (0, 0, 0, 255), outline_w=10)
    draw_star(decor_draw, star_top_cx, star_top_cy, 52, 22, (255, 255, 140, 255), (0, 0, 0, 0), outline_w=0)
    # Star badge inner dot
    decor_draw.ellipse([star_top_cx - 10, star_top_cy - 10, star_top_cx + 10, star_top_cy + 10], fill=(0, 110, 220, 255))
    
    # Cheerful Starfish anchors on left and right of "TOY PATROL"
    # Left starfish
    draw_star(decor_draw, start_x_toy - 65, base_y_toy + 130, 48, 20, (255, 140, 40, 255), (0, 0, 0, 255), outline_w=8)
    draw_star(decor_draw, start_x_toy - 65, base_y_toy + 130, 34, 14, (255, 205, 100, 255), (0, 0, 0, 0), outline_w=0)
    # Cute eye dots
    decor_draw.ellipse([start_x_toy - 73, base_y_toy + 124, start_x_toy - 69, base_y_toy + 130], fill=(0, 0, 0, 255))
    decor_draw.ellipse([start_x_toy - 61, base_y_toy + 124, start_x_toy - 57, base_y_toy + 130], fill=(0, 0, 0, 255))
    
    # Right starfish
    draw_star(decor_draw, start_x_toy + toy_total_w + 65, base_y_toy + 130, 48, 20, (255, 140, 40, 255), (0, 0, 0, 255), outline_w=8)
    draw_star(decor_draw, start_x_toy + toy_total_w + 65, base_y_toy + 130, 34, 14, (255, 205, 100, 255), (0, 0, 0, 0), outline_w=0)
    # Cute eye dots
    decor_draw.ellipse([start_x_toy + toy_total_w + 57, base_y_toy + 124, start_x_toy + toy_total_w + 61, base_y_toy + 130], fill=(0, 0, 0, 255))
    decor_draw.ellipse([start_x_toy + toy_total_w + 69, base_y_toy + 124, start_x_toy + toy_total_w + 73, base_y_toy + 130], fill=(0, 0, 0, 255))

    # Flanking Sparkle Stars
    draw_star(decor_draw, start_x_deep - 70, base_y_deep + 70, 38, 16, (255, 235, 70, 255), (0, 0, 0, 255), outline_w=6)
    draw_star(decor_draw, start_x_deep + deep_total_w + 70, base_y_deep + 70, 38, 16, (255, 235, 70, 255), (0, 0, 0, 255), outline_w=6)
    
    # Beautiful Glossy Ocean Bubbles
    bubble_spots = [
        (start_x_deep - 140, base_y_deep + 30, 36),
        (start_x_deep - 170, base_y_deep + 110, 24),
        (start_x_deep - 110, base_y_deep + 140, 18),
        (start_x_deep + deep_total_w + 140, base_y_deep + 30, 36),
        (start_x_deep + deep_total_w + 170, base_y_deep + 110, 24),
        (start_x_deep + deep_total_w + 110, base_y_deep + 140, 18),
        (start_x_toy - 130, base_y_toy + 50, 30),
        (start_x_toy + toy_total_w + 130, base_y_toy + 50, 30),
        (CW // 2 - 280, base_y_toy - 20, 22),
        (CW // 2 + 280, base_y_toy - 20, 22),
    ]
    for bx, by, br in bubble_spots:
        draw_bubble(decor_draw, bx, by, br)

    # Crop tightly to content + modest margin
    alpha = canvas.split()[3]
    bbox_all = alpha.getbbox()
    if bbox_all:
        margin = 15
        crop_box = (
            max(0, bbox_all[0] - margin),
            max(0, bbox_all[1] - margin),
            min(CW, bbox_all[2] + margin),
            min(CH, bbox_all[3] + margin)
        )
        cropped = canvas.crop(crop_box)
    else:
        cropped = canvas

    # Downsample with Lanczos for crystal-clear smooth edges
    target_w = 1100
    ratio = target_w / cropped.width
    target_h = int(cropped.height * ratio)
    final_logo = cropped.resize((target_w, target_h), Image.Resampling.LANCZOS)
    return final_logo

if __name__ == "__main__":
    logo = build_deluxe_title()
    paths = [
        "Assets/Resources/Art/Sprites/title_logo.png",
        "Assets/Art/Sprites/title_logo.png"
    ]
    for p in paths:
        os.makedirs(os.path.dirname(p), exist_ok=True)
        logo.save(p, "PNG")
        print(f"Deluxe Title Logo Saved to: {p} ({logo.size})")

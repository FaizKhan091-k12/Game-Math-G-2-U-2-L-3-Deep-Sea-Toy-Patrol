import os
import math
from PIL import Image, ImageDraw, ImageFont, ImageFilter, ImageOps

FONT_ARIAL_ROUND = "C:/Windows/Fonts/ARLRDBD.TTF"
FONT_COOPER = "C:/Windows/Fonts/COOPBL.TTF"
FONT_IMPACT = "C:/Windows/Fonts/impact.ttf"
if not os.path.exists(FONT_COOPER):
    FONT_COOPER = FONT_ARIAL_ROUND

def create_gradient_rect(w, h, top_col, bot_col):
    img = Image.new("RGBA", (w, h))
    for y in range(h):
        t = y / max(1, h - 1)
        r = int(top_col[0] + (bot_col[0] - top_col[0]) * t)
        g = int(top_col[1] + (bot_col[1] - top_col[1]) * t)
        b = int(top_col[2] + (bot_col[2] - top_col[2]) * t)
        a = int(top_col[3] + (bot_col[3] - top_col[3]) * t) if len(top_col) > 3 else 255
        for x in range(w):
            img.putpixel((x, y), (r, g, b, a))
    return img

def draw_star(draw, cx, cy, r_out, r_in, fill_col, outline_col=None, outline_w=0):
    points = []
    for i in range(10):
        angle = -math.pi / 2 + i * (math.pi / 5)
        r = r_out if i % 2 == 0 else r_in
        points.append((cx + r * math.cos(angle), cy + r * math.sin(angle)))
    if outline_col and outline_w > 0:
        draw.polygon(points, fill=outline_col)
        inner = []
        for i in range(10):
            angle = -math.pi / 2 + i * (math.pi / 5)
            r = max(1, (r_out - outline_w) if i % 2 == 0 else (r_in - outline_w))
            inner.append((cx + r * math.cos(angle), cy + r * math.sin(angle)))
        draw.polygon(inner, fill=fill_col)
    else:
        draw.polygon(points, fill=fill_col)

def draw_bubble(draw, cx, cy, radius):
    draw.ellipse([cx - radius, cy - radius, cx + radius, cy + radius], outline=(10, 40, 80, 240), width=4)
    draw.ellipse([cx - radius + 3, cy - radius + 3, cx + radius - 3, cy + radius - 3], fill=(180, 240, 255, 70), outline=(220, 250, 255, 230), width=3)
    hr = radius * 0.35
    draw.ellipse([cx - radius*0.35 - hr, cy - radius*0.35 - hr*0.6, cx - radius*0.35 + hr, cy - radius*0.35 + hr*0.6], fill=(255, 255, 255, 240))

def generate_nautical_shield_title():
    # Supersampling 2x
    W, H = 1920, 820
    canvas = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    draw = ImageDraw.Draw(canvas)
    
    cx = W // 2
    
    # ---------------------------------------------------------
    # 1. SHIP WHEEL HANDLES PEEKING FROM BEHIND THE EMBLEM
    # ---------------------------------------------------------
    spoke_angles = [-150, -120, -90, -60, -30]
    wheel_center = (cx, 440)
    spoke_len = 310
    for deg in spoke_angles:
        rad = math.radians(deg)
        sx = wheel_center[0] + spoke_len * math.cos(rad)
        sy = wheel_center[1] + spoke_len * math.sin(rad)
        # Draw wooden turned handle spoke
        # Black outline
        draw.line([wheel_center[0], wheel_center[1], sx, sy], fill=(0, 0, 0, 255), width=36)
        draw.ellipse([sx - 28, sy - 28, sx + 28, sy + 28], fill=(0, 0, 0, 255))
        # Warm wood fill
        draw.line([wheel_center[0], wheel_center[1], sx, sy], fill=(195, 125, 45, 255), width=24)
        draw.ellipse([sx - 20, sy - 20, sx + 20, sy + 20], fill=(245, 175, 75, 255), outline=(130, 70, 20, 255), width=4)
        # Gold knob cap
        draw.ellipse([sx - 12, sy - 12, sx + 12, sy + 12], fill=(255, 235, 100, 255))

    # ---------------------------------------------------------
    # 2. MAIN WOODEN / NAUTICAL SHIELD / PLAQUE (LOWER SECTION)
    # ---------------------------------------------------------
    plaque_w, plaque_h = 1420, 310
    px0 = cx - plaque_w // 2
    py0 = 360
    px1 = px0 + plaque_w
    py1 = py0 + plaque_h
    prad = 55
    
    # 3D Shadow Base
    depth = 28
    draw.rounded_rectangle([px0 - 18, py0 - 18 + depth, px1 + 18, py1 + 18 + depth], radius=prad + 18, fill=(0, 0, 0, 255))
    draw.rounded_rectangle([px0 - 18, py0 - 18, px1 + 18, py1 + 18], radius=prad + 18, fill=(0, 0, 0, 255))
    
    # Dark bronze base rim
    draw.rounded_rectangle([px0, py0 + depth, px1, py1 + depth], radius=prad, fill=(100, 50, 10, 255))
    
    # Golden Metallic Outer Border
    draw.rounded_rectangle([px0, py0, px1, py1], radius=prad, fill=(255, 215, 40, 255))
    draw.rounded_rectangle([px0 + 8, py0 + 8, px1 - 8, py1 - 8], radius=prad - 6, fill=(160, 95, 15, 255))
    
    # Plaque Face: Deep Marine Wooden Plank / Cyan Oceanic Slate
    plaque_mask = Image.new("L", (W, H), 0)
    pm_draw = ImageDraw.Draw(plaque_mask)
    pm_draw.rounded_rectangle([px0 + 14, py0 + 14, px1 - 14, py1 - 14], radius=prad - 10, fill=255)
    
    plaque_grad = create_gradient_rect(W, H, (15, 65, 125, 255), (6, 32, 70, 255))
    canvas.paste(plaque_grad, (0, 0), plaque_mask)
    
    # Plaque inner gold pinstripe
    draw.rounded_rectangle([px0 + 20, py0 + 20, px1 - 20, py1 - 20], radius=prad - 14, outline=(70, 160, 230, 200), width=4)

    # Rivets / Bolts on Plaque Corners
    rivet_pts = [
        (px0 + 45, py0 + 50), (px1 - 45, py0 + 50),
        (px0 + 45, py1 - 50), (px1 - 45, py1 - 50),
        (px0 + 75, py0 + 155), (px1 - 75, py0 + 155)
    ]
    for rx, ry in rivet_pts:
        draw.ellipse([rx - 16, ry - 16, rx + 16, ry + 16], fill=(0, 0, 0, 255))
        draw.ellipse([rx - 12, ry - 12, rx + 12, ry + 12], fill=(255, 215, 50, 255), outline=(150, 90, 10, 255), width=3)
        draw.ellipse([rx - 5, ry - 7, rx + 5, ry - 1], fill=(255, 255, 220, 255))

    # ---------------------------------------------------------
    # 3. TOP ARCHED RIBBON BANNER ("DEEP SEA")
    # ---------------------------------------------------------
    rw, rh = 1040, 190
    rx0 = cx - rw // 2
    ry0 = 150
    rx1 = rx0 + rw
    ry1 = ry0 + rh
    r_radius = 50
    
    # Ribbon Tails (Folded sides)
    # Left tail
    lt_pts = [(rx0 - 60, ry0 + 40), (rx0 + 40, ry0 + 15), (rx0 + 40, ry1 - 15), (rx0 - 60, ry1 + 25), (rx0 - 30, ry0 + rh//2 + 5)]
    draw.polygon(lt_pts, fill=(0, 0, 0, 255))
    # Left tail face
    lt_face = [(rx0 - 50, ry0 + 45), (rx0 + 35, ry0 + 23), (rx0 + 35, ry1 - 23), (rx0 - 50, ry1 + 17), (rx0 - 24, ry0 + rh//2 + 5)]
    draw.polygon(lt_face, fill=(0, 130, 200, 255))
    # Right tail
    rt_pts = [(rx1 + 60, ry0 + 40), (rx1 - 40, ry0 + 15), (rx1 - 40, ry1 - 15), (rx1 + 60, ry1 + 25), (rx1 + 30, ry0 + rh//2 + 5)]
    draw.polygon(rt_pts, fill=(0, 0, 0, 255))
    # Right tail face
    rt_face = [(rx1 + 50, ry0 + 45), (rx1 - 35, ry0 + 23), (rx1 - 35, ry1 - 23), (rx1 + 50, ry1 + 17), (rx1 + 24, ry0 + rh//2 + 5)]
    draw.polygon(rt_face, fill=(0, 130, 200, 255))

    # Ribbon Main Body Shadow
    draw.rounded_rectangle([rx0 - 14, ry0 - 14 + 18, rx1 + 14, ry1 + 14 + 18], radius=r_radius + 14, fill=(0, 0, 0, 255))
    draw.rounded_rectangle([rx0 - 14, ry0 - 14, rx1 + 14, ry1 + 14], radius=r_radius + 14, fill=(0, 0, 0, 255))
    
    # Ribbon Gold Border
    draw.rounded_rectangle([rx0, ry0, rx1, ry1], radius=r_radius, fill=(255, 230, 60, 255))
    draw.rounded_rectangle([rx0 + 7, ry0 + 7, rx1 - 7, ry1 - 7], radius=r_radius - 5, fill=(180, 110, 15, 255))
    
    # Ribbon Face Gradient (Vibrant Marine Aqua -> Royal Blue)
    ribbon_mask = Image.new("L", (W, H), 0)
    rm_draw = ImageDraw.Draw(ribbon_mask)
    rm_draw.rounded_rectangle([rx0 + 12, ry0 + 12, rx1 - 12, ry1 - 12], radius=r_radius - 8, fill=255)
    
    ribbon_grad = create_gradient_rect(W, H, (30, 215, 255, 255), (5, 120, 220, 255))
    canvas.paste(ribbon_grad, (0, 0), ribbon_mask)
    
    # Top curved gloss on ribbon
    rg_mask = Image.new("L", (W, H), 0)
    rg_draw = ImageDraw.Draw(rg_mask)
    rg_draw.rounded_rectangle([rx0 + 16, ry0 + 14, rx1 - 16, ry0 + int(rh * 0.42)], radius=r_radius - 10, fill=150)
    white_gloss = Image.new("RGBA", (W, H), (255, 255, 255, 180))
    canvas.paste(white_gloss, (0, 0), rg_mask)

    # ---------------------------------------------------------
    # 4. TYPOGRAPHY: "DEEP SEA" (Inside Top Ribbon)
    # ---------------------------------------------------------
    font_deep = ImageFont.truetype(FONT_COOPER, 125)
    text_deep = "DEEP SEA"
    
    bbox_d = draw.textbbox((0, 0), text_deep, font=font_deep)
    tw_d = bbox_d[2] - bbox_d[0]
    th_d = bbox_d[3] - bbox_d[1]
    
    tx_d = cx - tw_d // 2
    ty_d = ry0 + (rh - th_d) // 2 - 16
    
    # 3D shadow for DEEP SEA
    for dy in range(1, 14):
        draw.text((tx_d, ty_d + dy), text_deep, font=font_deep, fill=(2, 35, 75, 255))
        
    # Black stroke
    for dx in range(-12, 13, 2):
        for dy in range(-12, 13, 2):
            if dx*dx + dy*dy <= 144:
                draw.text((tx_d + dx, ty_d + dy), text_deep, font=font_deep, fill=(0, 0, 0, 255))
                
    # Gold bevel rim
    for dx in range(-4, 5, 2):
        for dy in range(-4, 5, 2):
            draw.text((tx_d + dx, ty_d + dy), text_deep, font=font_deep, fill=(255, 235, 120, 255))
            
    # Pure White-to-Ice Face
    d_mask = Image.new("L", (W, H), 0)
    dm_draw = ImageDraw.Draw(d_mask)
    dm_draw.text((tx_d, ty_d), text_deep, font=font_deep, fill=255)
    d_grad = create_gradient_rect(W, H, (255, 255, 255, 255), (210, 245, 255, 255))
    canvas.paste(d_grad, (0, 0), d_mask)

    # ---------------------------------------------------------
    # 5. TYPOGRAPHY: "TOY PATROL" (Inside Main Plaque)
    # ---------------------------------------------------------
    font_toy = ImageFont.truetype(FONT_COOPER, 180)
    text_toy = "TOY PATROL"
    
    bbox_t = draw.textbbox((0, 0), text_toy, font=font_toy)
    tw_t = bbox_t[2] - bbox_t[0]
    th_t = bbox_t[3] - bbox_t[1]
    
    tx_t = cx - tw_t // 2
    ty_t = py0 + (plaque_h - th_t) // 2 - 25
    
    # 3D Bottom Extrusion Shadow
    for dy in range(20, 0, -2):
        for dx in range(-16, 17, 3):
            for ddy in range(-16, 17, 3):
                draw.text((tx_t + dx, ty_t + dy + ddy), text_toy, font=font_toy, fill=(0, 0, 0, 255))
                
    for dy in range(1, 20):
        draw.text((tx_t, ty_t + dy), text_toy, font=font_toy, fill=(140, 55, 0, 255))
        
    # Black outer stroke
    for dx in range(-16, 17, 2):
        for dy in range(-16, 17, 2):
            if dx*dx + dy*dy <= 256:
                draw.text((tx_t + dx, ty_t + dy), text_toy, font=font_toy, fill=(0, 0, 0, 255))
                
    # Warm cream inner bevel
    for dx in range(-5, 6, 2):
        for dy in range(-5, 6, 2):
            draw.text((tx_t + dx, ty_t + dy), text_toy, font=font_toy, fill=(255, 245, 170, 255))
            
    # Brilliant Sunburst Gold Gradient Fill
    t_mask = Image.new("L", (W, H), 0)
    tm_draw = ImageDraw.Draw(t_mask)
    tm_draw.text((tx_t, ty_t), text_toy, font=font_toy, fill=255)
    t_grad = create_gradient_rect(W, H, (255, 235, 40, 255), (255, 105, 0, 255))
    canvas.paste(t_grad, (0, 0), t_mask)
    
    # Top gloss shine on TOY PATROL
    t_gloss = Image.new("L", (W, H), 0)
    for px in range(tx_t - 10, tx_t + tw_t + 10, 4):
        for py in range(ty_t, ty_t + int(th_t * 0.44), 4):
            if t_mask.getpixel((px, py)) > 0:
                t_gloss.putpixel((px, py), 160)
    canvas.paste(white_gloss, (0, 0), t_gloss)

    # ---------------------------------------------------------
    # 6. CREST CROWN: GOLDEN PATROL STAR BADGE AT TOP CENTER
    # ---------------------------------------------------------
    badge_cy = ry0 - 20
    # Gold Star Crest
    draw_star(draw, cx, badge_cy, 68, 30, (255, 220, 30, 255), outline_col=(0, 0, 0, 255), outline_w=10)
    draw_star(draw, cx, badge_cy, 54, 24, (255, 245, 100, 255), outline_col=(180, 110, 10, 255), outline_w=4)
    # Inner Anchor / Nautical Ring
    draw.ellipse([cx - 24, badge_cy - 24, cx + 24, badge_cy + 24], fill=(20, 80, 150, 255), outline=(0, 0, 0, 255), width=4)
    # Small inner star
    draw_star(draw, cx, badge_cy, 15, 7, (255, 235, 60, 255))

    # ---------------------------------------------------------
    # 7. CORNER ACCENTS: SEAFOAM BUBBLES & STARFISH
    # ---------------------------------------------------------
    # Cute smiling starfish on lower-left and lower-right corners
    draw_star(draw, px0 + 130, py1 - 25, 48, 22, (255, 125, 40, 255), outline_col=(0, 0, 0, 255), outline_w=8)
    draw_star(draw, px0 + 130, py1 - 25, 36, 16, (255, 165, 80, 255))
    # Eyes on left starfish
    draw.ellipse([px0 + 122, py1 - 32, px0 + 128, py1 - 24], fill=(0, 0, 0, 255))
    draw.ellipse([px0 + 134, py1 - 32, px0 + 140, py1 - 24], fill=(0, 0, 0, 255))
    draw.arc([px0 + 124, py1 - 28, px0 + 138, py1 - 18], start=20, end=160, fill=(0, 0, 0, 255), width=3)

    draw_star(draw, px1 - 130, py1 - 25, 48, 22, (255, 125, 40, 255), outline_col=(0, 0, 0, 255), outline_w=8)
    draw_star(draw, px1 - 130, py1 - 25, 36, 16, (255, 165, 80, 255))
    # Eyes on right starfish
    draw.ellipse([px1 - 138, py1 - 32, px1 - 132, py1 - 24], fill=(0, 0, 0, 255))
    draw.ellipse([px1 - 126, py1 - 32, px1 - 120, py1 - 24], fill=(0, 0, 0, 255))
    draw.arc([px1 - 136, py1 - 28, px1 - 122, py1 - 18], start=20, end=160, fill=(0, 0, 0, 255), width=3)

    # Bubbles around the crest
    bubble_list = [
        (rx0 - 15, ry0 - 15, 30),
        (rx0 - 55, ry0 + 50, 20),
        (rx1 + 15, ry0 - 15, 30),
        (rx1 + 55, ry0 + 50, 20),
        (px0 - 35, py1 - 40, 26),
        (px1 + 35, py1 - 40, 26),
    ]
    for bx, by, br in bubble_list:
        draw_bubble(draw, bx, by, br)

    # Downscale with Lanczos for smooth anti-aliased cartoon edges
    final_img = canvas.resize((W // 2, H // 2), Image.Resampling.LANCZOS)
    return final_img

# Execute generation
crest_img = generate_nautical_shield_title()

# Save
paths = [
    "Assets/Resources/Art/Sprites/title_logo.png",
    "Assets/Art/Sprites/title_logo.png"
]
for p in paths:
    os.makedirs(os.path.dirname(p), exist_ok=True)
    crest_img.save(p, "PNG")
    print(f"Saved: {p} ({crest_img.size})")

print("NAUTICAL_CREST_TITLE_GENERATED_SUCCESSFULLY")

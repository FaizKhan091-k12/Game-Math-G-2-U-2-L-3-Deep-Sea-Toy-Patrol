import os
import math
import uuid
from PIL import Image, ImageDraw, ImageFont, ImageFilter

out_dirs = [
    r"Assets\Art\Sprites",
    r"Assets\Resources\Art\Sprites"
]
for d in out_dirs:
    os.makedirs(d, exist_ok=True)

# -------------------------------------------------------------
# 1. TITLE LOGO IMAGE ("DEEP SEA TOY PATROL")
# -------------------------------------------------------------
w, h = 1000, 320
img = Image.new("RGBA", (w, h), (0, 0, 0, 0))

# Try font
font_path = r"C:\Windows\Fonts\impact.ttf"
if not os.path.exists(font_path):
    font_path = r"C:\Windows\Fonts\arialbd.ttf"

font_top = ImageFont.truetype(font_path, 88)
font_bot = ImageFont.truetype(font_path, 98)

# We want 3D extruded cartoon text with thick black outline
def draw_3d_text(canvas, text, font, center_x, center_y, fill_top, fill_bot, stroke_color=(15, 15, 20, 255), stroke_width=9, depth=10):
    bbox = font.getbbox(text)
    tw = bbox[2] - bbox[0]
    th = bbox[3] - bbox[1]
    x = center_x - tw // 2
    y = center_y - th // 2 - bbox[1]

    # Draw 3D shadow extrusion
    for d in range(depth, 0, -1):
        shadow_draw = ImageDraw.Draw(canvas)
        shadow_draw.text((x, y + d), text, font=font, fill=(10, 15, 25, 255), stroke_width=stroke_width, stroke_fill=stroke_color)

    # Base black outline
    draw = ImageDraw.Draw(canvas)
    draw.text((x, y), text, font=font, fill=fill_top, stroke_width=stroke_width, stroke_fill=stroke_color)

    # Inner gradient / highlight
    # Render text to temp mask to blend top & bot color
    mask = Image.new("L", (w, h), 0)
    m_draw = ImageDraw.Draw(mask)
    m_draw.text((x, y), text, font=font, fill=255)

    grad = Image.new("RGBA", (w, h), fill_bot)
    grad_draw = ImageDraw.Draw(grad)
    for gy in range(y, y + th):
        factor = (gy - y) / max(1, th)
        r = int(fill_top[0] * (1 - factor) + fill_bot[0] * factor)
        g = int(fill_top[1] * (1 - factor) + fill_bot[1] * factor)
        b = int(fill_top[2] * (1 - factor) + fill_bot[2] * factor)
        grad_draw.line([(x - 20, gy), (x + tw + 20, gy)], fill=(r, g, b, 255))

    # Paste gradient onto text inside stroke
    canvas.paste(grad, (0, 0), mask)

    # Top glint highlight (top 30% of letters)
    glint_mask = Image.new("L", (w, h), 0)
    g_draw = ImageDraw.Draw(glint_mask)
    g_draw.text((x, y), text, font=font, fill=255)
    # cut bottom
    g_draw.rectangle([(0, y + int(th * 0.38)), (w, h)], fill=0)
    white_glint = Image.new("RGBA", (w, h), (255, 255, 255, 140))
    canvas.paste(white_glint, (0, 0), glint_mask)

# Draw Line 1: DEEP SEA (Aqua Cyan Gradient)
draw_3d_text(
    img, "DEEP SEA", font_top,
    center_x=w // 2, center_y=85,
    fill_top=(0, 240, 255), fill_bot=(0, 130, 230),
    stroke_color=(10, 20, 35, 255), stroke_width=10, depth=8
)

# Draw Line 2: TOY PATROL (Golden Yellow Gradient)
draw_3d_text(
    img, "TOY PATROL", font_bot,
    center_x=w // 2, center_y=205,
    fill_top=(255, 235, 50), fill_bot=(255, 120, 0),
    stroke_color=(25, 15, 5, 255), stroke_width=11, depth=10
)

# Decorative bubbles on title
d = ImageDraw.Draw(img)
for bx, by, br in [(85, 90, 26), (135, 60, 16), (885, 80, 24), (935, 115, 17), (70, 210, 20), (920, 220, 22)]:
    d.ellipse((bx - br, by - br, bx + br, by + br), fill=(255, 255, 255, 70), outline=(20, 20, 25, 255), width=3)
    d.ellipse((bx - br * 0.6, by - br * 0.6, bx - br * 0.2, by - br * 0.2), fill=(255, 255, 255, 220))

# Small star on title
def draw_star(canvas, cx, cy, r_out, r_in, fill_col):
    pts = []
    for i in range(10):
        ang = math.radians(i * 36 - 90)
        r = r_out if i % 2 == 0 else r_in
        pts.append((cx + r * math.cos(ang), cy + r * math.sin(ang)))
    sd = ImageDraw.Draw(canvas)
    sd.polygon(pts, fill=fill_col, outline=(20, 20, 25, 255), width=3)

draw_star(img, 190, 195, 28, 12, (255, 220, 30, 255))
draw_star(img, 810, 195, 28, 12, (255, 220, 30, 255))

for out_dir in out_dirs:
    img.save(os.path.join(out_dir, "title_logo.png"))
print("Saved title_logo.png")

# -------------------------------------------------------------
# 2. SOLID OPAQUE DIALOGUE CONTAINER BACKGROUND
# -------------------------------------------------------------
# Solid, 100% opaque, rich dark ocean blue with golden border and black outline
pw, ph = 1450, 170
p_img = Image.new("RGBA", (pw, ph), (0, 0, 0, 0))
pd = ImageDraw.Draw(p_img)

# Outer black cartoon border
pd.rounded_rectangle((4, 4, pw - 4, ph - 4), radius=30, fill=(15, 15, 20, 255))
# Golden accent border
pd.rounded_rectangle((8, 8, pw - 8, ph - 8), radius=26, fill=(245, 185, 35, 255))
# Solid Dark Navy interior (100% OPAQUE)
pd.rounded_rectangle((14, 14, pw - 14, ph - 14), radius=22, fill=(10, 32, 60, 255))
# Soft top inner glow / highlight
pd.rounded_rectangle((18, 18, pw - 18, int(ph * 0.42)), radius=16, fill=(22, 55, 96, 255))

for out_dir in out_dirs:
    p_img.save(os.path.join(out_dir, "panel_solid_dialogue.png"))
print("Saved panel_solid_dialogue.png")

# -------------------------------------------------------------
# 3. BIG JUICY PLAY BUTTON
# -------------------------------------------------------------
bw, bh = 340, 130
b_img = Image.new("RGBA", (bw, bh), (0, 0, 0, 0))
bd = ImageDraw.Draw(b_img)

# 3D shadow bottom
bd.rounded_rectangle((6, 16, bw - 6, bh - 6), radius=36, fill=(10, 45, 20, 255))
# Black outline
bd.rounded_rectangle((6, 6, bw - 6, bh - 16), radius=36, fill=(15, 15, 20, 255))
# Bright emerald green border
bd.rounded_rectangle((10, 10, bw - 10, bh - 20), radius=32, fill=(35, 175, 75, 255))
# Vibrant lime-green gradient
for gy in range(14, bh - 24):
    factor = (gy - 14) / (bh - 38)
    r = int(80 * (1 - factor) + 30 * factor)
    g = int(225 * (1 - factor) + 160 * factor)
    b = int(70 * (1 - factor) + 40 * factor)
    bd.line([(24, gy), (bw - 24, gy)], fill=(r, g, b, 255), width=1)
# Top glossy reflection
bd.rounded_rectangle((20, 14, bw - 20, int((bh - 20) * 0.45)), radius=22, fill=(255, 255, 255, 90))

# Play icon triangle
poly = [(85, 32), (85, 88), (135, 60)]
bd.polygon(poly, fill=(255, 255, 255, 255), outline=(15, 15, 20, 255), width=4)

# "PLAY" Text
play_font = ImageFont.truetype(font_path, 62)
p_bbox = play_font.getbbox("PLAY")
ptw = p_bbox[2] - p_bbox[0]
pth = p_bbox[3] - p_bbox[1]
px = 155
py = 60 - pth // 2 - p_bbox[1]
bd.text((px, py + 4), "PLAY", font=play_font, fill=(15, 60, 25, 255))
bd.text((px, py), "PLAY", font=play_font, fill=(255, 255, 255, 255), stroke_width=4, stroke_fill=(15, 15, 20, 255))

for out_dir in out_dirs:
    b_img.save(os.path.join(out_dir, "btn_big_play.png"))
print("Saved btn_big_play.png")

# -------------------------------------------------------------
# 4. SOLID WHITE / CREAM QUESTION CARD BACKGROUND
# -------------------------------------------------------------
cw, ch = 500, 180
c_img = Image.new("RGBA", (cw, ch), (0, 0, 0, 0))
cd = ImageDraw.Draw(c_img)
# Black cartoon border
cd.rounded_rectangle((4, 4, cw - 4, ch - 4), radius=28, fill=(15, 15, 20, 255))
# Cyan/Blue border
cd.rounded_rectangle((8, 8, cw - 8, ch - 8), radius=24, fill=(0, 145, 234, 255))
# Solid Cream/White interior (100% OPAQUE)
cd.rounded_rectangle((14, 14, cw - 14, ch - 14), radius=20, fill=(250, 252, 255, 255))
# Subtle top highlight
cd.rounded_rectangle((18, 16, cw - 18, 55), radius=14, fill=(235, 245, 255, 255))

for out_dir in out_dirs:
    c_img.save(os.path.join(out_dir, "panel_solid_card.png"))
print("Saved panel_solid_card.png")

print("All UI and Title assets successfully created!")

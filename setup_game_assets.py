import os
import glob
import math
import struct
import wave
import uuid
from PIL import Image, ImageDraw, ImageFilter

brain_dir = r"C:\Users\jisau\.gemini\antigravity-ide\brain\e97d23ae-5b80-4ddd-8719-e341e9e126ff"
assets_dir = r"Assets"
sprites_dir = os.path.join(assets_dir, "Art", "Sprites")
audio_dir = os.path.join(assets_dir, "Audio")
scripts_dir = os.path.join(assets_dir, "Scripts")

os.makedirs(sprites_dir, exist_ok=True)
os.makedirs(audio_dir, exist_ok=True)
os.makedirs(scripts_dir, exist_ok=True)

# -------------------------------------------------------------
# GUID registry so references are consistent across Unity scene
# -------------------------------------------------------------
GUIDS = {
    "bg_deep_sea": "10000000000000000000000000000001",
    "captain_finn_idle": "10000000000000000000000000000002",
    "captain_finn_cheer": "10000000000000000000000000000003",
    "yellow_submarine": "10000000000000000000000000000004",
    "pink_octopus": "10000000000000000000000000000005",
    "tropical_fish": "10000000000000000000000000000006",
    "toy_red_car": "10000000000000000000000000000007",
    "toy_blue_boat": "10000000000000000000000000000008",
    "toy_green_plane": "10000000000000000000000000000009",
    "starfish_button": "10000000000000000000000000000010",
    "ship_wheel": "10000000000000000000000000000011",
    "gold_star_badge": "10000000000000000000000000000012",
    "rock_tablet": "10000000000000000000000000000013",
    "icon_wave": "10000000000000000000000000000014",
    "icon_home": "10000000000000000000000000000015",
    "icon_replay": "10000000000000000000000000000016",
    "icon_sound_on": "10000000000000000000000000000017",
    "icon_sound_off": "10000000000000000000000000000018",
    "icon_star": "10000000000000000000000000000019",
    "bubble_particle": "10000000000000000000000000000020",
    "laser_glow": "10000000000000000000000000000021",
    "tally_mark": "10000000000000000000000000000022",
    "tally_slash": "10000000000000000000000000000023",
    "wheel_needle": "10000000000000000000000000000024",

    # Audio
    "sfx_click": "20000000000000000000000000000001",
    "sfx_star_press": "20000000000000000000000000000002",
    "sfx_correct": "20000000000000000000000000000003",
    "sfx_wrong": "20000000000000000000000000000004",
    "sfx_laser": "20000000000000000000000000000005",
    "sfx_tally_hit": "20000000000000000000000000000006",
    "sfx_bundle_slash": "20000000000000000000000000000007",
    "sfx_wheel_tick": "20000000000000000000000000000008",
    "sfx_badge_drop": "20000000000000000000000000000009",
    "sfx_bubble": "20000000000000000000000000000010",
    "bgm_underwater": "20000000000000000000000000000011",
}

def remove_white_bg(img, tolerance=235):
    img = img.convert("RGBA")
    w, h = img.size
    
    # Identify candidate white pixels
    mask = Image.new("L", (w, h), 0)
    data = img.getdata()
    mask_data = []
    for r, g, b, a in data:
        if r >= tolerance and g >= tolerance and b >= tolerance:
            mask_data.append(255)
        else:
            mask_data.append(0)
    mask.putdata(mask_data)

    # Floodfill from borders
    seeds = []
    for x in range(0, w, 8):
        seeds.extend([(x, 0), (x, h - 1)])
    for y in range(0, h, 8):
        seeds.extend([(0, y), (w - 1, y)])
        
    for s in seeds:
        if mask.getpixel(s) == 255:
            ImageDraw.floodfill(mask, s, 128)

    # Convert filled border to 0 alpha
    alpha_data = []
    for val in mask.getdata():
        if val == 128:
            alpha_data.append(0)
        else:
            alpha_data.append(255)

    final_alpha = Image.new("L", (w, h))
    final_alpha.putdata(alpha_data)
    final_alpha = final_alpha.filter(ImageFilter.GaussianBlur(radius=1.0))
    img.putalpha(final_alpha)
    return img

# 1. Background
bg_files = glob.glob(os.path.join(brain_dir, "deep_sea_bg_*.jpg"))
if bg_files:
    bg = Image.open(bg_files[0])
    bg.save(os.path.join(sprites_dir, "bg_deep_sea.png"))
    print("Processed bg_deep_sea.png")

# 2. Main sprites from brain
name_mapping = {
    "captain_finn_idle": "captain_finn_idle_*.jpg",
    "captain_finn_cheer": "captain_finn_cheer_*.jpg",
    "yellow_submarine": "yellow_submarine_*.jpg",
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

for target_name, pattern in name_mapping.items():
    matches = glob.glob(os.path.join(brain_dir, pattern))
    if matches:
        im = Image.open(matches[0])
        clean = remove_white_bg(im, tolerance=238)

        # Center wheel if needed
        if target_name == "ship_wheel":
            # crop to bounding box of content and place in square
            bbox = clean.getbbox()
            if bbox:
                cropped = clean.crop(bbox)
                cw, ch = cropped.size
                max_side = max(cw, ch) + 20
                sq = Image.new("RGBA", (max_side, max_side), (0, 0, 0, 0))
                sq.paste(cropped, ((max_side - cw) // 2, (max_side - ch) // 2))
                clean = sq

        clean.save(os.path.join(sprites_dir, f"{target_name}.png"))
        print(f"Processed {target_name}.png")

# 3. Generate Helper Vector / UI Sprites
# Wave icon for Bubble B
wave_img = Image.new("RGBA", (256, 256), (0, 0, 0, 0))
wd = ImageDraw.Draw(wave_img)
wd.ellipse((16, 16, 240, 240), fill=(225, 245, 254, 255), outline=(3, 169, 244, 255), width=8)
# Draw wave lines
points1 = [(40, 140), (70, 115), (105, 145), (140, 115), (175, 145), (210, 115)]
points2 = [(40, 175), (70, 150), (105, 180), (140, 150), (175, 180), (210, 150)]
wd.line(points1, fill=(2, 119, 189, 255), width=10, joint="curve")
wd.line(points2, fill=(3, 169, 244, 255), width=10, joint="curve")
wave_img.save(os.path.join(sprites_dir, "icon_wave.png"))

# Home icon
home_img = Image.new("RGBA", (128, 128), (0, 0, 0, 0))
hd = ImageDraw.Draw(home_img)
hd.ellipse((4, 4, 124, 124), fill=(255, 255, 255, 240), outline=(52, 152, 219, 255), width=6)
hd.polygon([(64, 30), (28, 62), (40, 62), (40, 96), (88, 96), (88, 62), (100, 62)], fill=(52, 152, 219, 255))
hd.rectangle([(56, 70), (72, 96)], fill=(255, 255, 255, 255))
home_img.save(os.path.join(sprites_dir, "icon_home.png"))

# Replay icon
rep_img = Image.new("RGBA", (128, 128), (0, 0, 0, 0))
rd = ImageDraw.Draw(rep_img)
rd.ellipse((4, 4, 124, 124), fill=(255, 255, 255, 240), outline=(46, 204, 113, 255), width=6)
rd.arc((32, 32, 96, 96), start=40, end=320, fill=(46, 204, 113, 255), width=10)
rd.polygon([(94, 40), (104, 20), (84, 22)], fill=(46, 204, 113, 255))
rep_img.save(os.path.join(sprites_dir, "icon_replay.png"))

# Sound On / Off icons
def make_sound_icon(sound_on=True):
    s_img = Image.new("RGBA", (128, 128), (0, 0, 0, 0))
    sd = ImageDraw.Draw(s_img)
    sd.ellipse((4, 4, 124, 124), fill=(255, 255, 255, 240), outline=(243, 156, 18, 255), width=6)
    # Speaker cone
    sd.polygon([(36, 50), (48, 50), (66, 36), (66, 92), (48, 78), (36, 78)], fill=(243, 156, 18, 255))
    if sound_on:
        sd.arc((55, 42, 85, 86), start=-50, end=50, fill=(243, 156, 18, 255), width=6)
        sd.arc((50, 32, 100, 96), start=-50, end=50, fill=(243, 156, 18, 255), width=6)
    else:
        sd.line([(76, 48), (96, 80)], fill=(231, 76, 60, 255), width=6)
        sd.line([(96, 48), (76, 80)], fill=(231, 76, 60, 255), width=6)
    return s_img

make_sound_icon(True).save(os.path.join(sprites_dir, "icon_sound_on.png"))
make_sound_icon(False).save(os.path.join(sprites_dir, "icon_sound_off.png"))

# Bubble particle
bub_img = Image.new("RGBA", (64, 64), (0, 0, 0, 0))
bd = ImageDraw.Draw(bub_img)
bd.ellipse((4, 4, 60, 60), fill=(255, 255, 255, 50), outline=(255, 255, 255, 230), width=3)
bd.ellipse((14, 12, 26, 24), fill=(255, 255, 255, 210))
bub_img.save(os.path.join(sprites_dir, "bubble_particle.png"))

# Laser glow beam
las_img = Image.new("RGBA", (256, 48), (0, 0, 0, 0))
ld = ImageDraw.Draw(las_img)
ld.rounded_rectangle((4, 6, 252, 42), radius=18, fill=(255, 50, 80, 180))
ld.rounded_rectangle((8, 12, 248, 36), radius=12, fill=(255, 120, 140, 220))
ld.rounded_rectangle((12, 18, 244, 30), radius=6, fill=(255, 255, 255, 255))
las_img.save(os.path.join(sprites_dir, "laser_glow.png"))

# Tally Mark Vertical Line (chunky carved style)
tm_img = Image.new("RGBA", (48, 180), (0, 0, 0, 0))
tmd = ImageDraw.Draw(tm_img)
tmd.rounded_rectangle((12, 8, 36, 172), radius=10, fill=(235, 75, 55, 255), outline=(150, 25, 20, 255), width=3)
tm_img.save(os.path.join(sprites_dir, "tally_mark.png"))

# Tally Slash (diagonal line to group 5)
ts_img = Image.new("RGBA", (220, 180), (0, 0, 0, 0))
tsd = ImageDraw.Draw(ts_img)
tsd.line([(16, 160), (204, 20)], fill=(235, 75, 55, 255), width=20)
tsd.line([(16, 160), (204, 20)], fill=(255, 150, 130, 255), width=8)
ts_img.save(os.path.join(sprites_dir, "tally_slash.png"))

# Wheel pointer needle
nd_img = Image.new("RGBA", (140, 64), (0, 0, 0, 0))
nd = ImageDraw.Draw(nd_img)
# Arrow pointing to the right
nd.polygon([(10, 16), (80, 16), (130, 32), (80, 48), (10, 48)], fill=(255, 215, 0, 255), outline=(180, 120, 0, 255), width=4)
nd.polygon([(18, 24), (76, 24), (114, 32), (76, 40), (18, 40)], fill=(255, 245, 130, 255))
nd_img.save(os.path.join(sprites_dir, "wheel_needle.png"))

# Small Star
st_img = Image.new("RGBA", (64, 64), (0, 0, 0, 0))
std = ImageDraw.Draw(st_img)
cx, cy = 32, 32
pts = []
for i in range(10):
    a = math.radians(i * 36 - 90)
    r = 28 if i % 2 == 0 else 12
    pts.append((cx + r * math.cos(a), cy + r * math.sin(a)))
std.polygon(pts, fill=(255, 215, 0, 255), outline=(218, 165, 32, 255), width=2)
st_img.save(os.path.join(sprites_dir, "icon_star.png"))

print("All textures and sprites generated!")

# -------------------------------------------------------------
# Generate .meta files for sprites
# -------------------------------------------------------------
meta_template_sprite = """fileFormatVersion: 2
guid: __GUID__
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
    flipGreenChannel: 0
  isReadable: 0
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMipmapLimit: 1
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: 1
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 0
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: 1
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {x: 0.5, y: 0.5}
  spritePixelsToUnits: 100
  spriteBorder: {x: 0, y: 0, z: 0, w: 0}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  swizzle: 50462976
  cookieLightType: 0
  platformSettings:
  - serializedVersion: 4
    buildTarget: DefaultTexturePlatform
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    ignorePlatformSupport: 0
    androidETC2FallbackOverride: 0
    forceMaximumCompressionQuality_BC6H_BC7: 0
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    physicsShape: []
    bones: []
    spriteID: __SPRITE_ID__
    internalID: 21300000
    vertices: []
    indices: 
    edges: []
    weights: []
  spritePackingTag: 
  pVRTCEventOp: 0
  pVRTCExportMode: 0
  isAtlas: 0
  editorData:
    serializedVersion: 1
    assetHash:
      serializedVersion: 2
      Hash: 
    data: []
"""

for fname in os.listdir(sprites_dir):
    if fname.endswith(".png"):
        stem = fname[:-4]
        mpath = os.path.join(sprites_dir, fname + ".meta")
        g = GUIDS.get(stem, uuid.uuid4().hex)
        sid = uuid.uuid4().hex
        content = meta_template_sprite.replace("__GUID__", g).replace("__SPRITE_ID__", sid)
        with open(mpath, "w", encoding="utf-8") as f:
            f.write(content)
        print(f"Sprite meta: {fname} -> {g}")

# -------------------------------------------------------------
# Synthesize Crisp Audio SFX and Music (16-bit PCM WAV)
# -------------------------------------------------------------
sample_rate = 44100

def write_wav(filename, samples):
    filepath = os.path.join(audio_dir, filename)
    with wave.open(filepath, "w") as wf:
        wf.setnchannels(1)
        wf.setsampwidth(2)
        wf.setframerate(sample_rate)
        raw = bytearray()
        for s in samples:
            val = max(-32767, min(32767, int(s * 32767)))
            raw.extend(struct.pack("<h", val))
        wf.writeframes(raw)
    stem = filename[:-4]
    g = GUIDS.get(stem, uuid.uuid4().hex)
    meta_path = filepath + ".meta"
    with open(meta_path, "w", encoding="utf-8") as mf:
        mf.write(f"""fileFormatVersion: 2
guid: {g}
AudioImporter:
  externalObjects: {{}}
  serializedVersion: 6
  defaultSettings:
    loadType: 0
    sampleRateSetting: 0
    sampleRateOverride: 44100
    compressionFormat: 0
    quality: 1
    conversionMode: 0
  platformSettingOverrides: {{}}
  forceToMono: 0
  normalize: 1
  preloadAudioData: 1
  loadInBackground: 0
  ambisonic: 0
  3D: 0
  userData: 
  assetBundleName: 
  assetBundleVariant: 
""")
    print(f"Generated WAV {filename} -> {g}")

# 1. SFX Click (Crisp bubble click)
duration = 0.08
num_samples = int(sample_rate * duration)
click_samples = []
for i in range(num_samples):
    t = i / sample_rate
    freq = 600 - t * 3000
    env = (1.0 - t / duration) ** 2
    s = math.sin(2 * math.pi * freq * t) * env * 0.6
    click_samples.append(s)
write_wav("sfx_click.wav", click_samples)

# 2. SFX Star Press (sparkly upbeat chime)
duration = 0.5
num_samples = int(sample_rate * duration)
star_samples = []
notes = [523.25, 659.25, 783.99, 1046.50] # C5, E5, G5, C6
for i in range(num_samples):
    t = i / sample_rate
    val = 0.0
    for idx, f in enumerate(notes):
        st = idx * 0.08
        if t >= st:
            nt = t - st
            env = math.exp(-nt * 8.0)
            val += math.sin(2 * math.pi * f * nt) * env * 0.25
    star_samples.append(val)
write_wav("sfx_star_press.wav", star_samples)

# 3. SFX Correct (cheerful high arpeggio + bell)
duration = 0.65
num_samples = int(sample_rate * duration)
correct_samples = []
c_notes = [440.0, 554.37, 659.25, 880.0] # A4, C#5, E5, A5
for i in range(num_samples):
    t = i / sample_rate
    val = 0.0
    for idx, f in enumerate(c_notes):
        st = idx * 0.1
        if t >= st:
            nt = t - st
            env = math.exp(-nt * 6.0)
            # bell harmonics
            val += (math.sin(2 * math.pi * f * nt) + 0.3 * math.sin(2 * math.pi * f * 2 * nt)) * env * 0.22
    correct_samples.append(val)
write_wav("sfx_correct.wav", correct_samples)

# 4. SFX Wrong (gentle cartoon wobble boing-down)
duration = 0.45
num_samples = int(sample_rate * duration)
wrong_samples = []
for i in range(num_samples):
    t = i / sample_rate
    # frequency drops with wobble
    freq = 280 - t * 240 + 15 * math.sin(2 * math.pi * 14 * t)
    env = (1.0 - t / duration) * 0.6
    s = math.sin(2 * math.pi * freq * t) * env
    wrong_samples.append(s)
write_wav("sfx_wrong.wav", wrong_samples)

# 5. SFX Laser (fun underwater sci-fi zap pew)
duration = 0.22
num_samples = int(sample_rate * duration)
laser_samples = []
for i in range(num_samples):
    t = i / sample_rate
    freq = 1400 * math.exp(-t * 18.0) + 120
    env = (1.0 - t / duration) * 0.7
    s = math.sin(2 * math.pi * freq * t) * env
    laser_samples.append(s)
write_wav("sfx_laser.wav", laser_samples)

# 6. SFX Tally Hit (carved stone chisel impact + cute chime)
duration = 0.18
num_samples = int(sample_rate * duration)
hit_samples = []
for i in range(num_samples):
    t = i / sample_rate
    env = math.exp(-t * 22.0)
    # sharp noise burst + tonal ring
    noise = (math.sin(t * 18320) % 0.5) * 0.3
    tone = math.sin(2 * math.pi * 780 * t) * 0.5
    s = (noise + tone) * env
    hit_samples.append(s)
write_wav("sfx_tally_hit.wav", hit_samples)

# 7. SFX Bundle Slash (5th diagonal mark energy swoosh + bell)
duration = 0.4
num_samples = int(sample_rate * duration)
slash_samples = []
for i in range(num_samples):
    t = i / sample_rate
    freq = 400 + 900 * (t / duration)
    env = math.sin(math.pi * t / duration) * 0.4
    chime_env = math.exp(-t * 8.0) * 0.35
    s = math.sin(2 * math.pi * freq * t) * env + math.sin(2 * math.pi * 1046.5 * t) * chime_env
    slash_samples.append(s)
write_wav("sfx_bundle_slash.wav", slash_samples)

# 8. SFX Wheel Tick (nautical wooden ratchet click)
duration = 0.05
num_samples = int(sample_rate * duration)
tick_samples = []
for i in range(num_samples):
    t = i / sample_rate
    env = (1.0 - t / duration) ** 3
    s = math.sin(2 * math.pi * (1100 + t * 400) * t) * env * 0.5
    tick_samples.append(s)
write_wav("sfx_wheel_tick.wav", tick_samples)

# 9. SFX Badge Drop (triumphant brass fanfare arpeggio)
duration = 1.2
num_samples = int(sample_rate * duration)
fanfare_samples = []
f_notes = [523.25, 659.25, 783.99, 1046.50, 1318.51] # C5, E5, G5, C6, E6
for i in range(num_samples):
    t = i / sample_rate
    val = 0.0
    for idx, f in enumerate(f_notes):
        st = idx * 0.12
        if t >= st:
            nt = t - st
            env = math.exp(-nt * 4.0)
            val += (math.sin(2 * math.pi * f * nt) + 0.3 * math.sin(2 * math.pi * f * 1.5 * nt)) * env * 0.25
    fanfare_samples.append(val)
write_wav("sfx_badge_drop.wav", fanfare_samples)

# 10. SFX Bubble (popping bubble)
duration = 0.12
num_samples = int(sample_rate * duration)
bubble_samples = []
for i in range(num_samples):
    t = i / sample_rate
    freq = 450 + 800 * (t / duration)
    env = (1.0 - t / duration) ** 2 * 0.5
    s = math.sin(2 * math.pi * freq * t) * env
    bubble_samples.append(s)
write_wav("sfx_bubble.wav", bubble_samples)

# 11. BGM Underwater (relaxing ambient loop with vibraphone notes and bubble texture)
bgm_duration = 8.0 # 8 second seamless loop
bgm_num_samples = int(sample_rate * bgm_duration)
bgm_samples = []
# gentle chord progression: Fmaj7 - G - C - Am
chords = [
    (0.0, [349.23, 440.0, 523.25, 659.25]),  # Fmaj7
    (2.0, [392.0, 493.88, 587.33]),          # G
    (4.0, [261.63, 329.63, 392.0, 523.25]),  # C
    (6.0, [220.0, 261.63, 329.63, 440.0]),   # Am
]
for i in range(bgm_num_samples):
    t = i / sample_rate
    val = 0.0
    for chord_time, freqs in chords:
        if t >= chord_time:
            dt = t - chord_time
            if dt < 2.0:
                env = math.sin(math.pi * dt / 2.0)
                for f in freqs:
                    val += math.sin(2 * math.pi * f * t) * env * 0.04
    # soft underwater ocean rumble/drone
    val += math.sin(2 * math.pi * 55 * t) * 0.03
    # gentle sine modulation
    val *= (0.85 + 0.15 * math.sin(2 * math.pi * 0.5 * t))
    bgm_samples.append(val)
write_wav("bgm_underwater.wav", bgm_samples)

print("Audio asset synthesis complete!")

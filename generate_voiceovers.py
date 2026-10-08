import os
import subprocess

ps_script_path = "generate_vo.ps1"

lines = [
    ("vo_menu.wav", "Welcome to the deep sea! The toys are lost in the sand. Let's sort them out. Press Play to start your submarine patrol!"),
    ("vo_lvl1_intro.wav", "We want to know which toy these five octopuses like best. Drag the question that will give us different answers we can count."),
    ("vo_lvl1_correct.wav", "Great choice! Every octopus can give a different answer. Now we can count how many like each toy. Let's remember this — we'll tally these votes very soon!"),
    ("vo_lvl1_wrong.wav", "Oops! Every octopus lives in the ocean, so they would all answer yes. That question can't be counted in different groups. Try the toy question instead."),
    ("vo_lvl2_task1.wav", "More fish have joined the octopuses and voted for the Red Car! Let's tally every vote for the Red Car so we know the total. Four fish picked the Red Car. Tap your laser once for each fish to draw four tally marks."),
    ("vo_lvl2_task1_correct.wav", "Perfect! One tally mark means one vote. We have counted four votes for the Red Car so far."),
    ("vo_lvl2_task2.wav", "One more fish picked the Red Car! That makes five. Tap once to draw a line across the first four marks and close the group of five."),
    ("vo_lvl2_task2_correct.wav", "Awesome! Every fifth vote gets a line across the first four. Now we have one neat group of five. Groups of five make counting super fast!"),
    ("vo_lvl2_task3.wav", "Three more fish picked the Red Car. Tap your laser once for each fish to add three more tally marks after the group of five."),
    ("vo_lvl2_task3_correct.wav", "Great counting! Five marks in one group, plus three more marks, makes eight tally marks in total. So eight sea creatures picked the Red Car. Let's write that number on our chart!"),
    ("vo_lvl3_intro.wav", "Look! The Red Car total on this wall is eight — the same number we just tallied together. The other two totals were counted by your ocean friends while you worked. Now let's read the whole chart!"),
    ("vo_lvl3_task1.wav", "Look at the numbers on the wall. Turn the wheel to point at the toy with the biggest total."),
    ("vo_lvl3_task1_correct.wav", "You got it! Eight is the biggest number on the chart. Red cars were found the most!"),
    ("vo_lvl3_task1_wrong.wav", "Not quite. Check each number again — which one is the highest? Try turning the wheel to that row."),
    ("vo_lvl3_task2.wav", "Now look at the chart again. Turn the wheel to point at the toy with the smallest total."),
    ("vo_lvl3_task2_correct.wav", "Spot on! Three is the smallest number on the chart. Blue boats were found the least!"),
    ("vo_lvl3_task2_wrong.wav", "Not yet. Look for the smallest number of all three — is it really this one? Try again."),
    ("vo_summary.wav", "Patrol complete! You asked a good question, drew neat tally marks, and read the total chart like a pro. The sea toys are safe because of you!"),
]

audio_dir = os.path.abspath(os.path.join("Assets", "Audio"))
os.makedirs(audio_dir, exist_ok=True)

res_audio_dir = os.path.abspath(os.path.join("Assets", "Resources", "Audio"))
os.makedirs(res_audio_dir, exist_ok=True)

ps_commands = [
    "Add-Type -AssemblyName System.Speech",
    "$synth = New-Object System.Speech.Synthesis.SpeechSynthesizer",
    "$synth.SelectVoice('Microsoft Zira Desktop')",
    "$synth.Rate = 0"
]

for filename, text in lines:
    out_path = os.path.join(audio_dir, filename).replace("/", "\\")
    clean_text = text.replace("'", "''").replace("—", "-")
    ps_commands.append(f"$synth.SetOutputToWaveFile('{out_path}')")
    ps_commands.append(f"$synth.Speak('{clean_text}')")

ps_commands.append("$synth.Dispose()")
ps_commands.append("Write-Host 'ALL_VO_GENERATED_SUCCESSFULLY'")

with open(ps_script_path, "w", encoding="utf-8") as f:
    f.write("\n".join(ps_commands))

print("Running PowerShell voice generation...")
proc = subprocess.run(["powershell", "-ExecutionPolicy", "Bypass", "-File", ps_script_path], capture_output=True, text=True)
print("STDOUT:", proc.stdout)
if proc.stderr:
    print("STDERR:", proc.stderr)

import shutil
for filename, _ in lines:
    src = os.path.join(audio_dir, filename)
    dst = os.path.join(res_audio_dir, filename)
    if os.path.exists(src):
        shutil.copy2(src, dst)

print("Copied all VO files to Resources/Audio!")

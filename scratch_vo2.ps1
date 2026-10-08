Add-Type -AssemblyName System.Speech
[System.Speech.Synthesis.SpeechSynthesizer] = New-Object System.Speech.Synthesis.SpeechSynthesizer
.SelectVoice('Microsoft Zira Desktop')
.SetOutputToWaveFile('Assets/Audio/vo_menu.wav')
.Speak('Welcome to the deep sea! The toys are lost in the sand. Let us sort them out.')
.Dispose()
Write-Host 'VO_SUCCESS'

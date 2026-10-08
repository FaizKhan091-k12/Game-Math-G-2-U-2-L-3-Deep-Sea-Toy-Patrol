
Add-Type -AssemblyName System.Speech
 = New-Object System.Speech.Synthesis.SpeechSynthesizer
.SelectVoice('Microsoft Zira Desktop')
.Rate = 0
.SetOutputToWaveFile('Assets/Audio/vo_menu.wav')
.Speak('Welcome to the deep sea! The toys are lost in the sand. Lets sort them out. Press the big star to start your sub!')
.Dispose()
Write-Output 'VO_SUCCESS'

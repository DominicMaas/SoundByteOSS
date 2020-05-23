$date = Get-Date -Format g

'{"build_mood":"master","build_no":"'+$Env:BUILD_BUILDNUMBER+'","build_time":"'+$date+'"}' | Out-File 'legacy/src/uwp/Assets/build_info.json'

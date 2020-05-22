$date = Get-Date -Format g

'{"build_branch":"master","build_no":"'+$Env:BUILD_BUILDNUMBER+'","build_time":"'+$date+'"}' | Out-File 'legacy/uwp/Assets/build_info.json'
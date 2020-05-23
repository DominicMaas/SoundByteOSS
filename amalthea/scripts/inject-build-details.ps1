$date = Get-Date -Format g
$branch = $Env:BUILD_SOURCEBRANCH
$commit = $Env:BUILD_SOURCEVERSION

'{"branch": "' + $branch + '","commit":"' + $commit + '","platform":"UWP","time":"' + $date + '"}' | Out-File 'amalthea/src/apps/uwp/build_info.json'
'{"branch": "' + $branch + '","commit":"' + $commit + '","platform":"iOS","time":"' + $date + '"}' | Out-File 'amalthea/src/apps/ios/Resources/build_info.json'
'{"branch": "' + $branch + '","commit":"' + $commit + '","platform":"Android","time":"' + $date + '"}' | Out-File 'amalthea/src/apps/android/Assets/build_info.json'
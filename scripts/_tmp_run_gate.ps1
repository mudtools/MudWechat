$p = Start-Process -FilePath "powershell" -ArgumentList "-NoProfile","-ExecutionPolicy","Bypass","-File","./scripts/verify-build.ps1" -WindowStyle Hidden -RedirectStandardOutput ".\test-reports\gate-p04.log" -PassThru
$p.Id

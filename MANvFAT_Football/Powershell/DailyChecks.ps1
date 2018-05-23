#
# DailyChecks.ps1
# Scheduled health checks for ManVFatFootball to ensure:
#   - database backups - full and trans logs
#	- scheduled tasks
# are all running correctly.
#
# Note: Checking Norton via PowerShell is not possible

$NotificationSuccessSubject = "Successful check on ManVFat Football"
$NotificationErrorSubject = "Errors found on ManVFat Football"
$NotificationSubject = ""

$MailHost = "smtp.mandrillapp.com"
$MailPort = "587"
$MailUser = "MAN v FAT"
$MailPass = "LMPoi1kWRMJJQMWCHFsJIQ"
$MailFrom = "admin@manvfat.com"
$MailTo = "colin@factory73.com"

$OutString = ""
$Success = 1;

$SQLBackupDir = "C:\IBS\SQLBackups\MANvFATFootball"
$ScheduledTaskName = "MANvFAT Live Site"

# Check SQL backup folders - Transaction Log backups
$SQLFolderCheck = Get-ChildItem -Path $SQLBackupDir | Where-Object {$_.LastWriteTime.Date -eq (Get-Date).Date.AddDays(-1)} | Where {$_.extension -eq ".bak"}

If ($SQLFolderCheck.length -gt 0) {
    $OutString += "SQL Full Backups for yesterday"
    $OutString += $SQLFolderCheck | Out-String
    $OutString += "`n"
}
else{
    $OutString += "***NO TRANSACTION LOG BACKUPS FOUND!***`n`n"
    $Success = 0
    }

# Check SQL backup folders - Full DB backup
$SQLFolderCheck = Get-ChildItem -Path $SQLBackupDir | Where-Object {$_.LastWriteTime.Date -eq (Get-Date).Date.AddDays(-1)} | Where {$_.extension -eq ".trn"}

If ($SQLFolderCheck.length -gt 0) {
    $OutString += "SQL Trans Log Backups for yesterday"
    $OutString += $SQLFolderCheck | Out-String
    $OutString += "`n"
}
else{
    $OutString += "***NO FULL BACKUPS FOUND!***`n`n"
    $Success = 0
    }

# Check scheduled tasks status

$ScheduledTaskOutput = Get-ScheduledTask $ScheduledTaskName | Get-ScheduledTaskInfo | Out-String
$ScheduledTaskCheck = $ScheduledTaskOutput | Select-String -Pattern "LastTaskResult     : 0"

If ($ScheduledTaskCheck.length -gt 0) {
    $OutString += "Scheduled Task Successful - Results"
    $OutString += $ScheduledTaskOutput | Out-String
    $OutString += "`n"
}
else{
    $OutString += "***SCHEDULED TASK FAILED***"
    $OutString += $ScheduledTaskOutput | Out-String
    $OutString += "`n"
    $Success = 0
    }

If($Success -eq 1){
    $NotificationSubject = $NotificationSuccessSubject
}
else{
    $NotificationSubject = $NotificationErrorSubject
}

# Send Email
$SecurePass = $MailPass | ConvertTo-SecureString -AsPlainText -Force
$Credentials = New-Object System.Management.Automation.PSCredential -ArgumentList $MailUser, $SecurePass


Send-MailMessage -SmtpServer $MailHost -Port $MailPort -Credential $Credentials -To $MailTo -From $MailFrom -Subject $NotificationSubject -Body $OutString

Write-Output $OutString
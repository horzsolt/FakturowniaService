<#
.SYNOPSIS
    Reports Windows Server restart history: uptime, who restarted, planned vs unplanned, downtime.

.DESCRIPTION
    Correlates System log events (1074, 6006, 6005, 6008, 41, 12) to build a restart timeline.
    Run on the server under investigation (elevated recommended for full log access).

.PARAMETER Days
    How many days of history to scan. Default: 30

.PARAMETER ExportCsv
    Optional path to export the restart table as CSV.

.EXAMPLE
    .\Get-ServerRestartHistory.ps1
    .\Get-ServerRestartHistory.ps1 -Days 90 -ExportCsv C:\temp\restarts.csv
#>
[CmdletBinding()]
param(
    [int] $Days = 30,
    [string] $ExportCsv
)

$ErrorActionPreference = 'Stop'

function Format-Duration {
    param([TimeSpan] $Span)
    if ($Span.TotalSeconds -lt 0) { return 'n/a' }
    $parts = @()
    if ($Span.Days -gt 0) { $parts += "$($Span.Days)d" }
    if ($Span.Hours -gt 0) { $parts += "$($Span.Hours)h" }
    if ($Span.Minutes -gt 0) { $parts += "$($Span.Minutes)m" }
    if ($parts.Count -eq 0) { $parts += "$([int]$Span.TotalSeconds)s" }
    return ($parts -join ' ')
}

function Get-ShutdownReasonLabel {
    param([int] $ReasonCode)
    $labels = @{
        0x80070000 = 'Other (often unplanned)'
        0x80030002 = 'OS reconfiguration (planned)'
        0x80020002 = 'Application install (planned)'
        0x80020003 = 'Application removal (planned)'
        0x80020004 = 'Application unresponsive (planned)'
        0x80020005 = 'Application unstable (planned)'
        0x80020010 = 'Hardware maintenance (planned)'
        0x80020011 = 'Hardware install (planned)'
        0x80020013 = 'Hardware failure (unplanned)'
        0x80020015 = 'Hardware incompatibility (unplanned)'
        0x80020016 = 'Hardware hotfix (planned)'
        0x80020021 = 'Power loss / ACPI (unplanned)'
        0x8002003B = 'Shutdown API (planned)'
        0x8002003D = 'Bugcheck / BSOD (unplanned)'
    }
    if ($labels.ContainsKey($ReasonCode)) { return $labels[$ReasonCode] }
    return ('0x{0:X8}' -f $ReasonCode)
}

function Parse-Event1074 {
    param($Event)
    $msg = $Event.Message
    $user = if ($msg -match 'User:\s+(.+?)\r?\n') { $Matches[1].Trim() } else { $null }
    $reason = if ($msg -match 'Reason Code:\s+0x([0-9A-Fa-f]+)') { [int]"0x$($Matches[1])" } else { $null }
    $type = if ($msg -match 'Type:\s+(.+?)\r?\n') { $Matches[1].Trim() } else { $null }
    $comment = if ($msg -match 'Comment:\s+(.+?)(\r?\n|$)') { $Matches[1].Trim() } else { '' }
    $process = if ($msg -match 'Process:\s+(.+?)\r?\n') { $Matches[1].Trim() } else { $null }
    [pscustomobject]@{
        Time        = $Event.TimeCreated
        User        = $user
        Type        = $type
        ReasonCode  = $reason
        ReasonLabel = if ($null -ne $reason) { Get-ShutdownReasonLabel $reason } else { $null }
        Comment     = $comment
        Process     = $process
        Message     = $msg
    }
}

# --- Current uptime ---
$os = Get-CimInstance Win32_OperatingSystem
$bootTime = $os.LastBootUpTime
$uptime = (Get-Date) - $bootTime

Write-Host ''
Write-Host '=== CURRENT SERVER STATE ===' -ForegroundColor Cyan
Write-Host ("Computer     : {0}" -f $env:COMPUTERNAME)
Write-Host ("Last boot    : {0:yyyy-MM-dd HH:mm:ss}" -f $bootTime)
Write-Host ("Uptime       : {0}" -f (Format-Duration $uptime))
Write-Host ("OS           : {0}" -f $os.Caption)
Write-Host ''

$since = (Get-Date).AddDays(-$Days)
Write-Host ("=== RESTART HISTORY (last {0} days) ===" -f $Days) -ForegroundColor Cyan

$e1074 = @(Get-WinEvent -FilterHashtable @{
    LogName   = 'System'
    Id        = 1074
    StartTime = $since
} -ErrorAction SilentlyContinue | ForEach-Object { Parse-Event1074 $_ })

$e6008 = @(Get-WinEvent -FilterHashtable @{
    LogName   = 'System'
    Id        = 6008
    StartTime = $since
} -ErrorAction SilentlyContinue)

$e6005 = @(Get-WinEvent -FilterHashtable @{
    LogName   = 'System'
    Id        = 6005
    StartTime = $since
} -ErrorAction SilentlyContinue)

$e6006 = @(Get-WinEvent -FilterHashtable @{
    LogName   = 'System'
    Id        = 6006
    StartTime = $since
} -ErrorAction SilentlyContinue)

$e41 = @(Get-WinEvent -FilterHashtable @{
    LogName      = 'System'
    ProviderName = 'Microsoft-Windows-Kernel-Power'
    Id           = 41
    StartTime    = $since
} -ErrorAction SilentlyContinue)

$e12 = @(Get-WinEvent -FilterHashtable @{
    LogName      = 'System'
    ProviderName = 'Microsoft-Windows-Kernel-General'
    Id           = 12
    StartTime    = $since
} -ErrorAction SilentlyContinue)

$shutdowns = [System.Collections.Generic.List[object]]::new()
foreach ($ev in $e1074) {
    $shutdowns.Add([pscustomobject]@{
        Time    = $ev.Time
        Kind    = 'UserInitiated'
        Planned = ($ev.Type -match 'planned')
        User    = $ev.User
        Reason  = $ev.ReasonLabel
        Comment = $ev.Comment
        Process = $ev.Process
        Detail  = $ev.Type
    })
}
foreach ($ev in $e6006) {
    $shutdowns.Add([pscustomobject]@{
        Time    = $ev.TimeCreated
        Kind    = 'CleanShutdown'
        Planned = $null
        User    = $null
        Reason  = 'Event log service stopped (6006)'
        Comment = ''
        Process = $null
        Detail  = $ev.Message
    })
}

$boots = [System.Collections.Generic.List[object]]::new()
foreach ($ev in $e6005) {
    $boots.Add([pscustomobject]@{
        Time   = $ev.TimeCreated
        Kind   = 'Boot6005'
        Detail = 'Event log service started'
    })
}
foreach ($ev in $e12) {
    $bootDetail = if ($ev.Message -match 'System boot time:\s+(.+)') { $Matches[1].Trim() } else { $ev.Message }
    $boots.Add([pscustomobject]@{
        Time   = $ev.TimeCreated
        Kind   = 'Boot12'
        Detail = $bootDetail
    })
}

$unexpected = [System.Collections.Generic.List[object]]::new()
foreach ($ev in $e6008) {
    $unexpected.Add([pscustomobject]@{
        Time   = $ev.TimeCreated
        Kind   = 'Unexpected6008'
        Detail = $ev.Message
    })
}
foreach ($ev in $e41) {
    $unexpected.Add([pscustomobject]@{
        Time   = $ev.TimeCreated
        Kind   = 'KernelPower41'
        Detail = $ev.Message
    })
}

$allBoots = $boots | Sort-Object Time -Unique
$allShutdowns = $shutdowns | Sort-Object Time
$restarts = [System.Collections.Generic.List[object]]::new()

foreach ($boot in $allBoots) {
    $priorShutdown = $allShutdowns | Where-Object { $_.Time -lt $boot.Time } |
        Sort-Object Time -Descending | Select-Object -First 1

    $downtime = if ($priorShutdown) { $boot.Time - $priorShutdown.Time } else { $null }

    $unplannedNear = $unexpected | Where-Object {
        $_.Time -ge $boot.Time.AddMinutes(-5) -and $_.Time -le $boot.Time.AddMinutes(5)
    } | Select-Object -First 1

    $classification = 'Unknown'
    if ($priorShutdown -and $priorShutdown.Planned -eq $true) {
        $classification = 'Planned'
    }
    elseif ($priorShutdown -and $priorShutdown.Planned -eq $false) {
        $classification = 'Unplanned (user-initiated)'
    }
    elseif ($unplannedNear) {
        $classification = 'Unplanned (unexpected shutdown / power loss)'
    }
    elseif ($priorShutdown) {
        $classification = 'Shutdown recorded (planned flag unclear)'
    }

    $restarts.Add([pscustomobject]@{
        BootTime        = $boot.Time
        ShutdownTime    = if ($priorShutdown) { $priorShutdown.Time } else { $null }
        Downtime        = if ($downtime) { Format-Duration $downtime } else { 'n/a (no prior shutdown event)' }
        DowntimeSeconds = if ($downtime) { [int]$downtime.TotalSeconds } else { $null }
        Classification  = $classification
        InitiatedBy     = if ($priorShutdown) { $priorShutdown.User } else { $null }
        Reason          = if ($priorShutdown) { $priorShutdown.Reason } else { if ($unplannedNear) { $unplannedNear.Detail } else { $null } }
        Comment         = if ($priorShutdown) { $priorShutdown.Comment } else { '' }
        Process         = if ($priorShutdown) { $priorShutdown.Process } else { $null }
        UnexpectedNote  = if ($unplannedNear) { "$($unplannedNear.Kind) at $($unplannedNear.Time)" } else { '' }
    })
}

if ($restarts.Count -eq 0) {
    Write-Host "No boot events found in the last $Days days (or logs were cleared)." -ForegroundColor Yellow
}
else {
    $restarts | Sort-Object BootTime -Descending | Format-Table -AutoSize `
        BootTime, ShutdownTime, Downtime, Classification, InitiatedBy, Reason, Comment
}

Write-Host ''
Write-Host '=== USER-INITIATED SHUTDOWN/RESTART (Event 1074) ===' -ForegroundColor Cyan
if ($e1074.Count -gt 0) {
    $e1074 | Sort-Object Time -Descending | Format-Table -AutoSize Time, Type, User, ReasonLabel, Comment, Process
}
else {
    Write-Host 'No Event ID 1074 in this period (restart may have been unplanned, power loss, or crash).' -ForegroundColor Yellow
}

Write-Host ''
Write-Host '=== UNEXPECTED SHUTDOWN MARKERS (6008 / Kernel-Power 41) ===' -ForegroundColor Cyan
if ($unexpected.Count -gt 0) {
    $unexpected | Sort-Object Time -Descending | Format-Table -AutoSize Time, Kind, Detail -Wrap
}
else {
    Write-Host 'None in this period.'
}

Write-Host ''
Write-Host '=== WINDOWS UPDATE RESTART HINTS ===' -ForegroundColor Cyan
$wu = @(Get-WinEvent -FilterHashtable @{
    LogName      = 'System'
    ProviderName = 'Microsoft-Windows-WindowsUpdateClient'
    StartTime    = $since
} -MaxEvents 100 -ErrorAction SilentlyContinue |
    Where-Object { $_.Id -in 19, 20, 21, 22, 43, 44 })

if ($wu.Count -gt 0) {
    $wu | Sort-Object TimeCreated -Descending | Select-Object -First 10 |
        Format-Table -AutoSize TimeCreated, Id, Message -Wrap
}
else {
    Write-Host 'No recent Windows Update client events (or provider not logged).'
}

if ($ExportCsv -and $restarts.Count -gt 0) {
    $restarts | Sort-Object BootTime -Descending | Export-Csv -Path $ExportCsv -NoTypeInformation -Encoding UTF8
    Write-Host ''
    Write-Host "Exported to $ExportCsv" -ForegroundColor Green
}

Write-Host ''
Write-Host 'Notes:' -ForegroundColor DarkGray
Write-Host '  - Downtime = time between last shutdown event and next boot event in the System log.'
Write-Host '  - Event 1074 Type "planned" = scheduled/user shutdown with reason; otherwise unplanned.'
Write-Host '  - Compare BootTime / ShutdownTime with your Grafana telemetry gap.' -ForegroundColor DarkGray

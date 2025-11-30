[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, HelpMessage = "Provide the version number in the format 'X.Y.Z', e.g., '3.0.0'.")]
    [string]$VersionNumber,

    [Parameter(Mandatory = $true)]
    [bool]$IsPreview
)

# Validate version number format X.Y.Z
if ($VersionNumber -notmatch '^\d+\.\d+\.\d+$') {
    Write-Host "❌ Error: Version number must be in the format 'X.Y.Z', e.g., '3.0.0'." -ForegroundColor Red
    exit 1
}

# Build preview suffix (valid SemVer identifiers, no leading zeros)
$DatePart  = (Get-Date -Format "yyyyMMdd")   # e.g 20250822
$TimePart  = (Get-Date -Format "Hmm")        # e.g 845 (not 0845, no leading zero!)
$PreviewId = "$DatePart$TimePart"            # → "20250822845" (no dots!)

# Construct the tag
if ($IsPreview) {
    # format: vX.Y.Z-previewYYYYMMDDHHmm (NuGet-compliant)
    $TagName = "v$VersionNumber-preview$PreviewId"
} else {
    $TagName = "v$VersionNumber"
}
$Message = "Release version $TagName"

# Confirm with user
Write-Host "🚀 You are about to release:" -ForegroundColor Yellow
Write-Host "   Tag:     $TagName"   -ForegroundColor Cyan
Write-Host "   Message: $Message"   -ForegroundColor Cyan
$response = Read-Host "Are you sure you want to proceed? (yes/no)"

if ($response -ne "yes") {
    Write-Host "❌ Release canceled by the user." -ForegroundColor Red
    exit 0
}

# Git commands
Write-Host "🔖 Creating tag $TagName with message: $Message"
git tag -a $TagName -m $Message

Write-Host "⬆️  Pushing tag $TagName to origin"
git push origin $TagName

Write-Host "✅ Tag $TagName pushed successfully." -ForegroundColor Green

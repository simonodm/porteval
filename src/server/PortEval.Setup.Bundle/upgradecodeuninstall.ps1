param (
 [parameter(Mandatory=$true)]
 [string]$UpgradeCode
)
$guidOrder = @(7, 6, 5, 4, 3, 2, 1, 0, 11, 10, 9, 8, 15, 14, 13, 12, 17, 16, 19, 18, 21, 20, 23, 22, 25, 24, 27, 26, 29, 28, 31, 30);

$UpgradeCodeStripped = $UpgradeCode -replace "[-{}]", "";
$UpgradeCodeCompressed = "";
for ($i = 0; $i -lt $UpgradeCodeStripped.Length; $i++) {
 $UpgradeCodeCompressed += $UpgradeCodeStripped.Substring($guidOrder[$i], 1);
}

if (Test-Path -Path "HKLM:\Software\Classes\Installer\UpgradeCodes\$UpgradeCodeCompressed") {
 $ProductCodeCompresssed = (Get-Item -Path "HKLM:\Software\Classes\Installer\UpgradeCodes\$UpgradeCodeCompressed").Property[0];
 $ProductCode = "{";
 for ($i = 0; $i -lt $ProductCodeCompresssed.Length; $i++) {
 $ProductCode += $ProductCodeCompresssed.Substring($guidOrder[$i], 1);
 }
 $ProductCode = $ProductCode.Insert(9, '-').Insert(14, '-').Insert(19, '-').Insert(24, '-').Insert(37, '}');
}

if ((Test-Path -Path "HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\$ProductCode") -or (Test-Path -Path "HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\$ProductCode")) {
 $proc = Start-Process -FilePath "msiexec.exe" -ArgumentList "/x", "$ProductCode", "/q", "RESTART=ReallySuppress" -PassThru -Wait;
 exit 99;
} else {
 exit 0;
}
#
# Substitute the version number for each AssemblyInfo.cs and project.json file
#
param (
	[string]$assemblyVersion = "1.0.0.0",
	[string]$packageVersion = "1.0.0-dev.0"
)
echo $assemblyVersion
echo $packageVersion
$files = Get-ChildItem src/TetrisSharp -include Build.props -Recurse
foreach ($file in $files)
{
	(Get-Content $file.FullName) |
	ForEach-Object { $_ -replace "<TetrisPackageVersion>0.999.0-dev</TetrisPackageVersion>", "<TetrisPackageVersion>$($packageVersion)</TetrisPackageVersion>" } |
	Set-Content $file.FullName

	(Get-Content $file.FullName) |
	ForEach-Object { $_ -replace "<TetrisAssemblyVersion>0.999.0.0</TetrisAssemblyVersion>", "<TetrisAssemblyVersion>$($assemblyVersion)</TetrisAssemblyVersion>" } |
	Set-Content $file.FullName

	(Get-Content $file.FullName) |
	ForEach-Object { $_ -replace "<TetrisAssemblyFileVersion>0.999.0.0</TetrisAssemblyFileVersion>", "<TetrisAssemblyFileVersion>$($assemblyVersion)</TetrisAssemblyFileVersion>" } |
	Set-Content $file.FullName
}

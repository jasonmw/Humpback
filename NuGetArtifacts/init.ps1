param($installPath, $toolsPath, $package)
Set-Alias hump (Join-Path $toolsPath hump.exe)

$sorted_list = new-object system.collections.SortedList
$parent_path = Join-Path $installPath ".."
foreach($f in Get-ChildItem $parent_path -Filter Humpback* | Foreach {$_.FullName}){
	$sorted_list.Add($f,$f)
}
if($sorted_list.Count -gt 1){
	$old_path = $sorted_list.Values[$sorted_list.Count - 2]
	$new_path = Join-Path $installPath "tools"
	$current_settings = Join-Path $new_path "settings.js"
	$has_current_settings = Test-Path $current_settings
	if($has_current_settings -eq $false){
		$old_settings = Join-Path $old_path "tools\settings.js"
		Copy-Item $old_settings  $new_path
	}
}


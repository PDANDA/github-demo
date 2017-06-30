cd ..\..
$currentDir = Convert-Path .
$toContentDir = Join-Path $currentDir "content"
$toProjectName = Split-Path $currentDir -Leaf
$toProjectFile = Join-Path $currentDir "$($toProjectName).csproj"
$fromProjectName = $toProjectName.Replace(".NuGet.Packager", "")
$parentDir = Join-Path $currentDir ".."
$fromProjectDir = Join-Path $parentDir $fromProjectName

$excludes = "bin", "obj", "Properties"
Get-ChildItem $fromProjectDir | `
    Where-Object{$_.Name -notin $excludes} | `
    Copy-Item -Destination $toContentDir -Recurse -Force

$toProject = [xml](Get-Content $toProjectFile)
$toProjectItemGroup = $toProject.SelectSingleNode("//Project/ItemGroup[@Label=`"NuGetPackageContent`"]")
$ns = "http://schemas.microsoft.com/developer/msbuild/2003"

Get-ChildItem $toContentDir -Include @("*.*") -Recurse | `
    Select-Object FullName | foreach { `
        $contentPath = $_.FullName.Replace($currentDir, "").TrimStart('\')
        $target = $toProject.SelectSingleNode("//Project/ItemGroup[@Label=`"NuGetPackageContent`"]/None[@Include=`"$($contentPath)`"]")
        if ([string]::IsNullOrEmpty($target.OuterXml)) {
            if ([string]::IsNullOrEmpty($toProjectItemGroup.OuterXml)) {
                $toProjectItemGroup = $toProject.CreateElement("ItemGroup", $ns)
                $toProject.Project.AppendChild($toProjectItemGroup)
                $toProjectItemGroupLabel = $toProject.CreateAttribute("Label")
                $toProjectItemGroupLabel.Value = "NuGetPackageContent"
                $toProjectItemGroup.Attributes.Append($toProjectItemGroupLabel)
            }
            $contentNode = $toProject.CreateElement("None", $ns)
            $toProjectItemGroup.AppendChild($contentNode)
            $contentNodeInclude = $toProject.CreateAttribute("Include")
            $contentNodeInclude.Value = $contentPath
            $contentNode.Attributes.Append($contentNodeInclude)
        }
    }
$toProject.Save($toProjectFile)

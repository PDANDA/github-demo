// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake
open System.IO

// Directories
let buildDir  = "./build/"
let deployDir = "./deploy/"


// Filesets
let appReferences  =
    !! "/**/*.csproj"
    ++ "/**/*.fsproj"

// version info
let version = "0.1"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir]
)

Target "Build" (fun _ ->
    // compile all projects below src/app/
    MSBuildDebug buildDir "Build" appReferences
    |> Log "AppBuild-Output: "
)

Target "Deploy" (fun _ ->
    !! (buildDir + "/**/*.*")
    -- "*.zip"
    |> Zip buildDir (deployDir + "ApplicationName." + version + ".zip")
)

// Target "CreatePackage" (fun _ ->
//     File.Copy("""C:\Program Files (x86)\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.0.0\FSharp.Core.optdata""", __SOURCE_DIRECTORY__ + """build\FSharp.Core.optdata""")
//     File.Copy("""C:\Program Files (x86)\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.0.0\FSharp.Core.sigdata""", __SOURCE_DIRECTORY__ + """build\FSharp.Core.sigdata""") 

//     NuGet (fun p -> { p with
//                         Authors = ["Stuart Davies"];
//                         Project = "Interprit.Common";
//                         Description = "Common FSharp Interprit Library";
//                         OutputPath = __SOURCE_DIRECTORY__ + "\nugets";                        
//                         WorkingDir = __SOURCE_DIRECTORY__;
//                         Version = "1.4"                        
//                         Publish = true }) "Interprit.Common.1.4.nuspec")

// Build order
"Clean"
  ==> "Build"
  ==> "Deploy"

// start build
RunTargetOrDefault "Build"

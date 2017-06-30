copy "C:\Program Files (x86)\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.0.0\FSharp.Core.optdata" build\FSharp.Core.optdata
copy "C:\Program Files (x86)\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.4.0.0\FSharp.Core.sigdata" build\FSharp.Core.sigdata 
.paket\paket pack output nugets version 1.0.674
nugets\nuget.exe push -Source "LinfoxHorizon" -ApiKey VSTS nugets\Interprit.Common.1.0.674.nupkg
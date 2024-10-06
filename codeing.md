# Create folder

Solution folder IdleGame

## Rest Api & Model

Use console in folder to create and add

`dotnet new sln`

`dotnet new classlib -o Model`

`dotnet sln add Model`

`dotnet new webapi -o WorldApi`

`dotnet sln add WorldApi`

https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-sln

## Add Project Reference

Use console

`cd WorldApi`

`dotnet add WorldApi.csproj reference ../Model/Model.csproj`

`cd ..`

## Build

`dotnet build`

## More

`dotnet new classlib -o Storage`

`dotnet sln add Storage`

`dotnet add WorldApi/WorldApi.csproj reference Storage/Storage.csproj`

`dotnet new classlib -o Generator`

`dotnet sln add Generator`
`dotnet add Generator/Generator.csproj reference Model/Model.csproj`
`dotnet add WorldApi/WorldApi.csproj reference Generator/Generator.csproj`

`cd Storage`

`dotnet add package Microsoft.EntityFrameworkCore.Sqlite`

`dotnet add WorldApi/WorldApi.csproj reference DebugMapGenerator/DebugMapGenerator.csproj`

`dotnet add DebugMapGenerator/DebugMapGenerator.csproj reference Generator/Generator.csproj`

`dotnet new classlib -o MapLogistics`
`dotnet sln add MapLogistics`
`dotnet add MapLogistics/MapLogistics.csproj reference Model/Model.csproj`
`dotnet add MapLogistics/MapLogistics.csproj reference Storage/Storage.csproj`
`dotnet add MapLogistics/MapLogistics.csproj reference Generator/Generator.csproj`
`dotnet add WorldApi/WorldApi.csproj reference MapLogistics/MapLogistics.csproj`

## Console App Proj

dotnet new consoleasync -n MyProject
`dotnet new console -o Janitor`
`dotnet sln add Janitor`

`dotnet add Janitor/Janitor.csproj reference Model/Model.csproj`
`dotnet add Janitor/Janitor.csproj reference Storage/Storage.csproj`
`dotnet add Janitor/Janitor.csproj reference Generator/Generator.csproj`

## Picture Generation with ComfyUI & Stable Diffusion

https://github.com/comfyanonymous/ComfyUI/issues/930

ComfyUI plain install
or
ComfyUiStandalone

## LLM with LM studio and LlaMa

https://lmstudio.ai/

## Documetation in Swagger

`dotnet add WorldApi/WorldApi.csproj package Swashbuckle.AspNetCore -v 6.6.1`

## Extensions

Added NuGet Gallery for easy installation of AutoGen.Core & AutoGen.LMStudio

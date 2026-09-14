@echo off

dotnet build src/Limbo.Umbraco.Spa --configuration Release /t:rebuild /t:pack -p:PackageOutputPath=../../releases/nuget
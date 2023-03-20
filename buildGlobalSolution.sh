rm -rf Lisergy-RTS.sln
dotnet new sln
dotnet sln add LisergyServer/**/*.csproj
dotnet sln add LisergyClient/**/*.csproj
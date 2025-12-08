# Install if not installed
# dotnet tool install --global dotnet-validate --prerelease 
# from solution root run:
dotnet clean -c Release
dotnet build -c Release
dotnet pack -c Release
dotnet-validate package local .\CryptoNet\bin\Release\*.nupkg
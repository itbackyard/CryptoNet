# Install if not installed
# dotnet tool install --global dotnet-validate --prerelease 
# from solution root run:
dotnet pack -c Release
dotnet-validate package local .\CryptoNet\bin\Release\*.nupkg
echo "Compiling project..."
dotnet publish ./Stride.CLI/Stride.CLI.csproj -c Release -r osx-arm64 --self-contained true -o ./build

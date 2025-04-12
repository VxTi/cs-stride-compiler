echo "Compiling project..."

dotnet publish ./Stride.CLI/Stride.CLI.csproj \
  -c Release \
  -r osx-arm64 \
  --self-contained true \
  /p:PublishSingleFile=true \
  /p:PublishTrimmed=true \
  /p:EnableCompressionInSingleFile=true \
  -o /Users/"$USER"/bin
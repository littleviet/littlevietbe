$SecretsPath = Read-Host "Please enter the path to your secrets json file (e.g. C:/file.json)"
type $SecretsPath | dotnet user-secrets set --project ..\..\LittleViet.Api\

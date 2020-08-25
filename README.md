# identity-server
An ASP.NET core Identity server:

- Should use Windows Terminal to run multiple power shell windows
- Execute below command on each power shell window:
 + dotnet watch --project .\WebApi.csproj run
 + dotnet watch --project .\IdentityServer.csproj run
 + dotnet run  (in ClientConsole folder)

Note: 
Run: "dotnet dev-certs https --trust" command to make a certificate for https request if you have not any cert

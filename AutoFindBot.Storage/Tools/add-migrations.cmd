// ef commands templates for the Rider's terminal

cd AutoFindBot.Storage
dotnet restore
dotnet ef -h
dotnet ef migrations add Initial --verbose --project ../AutoFindBot.Storage --startup-project ../AutoFindBot.Web
dotnet ef database update --verbose --project ../AutoFindBot.Storage --startup-project ../AutoFindBot.Web
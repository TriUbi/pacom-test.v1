# pacom-test.v1 - Webb App för att hantera en Modbus-simulator

C# och Blazor
👉🏻Detta projekt är ett fullstack web app för att visa och styra enheter i ett sjukhusmiljö via en Modbus-simulator.
Det innehåller:

- 🔧 Backend i **ASP.NET Core Web API** (`DeviceStatusApi`)
- 💻 Frontend i **Blazor Server** (`DeviceFrontend`)
- 🐳 En **Modbus TCP-simulator** som körs i Docker
- 🗄️ MySQL-databas (via MAMP)

## Steg-för-steg: Kom igång / Step-by-step: Get started

Innan du börjar, se till att ha:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [MAMP](https://www.mamp.info/) – för MySQL (port 8889)
- (Valfritt) Visual Studio Code

### Starta frontend lokalt

````bash
cd DeviceFrontend
dotnet build
dotnet run

### Starta backend lokalt

```bash
cd DeviceStatusApi
dotnet build
dotnet run

### Starta Modbus Simulator (Docker)
```bash
docker run -p 5020:5020 -it --rm oitc/modbus-server


## 👤 Inloggning

Använd dessa uppgifter:
- användarnamn = admin
- lösernord = 1234


````

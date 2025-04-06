# pacom-test.v1 - Webb App för att hantera en Modbus-simulator

C# och Blazor
👉🏻Detta projekt är ett fullstack web app för att visa och styra enheter i ett sjukhusmiljö via en Modbus-simulator.

- 🔧 Backend i **ASP.NET Core Web API** (`DeviceStatusApi`)
- 💻 Frontend i **Blazor Server** (`DeviceFrontend`)
- 🐳 En **Modbus TCP-simulator** som körs i Docker
- 🗄️ MySQL-databas (via MAMP)

## Steg-för-steg: Kom igång

Innan du börjar, se till att ha:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [MAMP](https://www.mamp.info/) – för MySQL 

### Starta frontend

```bash
cd DeviceFrontend
dotnet build
dotnet run
```

### Starta backend 

```bash
cd DeviceStatusApi
dotnet build
dotnet run
```
## Starta Modbus Simulator (Docker)
### Alternativ 1 - Manuell körning

```bash
docker run -p 5020:5020 -it --rm oitc/modbus-server
```

### Alternativ 2 - då docker-compose.yml finns 
```bash
docker compose up -d
```

## 👤 Inloggning

Använd dessa uppgifter:

- användarnamn = admin
- lösernord = 1234




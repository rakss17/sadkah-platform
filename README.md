# Sadkah

Sadkah is a crowdfunding/donation platform made up of two projects:

- **Sadkah.API** — ASP.NET Core Web API (auth, campaigns, donations, donation methods, Cloudinary image uploads).
- **Sadkah.Web** — Blazor Server frontend that consumes the API.

## Prerequisites

- .NET 10 SDK
- SQL Server / SQL Server Express (the default connection string uses Integrated Security against a local `SQLEXPRESS` instance)
- A [Cloudinary](https://cloudinary.com/) account (for donation method QR code uploads)

## Required secrets

`Sadkah.API/appsettings.json` intentionally does **not** contain credentials. Before running the API, configure these via [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) (loaded automatically in the `Development` environment):

```bash
cd Sadkah.API
dotnet user-secrets init   # only needed once, already done for this repo
dotnet user-secrets set "Cloudinary:ApiKey" "<your-cloudinary-api-key>"
dotnet user-secrets set "Cloudinary:ApiSecret" "<your-cloudinary-api-secret>"
dotnet user-secrets set "JWT:SigningKey" "<a-long-random-string>"
```

| Key | Purpose |
|---|---|
| `Cloudinary:ApiKey` | Cloudinary API key (`Cloudinary:CloudName` stays in `appsettings.json`, it isn't sensitive) |
| `Cloudinary:ApiSecret` | Cloudinary API secret |
| `JWT:SigningKey` | Symmetric key used to sign/validate access tokens |

Without these, `Sadkah.API` will throw on startup (`Cloudinary:CloudName/ApiKey/ApiSecret is not configured`) or silently fall back to an insecure default JWT key.

For non-Development environments, set the equivalent environment variables instead (double underscore separates config sections):

```bash
Cloudinary__ApiKey=...
Cloudinary__ApiSecret=...
JWT__SigningKey=...
```

## Running locally

```bash
# Terminal 1 — API (http://localhost:5276)
cd Sadkah.API
dotnet ef database update   # apply migrations
dotnet run
or
dotnet watch run

# Terminal 2 — Web (http://localhost:5147)
cd Sadkah.Web
dotnet run
or
dotnet watch run
```

`Sadkah.Web` talks to the API via `Api:BaseUrl` in `Sadkah.Web/appsettings.json`, which defaults to `http://localhost:5276`.

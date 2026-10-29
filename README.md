# IssuePortal 🚧

Ett system för att hantera projekt, uppgifter (issues) och kommentarer, med inloggning och rollbaserad behörighet.

> **Status:** Under utveckling. Backend-API:t (MVP) är klart. Frontend, tester, Docker och CI/CD är nästa steg.

## 🎯 Syfte

Jag bygger IssuePortal för att lära mig hela kedjan i ett riktigt system, steg för steg:

- Backendutveckling
- Frontendutveckling
- Databaser
- Docker
- DevOps

## 🛠️ Teknik

| Del | Teknik |
|---|---|
| Backend | C#, ASP.NET Core Web API (.NET 10), Entity Framework Core |
| Databas | PostgreSQL |
| Autentisering | JWT, rollbaserad behörighet (User / Developer / Admin) |
| Frontend (planerad) | React, TypeScript, Tailwind CSS |

## ✨ Funktioner

- Registrering och inloggning med JWT
- Projekt med medlemmar
- Issues med status, prioritet, tilldelning och filtrering (t.ex. "mina issues")
- Kommentarer på issues
- Behörighet per projekt: man ser bara projekt man är medlem i
- Databas med migrationer (PostgreSQL och EF Core)
- CORS konfigurerat för den kommande frontenden

### Roller

| Roll | Kan göra |
|---|---|
| User | Skapa issues och kommentera |
| Developer | Uppdatera issues |
| Admin | Hantera användare, roller och projekt |

## 📊 Översikt

| Del | Status |
|---|---|
| Backend-API (MVP) | ✅ Klart |
| Databas och migrationer | ✅ Klart |
| Säkerhet (JWT, roller, behörighet per projekt) | ✅ Klart |
| Dokumentation | ✅ Klart |
| Automatiska tester | ⏳ Ej påbörjat |
| Frontend | ⏳ Ej påbörjat |
| Docker | ⏳ Ej påbörjat |
| DevOps (CI/CD) | ⏳ Ej påbörjat |

## 🗺️ Roadmap

### Backend: sista finputsen
- [ ] Validering vid registrering (lösenordslängd, e-postformat)
- [ ] Tydligt felmeddelande när en användare med kommentarer tas bort
- [ ] Uppdatera `.http`-filen med exempel som använder token

### Frontend (React + TypeScript + Tailwind)
- [ ] Grundsetup med Vite, routing och en API-klient som skickar med token
- [ ] Inloggning och registrering
- [ ] Projektlista och projektvy med issues och filter
- [ ] Issue-detalj med kommentarer och statusändring
- [ ] "Mina issues"
- [ ] Adminsidor för användare, roller och projektmedlemmar

### Tester
- [ ] xUnit-tester för services och behörighetsregler

### Docker
- [ ] Dockerfile för backenden
- [ ] `docker-compose.yml` som startar API och PostgreSQL

### DevOps
- [ ] GitHub Actions som bygger och testar vid varje push
- [ ] Deployment

## 🚀 Kom igång (backend)

### Krav

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL
- `dotnet-ef`:

```bash
dotnet tool install --global dotnet-ef
```

### Installation

```bash
cd Backend/IssuePortal.Api

# Hemligheter (sparas utanför repot)
dotnet user-secrets set "Jwt:Key" "<en lång slumpmässig nyckel, minst 32 tecken>"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=IssuePortalDb;Username=postgres;Password=<ditt lösenord>"

# Skapa databasen och starta API:t
dotnet ef database update
dotnet run
```

### Testa API:t i Swagger

Öppna <http://localhost:5126/swagger> och:

1. Registrera en användare med `POST /api/auth/register`.
2. Logga in med `POST /api/auth/login` och kopiera token.
3. Klicka på **Authorize** och klistra in token.

### Skapa den första admin-användaren

Den första admin-användaren sätts direkt i databasen:

```sql
UPDATE "Users" SET "Role" = 'Admin' WHERE "Email" = 'din@email.com';
```

Logga sedan in igen, så att din token får den nya rollen.

## 📚 Dokumentation

- [Kravspecifikation](docs/kravspecifikation.md)
- [Systemdesign](docs/systemdesign.md)
- [Databasdesign](docs/databasdesign.md)
- [API-design](docs/api-design.md): alla endpoints, behörigheter och exempel

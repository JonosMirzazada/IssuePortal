# IssuePortal 🚧

> **Status: Under utveckling**

Ett system för att hantera projekt och uppgifter.

## 🎯 Mitt mål

Jag bygger detta projekt för att utveckla mina kunskaper inom:

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

## 🚧 Projektstatus

**Projektet är under utveckling och är ännu inte färdigt.**

Jag bygger projektet steg för steg och lägger kontinuerligt till nya funktioner, förbättringar och dokumentation.

### 📊 Översikt

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

### ✅ Klart

- **Backend-API för MVP:n**
  - Registrering och inloggning med JWT
  - Projekt med medlemmar
  - Issues med status, prioritet, tilldelning och filtrering (t.ex. "mina issues")
  - Kommentarer
- **Roller**
  - User skapar issues och kommenterar.
  - Developer uppdaterar issues.
  - Admin hanterar användare, roller och projekt.
- **Behörighet per projekt:** man ser bara projekt man är medlem i.
- **Databas med migrationer:** PostgreSQL och EF Core.
- **CORS** för den kommande frontenden.
- **Dokumentation** av krav, systemdesign, databas och API.

### 🗺️ Roadmap

1. **Backend: sista finputsen**
   - [ ] Validering vid registrering (lösenordslängd, e-postformat)
   - [ ] Tydligt felmeddelande när en användare med kommentarer tas bort
   - [ ] Uppdatera `.http`-filen med exempel som använder token
2. **Frontend** (React + TypeScript + Tailwind)
   - [ ] Grundsetup med Vite, routing och en API-klient som skickar med token
   - [ ] Inloggning och registrering
   - [ ] Projektlista och projektvy med issues och filter
   - [ ] Issue-detalj med kommentarer och statusändring
   - [ ] "Mina issues"
   - [ ] Adminsidor för användare, roller och projektmedlemmar
3. **Tester**
   - [ ] xUnit-tester för services och behörighetsregler
4. **Docker**
   - [ ] Dockerfile för backenden
   - [ ] `docker-compose.yml` som startar API och PostgreSQL
5. **DevOps**
   - [ ] GitHub Actions som bygger och testar vid varje push
   - [ ] Deployment

## 🚀 Kom igång (backend)

Krav: [.NET 10 SDK](https://dotnet.microsoft.com/), PostgreSQL och `dotnet-ef`, som installeras med `dotnet tool install --global dotnet-ef`.

```bash
cd Backend/IssuePortal.Api

# Hemligheter (sparas utanför repot)
dotnet user-secrets set "Jwt:Key" "<en lång slumpmässig nyckel, minst 32 tecken>"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=IssuePortalDb;Username=postgres;Password=<ditt lösenord>"

# Skapa databasen och starta API:t
dotnet ef database update
dotnet run
```

Öppna sedan **http://localhost:5126/swagger** för att testa API:t:

1. Registrera en användare med `POST /api/auth/register`.
2. Logga in med `POST /api/auth/login` och kopiera `token`.
3. Klicka på **Authorize** och klistra in token.

Den första admin-användaren sätts direkt i databasen:

```sql
UPDATE "Users" SET "Role" = 'Admin' WHERE "Email" = 'din@email.com';
```

Logga sedan in igen, så att din token får den nya rollen.

## 📚 Dokumentation

- [Kravspecifikation](Documentation/Requirements.md)
- [Systemdesign](Documentation/SystemDesign.md)
- [Databasdesign](Documentation/DatabaseDesign.md)
- [API-design](Documentation/ApiDesign.md): alla endpoints, behörigheter och exempel

> 🔨 **Work in Progress** — Projektet utvecklas fortfarande.

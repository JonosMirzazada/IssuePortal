# IssuePortal 🚧

Ett system för att hantera projekt, uppgifter (issues) och kommentarer, med inloggning och rollbaserad behörighet.

> **Status:** Under utveckling. Backend-API:t och frontenden är klara. Tester, Docker och CI/CD är nästa steg.

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
| Frontend | React, TypeScript, Vite, Tailwind CSS, React Router |

## ✨ Funktioner

- Registrering och inloggning med JWT
- Projekt med medlemmar
- Issues med status, prioritet, tilldelning och filtrering (t.ex. "mina issues")
- Kommentarer på issues
- Behörighet per projekt: man ser bara projekt man är medlem i
- Databas med migrationer (PostgreSQL och EF Core)

### I webbappen

- Registrera konto, logga in och logga ut
- Se sina projekt och skapa nya
- Projektvy med medlemmar och issues, och formulär för nya issues
- Issue-vy med kommentarer, statusändring, prioritet och "tilldela mig"
- "Mina issues": alla issues som är tilldelade en själv
- Adminsida för att ändra användares roller
- Admin kan lägga till medlemmar i projekt och ta bort issues
- Knappar visas bara för den som har behörighet att använda dem
- Automatisk utloggning när token har gått ut
- Tydliga meddelanden vid laddning och fel, t.ex. när servern inte går att nå

### Roller

| Roll | Kan göra |
|---|---|
| User | Skapa issues och kommentera |
| Developer | Som User, plus uppdatera issues (status, prioritet, tilldelning) |
| Admin | Som Developer, plus hantera användare, roller, projektmedlemmar och ta bort issues. Ser alla projekt. |

## 📊 Översikt

| Del | Status |
|---|---|
| Backend-API (MVP) | ✅ Klart |
| Databas och migrationer | ✅ Klart |
| Säkerhet (JWT, roller, behörighet per projekt) | ✅ Klart |
| Dokumentation | ✅ Klart |
| Frontend | ✅ Klart |
| Automatiska tester | ⏳ Ej påbörjat |
| Docker | ⏳ Ej påbörjat |
| DevOps (CI/CD) | ⏳ Ej påbörjat |

## 🗺️ Roadmap

### Backend: sista finputsen
- [ ] Validering vid registrering (lösenordslängd, e-postformat)
- [ ] Tydligt felmeddelande när en användare med kommentarer tas bort
- [ ] Uppdatera `.http`-filen med exempel som använder token

### Frontend (React + TypeScript + Tailwind)
- [x] Grundsetup med Vite, routing och en API-klient som skickar med token
- [x] Inloggning och registrering
- [x] Projektlista och projektvy med issues
- [x] Issue-detalj med kommentarer och statusändring
- [x] "Mina issues"
- [x] Adminsidor för användare, roller och projektmedlemmar
- [ ] Anpassad layout för mobil
- [ ] Arkivering av projekt (istället för att ta bort dem)

### Tester
- [ ] xUnit-tester för services och behörighetsregler

### Docker
- [ ] Dockerfile för backenden
- [ ] `docker-compose.yml` som startar API och PostgreSQL

### DevOps
- [ ] GitHub Actions som bygger och testar vid varje push
- [ ] Deployment

## 🚀 Kom igång

Appen består av två delar som körs samtidigt, i var sin terminal:

| Del | Mapp | Kommando | Adress |
|---|---|---|---|
| Backend | `Backend/IssuePortal.Api` | `dotnet run` | <http://localhost:5126> |
| Frontend | `Frontend` | `npm run dev` | <http://localhost:5173> |

### Krav

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL
- `dotnet-ef`:

```bash
dotnet tool install --global dotnet-ef
```

- [Node.js](https://nodejs.org/) (LTS), till exempel installerat med [nvm](https://github.com/nvm-sh/nvm)

### 1. Backend

```bash
cd Backend/IssuePortal.Api

# Hemligheter (sparas utanför repot)
dotnet user-secrets set "Jwt:Key" "<en lång slumpmässig nyckel, minst 32 tecken>"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=IssuePortalDb;Username=postgres;Password=<ditt lösenord>"

# Skapa databasen och starta API:t
dotnet ef database update
dotnet run
```

### 2. Frontend

I en ny terminal:

```bash
cd Frontend
npm install
npm run dev
```

Öppna <http://localhost:5173> och registrera ett konto.

> **Obs:** Använd `localhost`, inte `127.0.0.1`. Backendens CORS tillåter bara `http://localhost:5173`.

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

Logga sedan ut och in igen, så att din token får den nya rollen. Därefter kan fler roller ändras direkt på adminsidan i webbappen.

## 💡 Designbeslut

- **Projekt kan inte tas bort från webbappen.** Att ta bort ett projekt raderar också alla dess issues, kommentarer och medlemskap (cascade delete). Det är riskabelt i ett riktigt team, så istället är arkivering planerad.
- **Säkerheten ligger i backenden.** Frontenden döljer knappar som användaren inte får använda, men det är backenden som faktiskt skyddar datan (401/403).
- **Rollen ligger i token.** En ändrad roll gäller först efter att användaren loggat in igen.

## 📚 Dokumentation

- [Kravspecifikation](Documentation/Requirements.md)
- [Systemdesign](Documentation/SystemDesign.md)
- [Databasdesign](Documentation/DatabaseDesign.md)
- [API-design](Documentation/ApiDesign.md): alla endpoints, behörigheter och exempel

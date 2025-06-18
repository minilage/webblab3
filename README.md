# ☕ The Loading Bean – E-handelsapplikation

En fullstack webbshop byggd i .NET 8 och Blazor WebAssembly för en mysig och nördig kaffeupplevelse.  
Utvecklad som en del av min .NET-fullstackutbildning på IT-Högskolan.

---

## ✨ Funktioner

### Produkthantering
- Visa alla produkter
- Lägg till, uppdatera och ta bort produkter (endast admin)
- Markera produkter som utgångna
- Produkterna är kategoriserade (t.ex. kaffe, te, muggar)

### Kundhantering
- Registrera konto och logga in
- Uppdatera din profil
- Se din orderhistorik
- Säker inloggning med JWT och rollbaserad åtkomst

### Orderhantering
- Lägg beställningar från varukorgen
- Se orderhistorik med totalsumma och innehåll
- Admin kan se alla kunders ordrar

---

## 🛡️ Säkerhet

- JWT-baserad autentisering
- Rollbaserad behörighet ("Admin" och "User")
- Skyddade endpoints i både API och Blazor-klienten

---

## ⚙️ Teknisk stack

### Backend

- ASP.NET Core 8.0 Web API
- MongoDB Atlas
- Repository Pattern & Unit of Work
- JWT (System.IdentityModel.Tokens.Jwt)

### Frontend

- Blazor WebAssembly (.NET 8)
- MudBlazor-komponenter för UI
- Blazored.LocalStorage + Blazored.Toast

---

## 🚀 Kom igång

### Förutsättningar

- .NET 8 SDK  
- Konto på MongoDB Atlas  
- Visual Studio 2022 eller senare

### MongoDB Atlas-konfiguration

1. Gå till [https://cloud.mongodb.com](https://cloud.mongodb.com) och logga in
2. Använd anslutningssträngen (lagrad **utanför denna README**, se separat inlämning för inloggning)
3. Uppdatera `appsettings.json` i TheLoadingBean.API så här:

```json
"MongoDB": {
  "ConnectionString": "<din MongoDB Atlas connection string>",
  "DatabaseName": "TheLoadingBeanDB"
}
```

> ⚠️ **För bedömning:** En testanvändare finns tillgänglig och är dokumenterad i den separata inlämningen.

### Starta applikationen

```bash
# Starta API
cd TheLoadingBean.API
dotnet run

# Starta klienten
cd TheLoadingBean.Client
dotnet run
```

---

## 👩‍💻 Utvecklare

**Tina Lagesson**  
tina.lagesson@gmail.com

Detta projekt är utvecklat med kärlek (och koffein) som en del av min .NET-fullstackutbildning.

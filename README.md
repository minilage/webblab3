
---

## ✅ `README.md`

```markdown
# ☕ The Loading Bean – E-Commerce Web Application

A fullstack webshop built in .NET 8 and Blazor WebAssembly for a cozy and nerdy coffee experience.  
Created as part of my .NET fullstack course at IT-Högskolan.

---

## ✨ Features

### Product Management
- View all products
- Add, update, and delete products (Admin only)
- Mark products as discontinued
- Products are categorized (e.g., Coffee, Tea, Mugs)

### Customer Management
- Register and log in
- Update your profile
- View your order history
- Secure JWT authentication with role-based access

### Order Management
- Place orders from your cart
- See order history with total and content
- Admins can view all customer orders

---

## 🛡️ Security

- JWT-based authentication
- Role-based authorization (`Admin` and `User`)
- Protected routes in both API and Blazor frontend

---

## ⚙️ Tech Stack

### Backend

- ASP.NET Core 8.0 Web API
- MongoDB (hosted on Atlas)
- Repository Pattern & Unit of Work
- JWT (System.IdentityModel.Tokens.Jwt)

### Frontend

- Blazor WebAssembly (.NET 8)
- MudBlazor components for UI
- Blazored.LocalStorage + Blazored.Toast

---

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK  
- MongoDB Atlas or MongoDB Compass  
- Visual Studio 2022 or newer

### MongoDB Setup

1. Create a new database: `TheLoadingBean`
2. Import the JSON collections:  
   - `TheLoadingBeanDb.Products.json`  
   - `TheLoadingBeanDb.Customers.json`  
   - `TheLoadingBeanDb.Orders.json`

3. Update the connection string in `appsettings.json`.

### Run the App

```bash
# Start the API
cd TheLoadingBean.API
dotnet run

# Start the client
cd TheLoadingBean.Client
dotnet run

Tina Lagesson
tina.lagesson@gmail.com

This project was developed with love (and caffeine) as part of my .NET fullstack course.
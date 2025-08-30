# Incident Hub

Incident Hub is a full-stack application for tracking, creating, and managing incidents.  
It’s built as a **.NET 9 Web API** backend with **PostgreSQL** and a **Next.js 15 (TypeScript + Tailwind CSS)** frontend.  

---

## 🚀 Features
- ✅ **.NET 9 Web API** with Entity Framework Core and PostgreSQL  
- ✅ Database migrations via **EF Core**  
- ✅ **Swagger UI** for exploring and testing API endpoints  
- ✅ **Next.js 15** frontend with App Router, TypeScript, Tailwind CSS  
- ✅ Create and list incidents through a clean UI  
- ✅ API & Web apps run together using environment variables and Docker  

---

## 🏗 Project Structure
```
incident-hub/
  api/
    IncidentHub.Api/     # .NET 9 Web API (C#, EF Core, Swagger)
  web/
    incident-hub-web/    # Next.js 15 frontend (TypeScript, Tailwind CSS)
  docker-compose.yml     # Postgres service
```

---

## ⚙️ Setup

### 1. Clone the repo
```bash
git clone https://github.com/<your-username>/incident-hub.git
cd incident-hub
```

### 2. Start the database
Make sure you have [Docker](https://www.docker.com/products/docker-desktop/) running.

```bash
docker compose up -d
```

This runs PostgreSQL 16 with a persistent volume.

---

### 3. Run the API
```bash
cd api/IncidentHub.Api
dotnet restore
dotnet ef database update   # apply migrations
dotnet watch run
```

The API will be available at:

- Swagger: [http://localhost:5290/swagger](http://localhost:5290/swagger)  
- GET `/incidents` → list incidents  
- POST `/incidents` → create a new incident  

---

### 4. Run the Web App
```bash
cd web/incident-hub-web
npm install
npm run dev
```

Frontend will be available at:  
👉 [http://localhost:3000/incidents](http://localhost:3000/incidents)

---

## 🔑 Environment Variables

### API
Set your DB connection string in `appsettings.Development.json` (or via Docker env vars).

### Web
Create `.env.local` in `web/incident-hub-web`:
```
NEXT_PUBLIC_API_BASE_URL=http://localhost:5290
```

---

## 👤 Author
**Collins Ilo**  
Senior Software Developer | React | JavaScript | .NET Enthusiast  

---

## 📄 License
This project is licensed under the MIT License.

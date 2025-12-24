# 🏙️ City Secrets

> Discover and share hidden gems in your city

A community-driven platform where locals share their favorite spots and visitors discover authentic experiences beyond typical tourist destinations.

## ✨ Features

- 🔍 Browse and search places by category and location
- ⭐ Write reviews and rate places
- 📍 Find nearby hidden gems with location-based search
- 💝 Save favorite places
- 🎯 Get personalized recommendations
- 🛡️ Admin moderation tools

## 🛠️ Tech Stack

**Backend:** ASP.NET Core 8.0, Entity Framework Core, SQL Server, JWT Authentication

**Frontend:** React.js, Axios, React Router

## 🚀 Quick Start

### Backend Setup

1. **Clone and navigate**
   ```bash
   git clone https://github.com/yourusername/city-secrets.git
   cd city-secrets/CitySecrets
   ```

2. **Update connection string in `appsettings.json`**
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CitySecretsDb;Trusted_Connection=true"
   }
   ```

3. **Run migrations and start**
   ```bash
   dotnet restore
   dotnet ef database update
   dotnet run
   ```

4. **API available at:** `https://localhost:7001` | **Swagger:** `https://localhost:7001/swagger`

### Frontend Setup

1. **Navigate and install**
   ```bash
   cd frontend
   npm install
   ```

2. **Update API URL in `src/services/api.js`**
   ```javascript
   const API_BASE_URL = 'https://localhost:7001/api';
   ```

3. **Start development server**
   ```bash
   npm start
   ```

## 📁 Project Structure

```
CitySecrets/
├── Controllers/          # API endpoints
├── Services/            # Business logic
├── Models/              # Database entities
├── DTOs/                # Data transfer objects
└── Data/                # Database context

frontend/
├── src/
│   ├── components/      # UI components
│   ├── pages/           # Page components
│   └── services/        # API calls
```

## 🔐 Authentication

JWT-based authentication with:
- Email verification
- Access tokens (24h) & refresh tokens (30 days)
- Rate limiting (5 attempts per 15 minutes)
- BCrypt password hashing

## 🧪 Testing

Access Swagger UI at `https://localhost:7001/swagger` to test API endpoints interactively.

## 👥 Team

**Nile University - Software Engineering Course**

- Myar Sadek
- Noran Elhalwagui
- Ayten Mohamed
- Habibatallah Mahdi
- Philopater Ayman


## 📄 License

Academic project - Nile University © 2024



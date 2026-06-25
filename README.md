# 🏛️ MuseWay - Grand Egyptian Museum Smart Guide

MuseWay is a smart mobile application designed to enhance the visitor experience inside the Grand Egyptian Museum (GEM).

The application combines Artificial Intelligence, Indoor Navigation, QR Code Scanning, Voice Assistance, and Smart Tours to provide visitors with an interactive and personalized museum experience.

This repository contains the backend implementation built with ASP.NET Core using Clean Architecture principles.

---

# 🚀 Features

### 🔐 Authentication & Authorization
- User Registration
- Secure Login
- JWT Authentication
- Forgot Password
- OTP Verification
- Password Reset
- Change User Name

---

### 🤖 AI Chat Assistant
- General Museum Chat
- Artifact-Specific Chat
- Context-aware Responses
- Multi-language Support

---

### 🏺 Artifact Management
- Artifact Information
- QR Code Scanning
- Multi-language Translations
- Text Narration
- Categories & Historical Periods

---

### 🗺 Indoor Navigation
- Interactive Museum Maps
- Room Management
- Dijkstra Shortest Path Algorithm
- Navigation Between Rooms

---

### ⭐ Smart Tours
- Recommended Tours
- Tour Details
- Tour Routes
- Tour Pieces
- Multi-language Tours

---

### 📢 Notifications
- User Notifications
- Localized Messages

---

### 💬 Feedback System
- Visitor Feedback
- Rating Support

---

### 🌍 Localization
- Arabic
- English

---

# 🏗 Clean Architecture

The backend follows the Clean Architecture approach to ensure scalability, maintainability, and separation of concerns.

```
Presentation Layer (API)
        │
        ▼
Application Layer
        │
        ▼
Domain Layer
        │
        ▼
Infrastructure Layer
```

### API Layer
Responsible for:

- Controllers
- Authentication
- Swagger
- HTTP Requests & Responses

---

### Application Layer

Contains all business logic including:

- Services
- DTOs
- Interfaces
- Validation
- Use Cases

---

### Domain Layer

Contains the core business entities.

Examples:

- ApplicationUser
- Pieces
- Tours
- Rooms
- Maps
- Chat
- Feedback
- Notifications

---

### Infrastructure Layer

Responsible for:

- Entity Framework Core
- SQL Server
- Repositories
- ASP.NET Identity
- SMTP Email Service
- External Integrations

---

# 🛠 Technologies

## Backend

- ASP.NET Core 8
- C#
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- JWT Authentication
- Swagger

---

## Architecture

- Clean Architecture
- Repository Pattern
- Dependency Injection

---

## AI

- OpenAI API
- Prompt Engineering

---

## Algorithms

- Dijkstra Algorithm

---

## Storage

- Supabase Storage

---

## Email

- SMTP (Gmail)

---

# 📂 Project Structure

```
EgyptianMuseum.API
│
├── Controllers
├── Middleware
├── Program.cs
└── appsettings.json

EgyptianMuseum.Application
│
├── DTOs
├── Interfaces
├── Services
└── Validators

EgyptianMuseum.Domain
│
├── Entities
├── Enums
└── Common

EgyptianMuseum.Infrastructure
│
├── Data
├── Repositories
├── Identity
├── Services
└── Migrations
```

---

# 📚 API Endpoints

### Authentication

```
POST /api/auth/register

POST /api/auth/login

POST /api/auth/forgot-password

POST /api/auth/verify-otp

POST /api/auth/reset-password
```

---

### Chat

```
POST /api/chat/general

POST /api/chat/artifact
```

---

### Pieces

```
GET /api/pieces

GET /api/pieces/{id}

POST /api/pieces
```

---

### Maps

```
GET /api/maps

GET /api/maps/{id}
```

---

### Rooms

```
GET /api/rooms
```

---

### Tours

```
GET /api/tours

GET /api/tours/recommended

POST /api/tours
```

---

### Notifications

```
GET /api/notifications
```

---

### Feedback

```
POST /api/feedback
```

---

# 💡 Key Features

- Clean Architecture
- Repository Pattern
- Dependency Injection
- JWT Authentication
- OTP Authentication
- Entity Framework Core
- SQL Server
- Dijkstra Algorithm
- AI Chat Assistant
- QR Code Scanning
- Indoor Navigation
- Multi-language Support
- Smart Tour Recommendation
- Text Narration
- RESTful API

---

# 🚀 Getting Started

## Clone Repository

```bash
git clone https://github.com/your-username/MuseWay-Backend.git
```

---

## Restore Packages

```bash
dotnet restore
```

---

## Update Database

```bash
dotnet ef database update
```

---

## Run Project

```bash
dotnet run
```

---

## Open Swagger

```
https://localhost:xxxx/swagger
```

---

# 📸 Screenshots

### Swagger

(Add Swagger Screenshot)

---

### Database Diagram

(Add ERD Screenshot)

---

### Clean Architecture

(Add Architecture Diagram)

---

### Mobile Application

(Add Flutter Screenshots)

---

# 🔮 Future Improvements

- Refresh Token Authentication
- Push Notifications
- AI Voice Assistant
- Offline Navigation
- Analytics Dashboard
- Museum Administration Panel

---

# 👨‍💻 Developed By

Graduation Project

Faculty of Computers and Artificial Intelligence

Benha University

2026

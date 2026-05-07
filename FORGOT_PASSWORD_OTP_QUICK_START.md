# Forgot Password + OTP System - Quick Testing Guide

## 🚀 Quick Start

### 1. Apply Database Migration
```powershell
Update-Database
```

### 2. Start the Application
```powershell
dotnet run
```

---

## 📧 Test Endpoints (Postman/Swagger)

### Endpoint 1: Forgot Password
```
POST http://localhost:5000/api/auth/forgot-password
Content-Type: application/json

{
  "email": "your-email@gmail.com"
}
```

**Response:**
```json
{
  "success": true,
  "message": "If an account with this email exists, an OTP will be sent"
}
```

**Check Email:** You'll receive an OTP (e.g., 123456) from usiif.ahmed@gmail.com

---

### Endpoint 2: Verify OTP
```
POST http://localhost:5000/api/auth/verify-otp
Content-Type: application/json

{
  "email": "your-email@gmail.com",
  "otp": "123456"
}
```

**Response:**
```json
{
  "success": true,
  "message": "OTP verified successfully"
}
```

---

### Endpoint 3: Reset Password
```
POST http://localhost:5000/api/auth/reset-password
Content-Type: application/json

{
  "email": "your-email@gmail.com",
  "otp": "123456",
  "newPassword": "NewPassword123!"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Password has been reset successfully"
}
```

---

### Endpoint 4: Login with New Password
```
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "your-email@gmail.com",
  "password": "NewPassword123!"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "userId": "user-id-here",
    "email": "your-email@gmail.com",
    "name": "User Name"
  }
}
```

---

## ⚠️ Error Scenarios

### Invalid OTP
```json
{
  "success": false,
  "message": "Invalid OTP"
}
```

### OTP Expired (after 10 minutes)
```json
{
  "success": false,
  "message": "OTP has expired"
}
```

### Already Used OTP
```json
{
  "success": false,
  "message": "OTP has already been used"
}
```

### User Not Found
```json
{
  "success": false,
  "message": "User not found"
}
```

---

## 🔒 Security Notes

✅ **OTP is 6 digits** (random: 100000-999999)
✅ **OTP expires in 10 minutes**
✅ **OTP is single-use only**
✅ **OTP NOT returned in any API response**
✅ **Forgot-Password always returns success** (no user enumeration)
✅ **Gmail SMTP with SSL/TLS enabled**
✅ **Password policies enforced by Identity**

---

## 📊 Flow Summary

1. User clicks "Forgot Password"
2. Enters email → receives OTP via email
3. Enters email + OTP → verification
4. Enters email + OTP + new password → password reset
5. Login with new password

---

## 🗄️ Database Schema

**PasswordResetOtps Table:**
```sql
CREATE TABLE [PasswordResetOtps] (
    [Id] int PRIMARY KEY IDENTITY(1,1),
    [UserId] nvarchar(450) NOT NULL,
    [Code] nvarchar(6) NOT NULL,
    [ExpiryTime] datetime2 NOT NULL,
    [IsUsed] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
);

CREATE INDEX IX_PasswordResetOtps_UserId_Code_IsUsed 
ON [PasswordResetOtps]([UserId], [Code], [IsUsed]);
```

---

## 🛠️ Files Modified/Created

### ✅ Created:
- `EgyptianMuseum.Domain/Entities/PasswordResetOtp.cs`
- `EgyptianMuseum.Application/Interfaces/IEmailService.cs`
- `EgyptianMuseum.Application/Interfaces/IPasswordResetOtpRepository.cs`
- `EgyptianMuseum.Application/Services/Email/EmailService.cs`
- `EgyptianMuseum.Application/DTOs/Auth/ForgotPasswordRequestDto.cs`
- `EgyptianMuseum.Application/DTOs/Auth/VerifyOtpRequestDto.cs`
- `EgyptianMuseum.Application/DTOs/Auth/ResetPasswordRequestDto.cs`
- `EgyptianMuseum.Infrastructure/Repositories/PasswordResetOtpRepository.cs`
- `EgyptianMuseum.Infrastructure/Migrations/20250101000001_AddPasswordResetOtpEntity.cs`

### ✅ Modified:
- `EgyptianMuseum.Application/Services/Auth/IAuthService.cs` (added 3 new methods)
- `EgyptianMuseum.Application/Services/Auth/AuthService.cs` (implemented 3 new methods)
- `EgyptianMuseum.API/Controllers/AuthController.cs` (added 3 new endpoints)
- `EgyptianMuseum.Infrastructure/Data/AppDbContext.cs` (added DbSet + configuration)
- `EgyptianMuseum.API/Program.cs` (registered services)
- `EgyptianMuseum.API/appsettings.json` (added Smtp section)

---

## ✨ Key Features

1. **Clean Architecture** - All layers properly separated
2. **SOLID Principles** - Single responsibility, dependency injection
3. **Async/Await** - Fully asynchronous operations
4. **Error Handling** - Proper exception handling in all layers
5. **Validation** - Request and OTP validation
6. **Security** - SSL, no user enumeration, single-use OTP
7. **Configuration-Driven** - All settings in appsettings.json
8. **HTML Email** - Professional email template

---

## 🎯 Testing Checklist

- [ ] Register a new user
- [ ] Forgot password with email
- [ ] Check email for OTP
- [ ] Verify OTP
- [ ] Reset password with OTP
- [ ] Login with new password
- [ ] Try invalid OTP (should fail)
- [ ] Wait 10 minutes, try expired OTP (should fail)
- [ ] Try OTP twice (should fail on second attempt)

---

## 📞 Troubleshooting

**Email not sent?**
- Check SMTP credentials in appsettings.json
- Verify Gmail security settings (less secure apps enabled)
- Check firewall/network access to smtp.gmail.com:587

**OTP not working?**
- Ensure database migration ran: `Update-Database`
- Check OTP hasn't expired (10 minutes max)
- Verify OTP wasn't already used

**Build fails?**
- Run `dotnet restore`
- Check all NuGet packages are installed
- Verify .NET 8 is installed

---

## 📚 Configuration Reference

**appsettings.json:**
```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "usiif.ahmed@gmail.com",
    "Password": "tbxl wdoz dept zqil",
    "From": "usiif.ahmed@gmail.com"
  },
  "Jwt": {
    "Issuer": "https://localhost:44330",
    "Audience": "http://localhost:5500",
    "SecretKey": "SUPER_LONG_SECRET_KEY_123456789_ABC",
    "ExpiryMinutes": 60
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=db48609.public.databaseasp.net; ..."
  }
}
```

---

## 🚀 Production Checklist

Before deploying to production:

- [ ] Update SMTP credentials with production email
- [ ] Use environment variables for sensitive data
- [ ] Enable HTTPS only
- [ ] Add rate limiting middleware
- [ ] Add logging for OTP attempts
- [ ] Set up monitoring for email failures
- [ ] Test email delivery at scale
- [ ] Update email template with company branding
- [ ] Configure CORS properly
- [ ] Enable HSTS headers
- [ ] Add request validation middleware
- [ ] Set up database backups

---

Made with ❤️ - Egyptian Museum API

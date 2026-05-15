# Kafai Portfolio — Contact API

A lightweight ASP.NET Core Web API that handles contact form submissions from my portfolio website and delivers them directly to my inbox via Gmail SMTP.

---

## Tech Stack

- **Framework** — ASP.NET Core 8 Web API
- **Email** — MailKit (Gmail SMTP)
- **Security** — API Key Middleware
- **Language** — C# .NET 8

---

## Project Structure

portfolio-api/
├── Controllers/
│   └── ContactController.cs    ← handles POST /api/contact
├── Interfaces/
│   ├── Services/
│   │   ├── IEmailService.cs
│   │   └── IApiKeyService.cs
│   └── Models/
│       └── IContactRequest.cs
├── Middlewares/
│   └── ApiKeyMiddleware.cs     ← validates x-api-key header
├── Models/
│   └── ContactRequest.cs
├── Services/
│   ├── EmailService.cs         ← sends email via Gmail SMTP
│   └── ApiKeyService.cs        ← validates API key
├── Templates/
│   └── EmailTemplates.cs       ← HTML email template
├── Program.cs
├── appsettings.json
└── appsettings.Development.json

---

## API Endpoints

### `POST /api/contact`

Sends a contact email to the portfolio owner.

**Headers:**
Content-Type: application/json
x-api-key: your-api-key

**Request Body:**
```json
{
  "name":    "John Smith",
  "email":   "john@company.com",
  "company": "Acme Corp",
  "message": "Hello, I'd like to discuss an opportunity."
}
```

**Responses:**
| Status | Description |
|--------|-------------|
| 200    | Email sent successfully |
| 400    | Missing required fields |
| 401    | API key missing |
| 403    | Invalid API key |
| 500    | Failed to send email |

---

## Local Development Setup

### Prerequisites
- .NET 8 SDK → [dotnet.microsoft.com](https://dotnet.microsoft.com)
- A Gmail account with 2-Step Verification enabled

### Step 1 — Clone the repo
```bash
git clone https://github.com/your-username/kafai-portfolio-api.git
cd kafai-portfolio-api
```

### Step 2 — Install dependencies
```bash
dotnet restore
```

### Step 3 — Configure environment
Create `appsettings.Development.json`:
```json
{
  "ApiSettings": {
    "ApiKey": "your-secret-api-key"
  },
  "SmtpSettings": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "SenderEmail": "your-gmail@gmail.com",
    "ReceiverEmail": "your-gmail@gmail.com",
    "Password": "your-16-char-app-password"
  }
}
```

> **Gmail App Password** — Go to [myaccount.google.com/apppasswords](https://myaccount.google.com/apppasswords) to generate one.

### Step 4 — Run
```bash
dotnet run
```

API runs on `http://localhost:5268`
Swagger UI at `http://localhost:5268/swagger`

---

## Environment Variables (Production)

Set these on your hosting platform instead of using appsettings:

| Key | Description |
|-----|-------------|
| `ApiSettings__ApiKey` | Your secret API key |
| `SmtpSettings__Host` | SMTP host (smtp.gmail.com) |
| `SmtpSettings__Port` | SMTP port (587) |
| `SmtpSettings__SenderEmail` | Your Gmail address |
| `SmtpSettings__ReceiverEmail` | Your Gmail address |
| `SmtpSettings__Password` | Gmail App Password |

---

## Security

- All endpoints protected by `x-api-key` header validation
- CORS restricted to portfolio domain only
- HTTPS enforced in production
- Secrets never committed to source control

---

## License

MIT
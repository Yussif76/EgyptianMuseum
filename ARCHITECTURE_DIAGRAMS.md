# Feedback System - Architecture & Flow Diagrams

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    CLIENT / FRONTEND                        │
└────────────────────┬────────────────────────────────────────┘
                     │ HTTP + JWT Bearer Token
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                    API LAYER                                │
│            FeedbackController                               │
│  • POST   /api/Feedback                                     │
│  • GET    /api/Feedback                                     │
│  • GET    /api/Feedback/target/{type}?targetId={id}        │
│  • DELETE /api/Feedback/{id}                                │
└────────────────────┬────────────────────────────────────────┘
                     │ Depends on
                     ▼
┌─────────────────────────────────────────────────────────────┐
│              APPLICATION LAYER                              │
│  • FeedbackService (Business Logic)                         │
│  • IFeedbackService Interface                               │
│  • DTOs (CreateFeedbackRequestDto, FeedbackDto)            │
│                                                              │
│  Validation Rules:                                          │
│  ├─ targetType must be: Artifact, Chat, or App             │
│  ├─ Rating: 1-5 only                                       │
│  ├─ If App: targetId must be null                          │
│  ├─ If Artifact/Chat: targetId is required                 │
│  └─ Comment: optional, max 1000 chars                      │
└────────────────────┬────────────────────────────────────────┘
                     │ Depends on
                     ▼
┌─────────────────────────────────────────────────────────────┐
│            INFRASTRUCTURE LAYER                             │
│  • FeedbackRepository                                       │
│  • IFeedbackRepository Interface                            │
│  • IPieceRepository (Artifact validation)                   │
│  • IChatConversationRepository (Chat validation)            │
└────────────────────┬────────────────────────────────────────┘
                     │ Reads/Writes from
                     ▼
┌─────────────────────────────────────────────────────────────┐
│             DOMAIN LAYER                                    │
│  • Feedback Entity                                          │
│  • FeedbackTargetType Enum (Artifact=1, Chat=2, App=3)    │
│  • ApplicationUser Entity                                   │
└────────────────────┬────────────────────────────────────────┘
                     │ Persists to
                     ▼
┌─────────────────────────────────────────────────────────────┐
│              DATABASE (SQL Server)                          │
│  Feedbacks Table:                                           │
│  ├─ Id (int, PK)                                           │
│  ├─ UserId (string, FK)                                    │
│  ├─ TargetType (int, enum: 1=Artifact, 2=Chat, 3=App)    │
│  ├─ TargetId (int?, nullable)                             │
│  ├─ Rating (int, 1-5)                                      │
│  ├─ Comment (nvarchar(1000))                              │
│  └─ CreatedAt (datetime2)                                 │
└─────────────────────────────────────────────────────────────┘
```

---

## Request/Response Flow

### POST /api/Feedback - Create Feedback

```
┌─────────────────────────────────────────┐
│  CLIENT                                 │
│                                         │
│  POST /api/Feedback                    │
│  {                                     │
│    "targetType": "App|Artifact|Chat",  │
│    "targetId": 123,  // optional      │
│    "rating": 5,                       │
│    "comment": "..."  // optional      │
│  }                                    │
└────────────┬────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────────────────────────┐
│  FeedbackController.CreateFeedback()                        │
│  • Extract JWT token → UserId                              │
│  • Model validation via attributes                         │
└────────────┬────────────────────────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────────────────────────┐
│  FeedbackService.CreateAsync()                              │
│  ✓ Parse TargetType string to enum                         │
│  ✓ Validate rating (1-5)                                   │
│  ✓ Validate comment (not empty)                            │
│  ✓ Type-specific validation:                               │
│    • App: targetId MUST be null                            │
│    • Artifact: targetId REQUIRED + exist check             │
│    • Chat: targetId REQUIRED + exist check                 │
│  ✓ Create Feedback entity                                  │
└────────────┬────────────────────────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────────────────────────┐
│  FeedbackRepository.AddAsync()                              │
│  • Add to Feedbacks DbSet                                  │
│  • SaveChangesAsync()                                      │
└────────────┬────────────────────────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────────────────────────┐
│  DATABASE                                                   │
│  INSERT INTO Feedbacks (UserId, TargetType, TargetId,     │
│                         Rating, Comment, CreatedAt)        │
└────────────┬────────────────────────────────────────────────┘
             │
             ▼
┌─────────────────────────────────────────┐
│  RESPONSE (201 Created)                 │
│  {                                      │
│    "success": true,                    │
│    "message": "...",                   │
│    "data": {                           │
│      "id": 1,                          │
│      "targetType": "App",              │
│      "targetId": null,                 │
│      "rating": 5,                      │
│      "comment": "...",                 │
│      "createdAt": "ISO8601"            │
│    }                                   │
│  }                                     │
└─────────────────────────────────────────┘
```

---

## GET /api/Feedback/target/{targetType}?targetId={id}

```
┌──────────────────────────────────────┐
│  CLIENT                              │
│                                      │
│  GET /api/Feedback/target/App       │
│  (or with ?targetId=5 for others)   │
└────────────┬───────────────────────────┘
             │
             ▼
┌──────────────────────────────────────────────────────────────┐
│  FeedbackController.GetByTarget()                            │
│  • Validate request (targetId required except for App)      │
└────────────┬───────────────────────────────────────────────────┘
             │
             ▼
┌──────────────────────────────────────────────────────────────┐
│  FeedbackService.GetByTargetAsync()                          │
│  • Parse targetType string to enum                          │
│  • Validate targetId rules:                                │
│    • App + targetId provided → ERROR                        │
│    • Artifact/Chat without targetId → ERROR                 │
└────────────┬───────────────────────────────────────────────────┘
             │
             ▼
┌──────────────────────────────────────────────────────────────┐
│  FeedbackRepository.GetByTargetAsync()                       │
│  SELECT * FROM Feedbacks                                    │
│  WHERE TargetType = @type                                   │
│    AND (TargetId = @id OR @id IS NULL)                      │
│  ORDER BY CreatedAt DESC                                    │
└────────────┬───────────────────────────────────────────────────┘
             │
             ▼
┌──────────────────────────────────────────────────────────────┐
│  Filter by current UserId                                    │
│  (Service layer - only return user's feedback)              │
└────────────┬───────────────────────────────────────────────────┘
             │
             ▼
┌──────────────────────────────────┐
│  RESPONSE (200 OK)               │
│  {                              │
│    "success": true,             │
│    "message": "...",            │
│    "data": [                    │
│      {                          │
│        "id": 1,                 │
│        "targetType": "App",     │
│        "targetId": null,        │
│        "rating": 5,             │
│        "comment": "...",        │
│        "createdAt": "..."       │
│      }                          │
│    ]                            │
│  }                              │
└──────────────────────────────────┘
```

---

## Validation Decision Tree

```
                    Feedback Request
                         │
                         ▼
            Is targetType valid?
           (Artifact|Chat|App)
            ✓ Yes / ❌ No (400)
                    │
                    ▼
            Is rating 1-5?
            ✓ Yes / ❌ No (400)
                    │
                    ▼
        Is comment non-empty?
        ✓ Yes / ❌ No (400)
                    │
                    ▼
        ┌───────────┴──────────────┬────────────┐
        │                          │            │
        ▼                          ▼            ▼
    Artifact?               Chat?          App?
        │                    │             │
        ▼                    ▼             ▼
    TargetId                TargetId        TargetId
    required?               required?       must be
    ❌ No (400)             ❌ No (400)      null?
    ✓ Yes                   ✓ Yes           ✓ Yes
        │                    │             │
        ▼                    ▼             ✓ Create
    Exists in                Exists in       Feedback
    Pieces?                  Chats?         (201)
    ❌ No (404)              ❌ No (404)
    ✓ Yes                    ✓ Yes
        │                    │
        ▼                    ▼
    ✓ Create               ✓ Create
      Feedback               Feedback
      (201)                  (201)
```

---

## Error Handling Map

```
Error                           HTTP Code   Message
─────────────────────────────────────────────────────────────
Invalid targetType              400         "Target type must be 'Artifact', 'Chat', or 'App'"
Invalid rating (not 1-5)        400         "Rating must be between 1 and 5"
Empty comment                   400         "Comment cannot be empty"
App + targetId provided         400         "Target ID must be null for App feedback"
Artifact/Chat without targetId  400         "Target ID is required for X feedback"
Artifact/Chat with invalid ID   404         "Artifact/Chat with ID X not found"
Feedback not found (delete)     404         "Feedback not found"
Wrong owner (delete)            403         Forbid()
Unauthorized (no token)         401         "User not authenticated"
```

---

## State Diagram

```
                         ┌─────────────────┐
                         │  REQUEST READY  │
                         └────────┬────────┘
                                  │
                    Parse & Validate DTOs
                                  │
                    ┌─────────────┴──────────────┐
                    │                            │
                ❌ Invalid                  ✓ Valid
                    │                            │
                    ▼                            ▼
         ┌───────────────────┐         ┌──────────────────┐
         │ VALIDATION ERROR  │         │ SERVICE LAYER    │
         │ (400 Bad Request) │         └────────┬─────────┘
         └───────────────────┘                  │
                                    ┌───────────┴──────────┐
                                    │                      │
                            Additional Checks:    
                    (Target Exists? Type Rules?)
                            │          │
                        ❌ No      ✓ Yes
                        │         │
                        ▼         ▼
        ┌─────────────────────┐  ┌──────────────────┐
        │ NOT FOUND (404)     │  │ CREATE FEEDBACK  │
        │ or BAD REQUEST (400)│  │ & SAVE TO DB     │
        └─────────────────────┘  └────────┬─────────┘
                                           │
                                    ┌──────┴──────┐
                                    │             │
                                ✓ Success    ❌ Error
                                    │             │
                                    ▼             ▼
                          ┌──────────────────┐ ┌──────────┐
                          │ SUCCESS (201)    │ │ 500      │
                          │ Return FeedbackDto
                          └──────────────────┘ └──────────┘
```

---

## Data Flow for Different Feedback Types

### App Feedback Flow
```
User → POST /api/Feedback with targetType="App" → 
Service validates (targetId must be null) → 
Repository saves without targetId → 
Database stores with TargetId = NULL → 
Response returns FeedbackDto with targetId = null
```

### Artifact Feedback Flow
```
User → POST /api/Feedback with targetType="Artifact", targetId=42 → 
Service validates (targetId required) → 
Service checks if Artifact #42 exists → 
Repository saves with targetId=42 → 
Database stores with TargetId = 42 → 
Response returns FeedbackDto with targetId = 42
```

### Chat Feedback Flow
```
User → POST /api/Feedback with targetType="Chat", targetId=5 → 
Service validates (targetId required) → 
Service checks if Chat #5 exists → 
Repository saves with targetId=5 → 
Database stores with TargetId = 5 → 
Response returns FeedbackDto with targetId = 5
```

---

## Query Performance Notes

```sql
-- High-frequency query: Get all app feedback (optimized)
SELECT * FROM Feedbacks 
WHERE TargetType = 3 
ORDER BY CreatedAt DESC
-- Performance: Index on (TargetType, CreatedAt DESC)

-- Medium-frequency query: Get user's feedback
SELECT * FROM Feedbacks 
WHERE UserId = @userId 
ORDER BY CreatedAt DESC
-- Performance: Index on (UserId, CreatedAt DESC)

-- Low-frequency query: Get feedback for specific target
SELECT * FROM Feedbacks 
WHERE TargetType = @type AND TargetId = @id 
ORDER BY CreatedAt DESC
-- Performance: Composite index on (TargetType, TargetId, CreatedAt DESC)
```

---

## Integration Points

```
┌─────────────────────────────────────────────────┐
│         Feedback Module                         │
├─────────────────────────────────────────────────┤
│                                                 │
│  Integrates with:                              │
│  ├─ Authentication (JWT Bearer)                │
│  ├─ PieceRepository (Artifact validation)      │
│  ├─ ChatConversationRepository (Chat validation)
│  └─ AppDbContext (Entity Framework)            │
│                                                 │
│  Consumed by:                                  │
│  ├─ Dashboard (show feedback stats)            │
│  ├─ Admin Panel (moderate feedback)            │
│  ├─ User Profile (view own feedback)           │
│  └─ Analytics (sentiment analysis)             │
│                                                 │
└─────────────────────────────────────────────────┘
```

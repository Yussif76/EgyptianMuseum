# Change Username Endpoint Implementation

## Overview
Successfully implemented a secure, authenticated endpoint that allows logged-in users to change their username. The implementation follows Clean Architecture principles and maintains API security.

---

## Endpoint Details

### Route
```
PUT /api/auth/change-username
```

### Authentication
- **Required**: JWT Bearer Token
- **Scheme**: JwtBearerDefaults.AuthenticationScheme
- **User Identity**: Extracted from JWT claims (NameIdentifier claim type)

---

## Request Specification

### Request Body
```json
{
  "newUserName": "new_username"
}
```

### Headers
```
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json
```

---

## Response Specifications

### Success (200 OK)
```json
{
  "success": true,
  "message": "Username updated successfully"
}
```

### Validation Error (400 Bad Request)
```json
{
  "success": false,
  "message": "Username must be between 3 and 50 characters"
}
```

**Possible Validation Messages:**
- "Username cannot be empty"
- "Username must be between 3 and 50 characters"
- "Request cannot be empty"
- "User ID not found in token"

### Username Already Taken (409 Conflict)
```json
{
  "success": false,
  "message": "Username is already taken"
}
```

### Unauthorized (401)
```json
{
  "success": false,
  "message": "Unauthorized"
}
```
Returned when JWT token is missing, invalid, or expired.

---

## Implementation Details

### 1. DTO Created
**File**: `EgyptianMuseum.Application\DTOs\Auth\ChangeUserNameRequestDto.cs`

```csharp
public class ChangeUserNameRequestDto
{
    /// <summary>
    /// The new username to set for the current user.
    /// </summary>
    public string NewUserName { get; set; } = null!;
}
```

### 2. Service Interface Updated
**File**: `EgyptianMuseum.Application\Services\Auth\IAuthService.cs`

Added method:
```csharp
/// <summary>
/// Changes the username for the specified user.
/// </summary>
/// <param name="userId">The ID of the user to update.</param>
/// <param name="request">The request containing the new username.</param>
/// <exception cref="ArgumentException">Thrown when username validation fails.</exception>
/// <exception cref="InvalidOperationException">Thrown when username is already taken or user not found.</exception>
Task ChangeUserNameAsync(string userId, ChangeUserNameRequestDto request);
```

### 3. Service Implementation
**File**: `EgyptianMuseum.Application\Services\Auth\AuthService.cs`

Implementation includes:

#### Validations:
1. **Empty Check**: Username cannot be null or whitespace
2. **Length Check**: Username must be between 3 and 50 characters
3. **Uniqueness Check**: New username is not already taken by another user
4. **User Existence Check**: User with given ID exists

#### Security Features:
- Only allows changing own username (user ID from JWT claims)
- Cannot modify another user's username
- NormalizedUserName is updated automatically (converted to uppercase)
- Uses UserManager for ASP.NET Identity integration
- Exception handling with meaningful error messages

#### Code Snippet:
```csharp
public async Task ChangeUserNameAsync(string userId, ChangeUserNameRequestDto request)
{
    // Validation: Check if newUserName is provided
    if (string.IsNullOrWhiteSpace(request?.NewUserName))
    {
        throw new ArgumentException("Username cannot be empty");
    }

    var newUserName = request.NewUserName.Trim();

    // Validation: Check username length (3-50 characters)
    if (newUserName.Length < 3 || newUserName.Length > 50)
    {
        throw new ArgumentException("Username must be between 3 and 50 characters");
    }

    // Get the current user
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
        throw new InvalidOperationException("User not found");
    }

    // Check if the new username is already taken by another user
    var existingUser = await _userManager.FindByNameAsync(newUserName);
    if (existingUser != null && existingUser.Id != userId)
    {
        throw new InvalidOperationException("Username is already taken");
    }

    // Update username
    user.UserName = newUserName;
    user.NormalizedUserName = newUserName.ToUpper();

    var result = await _userManager.UpdateAsync(user);
    if (!result.Succeeded)
    {
        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        throw new InvalidOperationException($"Failed to update username: {errors}");
    }
}
```

### 4. Controller Implementation
**File**: `EgyptianMuseum.API\Controllers\AuthController.cs`

Endpoint implementation includes:

#### Features:
- **Authorization**: Requires JWT authentication
- **User Extraction**: Extracts user ID from JWT claims (NameIdentifier)
- **Error Handling**: Proper exception handling with appropriate HTTP status codes
- **Validation**: Input validation before service call
- **Response Mapping**: Returns standardized JSON responses

#### Decorators:
```csharp
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[HttpPut("change-username")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status409Conflict)]
```

#### Code Snippet:
```csharp
public async Task<IActionResult> ChangeUserName([FromBody] ChangeUserNameRequestDto request)
{
    try
    {
        // Get the current user ID from JWT claims
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { success = false, message = "User ID not found in token" });
        }

        // Validate request
        if (request == null)
        {
            return BadRequest(new { success = false, message = "Request cannot be empty" });
        }

        // Call the service
        await _authService.ChangeUserNameAsync(userId, request);

        return Ok(new { success = true, message = "Username updated successfully" });
    }
    catch (ArgumentException ex)
    {
        return BadRequest(new { success = false, message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        // Check if it's a "username already taken" error for 409 response
        if (ex.Message.Contains("already taken"))
        {
            return StatusCode(StatusCodes.Status409Conflict, 
                new { success = false, message = "Username is already taken" });
        }

        return BadRequest(new { success = false, message = ex.Message });
    }
    catch (Exception ex)
    {
        return BadRequest(new { success = false, message = $"An error occurred: {ex.Message}" });
    }
}
```

---

## Security Considerations

### ✅ Implemented Security Features:

1. **JWT Authentication Required**: Endpoint only accessible with valid JWT token
2. **User Isolation**: Can only change own username (verified via JWT claims)
3. **Username Uniqueness**: Validates new username is not taken by another user
4. **Input Validation**: 
   - Empty check
   - Length validation (3-50 characters)
5. **No Other Modifications**: 
   - Email is NOT modified
   - Password is NOT modified
   - PreferredLanguage is NOT modified
5. **Error Handling**: Appropriate HTTP status codes for different error scenarios
6. **ASP.NET Identity Integration**: Uses UserManager for secure user management

---

## Testing Scenarios

### Test Case 1: Successful Username Change
```
Method: PUT
URL: /api/auth/change-username
Header: Authorization: Bearer <VALID_TOKEN>
Body: { "newUserName": "newusername123" }

Expected: 200 OK
{
  "success": true,
  "message": "Username updated successfully"
}
```

### Test Case 2: Empty Username
```
Method: PUT
URL: /api/auth/change-username
Header: Authorization: Bearer <VALID_TOKEN>
Body: { "newUserName": "" }

Expected: 400 Bad Request
{
  "success": false,
  "message": "Username cannot be empty"
}
```

### Test Case 3: Username Too Short
```
Method: PUT
URL: /api/auth/change-username
Header: Authorization: Bearer <VALID_TOKEN>
Body: { "newUserName": "ab" }

Expected: 400 Bad Request
{
  "success": false,
  "message": "Username must be between 3 and 50 characters"
}
```

### Test Case 4: Username Too Long
```
Method: PUT
URL: /api/auth/change-username
Header: Authorization: Bearer <VALID_TOKEN>
Body: { "newUserName": "this_is_a_very_long_username_that_exceeds_fifty_characters_limit_test" }

Expected: 400 Bad Request
{
  "success": false,
  "message": "Username must be between 3 and 50 characters"
}
```

### Test Case 5: Username Already Taken
```
Method: PUT
URL: /api/auth/change-username
Header: Authorization: Bearer <VALID_TOKEN>
Body: { "newUserName": "existingusername" }

Expected: 409 Conflict
{
  "success": false,
  "message": "Username is already taken"
}
```

### Test Case 6: Missing JWT Token
```
Method: PUT
URL: /api/auth/change-username
Header: Authorization: (missing)
Body: { "newUserName": "newusername123" }

Expected: 401 Unauthorized
```

### Test Case 7: Invalid JWT Token
```
Method: PUT
URL: /api/auth/change-username
Header: Authorization: Bearer invalid_token
Body: { "newUserName": "newusername123" }

Expected: 401 Unauthorized
```

---

## Files Modified/Created

### Created:
- ✅ `EgyptianMuseum.Application\DTOs\Auth\ChangeUserNameRequestDto.cs`

### Modified:
- ✅ `EgyptianMuseum.Application\Services\Auth\IAuthService.cs`
- ✅ `EgyptianMuseum.Application\Services\Auth\AuthService.cs`
- ✅ `EgyptianMuseum.API\Controllers\AuthController.cs`

---

## Clean Architecture Compliance

✅ **Domain Layer**: No changes (existing ApplicationUser entity used)
✅ **Application Layer**: 
   - New DTO created (ChangeUserNameRequestDto)
   - Service interface updated (IAuthService)
   - Service implementation (AuthService)

✅ **API Layer**: 
   - New endpoint added to AuthController

✅ **Infrastructure Layer**: 
   - No changes needed (UserManager handles data access via Identity)

✅ **Dependency Injection**: 
   - No changes needed (IAuthService already registered)

---

## Build Status

✅ **Build**: Successful
✅ **No Breaking Changes**: All existing endpoints remain functional
✅ **Backward Compatibility**: Fully maintained

---

## API Documentation Summary

### Endpoint
- **Route**: `PUT /api/auth/change-username`
- **Authentication**: Required (JWT Bearer)
- **Content-Type**: application/json

### Request
```json
{
  "newUserName": "string (3-50 characters)"
}
```

### Responses
- **200 OK**: Username successfully changed
- **400 Bad Request**: Validation failed
- **401 Unauthorized**: Missing or invalid JWT token
- **409 Conflict**: Username already taken

---

## Usage Example

### Using cURL
```bash
curl -X PUT http://localhost:5000/api/auth/change-username \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "newUserName": "new_username_123"
  }'
```

### Using Postman
1. Set Method to **PUT**
2. Set URL to `http://localhost:5000/api/auth/change-username`
3. Go to **Headers** tab:
   - Add `Authorization: Bearer <your_jwt_token>`
   - Add `Content-Type: application/json`
4. Go to **Body** tab (select **raw** and **JSON**):
   ```json
   {
     "newUserName": "new_username_123"
   }
   ```
5. Click **Send**

---

## Important Notes

1. **No Email/Password Changes**: This endpoint ONLY changes username
2. **Async/Await**: Implementation uses async/await throughout
3. **Error Messages**: Clear, user-friendly error messages
4. **Database Updates**: Username and NormalizedUserName both updated in Identity database
5. **JWT Claims**: User ID extracted from JWT's NameIdentifier claim
6. **Existing Endpoints**: All existing auth endpoints remain unchanged

---

## Validation Summary

| Validation | Rule | Error Message |
|-----------|------|---------------|
| Empty Username | Must not be null/empty/whitespace | "Username cannot be empty" |
| Username Length | Between 3 and 50 characters | "Username must be between 3 and 50 characters" |
| Uniqueness | Not taken by another user | "Username is already taken" |
| User Existence | User must exist | "User not found" |
| Authentication | Valid JWT required | 401 Unauthorized |

---

# Image Upload Task API

A .NET 8 Web API that allows customers to upload up to 10 images each, with full CRUD operations for both customers and their images.

## Features

- **Customer Management**: Create, read, update, and delete customers
- **Image Upload**: Upload single or multiple images per customer
- **Image Limit**: Enforced limit of 10 images per customer
- **Image Management**: List, view, and delete customer images
- **File Storage**: Images stored in `wwwroot/uploads/{customerId}/` directory
- **Database**: Entity Framework Core with SQL Server LocalDB

## Database Models

### Customer
- `Id` (Primary Key)
- `FirstName` (Required, Max 100 chars)
- `LastName` (Required, Max 100 chars)
- `Email` (Optional, Max 200 chars)
- `Phone` (Optional, Max 20 chars)
- `CreatedAt` (Auto-generated)
- `UpdatedAt` (Auto-generated)

### CustomerImage
- `Id` (Primary Key)
- `CustomerId` (Foreign Key)
- `FileName` (Original filename)
- `FilePath` (Physical file path)
- `ContentType` (MIME type)
- `FileSizeBytes` (File size in bytes)
- `UploadedAt` (Auto-generated)
- `Description` (Optional description)

## API Endpoints

### Customer Endpoints

#### Create Customer
```
POST /api/customer
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phone": "123-456-7890"
}
```

#### Get All Customers
```
GET /api/customer
```

#### Get Customer by ID
```
GET /api/customer/{id}
```

#### Update Customer
```
PUT /api/customer/{id}
Content-Type: application/json

{
  "id": 1,
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe.updated@example.com",
  "phone": "123-456-7890"
}
```

#### Delete Customer
```
DELETE /api/customer/{id}
```

### Image Endpoints

#### Upload Images
```
POST /api/image/upload/{customerId}
Content-Type: multipart/form-data

files: [image files]
description: "Optional description"
```

#### Get Customer Images
```
GET /api/image/customer/{customerId}
```

#### Get Image Count
```
GET /api/image/count/{customerId}
```

#### Serve Image File
```
GET /api/image/serve/{imageId}
```

#### Delete Image
```
DELETE /api/image/{imageId}
```

## Image Upload Constraints

- **Maximum 10 images per customer** - enforced at both service and database levels
- **Supported formats**: JPEG, JPG, PNG, GIF, WebP
- **Maximum file size**: 10MB per image
- **Storage location**: `wwwroot/uploads/{customerId}/`

## Response Format

All API responses follow this format:

```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": { ... },
  "errors": []
}
```

## Setup Instructions

1. **Prerequisites**:
   - .NET 8 SDK
   - SQL Server LocalDB (included with Visual Studio)

2. **Run the application**:
   ```bash
   dotnet run
   ```

3. **Database**: The database will be created automatically on first run

4. **API Documentation**: Available at `https://localhost:7000/swagger`

## Testing

Use the provided `ImageUploadTask.http` file in Visual Studio or VS Code with the REST Client extension to test the API endpoints.

## Architecture

- **Controllers**: Handle HTTP requests and responses
- **Services**: Business logic and file operations
- **Models**: Data entities and DTOs
- **Data**: Entity Framework DbContext
- **Configuration**: Dependency injection and middleware setup

## Error Handling

- Comprehensive error logging
- Graceful error responses
- File validation
- Database constraint enforcement
- Image limit validation

# Blazor Image Upload Web Application

A comprehensive Blazor Server application for managing customer profiles with image galleries. Features drag-and-drop image upload, responsive carousel/gallery display, and Base64 storage integration.

## 🚀 Features

### ✅ **Image Upload & Management**
- **Drag & Drop Upload**: Intuitive file upload with drag-and-drop support
- **File Validation**: Type and size validation (JPEG, PNG, GIF, WebP, max 5MB)
- **Base64 Conversion**: Automatic conversion to Base64 for database storage
- **Progress Tracking**: Real-time upload progress indication
- **Error Handling**: Comprehensive error messages and validation

### ✅ **Image Gallery & Display**
- **Dual View Modes**: Switch between grid and carousel views
- **Responsive Design**: Works perfectly on desktop, tablet, and mobile
- **Image Modal**: Full-screen image viewing with details
- **Thumbnail Navigation**: Quick navigation between images
- **Image Management**: Easy deletion with confirmation

### ✅ **Customer Management**
- **Customer Profiles**: Complete customer information management
- **Image Limits**: Enforce maximum 10 images per customer
- **Real-time Counts**: Live tracking of image counts and limits
- **CRUD Operations**: Create, read, update, and delete customers

### ✅ **User Experience**
- **Intuitive Interface**: Clean, modern UI with smooth animations
- **Responsive Layout**: Mobile-first design approach
- **Loading States**: Visual feedback during operations
- **Error Messages**: Clear, actionable error messages
- **Success Feedback**: Confirmation of successful operations

## 🏗️ Architecture

### **Frontend (Blazor Server)**
- **Pages**: Customer management and profile pages
- **Components**: Reusable image upload and gallery components
- **Services**: API communication and data management
- **Models**: Data transfer objects and entities

### **Backend Integration**
- **HTTP Client**: Configured for API communication
- **Base64 Storage**: Images stored as Base64 in database
- **RESTful API**: Full CRUD operations for customers and images

## 📁 Project Structure

```
ImageUploadWeb/
├── Components/
│   ├── ImageUpload.razor          # Drag-and-drop upload component
│   └── ImageGallery.razor         # Responsive gallery/carousel component
├── Models/
│   ├── ApiResponse.cs             # Standardized API response wrapper
│   ├── Customer.cs                # Customer entity
│   ├── CustomerImage.cs           # Image entity
│   └── Base64ImageUploadRequest.cs # Upload request DTO
├── Pages/
│   ├── Index.razor                # Landing page with features
│   ├── Customers.razor            # Customer list management
│   └── CustomerProfile.razor      # Individual customer profile
├── Services/
│   ├── IImageService.cs           # Image service interface
│   ├── ImageService.cs            # Image API communication
│   ├── ICustomerService.cs        # Customer service interface
│   └── CustomerService.cs         # Customer API communication
├── wwwroot/
│   └── js/
│       └── image-upload.js        # JavaScript utilities
└── Program.cs                     # Service configuration
```

## 🛠️ Setup & Configuration

### **1. Prerequisites**
- .NET 7.0 SDK
- Visual Studio 2022 or VS Code
- SQL Server (for backend API)

### **2. API Configuration**
Update the API base URL in `Program.cs`:
```csharp
client.BaseAddress = new Uri("https://localhost:7000/"); // Your API URL
```

### **3. Run the Application**
```bash
cd ImageUploadWeb
dotnet run
```

### **4. Access the Application**
- Navigate to `https://localhost:5001` (or your configured port)
- The application will connect to the backend API

## 🎨 Components Overview

### **ImageUpload Component**
```razor
<ImageUpload CustomerId="@customerId" 
              OnImagesUploaded="OnImagesUploaded" 
              OnImageCountChanged="OnImageCountChanged" />
```

**Features:**
- Drag-and-drop file upload
- File type and size validation
- Progress tracking
- Error handling
- Real-time count updates

### **ImageGallery Component**
```razor
<ImageGallery Images="@customerImages" 
              OnImageDeleted="OnImageDeleted" />
```

**Features:**
- Grid and carousel view modes
- Image modal for full-screen viewing
- Thumbnail navigation
- Delete functionality
- Responsive design

## 🔧 API Integration

### **Image Service**
- `GetCustomerImagesAsync()` - Retrieve customer images
- `UploadBase64ImageAsync()` - Upload Base64 image data
- `DeleteImageAsync()` - Delete specific image
- `GetImageCountAsync()` - Get current image count
- `ConvertFileToBase64Async()` - Convert files to Base64

### **Customer Service**
- `GetCustomersAsync()` - Retrieve all customers
- `GetCustomerAsync()` - Get specific customer
- `CreateCustomerAsync()` - Create new customer
- `UpdateCustomerAsync()` - Update customer details
- `DeleteCustomerAsync()` - Delete customer

## 📱 Responsive Design

### **Mobile-First Approach**
- Touch-friendly interface
- Optimized for small screens
- Swipe gestures for carousel
- Collapsible navigation

### **Breakpoints**
- **Mobile**: < 768px
- **Tablet**: 768px - 1024px
- **Desktop**: > 1024px

## 🎯 User Workflows

### **1. Adding a New Customer**
1. Navigate to Customers page
2. Click "Add New Customer"
3. Fill in customer details
4. Save customer
5. Navigate to customer profile

### **2. Uploading Images**
1. Go to customer profile
2. Drag images to upload area OR click to browse
3. Images are automatically converted to Base64
4. Upload progress is shown
5. Images appear in gallery

### **3. Managing Images**
1. View images in grid or carousel mode
2. Click image for full-screen modal
3. Use navigation arrows or thumbnails
4. Delete images with delete button
5. Switch between view modes

## 🔒 Security & Validation

### **Client-Side Validation**
- File type checking
- File size validation
- Image count limits
- Input sanitization

### **Server-Side Integration**
- API response validation
- Error handling
- Secure HTTP communication
- Data validation

## 🚀 Performance Optimizations

### **Image Handling**
- Base64 conversion optimization
- Memory stream management
- Efficient file processing
- Lazy loading for large galleries

### **UI Performance**
- Component state management
- Minimal re-renders
- Efficient event handling
- Responsive animations

## 🧪 Testing

### **Manual Testing**
1. **Upload Testing**:
   - Test drag-and-drop functionality
   - Verify file type validation
   - Check file size limits
   - Test multiple file uploads

2. **Gallery Testing**:
   - Test grid and carousel views
   - Verify image modal functionality
   - Test thumbnail navigation
   - Check delete functionality

3. **Responsive Testing**:
   - Test on different screen sizes
   - Verify mobile interactions
   - Check touch gestures

### **API Testing**
Use the provided `Base64ImageTest.http` file to test API endpoints.

## 🔧 Troubleshooting

### **Common Issues**

1. **API Connection Failed**
   - Check API URL in `Program.cs`
   - Verify API is running
   - Check CORS configuration

2. **Image Upload Fails**
   - Verify file type and size
   - Check Base64 conversion
   - Review API error messages

3. **Images Not Displaying**
   - Check Base64 data format
   - Verify content type
   - Review browser console

### **Debug Mode**
Enable detailed logging in `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "ImageUploadWeb": "Debug"
    }
  }
}
```

## 📈 Future Enhancements

### **Planned Features**
- Image compression before upload
- Batch image operations
- Image metadata extraction
- Advanced filtering and search
- Image sharing capabilities
- Export functionality

### **Performance Improvements**
- Image caching
- Lazy loading
- Virtual scrolling
- CDN integration
- Progressive loading

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Blazor Server framework
- Bootstrap for styling
- Open Iconic for icons
- ASP.NET Core for backend integration

---

**Built with ❤️ using Blazor Server and ASP.NET Core**

# Blazor Image Upload Testing Guide

This guide provides comprehensive testing instructions for the Blazor Image Upload application.

## 🧪 Testing Overview

The application includes multiple testing approaches:
- **Manual Testing**: Interactive testing through the UI
- **API Testing**: Direct API endpoint testing
- **Component Testing**: Individual component functionality
- **Integration Testing**: End-to-end workflow testing

## 🚀 Quick Start Testing

### **1. Start the Applications**
```bash
# Terminal 1 - Start the API
cd ImageUploadTask
dotnet run

# Terminal 2 - Start the Blazor App
cd ImageUploadWeb
dotnet run
```

### **2. Access the Applications**
- **Blazor App**: https://localhost:5001
- **API**: https://localhost:7000
- **API Documentation**: https://localhost:7000/swagger

## 📋 Test Scenarios

### **Scenario 1: Basic Image Upload**

#### **Test Steps:**
1. Navigate to the home page
2. Click "Manage Customers" or go to `/customers`
3. Click "Add New Customer"
4. Fill in customer details:
   - First Name: `John`
   - Last Name: `Doe`
   - Email: `john.doe@example.com`
   - Phone: `123-456-7890`
5. Click "Create Customer"
6. Click on the created customer to view profile
7. In the image upload area, drag and drop an image file
8. Verify the image appears in the gallery

#### **Expected Results:**
- Customer is created successfully
- Image upload shows progress bar
- Image appears in both grid and carousel views
- Image count updates to 1/10

### **Scenario 2: Drag and Drop Upload**

#### **Test Steps:**
1. Go to a customer profile page
2. Prepare multiple image files (JPEG, PNG, GIF, WebP)
3. Drag multiple files onto the upload area
4. Verify all files are processed
5. Check that images appear in the gallery

#### **Expected Results:**
- Drag area highlights when files are dragged over
- All valid image files are uploaded
- Invalid files show error messages
- Progress bar shows upload progress

### **Scenario 3: Image Gallery Views**

#### **Test Steps:**
1. Upload several images to a customer
2. Test grid view:
   - Images display in a grid layout
   - Hover effects work
   - Click to open modal
3. Switch to carousel view:
   - Click "Carousel View" button
   - Use navigation arrows
   - Click thumbnails to navigate
   - Test responsive behavior

#### **Expected Results:**
- Grid view shows images in organized layout
- Carousel view allows smooth navigation
- Modal opens with full-size image
- Thumbnails are clickable
- View switching works seamlessly

### **Scenario 4: Image Limits**

#### **Test Steps:**
1. Create a new customer
2. Upload 10 images (one by one)
3. Try to upload an 11th image
4. Verify limit enforcement
5. Delete one image
6. Try uploading again

#### **Expected Results:**
- Upload is blocked at 10 images
- Clear error message about limit
- After deletion, upload works again
- Count display updates correctly

### **Scenario 5: File Validation**

#### **Test Steps:**
1. Try uploading invalid file types:
   - Text files (.txt)
   - PDF files (.pdf)
   - Video files (.mp4)
2. Try uploading oversized files (>5MB)
3. Try uploading corrupted image files

#### **Expected Results:**
- Invalid file types show error messages
- Oversized files are rejected
- Clear validation messages appear
- Only valid images are processed

### **Scenario 6: Responsive Design**

#### **Test Steps:**
1. Test on desktop (1920x1080)
2. Test on tablet (768x1024)
3. Test on mobile (375x667)
4. Test different orientations
5. Test touch interactions

#### **Expected Results:**
- Layout adapts to screen size
- Touch interactions work on mobile
- Navigation is accessible
- Images scale appropriately

## 🔧 Component Testing

### **ImageUpload Component**

#### **Test Cases:**
1. **File Selection**:
   - Click to open file dialog
   - Select single file
   - Select multiple files
   - Cancel file selection

2. **Drag and Drop**:
   - Drag files over area
   - Drop files on area
   - Drag files outside area
   - Test with different file types

3. **Validation**:
   - Test file type validation
   - Test file size validation
   - Test image count limits
   - Test error message display

4. **Progress Tracking**:
   - Monitor upload progress
   - Test progress bar animation
   - Test completion states

### **ImageGallery Component**

#### **Test Cases:**
1. **Grid View**:
   - Display images in grid
   - Test hover effects
   - Test click to modal
   - Test delete functionality

2. **Carousel View**:
   - Navigate with arrows
   - Click thumbnails
   - Test keyboard navigation
   - Test touch gestures

3. **Modal**:
   - Open image modal
   - Close modal
   - Test image scaling
   - Test image information display

## 🌐 API Integration Testing

### **Test API Endpoints**

Use the provided `Base64ImageTest.http` file or test manually:

#### **1. Customer Management**
```http
# Get all customers
GET https://localhost:7000/api/Customer

# Create customer
POST https://localhost:7000/api/Customer
Content-Type: application/json
{
  "firstName": "Test",
  "lastName": "Customer",
  "email": "test@example.com",
  "phone": "123-456-7890"
}
```

#### **2. Image Management**
```http
# Upload Base64 image
POST https://localhost:7000/api/Image/upload-base64
Content-Type: application/json
{
  "customerId": 1,
  "base64Data": "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==",
  "fileName": "test.png",
  "contentType": "image/png",
  "description": "Test image"
}

# Get customer images
GET https://localhost:7000/api/Image/customer/1

# Get image count
GET https://localhost:7000/api/Image/count/1
```

## 🐛 Common Issues & Solutions

### **Issue 1: Images Not Uploading**
**Symptoms**: Upload progress shows but images don't appear
**Solutions**:
- Check API connection in browser dev tools
- Verify API is running on correct port
- Check CORS configuration
- Review console for JavaScript errors

### **Issue 2: Images Not Displaying**
**Symptoms**: Images upload but don't show in gallery
**Solutions**:
- Check Base64 data format
- Verify content type headers
- Review image data in browser dev tools
- Test with different image formats

### **Issue 3: Drag and Drop Not Working**
**Symptoms**: Drag area doesn't respond to files
**Solutions**:
- Check JavaScript file is loaded
- Verify browser compatibility
- Test with different browsers
- Check for JavaScript errors

### **Issue 4: Mobile Issues**
**Symptoms**: Touch interactions don't work
**Solutions**:
- Test on actual mobile device
- Check responsive CSS
- Verify touch event handling
- Test different mobile browsers

## 📊 Performance Testing

### **Load Testing**
1. Upload multiple large images simultaneously
2. Test with many customers and images
3. Monitor memory usage
4. Check response times

### **Stress Testing**
1. Rapidly upload/delete images
2. Test with maximum image count
3. Test concurrent users
4. Monitor API performance

## 🔍 Debugging Tips

### **Browser Dev Tools**
1. **Console**: Check for JavaScript errors
2. **Network**: Monitor API calls and responses
3. **Application**: Check local storage and session
4. **Performance**: Monitor rendering performance

### **Server Logs**
1. Check API logs for errors
2. Monitor database queries
3. Check file system operations
4. Review authentication logs

### **Blazor Debugging**
1. Use `@inject ILogger<T>` for logging
2. Add breakpoints in C# code
3. Use browser dev tools for JavaScript
4. Check component state updates

## ✅ Test Checklist

### **Functionality Tests**
- [ ] Customer creation works
- [ ] Customer editing works
- [ ] Customer deletion works
- [ ] Image upload works (drag & drop)
- [ ] Image upload works (click to browse)
- [ ] Image gallery displays correctly
- [ ] Grid view works
- [ ] Carousel view works
- [ ] Image modal works
- [ ] Image deletion works
- [ ] Image limits enforced
- [ ] File validation works
- [ ] Error messages display
- [ ] Success messages display

### **UI/UX Tests**
- [ ] Responsive design works
- [ ] Touch interactions work
- [ ] Hover effects work
- [ ] Loading states display
- [ ] Animations are smooth
- [ ] Navigation works
- [ ] Forms validate correctly
- [ ] Accessibility features work

### **Integration Tests**
- [ ] API communication works
- [ ] Data persistence works
- [ ] Real-time updates work
- [ ] Error handling works
- [ ] Performance is acceptable
- [ ] Security measures work

## 📝 Test Results Template

```
Test Date: ___________
Tester: ___________
Browser: ___________
Device: ___________

### Test Results:
- [ ] Pass - Customer Management
- [ ] Pass - Image Upload
- [ ] Pass - Image Gallery
- [ ] Pass - Responsive Design
- [ ] Pass - Error Handling
- [ ] Pass - Performance

### Issues Found:
1. Issue: ___________
   Severity: High/Medium/Low
   Status: Fixed/Open/Deferred

2. Issue: ___________
   Severity: High/Medium/Low
   Status: Fixed/Open/Deferred

### Recommendations:
- ___________
- ___________
- ___________
```

## 🎯 Success Criteria

The application is considered successfully tested when:
- All core functionality works as expected
- UI is responsive and user-friendly
- Error handling is comprehensive
- Performance meets requirements
- No critical bugs remain
- User experience is intuitive

---

**Happy Testing! 🚀**

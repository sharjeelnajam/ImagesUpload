// JavaScript functions for image upload functionality

window.clickElement = (element) => {
    element.click();
};

window.getFilesFromInput = (element) => {
    return element.files;
};

window.getDroppedFiles = (dragEvent) => {
    return dragEvent.dataTransfer.files;
};

window.convertFileToBase64 = (file) => {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => {
            const base64 = reader.result.split(',')[1]; // Remove data:image/...;base64, prefix
            resolve(base64);
        };
        reader.onerror = error => reject(error);
    });
};

window.validateImageFile = (file) => {
    const validTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
    const maxSize = 5 * 1024 * 1024; // 5MB
    
    if (!validTypes.includes(file.type.toLowerCase())) {
        return { valid: false, error: `Invalid file type: ${file.type}. Supported types: JPEG, PNG, GIF, WebP` };
    }
    
    if (file.size > maxSize) {
        return { valid: false, error: `File too large: ${(file.size / 1024 / 1024).toFixed(2)}MB. Maximum size: 5MB` };
    }
    
    return { valid: true };
};

window.formatFileSize = (bytes) => {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
};

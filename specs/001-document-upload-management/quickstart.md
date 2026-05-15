# Quick Start: Document Upload and Management

**Date**: 2026-05-15
**Feature**: specs/001-document-upload-management/spec.md

## For Users

### Getting Started

1. **Login** to the ContosoDashboard using your credentials
2. **Navigate** to the main dashboard or projects section
3. **Look for** the "Documents" menu item or "Upload Document" button

### Uploading Your First Document

1. Click "Upload Document" or "Add Document"
2. **Select a file** from your computer (PDF, Word, Excel, PowerPoint, text, or images)
3. **Fill in metadata**:
   - Title (required)
   - Description (optional)
   - Category (required - choose from dropdown)
   - Associated project (optional)
   - Tags (optional - comma-separated)
4. Click "Upload" and wait for the progress indicator
5. **Success!** Your document appears in "My Documents"

### Browsing Documents

- **My Documents**: View all documents you've uploaded
- **Project Documents**: See documents for specific projects (if you're a team member)
- **Shared with Me**: Documents others have shared with you

### Searching Documents

- Use the search box to find documents by title, description, tags, or uploader
- Filter by category or date range
- Sort by title, upload date, category, or file size

### Managing Documents

- **Download**: Click the download button next to any document
- **Preview**: Click to preview PDFs and images in your browser
- **Edit**: Click "Edit" to update title, description, category, or tags
- **Share**: Click "Share" to give access to other team members
- **Delete**: Click "Delete" to remove documents you own

## For Developers

### Architecture Overview

The document management feature follows clean architecture principles:

- **Data Layer**: Entity Framework entities and database context
- **Service Layer**: Business logic in DocumentService, FileStorageService
- **API Layer**: ASP.NET Core controllers for REST endpoints
- **UI Layer**: Blazor components for user interface

### Key Interfaces

```csharp
public interface IFileStorageService
{
    Task<string> UploadAsync(IFormFile file, string filePath);
    Task<Stream> DownloadAsync(string filePath);
    Task DeleteAsync(string filePath);
    Task<bool> ExistsAsync(string filePath);
}
```

### File Storage Pattern

Files are stored outside `wwwroot` for security:
- Path pattern: `{userId}/{projectId or "personal"}/{guid}.{extension}`
- Example: `uploads/123/personal/abc123-def456.pdf`

### Adding New File Types

1. Update allowed MIME types in `DocumentService`
2. Add file extension validation
3. Update UI file input accept attribute

### Extending Search

Current search uses simple LIKE queries. For advanced search:
1. Add full-text indexing to database
2. Implement search service with ranking
3. Update API endpoints to accept advanced filters

### Testing

Run tests with:
```bash
dotnet test --filter "Document"
```

Key test areas:
- File upload validation
- Access control
- Search functionality
- Storage operations

### Deployment Notes

- Ensure upload directory exists and is writable
- Configure file size limits in `appsettings.json`
- Set up proper permissions for file storage directory
- Monitor disk space for uploaded files
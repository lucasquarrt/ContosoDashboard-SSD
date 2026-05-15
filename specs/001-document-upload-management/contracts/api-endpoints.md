# API Contracts: Document Management Endpoints

**Date**: 2026-05-15
**Feature**: specs/001-document-upload-management/spec.md

## Overview

The document management feature exposes RESTful API endpoints for upload, retrieval, and management of documents. All endpoints require authentication and enforce role-based access control.

## Authentication

All endpoints require valid authentication cookie. User identity is extracted from claims.

## Common Response Formats

### Success Response
```json
{
  "success": true,
  "data": { ... },
  "message": "Operation completed successfully"
}
```

### Error Response
```json
{
  "success": false,
  "error": "Error message",
  "code": "ERROR_CODE"
}
```

## Endpoints

### 1. Upload Document

**Endpoint**: `POST /api/documents/upload`

**Authorization**: Any authenticated user

**Request**:
- Content-Type: `multipart/form-data`
- Fields:
  - `file`: IFormFile (required)
  - `title`: string (required)
  - `description`: string (optional)
  - `category`: string (required, from predefined list)
  - `projectId`: int (optional)
  - `tags`: string (optional, comma-separated)

**Response** (Success):
```json
{
  "success": true,
  "data": {
    "documentId": 123,
    "title": "Project Plan.pdf",
    "filePath": "uploads/456/789-abc-def.pdf"
  },
  "message": "Document uploaded successfully"
}
```

**Validation Errors**:
- File size exceeds 25MB: `FILE_TOO_LARGE`
- Unsupported file type: `INVALID_FILE_TYPE`
- Missing required fields: `VALIDATION_ERROR`

### 2. List Documents

**Endpoint**: `GET /api/documents`

**Authorization**: Any authenticated user (sees only accessible documents)

**Query Parameters**:
- `category`: string (optional filter)
- `projectId`: int (optional filter)
- `search`: string (optional search term)
- `sortBy`: string (optional: title, uploadDate, category, fileSize)
- `sortOrder`: string (optional: asc, desc)
- `page`: int (optional, default 1)
- `pageSize`: int (optional, default 20)

**Response** (Success):
```json
{
  "success": true,
  "data": {
    "documents": [
      {
        "documentId": 123,
        "title": "Project Plan.pdf",
        "category": "Project Documents",
        "uploadDate": "2026-05-15T10:30:00Z",
        "fileSize": 2048576,
        "uploaderName": "John Doe",
        "projectName": "Q3 Dashboard Update"
      }
    ],
    "totalCount": 45,
    "page": 1,
    "pageSize": 20
  }
}
```

### 3. Download Document

**Endpoint**: `GET /api/documents/{documentId}/download`

**Authorization**: User must have access to the document (owner, shared with, or project member)

**Response**: Binary file stream with appropriate Content-Type and Content-Disposition headers

**Error Codes**:
- Document not found: `DOCUMENT_NOT_FOUND`
- Access denied: `ACCESS_DENIED`

### 4. Update Document Metadata

**Endpoint**: `PUT /api/documents/{documentId}`

**Authorization**: Document owner only

**Request**:
```json
{
  "title": "Updated Title",
  "description": "Updated description",
  "category": "Reports",
  "tags": "updated,tags"
}
```

**Response**: Standard success/error format

### 5. Delete Document

**Endpoint**: `DELETE /api/documents/{documentId}`

**Authorization**: Document owner or project manager

**Response**: Standard success/error format

### 6. Share Document

**Endpoint**: `POST /api/documents/{documentId}/share`

**Authorization**: Document owner or project manager

**Request**:
```json
{
  "userIds": [456, 789]
}
```

**Response**: Standard success/error format

**Notes**:
- Triggers notification to shared users
- Validates users exist and have access to related project if applicable

## Error Codes Reference

- `VALIDATION_ERROR`: Input validation failed
- `FILE_TOO_LARGE`: File exceeds size limit
- `INVALID_FILE_TYPE`: File type not supported
- `DOCUMENT_NOT_FOUND`: Document does not exist
- `ACCESS_DENIED`: User lacks permission
- `STORAGE_ERROR`: File system error during upload/download
- `QUOTA_EXCEEDED`: User storage quota exceeded (future feature)
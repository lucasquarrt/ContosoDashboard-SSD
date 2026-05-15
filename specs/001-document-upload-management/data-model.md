# Data Model: Document Upload and Management

**Date**: 2026-05-15
**Feature**: specs/001-document-upload-management/spec.md

## Entities

### Document

Represents an uploaded file with its metadata.

**Attributes**:
- `DocumentId`: int (Primary Key, Auto-increment)
- `Title`: string (Required, max 255 chars)
- `Description`: string (Optional, max 1000 chars)
- `Category`: string (Required, enum values: "Project Documents", "Team Resources", "Personal Files", "Reports", "Presentations", "Other")
- `ProjectId`: int? (Foreign Key to Project.ProjectId, nullable)
- `Tags`: string (Optional, comma-separated values, max 500 chars)
- `UploadDate`: DateTime (Required, default: current timestamp)
- `UploaderId`: int (Foreign Key to User.UserId, Required)
- `FileSize`: long (Required, bytes)
- `FileType`: string (Required, MIME type, max 255 chars)
- `FilePath`: string (Required, relative path to stored file)

**Relationships**:
- Belongs to User (UploaderId) - Many-to-One
- Belongs to Project (ProjectId) - Many-to-One, Optional
- Has many DocumentShares - One-to-Many

**Validation Rules**:
- Title: Required, not empty, max 255 characters
- Category: Required, must be one of predefined values
- FileSize: Must be <= 25,000,000 bytes (25 MB)
- FileType: Must be in allowed MIME types (application/pdf, application/msword, etc.)
- FilePath: Must be unique, follow GUID-based pattern

**State Transitions**:
- Created: Initial state after successful upload
- Updated: Metadata can be edited by uploader
- Deleted: Can be deleted by uploader or project manager

### DocumentShare

Represents sharing of a document with another user.

**Attributes**:
- `DocumentShareId`: int (Primary Key, Auto-increment)
- `DocumentId`: int (Foreign Key to Document.DocumentId, Required)
- `SharedWithUserId`: int (Foreign Key to User.UserId, Required)
- `SharedByUserId`: int (Foreign Key to User.UserId, Required)
- `ShareDate`: DateTime (Required, default: current timestamp)

**Relationships**:
- Belongs to Document (DocumentId) - Many-to-One
- Belongs to SharedWith User (SharedWithUserId) - Many-to-One
- Belongs to SharedBy User (SharedByUserId) - Many-to-One

**Validation Rules**:
- Cannot share with self (SharedWithUserId != SharedByUserId)
- Document must exist and be accessible to sharer
- Share must be unique (no duplicate shares for same document-user pair)

**State Transitions**:
- Created: When document is shared
- Removed: When share is revoked (not implemented in initial version)

## Database Schema Notes

- Use integer primary keys for consistency with existing User and Project tables
- Category stored as string for simplicity (not enum)
- FileType field accommodates long MIME types for Office documents
- FilePath uses GUID-based naming for security
- Indexes recommended on: Document.UploaderId, Document.ProjectId, Document.Category, DocumentShare.DocumentId
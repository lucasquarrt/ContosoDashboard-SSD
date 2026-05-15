# Research Findings: Document Upload and Management

**Date**: 2026-05-15
**Feature**: specs/001-document-upload-management/spec.md

## Secure File Upload in ASP.NET Core

**Decision**: Use ASP.NET Core's IFormFile for file uploads with server-side validation for file type, size, and content. Store files outside wwwroot directory using GUID-based filenames to prevent path traversal attacks.

**Rationale**: IFormFile provides secure streaming upload without loading entire file in memory. Server-side validation ensures only allowed file types are accepted. Storing outside wwwroot requires controller endpoints for access, enabling authorization checks. GUID filenames prevent malicious filename attacks.

**Alternatives Considered**:
- Client-side validation only: Insufficient security, easily bypassed
- Direct file system access: Higher risk of path traversal vulnerabilities

## File Storage Abstraction Pattern

**Decision**: Implement IFileStorageService interface with LocalFileStorageService concrete implementation. Interface methods: UploadAsync, DownloadAsync, DeleteAsync, GetUrlAsync.

**Rationale**: Enables clean separation of storage logic and easy migration to cloud storage (Azure Blob Storage) by swapping implementations via dependency injection. Local implementation uses System.IO.File operations with proper error handling.

**Alternatives Considered**:
- Direct file operations in controllers: Tightly couples storage logic to business logic, harder to test and migrate
- Abstract base class: Less flexible than interface for different storage providers

## Dashboard Integration Patterns

**Decision**: Add "Recent Documents" widget to main dashboard, integrate document upload/download into project and task detail pages, extend notification system for document shares.

**Rationale**: Provides seamless user experience within existing workflow. Users can access documents without leaving their current context. Notifications ensure awareness of shared documents.

**Alternatives Considered**:
- Standalone document management page: Creates additional navigation burden, reduces adoption
- Minimal integration: Limits feature discoverability and usage

## Document Search Performance

**Decision**: Use database indexing on frequently searched fields (title, category, tags, uploader). Implement simple LIKE queries with pagination. Target sub-2-second response times.

**Rationale**: For expected scale (500 documents), database indexing provides sufficient performance without complexity. LIKE queries handle partial matches effectively. Pagination prevents large result sets.

**Alternatives Considered**:
- Full-text search (SQL Server FTS or Elasticsearch): Overkill for small dataset, increases complexity
- Client-side filtering: Poor performance for large lists, requires loading all data
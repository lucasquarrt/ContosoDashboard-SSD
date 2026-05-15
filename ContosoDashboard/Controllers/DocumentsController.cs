using ContosoDashboard.Models;
using ContosoDashboard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ContosoDashboard.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly DocumentService _documentService;
    private readonly IFileStorageService _fileStorage;

    // Allowed file types
    private static readonly string[] AllowedExtensions = { ".pdf", ".docx", ".xlsx", ".pptx", ".txt", ".jpg", ".jpeg", ".png" };
    private static readonly string[] AllowedMimeTypes = {
        "application/pdf",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "text/plain",
        "image/jpeg",
        "image/png"
    };

    private const long MaxFileSize = 25 * 1024 * 1024; // 25 MB

    public DocumentsController(DocumentService documentService, IFileStorageService fileStorage)
    {
        _documentService = documentService;
        _fileStorage = fileStorage;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<IActionResult> UploadDocument(
        IFormFile file,
        [FromForm] string title,
        [FromForm] string? description,
        [FromForm] string category,
        [FromForm] int? projectId,
        [FromForm] string? tags)
    {
        try
        {
            // Validate input
            if (file == null || file.Length == 0)
                return BadRequest(new { success = false, error = "No file provided" });

            if (string.IsNullOrWhiteSpace(title))
                return BadRequest(new { success = false, error = "Title is required" });

            if (string.IsNullOrWhiteSpace(category))
                return BadRequest(new { success = false, error = "Category is required" });

            // Validate file size
            if (file.Length > MaxFileSize)
                return BadRequest(new { success = false, error = "File size exceeds 25 MB limit" });

            // Validate file type
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return BadRequest(new { success = false, error = "File type not supported" });

            if (!AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
                return BadRequest(new { success = false, error = "Invalid file content type" });

            // Validate category
            if (!Enum.TryParse<DocumentCategory>(category.Replace(" ", ""), true, out var documentCategory))
                return BadRequest(new { success = false, error = "Invalid category" });

            // Get current user
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized(new { success = false, error = "Invalid user" });

            // Generate unique file path
            var uniqueId = Guid.NewGuid();
            var userFolder = projectId.HasValue ? $"{userId}/{projectId}" : $"{userId}/personal";
            var filePath = $"{userFolder}/{uniqueId}{extension}";

            // Upload file
            await _fileStorage.UploadAsync(file, filePath);

            // Create document record
            var document = new Document
            {
                Title = title.Trim(),
                Description = description?.Trim(),
                Category = documentCategory,
                ProjectId = projectId,
                Tags = tags?.Trim(),
                UploaderId = userId,
                FileSize = file.Length,
                FileType = file.ContentType,
                FilePath = filePath
            };

            var createdDocument = await _documentService.CreateDocumentAsync(document);

            return Ok(new
            {
                success = true,
                data = new
                {
                    documentId = createdDocument.DocumentId,
                    title = createdDocument.Title,
                    filePath = createdDocument.FilePath
                },
                message = "Document uploaded successfully"
            });
        }
        catch (Exception ex)
        {
            // Clean up file if database save failed
            // Implementation would need to track temp files

            return StatusCode(500, new { success = false, error = "Upload failed", details = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetDocuments(
        [FromQuery] string? category,
        [FromQuery] int? projectId,
        [FromQuery] string? search,
        [FromQuery] string? sortBy = "uploadDate",
        [FromQuery] string? sortOrder = "desc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            // Get user's documents
            var userDocuments = await _documentService.GetDocumentsByUserAsync(userId);

            // Get shared documents
            var sharedDocuments = await _documentService.GetSharedDocumentsAsync(userId);

            // Combine and filter
            var allDocuments = userDocuments.Concat(sharedDocuments).DistinctBy(d => d.DocumentId);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(category))
            {
                if (Enum.TryParse<DocumentCategory>(category.Replace(" ", ""), true, out var cat))
                {
                    allDocuments = allDocuments.Where(d => d.Category == cat);
                }
            }

            if (projectId.HasValue)
            {
                allDocuments = allDocuments.Where(d => d.ProjectId == projectId);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.ToLowerInvariant();
                allDocuments = allDocuments.Where(d =>
                    d.Title.ToLowerInvariant().Contains(searchTerm) ||
                    (d.Description?.ToLowerInvariant().Contains(searchTerm) ?? false) ||
                    (d.Tags?.ToLowerInvariant().Contains(searchTerm) ?? false) ||
                    (d.Uploader?.DisplayName.ToLowerInvariant().Contains(searchTerm) ?? false));
            }

            // Apply sorting
            allDocuments = sortBy?.ToLowerInvariant() switch
            {
                "title" => sortOrder == "asc" ? allDocuments.OrderBy(d => d.Title) : allDocuments.OrderByDescending(d => d.Title),
                "category" => sortOrder == "asc" ? allDocuments.OrderBy(d => d.Category) : allDocuments.OrderByDescending(d => d.Category),
                "filesize" => sortOrder == "asc" ? allDocuments.OrderBy(d => d.FileSize) : allDocuments.OrderByDescending(d => d.FileSize),
                _ => sortOrder == "asc" ? allDocuments.OrderBy(d => d.UploadDate) : allDocuments.OrderByDescending(d => d.UploadDate)
            };

            // Apply pagination
            var totalCount = allDocuments.Count();
            var documents = allDocuments
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new
                {
                    documentId = d.DocumentId,
                    title = d.Title,
                    category = d.Category.ToString(),
                    uploadDate = d.UploadDate,
                    fileSize = d.FileSize,
                    uploaderName = d.Uploader?.DisplayName,
                    projectName = d.Project?.Name
                })
                .ToList();

            return Ok(new
            {
                success = true,
                data = new
                {
                    documents,
                    totalCount,
                    page,
                    pageSize
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Failed to retrieve documents", details = ex.Message });
        }
    }

    [HttpGet("{documentId}/download")]
    public async Task<IActionResult> DownloadDocument(int documentId)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            // Check access
            if (!await _documentService.UserCanAccessDocumentAsync(userId, documentId))
                return Forbid();

            var document = await _documentService.GetDocumentByIdAsync(documentId);
            if (document == null)
                return NotFound(new { success = false, error = "Document not found" });

            var stream = await _fileStorage.DownloadAsync(document.FilePath);
            return File(stream, document.FileType, document.Title + Path.GetExtension(document.FilePath));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Download failed", details = ex.Message });
        }
    }
}
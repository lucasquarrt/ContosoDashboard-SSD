using ContosoDashboard.Models;
using ContosoDashboard.Data;
using Microsoft.EntityFrameworkCore;

namespace ContosoDashboard.Services;

public class DocumentService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileStorageService _fileStorage;

    public DocumentService(ApplicationDbContext context, IFileStorageService fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
    }

    public async Task<Document> CreateDocumentAsync(Document document)
    {
        _context.Documents.Add(document);
        await _context.SaveChangesAsync();
        return document;
    }

    public async Task<Document?> GetDocumentByIdAsync(int documentId)
    {
        return await _context.Documents
            .Include(d => d.Uploader)
            .Include(d => d.Project)
            .FirstOrDefaultAsync(d => d.DocumentId == documentId);
    }

    public async Task<IEnumerable<Document>> GetDocumentsByUserAsync(int userId)
    {
        return await _context.Documents
            .Where(d => d.UploaderId == userId)
            .Include(d => d.Uploader)
            .Include(d => d.Project)
            .OrderByDescending(d => d.UploadDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Document>> GetSharedDocumentsAsync(int userId)
    {
        return await _context.DocumentShares
            .Where(ds => ds.SharedWithUserId == userId)
            .Include(ds => ds.Document)
                .ThenInclude(d => d!.Uploader)
            .Include(ds => ds.Document)
                .ThenInclude(d => d!.Project)
            .Select(ds => ds.Document!)
            .OrderByDescending(d => d.UploadDate)
            .ToListAsync();
    }

    public async Task UpdateDocumentAsync(Document document)
    {
        _context.Documents.Update(document);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteDocumentAsync(int documentId)
    {
        var document = await GetDocumentByIdAsync(documentId);
        if (document != null)
        {
            // Delete file from storage
            await _fileStorage.DeleteAsync(document.FilePath);

            // Delete from database
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> UserCanAccessDocumentAsync(int userId, int documentId)
    {
        var document = await _context.Documents
            .FirstOrDefaultAsync(d => d.DocumentId == documentId);

        if (document == null)
            return false;

        // Owner can access
        if (document.UploaderId == userId)
            return true;

        // Check if shared with user
        var isShared = await _context.DocumentShares
            .AnyAsync(ds => ds.DocumentId == documentId && ds.SharedWithUserId == userId);

        if (isShared)
            return true;

        // Check if user is project member (for project documents)
        if (document.ProjectId.HasValue)
        {
            var isProjectMember = await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == document.ProjectId && pm.UserId == userId);

            if (isProjectMember)
                return true;
        }

        return false;
    }
}
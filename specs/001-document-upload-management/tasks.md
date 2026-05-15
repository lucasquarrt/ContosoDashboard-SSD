# Tasks: Document Upload and Management

**Input**: Design documents from `/specs/001-document-upload-management/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/, quickstart.md

**Tests**: Tests are included following TDD principles - write tests first, then implement.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Single project**: `src/`, `tests/` at repository root
- **Web app**: `backend/src/`, `frontend/src/`
- Paths shown below assume single project - adjust based on plan.md structure

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [X] T001 Create project structure per implementation plan
- [X] T002 Initialize C# .NET project with Blazor Server dependencies
- [X] T003 [P] Configure Entity Framework Core for SQLite database
- [X] T004 [P] Set up local file storage directory structure

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T005 Implement IFileStorageService interface in src/Services/IFileStorageService.cs
- [X] T006 [P] Create LocalFileStorageService implementation in src/Services/LocalFileStorageService.cs
- [X] T007 [P] Add Document and DocumentShare entities to src/Models/
- [X] T008 [P] Configure database context for document entities
- [X] T009 [P] Create base DocumentService in src/Services/DocumentService.cs
- [X] T010 [P] Set up ASP.NET Core controllers for document API endpoints
- [X] T011 [P] Configure dependency injection for file storage and document services

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Upload Documents (Priority: P1) 🎯 MVP

**Goal**: Enable users to securely upload documents with metadata to the centralized system

**Independent Test**: Can upload a valid file, verify storage and database record creation, and confirm accessibility

### Tests for User Story 1 (TDD - write tests first)

- [X] T012 [P] [US1] Create contract tests for file upload validation in tests/Contract/TestDocumentUpload.cs
- [X] T013 [P] [US1] Create integration tests for upload workflow in tests/Integration/TestDocumentUpload.cs

### Implementation for User Story 1

- [X] T014 [US1] Implement file upload endpoint in src/Controllers/DocumentsController.cs (POST /api/documents/upload)
- [X] T015 [US1] Add file validation logic (size, type) in DocumentService
- [X] T016 [US1] Implement metadata storage in database
- [X] T017 [US1] Add progress indication to upload UI
- [X] T018 [US1] Create upload form component in src/Pages/Documents/Upload.razor

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - Browse and Search Documents (Priority: P2)

**Goal**: Allow users to efficiently find and access their documents through browsing and search

**Independent Test**: Can search documents by various criteria and receive results within 2 seconds

### Tests for User Story 2

- [ ] T019 [P] [US2] Create contract tests for document search API in tests/Contract/TestDocumentSearch.cs
- [ ] T020 [P] [US2] Create integration tests for search functionality in tests/Integration/TestDocumentSearch.cs

### Implementation for User Story 2

- [ ] T021 [US2] Implement document list endpoint in DocumentsController.cs (GET /api/documents)
- [ ] T022 [US2] Add search and filter logic to DocumentService
- [ ] T023 [US2] Implement sorting and pagination
- [ ] T024 [US2] Create document list UI component in src/Pages/Documents/Index.razor
- [ ] T025 [US2] Add search and filter controls to UI

**Checkpoint**: User Story 2 complete - users can browse and search all documents

---

## Phase 5: User Story 3 - Share Documents (Priority: P3)

**Goal**: Enable collaboration by allowing document sharing with team members

**Independent Test**: Can share a document and verify recipient access and notifications

### Tests for User Story 3

- [ ] T026 [P] [US3] Create contract tests for document sharing in tests/Contract/TestDocumentSharing.cs
- [ ] T027 [P] [US3] Create integration tests for share workflow in tests/Integration/TestDocumentSharing.cs

### Implementation for User Story 3

- [ ] T028 [US3] Implement document sharing endpoint (POST /api/documents/{id}/share)
- [ ] T029 [US3] Add sharing logic to DocumentService
- [ ] T030 [US3] Integrate with notification system for share alerts
- [ ] T031 [US3] Create sharing UI in document detail view
- [ ] T032 [US3] Add "Shared with Me" document list

**Checkpoint**: User Story 3 complete - full document management functionality available

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Final enhancements, integrations, and quality improvements

- [ ] T033 [P] Add document preview functionality for PDFs and images
- [ ] T034 [P] Implement document download endpoint and UI
- [ ] T035 [P] Add document edit metadata functionality
- [ ] T036 [P] Implement document deletion with confirmation
- [ ] T037 [P] Integrate with project and task views
- [ ] T038 [P] Add "Recent Documents" widget to dashboard
- [ ] T039 [P] Add activity logging for audit purposes
- [ ] T040 [P] Implement role-based access control throughout
- [ ] T041 [P] Add comprehensive error handling and user feedback
- [ ] T042 [P] Performance optimization and testing
- [ ] T043 [P] Documentation and code comments

---

## Dependencies

**Story Completion Order**:
1. User Story 1 (Upload) - Foundation for all other features
2. User Story 2 (Browse/Search) - Builds on upload functionality
3. User Story 3 (Share) - Requires upload and browse to be useful

**Task Dependencies**:
- All Phase 1 tasks must complete before Phase 2
- All Phase 2 tasks must complete before any user story tasks
- Within each user story, tests should be written before implementation
- UI tasks depend on API implementation within the same story

## Parallel Execution Examples

**Per User Story**:
- US1: Run T012-T013 in parallel, then T014-T018 sequentially
- US2: Run T019-T020 in parallel, then T021-T025 sequentially
- US3: Run T026-T027 in parallel, then T028-T032 sequentially

**Cross-Story Parallelism**:
- After Phase 2 complete: US1 and US2 can run in parallel (different API endpoints)
- US3 depends on US1 completion but can run parallel with US2 polish

## Implementation Strategy

**MVP Scope**: User Story 1 (Upload Documents) - delivers core value immediately
**Incremental Delivery**: Add browse/search (US2), then sharing (US3)
**TDD Approach**: Write failing tests first, then implement minimal code to pass
**Quality Gates**: All tests pass, constitution principles followed, performance requirements met
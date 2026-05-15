<!--
Sync Impact Report
Version change: none → 1.0.0
List of modified principles: All principles added (Security First, Test-Driven Development, Clean Architecture, User-Centric Design, Maintainability and Documentation)
Added sections: Technology Stack and Constraints, Development Workflow
Removed sections: none
Templates requiring updates: none
Follow-up TODOs: none
-->
# ContosoDashboard Constitution

## Core Principles

### Security First
Prioritize security in all aspects; Implement mock authentication for training; Ensure defense in depth with authorization, IDOR protection, and secure headers.

### Test-Driven Development
Write tests before implementing features; Ensure code quality and reliability.

### Clean Architecture
Separate concerns with models, services, and UI layers; Maintain modularity and testability.

### User-Centric Design
Focus on intuitive user experience; Ensure responsive and accessible dashboard interface.

### Maintainability and Documentation
Write clear code with comments; Document features and limitations; Follow best practices for long-term maintenance.

## Technology Stack and Constraints
Built with Blazor Server on .NET 8/10, Entity Framework for data access, local SQLite database; Training-focused with mock auth; No external dependencies for offline training.

## Development Workflow
Use Spec-Driven Development with GitHub Spec Kit; Follow constitution principles; Implement features incrementally with testing.

## Governance
This constitution guides all development decisions; Amendments require review and justification; Compliance verified in code reviews and PRs.

**Version**: 1.0.0 | **Ratified**: 2026-05-15 | **Last Amended**: 2026-05-15

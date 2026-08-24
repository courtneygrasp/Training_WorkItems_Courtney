-- Training.WorkItems database schema
-- Target: TEST_COURTNEY
-- Run this script once to set up the schema before running the application.

USE [TEST_COURTNEY];
GO

CREATE TABLE dbo.WorkItemStorageRecord
(
    WorkItemId  UNIQUEIDENTIFIER   NOT NULL,
    TenantId    UNIQUEIDENTIFIER   NOT NULL,
    Title       NVARCHAR(120)      NOT NULL,
    Description NVARCHAR(4000)     NULL,
    Status      NVARCHAR(40)       NOT NULL,
    CreatedAt   DATETIMEOFFSET     NOT NULL,

    CONSTRAINT PK_WorkItemStorageRecord PRIMARY KEY (WorkItemId)
);
GO

-- All queries filter by TenantId; this index covers ListAsync and GetByIdAsync.
CREATE NONCLUSTERED INDEX IX_WorkItemStorageRecord_TenantId
    ON dbo.WorkItemStorageRecord (TenantId);
GO

CREATE TABLE dbo.WorkItemNoteStorageRecord
(
    NoteId      UNIQUEIDENTIFIER   NOT NULL,
    WorkItemId  UNIQUEIDENTIFIER   NOT NULL,
    TenantId    UNIQUEIDENTIFIER   NOT NULL,
    AuthorId    UNIQUEIDENTIFIER   NOT NULL,
    NoteText    NVARCHAR(2000)     NOT NULL,
    CreatedAt   DATETIMEOFFSET     NOT NULL,

    CONSTRAINT PK_WorkItemNoteStorageRecord PRIMARY KEY (NoteId),
    CONSTRAINT FK_WorkItemNoteStorageRecord_WorkItemStorageRecord
        FOREIGN KEY (WorkItemId) REFERENCES dbo.WorkItemStorageRecord (WorkItemId)
);
GO

-- Supports listing all notes for a work item scoped to a tenant.
CREATE NONCLUSTERED INDEX IX_WorkItemNoteStorageRecord_WorkItemId_TenantId
    ON dbo.WorkItemNoteStorageRecord (WorkItemId, TenantId);
GO

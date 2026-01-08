CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "Users" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Phone" text NOT NULL,
    "Email" text,
    "Role" text NOT NULL,
    "AuthId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    "LastLoginAt" timestamp with time zone,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE "Tests" (
    "Id" uuid NOT NULL,
    "Title" character varying(200) NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedBy" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "StartedAt" timestamp with time zone,
    "EndedAt" timestamp with time zone,
    "ConfigJson" jsonb,
    CONSTRAINT "PK_Tests" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Tests_Users_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AccessCodes" (
    "Id" uuid NOT NULL,
    "TestId" uuid NOT NULL,
    "Code" text NOT NULL,
    "Status" integer NOT NULL,
    "AssignedTo" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    "ActivatedAt" timestamp with time zone,
    "UsedAt" timestamp with time zone,
    "ExpiresAt" timestamp with time zone,
    "AssignedStudentId" uuid,
    CONSTRAINT "PK_AccessCodes" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AccessCodes_Tests_TestId" FOREIGN KEY ("TestId") REFERENCES "Tests" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AccessCodes_Users_AssignedStudentId" FOREIGN KEY ("AssignedStudentId") REFERENCES "Users" ("Id")
);

CREATE TABLE "AntiCheatLogs" (
    "Id" uuid NOT NULL,
    "TestId" uuid NOT NULL,
    "StudentId" uuid NOT NULL,
    "EventType" text NOT NULL,
    "Timestamp" timestamp with time zone NOT NULL,
    "DetailsJson" jsonb,
    CONSTRAINT "PK_AntiCheatLogs" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AntiCheatLogs_Tests_TestId" FOREIGN KEY ("TestId") REFERENCES "Tests" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AntiCheatLogs_Users_StudentId" FOREIGN KEY ("StudentId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Questions" (
    "Id" uuid NOT NULL,
    "TestId" uuid NOT NULL,
    "SectionIndex" integer NOT NULL,
    "QuestionNumber" integer NOT NULL,
    "QuestionText" text NOT NULL,
    "PassageText" text,
    "QuestionType" text NOT NULL,
    "OptionsJson" jsonb,
    "CorrectAnswer" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Questions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Questions_Tests_TestId" FOREIGN KEY ("TestId") REFERENCES "Tests" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Results" (
    "Id" uuid NOT NULL,
    "TestId" uuid NOT NULL,
    "StudentId" uuid NOT NULL,
    "TotalCorrect" integer NOT NULL,
    "TotalIncorrect" integer NOT NULL,
    "RawScore" integer NOT NULL,
    "Percentage" double precision NOT NULL,
    "IsPublished" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "PublishedAt" timestamp with time zone,
    CONSTRAINT "PK_Results" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Results_Tests_TestId" FOREIGN KEY ("TestId") REFERENCES "Tests" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Results_Users_StudentId" FOREIGN KEY ("StudentId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "StudentResponses" (
    "Id" uuid NOT NULL,
    "TestId" uuid NOT NULL,
    "StudentId" uuid NOT NULL,
    "QuestionId" uuid NOT NULL,
    "SelectedAnswer" text,
    "IsFlagged" boolean NOT NULL,
    "AutosavedAt" timestamp with time zone,
    "SubmittedAt" timestamp with time zone,
    "IsSubmitted" boolean NOT NULL,
    CONSTRAINT "PK_StudentResponses" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_StudentResponses_Questions_QuestionId" FOREIGN KEY ("QuestionId") REFERENCES "Questions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_StudentResponses_Tests_TestId" FOREIGN KEY ("TestId") REFERENCES "Tests" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_StudentResponses_Users_StudentId" FOREIGN KEY ("StudentId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_AccessCodes_AssignedStudentId" ON "AccessCodes" ("AssignedStudentId");

CREATE UNIQUE INDEX "IX_AccessCodes_Code" ON "AccessCodes" ("Code");

CREATE INDEX "IX_AccessCodes_TestId" ON "AccessCodes" ("TestId");

CREATE INDEX "IX_AntiCheatLogs_StudentId" ON "AntiCheatLogs" ("StudentId");

CREATE INDEX "IX_AntiCheatLogs_TestId" ON "AntiCheatLogs" ("TestId");

CREATE UNIQUE INDEX "IX_Questions_TestId_SectionIndex_QuestionNumber" ON "Questions" ("TestId", "SectionIndex", "QuestionNumber");

CREATE INDEX "IX_Results_StudentId" ON "Results" ("StudentId");

CREATE INDEX "IX_Results_TestId" ON "Results" ("TestId");

CREATE INDEX "IX_StudentResponses_QuestionId" ON "StudentResponses" ("QuestionId");

CREATE INDEX "IX_StudentResponses_StudentId" ON "StudentResponses" ("StudentId");

CREATE UNIQUE INDEX "IX_StudentResponses_TestId_StudentId_QuestionId" ON "StudentResponses" ("TestId", "StudentId", "QuestionId");

CREATE INDEX "IX_Tests_CreatedBy" ON "Tests" ("CreatedBy");

CREATE INDEX "IX_Users_Email" ON "Users" ("Email");

CREATE UNIQUE INDEX "IX_Users_Phone" ON "Users" ("Phone");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260107201523_InitialCreate', '10.0.0');

COMMIT;


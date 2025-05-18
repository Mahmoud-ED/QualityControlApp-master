IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [Discriminator] nvarchar(21) NOT NULL,
        [Age] int NULL,
        [LastAccessTime] datetime2 NULL,
        [CreatedDate] datetime2 NULL,
        [ModifiedDate] datetime2 NULL,
        [Approval] bit NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE TABLE [Company] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_Company] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE TABLE [Contact] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [Email] nvarchar(max) NOT NULL,
        [Phone] nvarchar(max) NOT NULL,
        [Facebook] nvarchar(max) NOT NULL,
        [Twitter] nvarchar(max) NOT NULL,
        [Instagram] nvarchar(max) NOT NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_Contact] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE TABLE [QuestionCategoryType] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [CategoryName] nvarchar(max) NOT NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_QuestionCategoryType] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE TABLE [SiteInfo] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [Name] nvarchar(max) NOT NULL,
        [Activity] nvarchar(200) NOT NULL,
        [About] nvarchar(2000) NOT NULL,
        [LogoUrl] nvarchar(max) NULL,
        [CoverImageUrl] nvarchar(max) NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_SiteInfo] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE TABLE [SiteState] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [State] bit NOT NULL,
        [ClosingMessage] nvarchar(max) NOT NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_SiteState] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE TABLE [Employees] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [Name] nvarchar(100) NOT NULL,
        [PhoneNumber] nvarchar(max) NOT NULL,
        [Address] nvarchar(200) NOT NULL,
        [YearsOfExperience] int NOT NULL,
        [Specialization] nvarchar(100) NOT NULL,
        [Bio] nvarchar(500) NOT NULL,
        [UserId] nvarchar(450) NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Employees_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE TABLE [UserProfiles] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [DisplayName] nvarchar(max) NOT NULL,
        [ImageUrl] nvarchar(max) NULL,
        [Gender] bit NOT NULL,
        [UserId] nvarchar(450) NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_UserProfiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserProfiles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE TABLE [CompanyQuestion] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [CompanyId] uniqueidentifier NOT NULL,
        [UserId] nvarchar(450) NULL,
        [Active] bit NOT NULL,
        [SaftyGrid] int NOT NULL,
        [SqurtyGrid] int NOT NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_CompanyQuestion] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CompanyQuestion_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]),
        CONSTRAINT [FK_CompanyQuestion_Company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE TABLE [QuestionType] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [TypeName] nvarchar(max) NOT NULL,
        [QuestionCategoryTypeId] uniqueidentifier NOT NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_QuestionType] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuestionType_QuestionCategoryType_QuestionCategoryTypeId] FOREIGN KEY ([QuestionCategoryTypeId]) REFERENCES [QuestionCategoryType] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE TABLE [Question] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [Content] nvarchar(max) NOT NULL,
        [QuestionTypeId] uniqueidentifier NOT NULL,
        [MaxGrid] int NOT NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_Question] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Question_QuestionType_QuestionTypeId] FOREIGN KEY ([QuestionTypeId]) REFERENCES [QuestionType] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE TABLE [CompanyQuestionContent] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [CompanyQuestionId] uniqueidentifier NOT NULL,
        [QuestionId] uniqueidentifier NOT NULL,
        [Score] int NOT NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_CompanyQuestionContent] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CompanyQuestionContent_CompanyQuestion_CompanyQuestionId] FOREIGN KEY ([CompanyQuestionId]) REFERENCES [CompanyQuestion] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CompanyQuestionContent_Question_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Question] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] ON;
    EXEC(N'INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
    VALUES (N''a1587a3b-9448-47d4-96a1-7ac6a0b728c6'', N''8ed2115c-fcdc-4dbd-abd1-ceca51247e15'', N''Prog'', N''PROG'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'Age', N'Approval', N'ConcurrencyStamp', N'CreatedDate', N'Discriminator', N'Email', N'EmailConfirmed', N'LastAccessTime', N'LockoutEnabled', N'LockoutEnd', N'ModifiedDate', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
        SET IDENTITY_INSERT [AspNetUsers] ON;
    EXEC(N'INSERT INTO [AspNetUsers] ([Id], [AccessFailedCount], [Age], [Approval], [ConcurrencyStamp], [CreatedDate], [Discriminator], [Email], [EmailConfirmed], [LastAccessTime], [LockoutEnabled], [LockoutEnd], [ModifiedDate], [NormalizedEmail], [NormalizedUserName], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [SecurityStamp], [TwoFactorEnabled], [UserName])
    VALUES (N''da84f427-4ebe-4706-939f-49a81f68f4ca'', 0, 0, NULL, N''ca9f198b-6aeb-43fa-bad7-46837c2bf09d'', ''2024-11-06T23:49:18.6612577Z'', N''ApplicationUser'', N''reema2783@gmail.com'', CAST(1 AS bit), NULL, CAST(1 AS bit), NULL, NULL, N''REEMA2783@GMAIL.COM'', N''REEMA2783@GMAIL.COM'', N''AQAAAAIAAYagAAAAEO4YnsWF8pMdrnFaGudoaM34armVU2VM+BOFPeGnSinAE02Eqw9c9zB7v6aSCGQzNg=='', NULL, CAST(1 AS bit), N''78e3ca43-37d2-4d28-9c34-49514841ad3f'', CAST(0 AS bit), N''reema2783@gmail.com'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'Age', N'Approval', N'ConcurrencyStamp', N'CreatedDate', N'Discriminator', N'Email', N'EmailConfirmed', N'LastAccessTime', N'LockoutEnabled', N'LockoutEnd', N'ModifiedDate', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
        SET IDENTITY_INSERT [AspNetUsers] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Created', N'Email', N'Facebook', N'Instagram', N'Modified', N'Phone', N'Twitter') AND [object_id] = OBJECT_ID(N'[Contact]'))
        SET IDENTITY_INSERT [Contact] ON;
    EXEC(N'INSERT INTO [Contact] ([Id], [Created], [Email], [Facebook], [Instagram], [Modified], [Phone], [Twitter])
    VALUES (''795f63f9-8dc6-4eeb-80f4-a2efcff453d4'', ''2024-11-07T01:49:18.6612296+02:00'', N''W.Wide@Gmail.com'', N''Worldwide Facebook'', N''Worldwide Instagram'', NULL, N''00218951234567'', N''Worldwide Twitter'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Created', N'Email', N'Facebook', N'Instagram', N'Modified', N'Phone', N'Twitter') AND [object_id] = OBJECT_ID(N'[Contact]'))
        SET IDENTITY_INSERT [Contact] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'About', N'Activity', N'CoverImageUrl', N'Created', N'LogoUrl', N'Modified', N'Name') AND [object_id] = OBJECT_ID(N'[SiteInfo]'))
        SET IDENTITY_INSERT [SiteInfo] ON;
    EXEC(N'INSERT INTO [SiteInfo] ([Id], [About], [Activity], [CoverImageUrl], [Created], [LogoUrl], [Modified], [Name])
    VALUES (''9d0d8c77-1127-4829-aaa1-fa9772775915'', N''We are a specialized news website covering political, sports, and economic events, along with various other sections of general interest to readers. We always strive to provide distinguished and reliable content that reflects the ongoing developments on both the local and global stages.'', N''News site'', NULL, ''2024-11-07T01:49:18.6612089+02:00'', NULL, NULL, N''Worldwide'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'About', N'Activity', N'CoverImageUrl', N'Created', N'LogoUrl', N'Modified', N'Name') AND [object_id] = OBJECT_ID(N'[SiteInfo]'))
        SET IDENTITY_INSERT [SiteInfo] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClosingMessage', N'Created', N'Modified', N'State') AND [object_id] = OBJECT_ID(N'[SiteState]'))
        SET IDENTITY_INSERT [SiteState] ON;
    EXEC(N'INSERT INTO [SiteState] ([Id], [ClosingMessage], [Created], [Modified], [State])
    VALUES (''631c1e49-c795-41ab-b0f9-7b85e22c0b6b'', N''The site is temporarily closed for development'', ''2024-11-07T01:49:18.6612326+02:00'', NULL, CAST(1 AS bit))');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClosingMessage', N'Created', N'Modified', N'State') AND [object_id] = OBJECT_ID(N'[SiteState]'))
        SET IDENTITY_INSERT [SiteState] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
        SET IDENTITY_INSERT [AspNetUserRoles] ON;
    EXEC(N'INSERT INTO [AspNetUserRoles] ([RoleId], [UserId])
    VALUES (N''a1587a3b-9448-47d4-96a1-7ac6a0b728c6'', N''da84f427-4ebe-4706-939f-49a81f68f4ca'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
        SET IDENTITY_INSERT [AspNetUserRoles] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE INDEX [IX_CompanyQuestion_CompanyId] ON [CompanyQuestion] ([CompanyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_CompanyQuestion_UserId] ON [CompanyQuestion] ([UserId]) WHERE [UserId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE INDEX [IX_CompanyQuestionContent_CompanyQuestionId] ON [CompanyQuestionContent] ([CompanyQuestionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE INDEX [IX_CompanyQuestionContent_QuestionId] ON [CompanyQuestionContent] ([QuestionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Employees_UserId] ON [Employees] ([UserId]) WHERE [UserId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE INDEX [IX_Question_QuestionTypeId] ON [Question] ([QuestionTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    CREATE INDEX [IX_QuestionType_QuestionCategoryTypeId] ON [QuestionType] ([QuestionCategoryTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_UserProfiles_UserId] ON [UserProfiles] ([UserId]) WHERE [UserId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241106234920_CreateDB'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241106234920_CreateDB', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241107090654_RemoveUniqueConstraintFromUserId'
)
BEGIN
    EXEC(N'DELETE FROM [AspNetUserRoles]
    WHERE [RoleId] = N''a1587a3b-9448-47d4-96a1-7ac6a0b728c6'' AND [UserId] = N''da84f427-4ebe-4706-939f-49a81f68f4ca'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241107090654_RemoveUniqueConstraintFromUserId'
)
BEGIN
    EXEC(N'DELETE FROM [Contact]
    WHERE [Id] = ''795f63f9-8dc6-4eeb-80f4-a2efcff453d4'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241107090654_RemoveUniqueConstraintFromUserId'
)
BEGIN
    EXEC(N'DELETE FROM [SiteInfo]
    WHERE [Id] = ''9d0d8c77-1127-4829-aaa1-fa9772775915'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241107090654_RemoveUniqueConstraintFromUserId'
)
BEGIN
    EXEC(N'DELETE FROM [SiteState]
    WHERE [Id] = ''631c1e49-c795-41ab-b0f9-7b85e22c0b6b'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241107090654_RemoveUniqueConstraintFromUserId'
)
BEGIN
    EXEC(N'DELETE FROM [AspNetRoles]
    WHERE [Id] = N''a1587a3b-9448-47d4-96a1-7ac6a0b728c6'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241107090654_RemoveUniqueConstraintFromUserId'
)
BEGIN
    EXEC(N'DELETE FROM [AspNetUsers]
    WHERE [Id] = N''da84f427-4ebe-4706-939f-49a81f68f4ca'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241107090654_RemoveUniqueConstraintFromUserId'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] ON;
    EXEC(N'INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
    VALUES (N''03dadcd8-5ef8-4275-80e5-8f028780efe8'', N''0865f657-caa4-4db4-926a-cf910c887709'', N''Prog'', N''PROG'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241107090654_RemoveUniqueConstraintFromUserId'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'Age', N'Approval', N'ConcurrencyStamp', N'CreatedDate', N'Discriminator', N'Email', N'EmailConfirmed', N'LastAccessTime', N'LockoutEnabled', N'LockoutEnd', N'ModifiedDate', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
        SET IDENTITY_INSERT [AspNetUsers] ON;
    EXEC(N'INSERT INTO [AspNetUsers] ([Id], [AccessFailedCount], [Age], [Approval], [ConcurrencyStamp], [CreatedDate], [Discriminator], [Email], [EmailConfirmed], [LastAccessTime], [LockoutEnabled], [LockoutEnd], [ModifiedDate], [NormalizedEmail], [NormalizedUserName], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [SecurityStamp], [TwoFactorEnabled], [UserName])
    VALUES (N''d37d7a70-4b78-41f9-9d1a-8bfefdad594a'', 0, 0, NULL, N''dce8ca92-f24c-417b-9a40-b42017f0df61'', ''2024-11-07T09:06:53.7854326Z'', N''ApplicationUser'', N''reema2783@gmail.com'', CAST(1 AS bit), NULL, CAST(1 AS bit), NULL, NULL, N''REEMA2783@GMAIL.COM'', N''REEMA2783@GMAIL.COM'', N''AQAAAAIAAYagAAAAEBgBEJ/q6JwmwdZJd8IaMs4erEMM9DxzEpoHfwBP4sGru8QYJe+o6iDAw/y1GXu9eQ=='', NULL, CAST(1 AS bit), N''7b1791bc-da73-4996-80a3-3661de45bdc1'', CAST(0 AS bit), N''reema2783@gmail.com'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'Age', N'Approval', N'ConcurrencyStamp', N'CreatedDate', N'Discriminator', N'Email', N'EmailConfirmed', N'LastAccessTime', N'LockoutEnabled', N'LockoutEnd', N'ModifiedDate', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
        SET IDENTITY_INSERT [AspNetUsers] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241107090654_RemoveUniqueConstraintFromUserId'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Created', N'Email', N'Facebook', N'Instagram', N'Modified', N'Phone', N'Twitter') AND [object_id] = OBJECT_ID(N'[Contact]'))
        SET IDENTITY_INSERT [Contact] ON;
    EXEC(N'INSERT INTO [Contact] ([Id], [Created], [Email], [Facebook], [Instagram], [Modified], [Phone], [Twitter])
    VALUES (''afa249d8-990b-42d2-b88d-b0e40d9e6e2c'', ''2024-11-07T11:06:53.7854119+02:00'', N''W.Wide@Gmail.com'', N''Worldwide Facebook'', N''Worldwide Instagram'', NULL, N''00218951234567'', N''Worldwide Twitter'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Created', N'Email', N'Facebook', N'Instagram', N'Modified', N'Phone', N'Twitter') AND [object_id] = OBJECT_ID(N'[Contact]'))
        SET IDENTITY_INSERT [Contact] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241107090654_RemoveUniqueConstraintFromUserId'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'About', N'Activity', N'CoverImageUrl', N'Created', N'LogoUrl', N'Modified', N'Name') AND [object_id] = OBJECT_ID(N'[SiteInfo]'))
        SET IDENTITY_INSERT [SiteInfo] ON;
    EXEC(N'INSERT INTO [SiteInfo] ([Id], [About], [Activity], [CoverImageUrl], [Created], [LogoUrl], [Modified], [Name])
    VALUES (''24e4a2c3-74c1-48ae-8647-f307dfb94f56'', N''We are a specialized news website covering political, sports, and economic events, along with various other sections of general interest to readers. We always strive to provide distinguished and reliable content that reflects the ongoing developments on both the local and global stages.'', N''News site'', NULL, ''2024-11-07T11:06:53.7853965+02:00'', NULL, NULL, N''Worldwide'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'About', N'Activity', N'CoverImageUrl', N'Created', N'LogoUrl', N'Modified', N'Name') AND [object_id] = OBJECT_ID(N'[SiteInfo]'))
        SET IDENTITY_INSERT [SiteInfo] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241107090654_RemoveUniqueConstraintFromUserId'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClosingMessage', N'Created', N'Modified', N'State') AND [object_id] = OBJECT_ID(N'[SiteState]'))
        SET IDENTITY_INSERT [SiteState] ON;
    EXEC(N'INSERT INTO [SiteState] ([Id], [ClosingMessage], [Created], [Modified], [State])
    VALUES (''659e692f-6400-4353-9b06-238c3ec36227'', N''The site is temporarily closed for development'', ''2024-11-07T11:06:53.7854142+02:00'', NULL, CAST(1 AS bit))');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClosingMessage', N'Created', N'Modified', N'State') AND [object_id] = OBJECT_ID(N'[SiteState]'))
        SET IDENTITY_INSERT [SiteState] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241107090654_RemoveUniqueConstraintFromUserId'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
        SET IDENTITY_INSERT [AspNetUserRoles] ON;
    EXEC(N'INSERT INTO [AspNetUserRoles] ([RoleId], [UserId])
    VALUES (N''03dadcd8-5ef8-4275-80e5-8f028780efe8'', N''d37d7a70-4b78-41f9-9d1a-8bfefdad594a'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
        SET IDENTITY_INSERT [AspNetUserRoles] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241107090654_RemoveUniqueConstraintFromUserId'
)
BEGIN
    DROP INDEX [IX_CompanyQuestion_UserId] ON [CompanyQuestion];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241107090654_RemoveUniqueConstraintFromUserId'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241107090654_RemoveUniqueConstraintFromUserId', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241215085849_AffterPrintUpdate'
)
BEGIN
    EXEC(N'DELETE FROM [AspNetUserRoles]
    WHERE [RoleId] = N''03dadcd8-5ef8-4275-80e5-8f028780efe8'' AND [UserId] = N''d37d7a70-4b78-41f9-9d1a-8bfefdad594a'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241215085849_AffterPrintUpdate'
)
BEGIN
    EXEC(N'DELETE FROM [Contact]
    WHERE [Id] = ''afa249d8-990b-42d2-b88d-b0e40d9e6e2c'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241215085849_AffterPrintUpdate'
)
BEGIN
    EXEC(N'DELETE FROM [SiteInfo]
    WHERE [Id] = ''24e4a2c3-74c1-48ae-8647-f307dfb94f56'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241215085849_AffterPrintUpdate'
)
BEGIN
    EXEC(N'DELETE FROM [SiteState]
    WHERE [Id] = ''659e692f-6400-4353-9b06-238c3ec36227'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241215085849_AffterPrintUpdate'
)
BEGIN
    EXEC(N'DELETE FROM [AspNetRoles]
    WHERE [Id] = N''03dadcd8-5ef8-4275-80e5-8f028780efe8'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241215085849_AffterPrintUpdate'
)
BEGIN
    EXEC(N'DELETE FROM [AspNetUsers]
    WHERE [Id] = N''d37d7a70-4b78-41f9-9d1a-8bfefdad594a'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241215085849_AffterPrintUpdate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] ON;
    EXEC(N'INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
    VALUES (N''312e4af3-5dab-4ec0-ae0b-68d658c63067'', N''85771e67-b656-4208-87e2-eb27e957880e'', N''Prog'', N''PROG'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241215085849_AffterPrintUpdate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'Age', N'Approval', N'ConcurrencyStamp', N'CreatedDate', N'Discriminator', N'Email', N'EmailConfirmed', N'LastAccessTime', N'LockoutEnabled', N'LockoutEnd', N'ModifiedDate', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
        SET IDENTITY_INSERT [AspNetUsers] ON;
    EXEC(N'INSERT INTO [AspNetUsers] ([Id], [AccessFailedCount], [Age], [Approval], [ConcurrencyStamp], [CreatedDate], [Discriminator], [Email], [EmailConfirmed], [LastAccessTime], [LockoutEnabled], [LockoutEnd], [ModifiedDate], [NormalizedEmail], [NormalizedUserName], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [SecurityStamp], [TwoFactorEnabled], [UserName])
    VALUES (N''c17b3d83-0b62-450a-b3ea-0c2ae75bfa77'', 0, 0, NULL, N''6f7801e7-b9c8-45e5-96cf-022539be9d85'', ''2024-12-15T08:58:48.6732984Z'', N''ApplicationUser'', N''libyanlacc@gmail.com'', CAST(1 AS bit), NULL, CAST(1 AS bit), NULL, NULL, N''LIBYANLACC@GMAIL.COM'', N''LIBYANLACC@GMAIL.COM'', N''AQAAAAIAAYagAAAAELDPgES1o/oa1amsQQ5EzqBRDFlq9z9mOiSAJr4l2sKf1PXGngfWgNj5utU+7S6unA=='', NULL, CAST(1 AS bit), N''659d9a3e-db84-494e-b51a-3918e183ff89'', CAST(0 AS bit), N''libyanlacc@gmail.com'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'Age', N'Approval', N'ConcurrencyStamp', N'CreatedDate', N'Discriminator', N'Email', N'EmailConfirmed', N'LastAccessTime', N'LockoutEnabled', N'LockoutEnd', N'ModifiedDate', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
        SET IDENTITY_INSERT [AspNetUsers] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241215085849_AffterPrintUpdate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Created', N'Email', N'Facebook', N'Instagram', N'Modified', N'Phone', N'Twitter') AND [object_id] = OBJECT_ID(N'[Contact]'))
        SET IDENTITY_INSERT [Contact] ON;
    EXEC(N'INSERT INTO [Contact] ([Id], [Created], [Email], [Facebook], [Instagram], [Modified], [Phone], [Twitter])
    VALUES (''bd850bc0-1f1d-404c-8b15-a24d565dccbe'', ''2024-12-15T10:58:48.6732670+02:00'', N''libyanlacc@gmail.com'', N''- Facebook'', N''- Instagram'', NULL, N''+218913832221'', N''- Twitter'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Created', N'Email', N'Facebook', N'Instagram', N'Modified', N'Phone', N'Twitter') AND [object_id] = OBJECT_ID(N'[Contact]'))
        SET IDENTITY_INSERT [Contact] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241215085849_AffterPrintUpdate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'About', N'Activity', N'CoverImageUrl', N'Created', N'LogoUrl', N'Modified', N'Name') AND [object_id] = OBJECT_ID(N'[SiteInfo]'))
        SET IDENTITY_INSERT [SiteInfo] ON;
    EXEC(N'INSERT INTO [SiteInfo] ([Id], [About], [Activity], [CoverImageUrl], [Created], [LogoUrl], [Modified], [Name])
    VALUES (''daf19eec-432a-49ee-a5f8-50ae1d2f8174'', N'''', N''LACC site'', NULL, ''2024-12-15T10:58:48.6732353+02:00'', NULL, NULL, N''LACC'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'About', N'Activity', N'CoverImageUrl', N'Created', N'LogoUrl', N'Modified', N'Name') AND [object_id] = OBJECT_ID(N'[SiteInfo]'))
        SET IDENTITY_INSERT [SiteInfo] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241215085849_AffterPrintUpdate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClosingMessage', N'Created', N'Modified', N'State') AND [object_id] = OBJECT_ID(N'[SiteState]'))
        SET IDENTITY_INSERT [SiteState] ON;
    EXEC(N'INSERT INTO [SiteState] ([Id], [ClosingMessage], [Created], [Modified], [State])
    VALUES (''7e2396d7-cf5b-43e9-9122-e51cffd5632e'', N''The site is temporarily closed for development'', ''2024-12-15T10:58:48.6732699+02:00'', NULL, CAST(1 AS bit))');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClosingMessage', N'Created', N'Modified', N'State') AND [object_id] = OBJECT_ID(N'[SiteState]'))
        SET IDENTITY_INSERT [SiteState] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241215085849_AffterPrintUpdate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
        SET IDENTITY_INSERT [AspNetUserRoles] ON;
    EXEC(N'INSERT INTO [AspNetUserRoles] ([RoleId], [UserId])
    VALUES (N''312e4af3-5dab-4ec0-ae0b-68d658c63067'', N''c17b3d83-0b62-450a-b3ea-0c2ae75bfa77'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
        SET IDENTITY_INSERT [AspNetUserRoles] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20241215085849_AffterPrintUpdate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20241215085849_AffterPrintUpdate', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415125850_AirPortTable'
)
BEGIN
    EXEC(N'DELETE FROM [AspNetUserRoles]
    WHERE [RoleId] = N''150d044e-31ab-49cc-be8c-3d66a66ce881'' AND [UserId] = N''f8fed33c-d102-4090-b53a-9e7903a427dc'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415125850_AirPortTable'
)
BEGIN
    EXEC(N'DELETE FROM [Contact]
    WHERE [Id] = ''6c02a228-2d66-4dc3-bfe8-72bb27a75f07'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415125850_AirPortTable'
)
BEGIN
    EXEC(N'DELETE FROM [SiteInfo]
    WHERE [Id] = ''40055244-415e-4d87-94b5-98fee2e6f269'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415125850_AirPortTable'
)
BEGIN
    EXEC(N'DELETE FROM [SiteState]
    WHERE [Id] = ''88107300-5b8b-4b04-a7e1-982ecc86fae7'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415125850_AirPortTable'
)
BEGIN
    EXEC(N'DELETE FROM [AspNetRoles]
    WHERE [Id] = N''150d044e-31ab-49cc-be8c-3d66a66ce881'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415125850_AirPortTable'
)
BEGIN
    EXEC(N'DELETE FROM [AspNetUsers]
    WHERE [Id] = N''f8fed33c-d102-4090-b53a-9e7903a427dc'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415125850_AirPortTable'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] ON;
    EXEC(N'INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
    VALUES (N''3ff1555d-ed4a-4482-bb85-99d2e33ab8c7'', N''111d2066-d17e-41ac-b26e-d98253f5261a'', N''Prog'', N''PROG'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415125850_AirPortTable'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'Age', N'Approval', N'ConcurrencyStamp', N'CreatedDate', N'Discriminator', N'Email', N'EmailConfirmed', N'LastAccessTime', N'LockoutEnabled', N'LockoutEnd', N'ModifiedDate', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
        SET IDENTITY_INSERT [AspNetUsers] ON;
    EXEC(N'INSERT INTO [AspNetUsers] ([Id], [AccessFailedCount], [Age], [Approval], [ConcurrencyStamp], [CreatedDate], [Discriminator], [Email], [EmailConfirmed], [LastAccessTime], [LockoutEnabled], [LockoutEnd], [ModifiedDate], [NormalizedEmail], [NormalizedUserName], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [SecurityStamp], [TwoFactorEnabled], [UserName])
    VALUES (N''f2a3bd26-bb76-487c-8689-6686f998b380'', 0, 0, NULL, N''109fa10a-fae7-4839-ac3b-2943b59990e2'', ''2025-04-15T12:58:49.1536949Z'', N''ApplicationUser'', N''libyanlacc@gmail.com'', CAST(1 AS bit), NULL, CAST(1 AS bit), NULL, NULL, N''LIBYANLACC@GMAIL.COM'', N''LIBYANLACC@GMAIL.COM'', N''AQAAAAIAAYagAAAAEG4OYAbPafYn/R0tB+DWEIWUt4wo5cR1B/9BmH6zWnbao3uEN2c4AQHVr+mAb2juuQ=='', NULL, CAST(1 AS bit), N''a9a41a3b-0e39-46bc-9aa0-aa7a34818804'', CAST(0 AS bit), N''libyanlacc@gmail.com'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'Age', N'Approval', N'ConcurrencyStamp', N'CreatedDate', N'Discriminator', N'Email', N'EmailConfirmed', N'LastAccessTime', N'LockoutEnabled', N'LockoutEnd', N'ModifiedDate', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
        SET IDENTITY_INSERT [AspNetUsers] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415125850_AirPortTable'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Created', N'Email', N'Facebook', N'Instagram', N'Modified', N'Phone', N'Twitter') AND [object_id] = OBJECT_ID(N'[Contact]'))
        SET IDENTITY_INSERT [Contact] ON;
    EXEC(N'INSERT INTO [Contact] ([Id], [Created], [Email], [Facebook], [Instagram], [Modified], [Phone], [Twitter])
    VALUES (''9e275975-e95f-43c4-b849-d599f253e7a6'', ''2025-04-15T14:58:49.1536562+02:00'', N''libyanlacc@gmail.com'', N''- Facebook'', N''- Instagram'', NULL, N''+218913832221'', N''- Twitter'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Created', N'Email', N'Facebook', N'Instagram', N'Modified', N'Phone', N'Twitter') AND [object_id] = OBJECT_ID(N'[Contact]'))
        SET IDENTITY_INSERT [Contact] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415125850_AirPortTable'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'About', N'Activity', N'CoverImageUrl', N'Created', N'LogoUrl', N'Modified', N'Name') AND [object_id] = OBJECT_ID(N'[SiteInfo]'))
        SET IDENTITY_INSERT [SiteInfo] ON;
    EXEC(N'INSERT INTO [SiteInfo] ([Id], [About], [Activity], [CoverImageUrl], [Created], [LogoUrl], [Modified], [Name])
    VALUES (''068184ef-f99d-44a7-874d-dec21ebe8e86'', N'''', N''LACC site'', NULL, ''2025-04-15T14:58:49.1536103+02:00'', NULL, NULL, N''LACC'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'About', N'Activity', N'CoverImageUrl', N'Created', N'LogoUrl', N'Modified', N'Name') AND [object_id] = OBJECT_ID(N'[SiteInfo]'))
        SET IDENTITY_INSERT [SiteInfo] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415125850_AirPortTable'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClosingMessage', N'Created', N'Modified', N'State') AND [object_id] = OBJECT_ID(N'[SiteState]'))
        SET IDENTITY_INSERT [SiteState] ON;
    EXEC(N'INSERT INTO [SiteState] ([Id], [ClosingMessage], [Created], [Modified], [State])
    VALUES (''c75ea291-fa97-4ad4-88dc-b1cfc358a419'', N''The site is temporarily closed for development'', ''2025-04-15T14:58:49.1536608+02:00'', NULL, CAST(1 AS bit))');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClosingMessage', N'Created', N'Modified', N'State') AND [object_id] = OBJECT_ID(N'[SiteState]'))
        SET IDENTITY_INSERT [SiteState] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415125850_AirPortTable'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
        SET IDENTITY_INSERT [AspNetUserRoles] ON;
    EXEC(N'INSERT INTO [AspNetUserRoles] ([RoleId], [UserId])
    VALUES (N''3ff1555d-ed4a-4482-bb85-99d2e33ab8c7'', N''f2a3bd26-bb76-487c-8689-6686f998b380'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
        SET IDENTITY_INSERT [AspNetUserRoles] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415125850_AirPortTable'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250415125850_AirPortTable', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415130953_a'
)
BEGIN
    EXEC(N'DELETE FROM [AspNetUserRoles]
    WHERE [RoleId] = N''3ff1555d-ed4a-4482-bb85-99d2e33ab8c7'' AND [UserId] = N''f2a3bd26-bb76-487c-8689-6686f998b380'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415130953_a'
)
BEGIN
    EXEC(N'DELETE FROM [Contact]
    WHERE [Id] = ''9e275975-e95f-43c4-b849-d599f253e7a6'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415130953_a'
)
BEGIN
    EXEC(N'DELETE FROM [SiteInfo]
    WHERE [Id] = ''068184ef-f99d-44a7-874d-dec21ebe8e86'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415130953_a'
)
BEGIN
    EXEC(N'DELETE FROM [SiteState]
    WHERE [Id] = ''c75ea291-fa97-4ad4-88dc-b1cfc358a419'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415130953_a'
)
BEGIN
    EXEC(N'DELETE FROM [AspNetRoles]
    WHERE [Id] = N''3ff1555d-ed4a-4482-bb85-99d2e33ab8c7'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415130953_a'
)
BEGIN
    EXEC(N'DELETE FROM [AspNetUsers]
    WHERE [Id] = N''f2a3bd26-bb76-487c-8689-6686f998b380'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415130953_a'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] ON;
    EXEC(N'INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
    VALUES (N''ddb8b485-d51b-473a-8035-7c4f1395cada'', N''ba826b88-d304-4663-9fc8-96d5e902377c'', N''Prog'', N''PROG'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415130953_a'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'Age', N'Approval', N'ConcurrencyStamp', N'CreatedDate', N'Discriminator', N'Email', N'EmailConfirmed', N'LastAccessTime', N'LockoutEnabled', N'LockoutEnd', N'ModifiedDate', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
        SET IDENTITY_INSERT [AspNetUsers] ON;
    EXEC(N'INSERT INTO [AspNetUsers] ([Id], [AccessFailedCount], [Age], [Approval], [ConcurrencyStamp], [CreatedDate], [Discriminator], [Email], [EmailConfirmed], [LastAccessTime], [LockoutEnabled], [LockoutEnd], [ModifiedDate], [NormalizedEmail], [NormalizedUserName], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [SecurityStamp], [TwoFactorEnabled], [UserName])
    VALUES (N''f00b18fd-b679-4b1b-97bb-a9c88caa627c'', 0, 0, NULL, N''e7c3b149-9eb0-466c-8577-a9b9e5bd9711'', ''2025-04-15T13:09:52.6330093Z'', N''ApplicationUser'', N''libyanlacc@gmail.com'', CAST(1 AS bit), NULL, CAST(1 AS bit), NULL, NULL, N''LIBYANLACC@GMAIL.COM'', N''LIBYANLACC@GMAIL.COM'', N''AQAAAAIAAYagAAAAEJtAYPQ1PXO8KLWowKpvspr6XV1UiVdZHwv3WYXb0JlWj4BRHZNQjPrmtJnkmUu5Cg=='', NULL, CAST(1 AS bit), N''ec30d573-9708-484f-83ca-fcbeb2292370'', CAST(0 AS bit), N''libyanlacc@gmail.com'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'Age', N'Approval', N'ConcurrencyStamp', N'CreatedDate', N'Discriminator', N'Email', N'EmailConfirmed', N'LastAccessTime', N'LockoutEnabled', N'LockoutEnd', N'ModifiedDate', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
        SET IDENTITY_INSERT [AspNetUsers] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415130953_a'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Created', N'Email', N'Facebook', N'Instagram', N'Modified', N'Phone', N'Twitter') AND [object_id] = OBJECT_ID(N'[Contact]'))
        SET IDENTITY_INSERT [Contact] ON;
    EXEC(N'INSERT INTO [Contact] ([Id], [Created], [Email], [Facebook], [Instagram], [Modified], [Phone], [Twitter])
    VALUES (''9b6b54ce-b29b-4d07-978d-2b416a0ab49f'', ''2025-04-15T15:09:52.6329619+02:00'', N''libyanlacc@gmail.com'', N''- Facebook'', N''- Instagram'', NULL, N''+218913832221'', N''- Twitter'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Created', N'Email', N'Facebook', N'Instagram', N'Modified', N'Phone', N'Twitter') AND [object_id] = OBJECT_ID(N'[Contact]'))
        SET IDENTITY_INSERT [Contact] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415130953_a'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'About', N'Activity', N'CoverImageUrl', N'Created', N'LogoUrl', N'Modified', N'Name') AND [object_id] = OBJECT_ID(N'[SiteInfo]'))
        SET IDENTITY_INSERT [SiteInfo] ON;
    EXEC(N'INSERT INTO [SiteInfo] ([Id], [About], [Activity], [CoverImageUrl], [Created], [LogoUrl], [Modified], [Name])
    VALUES (''85601cc4-ca65-4f3f-9734-a5f1f98aabf9'', N'''', N''LACC site'', NULL, ''2025-04-15T15:09:52.6329207+02:00'', NULL, NULL, N''LACC'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'About', N'Activity', N'CoverImageUrl', N'Created', N'LogoUrl', N'Modified', N'Name') AND [object_id] = OBJECT_ID(N'[SiteInfo]'))
        SET IDENTITY_INSERT [SiteInfo] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415130953_a'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClosingMessage', N'Created', N'Modified', N'State') AND [object_id] = OBJECT_ID(N'[SiteState]'))
        SET IDENTITY_INSERT [SiteState] ON;
    EXEC(N'INSERT INTO [SiteState] ([Id], [ClosingMessage], [Created], [Modified], [State])
    VALUES (''f6029bc0-19e9-4f44-84b2-09e51f099add'', N''The site is temporarily closed for development'', ''2025-04-15T15:09:52.6329705+02:00'', NULL, CAST(1 AS bit))');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClosingMessage', N'Created', N'Modified', N'State') AND [object_id] = OBJECT_ID(N'[SiteState]'))
        SET IDENTITY_INSERT [SiteState] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415130953_a'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
        SET IDENTITY_INSERT [AspNetUserRoles] ON;
    EXEC(N'INSERT INTO [AspNetUserRoles] ([RoleId], [UserId])
    VALUES (N''ddb8b485-d51b-473a-8035-7c4f1395cada'', N''f00b18fd-b679-4b1b-97bb-a9c88caa627c'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
        SET IDENTITY_INSERT [AspNetUserRoles] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415130953_a'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250415130953_a', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415132028_AirPortTable2'
)
BEGIN
    EXEC(N'DELETE FROM [AspNetUserRoles]
    WHERE [RoleId] = N''ddb8b485-d51b-473a-8035-7c4f1395cada'' AND [UserId] = N''f00b18fd-b679-4b1b-97bb-a9c88caa627c'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415132028_AirPortTable2'
)
BEGIN
    EXEC(N'DELETE FROM [Contact]
    WHERE [Id] = ''9b6b54ce-b29b-4d07-978d-2b416a0ab49f'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415132028_AirPortTable2'
)
BEGIN
    EXEC(N'DELETE FROM [SiteInfo]
    WHERE [Id] = ''85601cc4-ca65-4f3f-9734-a5f1f98aabf9'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415132028_AirPortTable2'
)
BEGIN
    EXEC(N'DELETE FROM [SiteState]
    WHERE [Id] = ''f6029bc0-19e9-4f44-84b2-09e51f099add'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415132028_AirPortTable2'
)
BEGIN
    EXEC(N'DELETE FROM [AspNetRoles]
    WHERE [Id] = N''ddb8b485-d51b-473a-8035-7c4f1395cada'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415132028_AirPortTable2'
)
BEGIN
    EXEC(N'DELETE FROM [AspNetUsers]
    WHERE [Id] = N''f00b18fd-b679-4b1b-97bb-a9c88caa627c'';
    SELECT @@ROWCOUNT');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415132028_AirPortTable2'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] ON;
    EXEC(N'INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
    VALUES (N''df5a934f-904d-4701-9252-4fca22541126'', N''22ae7e09-23d6-4580-93cf-2fbeb3b80a12'', N''Prog'', N''PROG'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415132028_AirPortTable2'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'Age', N'Approval', N'ConcurrencyStamp', N'CreatedDate', N'Discriminator', N'Email', N'EmailConfirmed', N'LastAccessTime', N'LockoutEnabled', N'LockoutEnd', N'ModifiedDate', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
        SET IDENTITY_INSERT [AspNetUsers] ON;
    EXEC(N'INSERT INTO [AspNetUsers] ([Id], [AccessFailedCount], [Age], [Approval], [ConcurrencyStamp], [CreatedDate], [Discriminator], [Email], [EmailConfirmed], [LastAccessTime], [LockoutEnabled], [LockoutEnd], [ModifiedDate], [NormalizedEmail], [NormalizedUserName], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [SecurityStamp], [TwoFactorEnabled], [UserName])
    VALUES (N''4191e943-f3cb-4cdc-ae1b-ea15e249ddca'', 0, 0, NULL, N''b293b53b-23ed-47a3-bab2-585022075404'', ''2025-04-15T13:20:27.7735705Z'', N''ApplicationUser'', N''libyanlacc@gmail.com'', CAST(1 AS bit), NULL, CAST(1 AS bit), NULL, NULL, N''LIBYANLACC@GMAIL.COM'', N''LIBYANLACC@GMAIL.COM'', N''AQAAAAIAAYagAAAAEM5kOKsJBS23GolOmp/KLGBjN3U5O4MZhheEeiNYSAvbkXTyWhj641PCB70D+ubFag=='', NULL, CAST(1 AS bit), N''cb6b24c5-d172-4c91-9bc7-190295a5013b'', CAST(0 AS bit), N''libyanlacc@gmail.com'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'Age', N'Approval', N'ConcurrencyStamp', N'CreatedDate', N'Discriminator', N'Email', N'EmailConfirmed', N'LastAccessTime', N'LockoutEnabled', N'LockoutEnd', N'ModifiedDate', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
        SET IDENTITY_INSERT [AspNetUsers] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415132028_AirPortTable2'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Created', N'Email', N'Facebook', N'Instagram', N'Modified', N'Phone', N'Twitter') AND [object_id] = OBJECT_ID(N'[Contact]'))
        SET IDENTITY_INSERT [Contact] ON;
    EXEC(N'INSERT INTO [Contact] ([Id], [Created], [Email], [Facebook], [Instagram], [Modified], [Phone], [Twitter])
    VALUES (''dff6d12b-46c4-4efe-8157-1b41f2ab1e51'', ''2025-04-15T15:20:27.7735428+02:00'', N''libyanlacc@gmail.com'', N''- Facebook'', N''- Instagram'', NULL, N''+218913832221'', N''- Twitter'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Created', N'Email', N'Facebook', N'Instagram', N'Modified', N'Phone', N'Twitter') AND [object_id] = OBJECT_ID(N'[Contact]'))
        SET IDENTITY_INSERT [Contact] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415132028_AirPortTable2'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'About', N'Activity', N'CoverImageUrl', N'Created', N'LogoUrl', N'Modified', N'Name') AND [object_id] = OBJECT_ID(N'[SiteInfo]'))
        SET IDENTITY_INSERT [SiteInfo] ON;
    EXEC(N'INSERT INTO [SiteInfo] ([Id], [About], [Activity], [CoverImageUrl], [Created], [LogoUrl], [Modified], [Name])
    VALUES (''ed7df3b2-0e46-423c-b050-b1de665a94b2'', N'''', N''LACC site'', NULL, ''2025-04-15T15:20:27.7735200+02:00'', NULL, NULL, N''LACC'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'About', N'Activity', N'CoverImageUrl', N'Created', N'LogoUrl', N'Modified', N'Name') AND [object_id] = OBJECT_ID(N'[SiteInfo]'))
        SET IDENTITY_INSERT [SiteInfo] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415132028_AirPortTable2'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClosingMessage', N'Created', N'Modified', N'State') AND [object_id] = OBJECT_ID(N'[SiteState]'))
        SET IDENTITY_INSERT [SiteState] ON;
    EXEC(N'INSERT INTO [SiteState] ([Id], [ClosingMessage], [Created], [Modified], [State])
    VALUES (''6655830f-6c8b-4cff-8a80-59f81d46a578'', N''The site is temporarily closed for development'', ''2025-04-15T15:20:27.7735462+02:00'', NULL, CAST(1 AS bit))');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClosingMessage', N'Created', N'Modified', N'State') AND [object_id] = OBJECT_ID(N'[SiteState]'))
        SET IDENTITY_INSERT [SiteState] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415132028_AirPortTable2'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
        SET IDENTITY_INSERT [AspNetUserRoles] ON;
    EXEC(N'INSERT INTO [AspNetUserRoles] ([RoleId], [UserId])
    VALUES (N''df5a934f-904d-4701-9252-4fca22541126'', N''4191e943-f3cb-4cdc-ae1b-ea15e249ddca'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
        SET IDENTITY_INSERT [AspNetUserRoles] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415132028_AirPortTable2'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250415132028_AirPortTable2', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [Discriminator] nvarchar(21) NOT NULL,
        [Age] int NULL,
        [LastAccessTime] datetime2 NULL,
        [CreatedDate] datetime2 NULL,
        [ModifiedDate] datetime2 NULL,
        [Approval] bit NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [Company] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_Company] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [Contact] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [Email] nvarchar(max) NOT NULL,
        [Phone] nvarchar(max) NOT NULL,
        [Facebook] nvarchar(max) NOT NULL,
        [Twitter] nvarchar(max) NOT NULL,
        [Instagram] nvarchar(max) NOT NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_Contact] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [QuestionCategoryType] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [CategoryName] nvarchar(max) NOT NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_QuestionCategoryType] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [SiteInfo] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [Name] nvarchar(max) NOT NULL,
        [Activity] nvarchar(200) NOT NULL,
        [About] nvarchar(2000) NOT NULL,
        [LogoUrl] nvarchar(max) NULL,
        [CoverImageUrl] nvarchar(max) NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_SiteInfo] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [SiteState] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [State] bit NOT NULL,
        [ClosingMessage] nvarchar(max) NOT NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_SiteState] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [AirPortRequests] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [Department] nvarchar(max) NOT NULL,
        [EntryTime] datetime2 NOT NULL,
        [RequestTime] datetime2 NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [SenderName] nvarchar(max) NOT NULL,
        [CompanyName] nvarchar(max) NOT NULL,
        [FlightDate] datetime2 NOT NULL,
        [AircraftType] nvarchar(max) NOT NULL,
        [AircraftRegistration] nvarchar(max) NOT NULL,
        [CallSign] nvarchar(max) NOT NULL,
        [FlightPath] nvarchar(max) NOT NULL,
        [LandingTakeoffTime] nvarchar(max) NOT NULL,
        [FlightPurpose] nvarchar(max) NOT NULL,
        [EntryExitPoints] nvarchar(max) NOT NULL,
        [Notes] nvarchar(max) NOT NULL,
        [RequestStatus] nvarchar(max) NOT NULL,
        [ApproverUserId] nvarchar(450) NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_AirPortRequests] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AirPortRequests_AspNetUsers_ApproverUserId] FOREIGN KEY ([ApproverUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [Employees] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [Name] nvarchar(100) NOT NULL,
        [PhoneNumber] nvarchar(max) NOT NULL,
        [Address] nvarchar(200) NOT NULL,
        [YearsOfExperience] int NOT NULL,
        [Specialization] nvarchar(100) NOT NULL,
        [Bio] nvarchar(500) NOT NULL,
        [UserId] nvarchar(450) NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Employees_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [UserProfiles] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [DisplayName] nvarchar(max) NOT NULL,
        [ImageUrl] nvarchar(max) NULL,
        [Gender] bit NOT NULL,
        [UserId] nvarchar(450) NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_UserProfiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserProfiles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [CompanyQuestion] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [CompanyId] uniqueidentifier NOT NULL,
        [UserId] nvarchar(450) NULL,
        [Active] bit NOT NULL,
        [SaftyGrid] int NOT NULL,
        [SqurtyGrid] int NOT NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_CompanyQuestion] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CompanyQuestion_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]),
        CONSTRAINT [FK_CompanyQuestion_Company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Company] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [QuestionType] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [TypeName] nvarchar(max) NOT NULL,
        [QuestionCategoryTypeId] uniqueidentifier NOT NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_QuestionType] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuestionType_QuestionCategoryType_QuestionCategoryTypeId] FOREIGN KEY ([QuestionCategoryTypeId]) REFERENCES [QuestionCategoryType] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [AirPortRequestFiles] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [FileName] nvarchar(max) NOT NULL,
        [FilePath] nvarchar(max) NOT NULL,
        [AirPortRequestId] uniqueidentifier NOT NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_AirPortRequestFiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AirPortRequestFiles_AirPortRequests_AirPortRequestId] FOREIGN KEY ([AirPortRequestId]) REFERENCES [AirPortRequests] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [Question] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [Content] nvarchar(max) NOT NULL,
        [QuestionTypeId] uniqueidentifier NOT NULL,
        [MaxGrid] int NOT NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_Question] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Question_QuestionType_QuestionTypeId] FOREIGN KEY ([QuestionTypeId]) REFERENCES [QuestionType] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE TABLE [CompanyQuestionContent] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWID()),
        [CompanyQuestionId] uniqueidentifier NOT NULL,
        [QuestionId] uniqueidentifier NOT NULL,
        [Score] int NOT NULL,
        [Created] datetime2 NOT NULL,
        [Modified] datetime2 NULL,
        CONSTRAINT [PK_CompanyQuestionContent] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CompanyQuestionContent_CompanyQuestion_CompanyQuestionId] FOREIGN KEY ([CompanyQuestionId]) REFERENCES [CompanyQuestion] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CompanyQuestionContent_Question_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Question] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] ON;
    EXEC(N'INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
    VALUES (N''d5f5e8e4-c6b4-4c80-bc14-dab4504cc44a'', N''46788256-1556-4536-8ac1-8fa5095e3fe4'', N''Prog'', N''PROG'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
        SET IDENTITY_INSERT [AspNetRoles] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'Age', N'Approval', N'ConcurrencyStamp', N'CreatedDate', N'Discriminator', N'Email', N'EmailConfirmed', N'LastAccessTime', N'LockoutEnabled', N'LockoutEnd', N'ModifiedDate', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
        SET IDENTITY_INSERT [AspNetUsers] ON;
    EXEC(N'INSERT INTO [AspNetUsers] ([Id], [AccessFailedCount], [Age], [Approval], [ConcurrencyStamp], [CreatedDate], [Discriminator], [Email], [EmailConfirmed], [LastAccessTime], [LockoutEnabled], [LockoutEnd], [ModifiedDate], [NormalizedEmail], [NormalizedUserName], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [SecurityStamp], [TwoFactorEnabled], [UserName])
    VALUES (N''28dca3d9-c93a-494f-90a1-20bf9b998ec3'', 0, 0, NULL, N''eb4047af-1465-4301-b521-8c3d86fdd3a7'', ''2025-04-15T14:11:52.1864839Z'', N''ApplicationUser'', N''libyanlacc@gmail.com'', CAST(1 AS bit), NULL, CAST(1 AS bit), NULL, NULL, N''LIBYANLACC@GMAIL.COM'', N''LIBYANLACC@GMAIL.COM'', N''AQAAAAIAAYagAAAAEK8mWe5O5A6bFw/a2YLg6gAFo4taXqQBKZyuQzh8ceJTGotUZ8bmUnOzexaT5Pku8Q=='', NULL, CAST(1 AS bit), N''2fb547da-7e62-49ed-beaa-855f5c6a1415'', CAST(0 AS bit), N''libyanlacc@gmail.com'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'Age', N'Approval', N'ConcurrencyStamp', N'CreatedDate', N'Discriminator', N'Email', N'EmailConfirmed', N'LastAccessTime', N'LockoutEnabled', N'LockoutEnd', N'ModifiedDate', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
        SET IDENTITY_INSERT [AspNetUsers] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Created', N'Email', N'Facebook', N'Instagram', N'Modified', N'Phone', N'Twitter') AND [object_id] = OBJECT_ID(N'[Contact]'))
        SET IDENTITY_INSERT [Contact] ON;
    EXEC(N'INSERT INTO [Contact] ([Id], [Created], [Email], [Facebook], [Instagram], [Modified], [Phone], [Twitter])
    VALUES (''772c7ee9-50ae-43ab-988a-f1fde0f7164d'', ''2025-04-15T16:11:52.1864404+02:00'', N''libyanlacc@gmail.com'', N''- Facebook'', N''- Instagram'', NULL, N''+218913832221'', N''- Twitter'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Created', N'Email', N'Facebook', N'Instagram', N'Modified', N'Phone', N'Twitter') AND [object_id] = OBJECT_ID(N'[Contact]'))
        SET IDENTITY_INSERT [Contact] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'About', N'Activity', N'CoverImageUrl', N'Created', N'LogoUrl', N'Modified', N'Name') AND [object_id] = OBJECT_ID(N'[SiteInfo]'))
        SET IDENTITY_INSERT [SiteInfo] ON;
    EXEC(N'INSERT INTO [SiteInfo] ([Id], [About], [Activity], [CoverImageUrl], [Created], [LogoUrl], [Modified], [Name])
    VALUES (''25d7e37b-67fe-4e55-ae1e-c347b32ce9cc'', N'''', N''LACC site'', NULL, ''2025-04-15T16:11:52.1863972+02:00'', NULL, NULL, N''LACC'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'About', N'Activity', N'CoverImageUrl', N'Created', N'LogoUrl', N'Modified', N'Name') AND [object_id] = OBJECT_ID(N'[SiteInfo]'))
        SET IDENTITY_INSERT [SiteInfo] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClosingMessage', N'Created', N'Modified', N'State') AND [object_id] = OBJECT_ID(N'[SiteState]'))
        SET IDENTITY_INSERT [SiteState] ON;
    EXEC(N'INSERT INTO [SiteState] ([Id], [ClosingMessage], [Created], [Modified], [State])
    VALUES (''d2c3d775-c34f-4fc0-b955-764542ecf5c8'', N''The site is temporarily closed for development'', ''2025-04-15T16:11:52.1864466+02:00'', NULL, CAST(1 AS bit))');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClosingMessage', N'Created', N'Modified', N'State') AND [object_id] = OBJECT_ID(N'[SiteState]'))
        SET IDENTITY_INSERT [SiteState] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
        SET IDENTITY_INSERT [AspNetUserRoles] ON;
    EXEC(N'INSERT INTO [AspNetUserRoles] ([RoleId], [UserId])
    VALUES (N''d5f5e8e4-c6b4-4c80-bc14-dab4504cc44a'', N''28dca3d9-c93a-494f-90a1-20bf9b998ec3'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserRoles]'))
        SET IDENTITY_INSERT [AspNetUserRoles] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE INDEX [IX_AirPortRequestFiles_AirPortRequestId] ON [AirPortRequestFiles] ([AirPortRequestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE INDEX [IX_AirPortRequests_ApproverUserId] ON [AirPortRequests] ([ApproverUserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE INDEX [IX_CompanyQuestion_CompanyId] ON [CompanyQuestion] ([CompanyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_CompanyQuestion_UserId] ON [CompanyQuestion] ([UserId]) WHERE [UserId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE INDEX [IX_CompanyQuestionContent_CompanyQuestionId] ON [CompanyQuestionContent] ([CompanyQuestionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE INDEX [IX_CompanyQuestionContent_QuestionId] ON [CompanyQuestionContent] ([QuestionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Employees_UserId] ON [Employees] ([UserId]) WHERE [UserId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE INDEX [IX_Question_QuestionTypeId] ON [Question] ([QuestionTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    CREATE INDEX [IX_QuestionType_QuestionCategoryTypeId] ON [QuestionType] ([QuestionCategoryTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_UserProfiles_UserId] ON [UserProfiles] ([UserId]) WHERE [UserId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250415141153_mssql.onprem_migration_476'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250415141153_mssql.onprem_migration_476', N'8.0.10');
END;
GO

COMMIT;
GO


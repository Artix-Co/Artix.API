# Graph Report - Artix.API  (2026-10-23)

## Corpus Check
- 547 files · ~95,007 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 31 file(s) not represented in the graph (top: (none) 22, .hiv 3, .props 2)

## Summary
- 3817 nodes · 8198 edges · 231 communities (200 shown, 31 thin omitted)
- Extraction: 97% EXTRACTED · 3% INFERRED · 0% AMBIGUOUS · INFERRED: 271 edges (avg confidence: 0.86)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `7e44f095`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- primitives
- ArtixCommandDbContext
- CancellationToken
- MigrationBuilder
- Guid
- VoiceTrack
- microsoft_extensions_logging
- UserSession
- ArtixCommandDbContext
- EntityTypeBuilder
- AppUser
- common
- AnomalyDetectionService
- microsoft_entityframeworkcore
- Artix.API.Core.Domain.Entities.Museum
- Notification
- Artix.API.Core.Contract.Primitives.Infra.Redis
- exceptions
- AuthorizeCheckOperationFilter.cs
- AppUser
- primitives_handlers
- CancellationToken
- Object
- MongoCommandContext
- microsoft_aspnetcore_http
- .AddRabbitMqService
- Artix.API.Core.Domain.Entities.Notification.Enums
- Museum
- MongoTimestampInterceptor
- .Handle
- ObjectBuilder
- AppUserBuilder
- IDomainEvent
- ClientPaginateObjectsDto
- ContinuousAuthorizationMiddleware
- CreateNewObjectCommandHandler.cs
- IEntityTypeConfiguration
- .Handle
- IActionResult
- Artix.API.Infra.Sql/DependencyInjection.cs
- XpRulesServiceTests
- .AddSqlServices
- NotificationMessage
- .AddRedis
- IEnumerable
- FileEntity
- Quiz
- ClientPaginateLoginHistoryDto
- .Handle
- DateTime
- Object
- مدل‌های اصلی
- OtpService.cs
- QueryHandlerBase
- ILogger
- core_domain_entities_common
- .Handle
- CommandHandlerBase
- .Handle
- ApiVersionCheckMiddleware
- IMuseumQueryRepository
- HostingExtension.cs
- Artix API — Security Vulnerability Backlog
- UserQuiz
- FileEntity
- Result
- UserXp
- HistoricalPeriod
- UserScan
- .AddArtixServices
- BruteForceProtectionService
- HttpPost
- GetVerifyOTPAuthQuery.cs
- AppVersion
- SeasonTask
- UploadSession
- IntegrationError
- MongoUnitOfWork
- Artix.API.Core.DomainService.csproj
- QuizQueryRepository.cs
- UserMuseumKey
- http
- ArtixQueryDbContext
- PaginatedResult
- IServiceCollection
- .AddElasticsearch
- NotificationHandlerBase
- Artix.API.WebService.csproj
- TierConfig
- QueryRepository
- GetAllTiersQuery
- system
- UserNotification
- UserSeasonProgress
- .AddIdentityService
- MongoDataSeeder
- Fact
- List
- ClientObjectDetailsByIdDto
- FileSettings
- .AddDomainServiceServices
- primitives_repositories
- .Handle
- Program.cs
- Artix.API.Core.Contract.csproj
- JournalEntry
- MuseumImage
- MuseumObject
- ObjectModel
- ObjectImage
- MarketplaceItem
- Artix.API.sln
- UserSessionService
- Artix.API.Utils
- ClientType
- IQuery
- AdminAuditEntry
- AuthenticationSettings
- TokenService
- .FindAsync
- Artix.API.Infra.Sql.csproj
- Artix.API.Tests.Integration.csproj
- FileWatcherService
- RateLimitAttribute
- mediatr
- Repository
- Artix.API.Core.ApplicationService/DependencyInjection.cs
- QuizCompletedEvent
- ClientPaginateLoginHistoryDto
- AnomalyDetectionResult
- OutboxMessage
- .GetProfileAsync
- UserStrike
- XpRulesServiceTests.cs
- ApplicationServiceNotFoundException
- BruteForceCheckResult
- IVersionQueryRepository.cs
- Friendship
- Client/Queries/GetLogout/GetLogoutQueryHandler.cs
- ElasticsearchSettings
- .RemoveMuseumAsync
- GeoFencingMiddleware
- AnomalyScore
- GetUserRecentMuseumsVisitsQueryHandler
- High
- TierConfig
- QuizStatus
- RabbitMqOptions
- Medium
- .Handle
- OutboxMessageStatus
- AppRole
- IMuseumQueryRepository.cs
- VoiceTrack
- JwtTokenResult
- ErrorResponse
- Artix.API.Core.Domain.csproj
- .IsMatch
- UserImage
- .Handle
- VerifyOTPResult
- RedisOptions
- QuizAction
- UserProfileDto
- JournalEntryReadConfiguration.cs
- Artix.API.Endpoints/DependencyInjection.cs
- Artix.API.Tests.EndToEnd.csproj
- IOtpService.cs
- ArtixCommandDbContext.cs
- GetReNewAccessTokenQuery.cs
- Artix API — Agent Guide
- PurposeType
- Artix.API.Infra.Mongo.csproj
- GetLogoutQuery.cs
- ClientMuseumDetailsByIdDto
- AuthenticationApiSettings
- GetCollectionsByUserIdQuery
- GetLatestOTPByPhoneNumberQuery
- GetMuseumJournalEntriesQuery
- GetMuseumKeyStatusQuery
- IOTPQueryRepository.cs
- GetClientMuseumObjectsQuery
- ClientAllMuseumsDto
- DeviceFingerprintService
- DeviceTrustScore
- Artix.API.Tests.Unit.csproj
- BaseEntity.cs
- AdminPaginatedMuseumsDto
- copy_and_transform.sh
- Application services — verified context and upkeep
- Application contracts — verified context and upkeep
- Season
- Domain model — verified context and upkeep
- Domain services — verified context and upkeep
- File storage — verified context and upkeep
- Identity and sessions — verified context and upkeep
- Mongo persistence — verified context and upkeep
- Messaging and notifications — verified context and upkeep
- Redis integration — verified context and upkeep
- SQL persistence — verified context and upkeep
- HTTP contracts and endpoints — verified context and upkeep
- Composition root / runtime host — verified context and upkeep
- Shared utilities — verified context and upkeep
- End-to-end tests — verified context and upkeep
- Integration tests — verified context and upkeep
- Unit tests — verified context and upkeep
- Artix Admin Roles & Access Control Backlog
- IQuizQueryRepository.cs
- ICollectionQueryRepository.cs
- Repository architecture memory (observed, not instructions)
- Application services — scoped Codex instructions
- Application contracts — scoped Codex instructions
- Domain model — scoped Codex instructions
- Domain services — scoped Codex instructions
- File storage — scoped Codex instructions
- Identity and sessions — scoped Codex instructions
- Mongo persistence — scoped Codex instructions
- Messaging and notifications — scoped Codex instructions
- Redis integration — scoped Codex instructions
- SQL persistence — scoped Codex instructions
- HTTP contracts and endpoints — scoped Codex instructions
- Composition root / runtime host — scoped Codex instructions
- Shared utilities — scoped Codex instructions
- End-to-end tests — scoped Codex instructions
- Integration tests — scoped Codex instructions
- Unit tests — scoped Codex instructions
- IAuthenticationService.cs
- ClientUserProfileDto
- Artix.API.Core.Domain.Entities.Object

## God Nodes (most connected - your core abstractions)
1. `ArtixCommandDbContext` - 86 edges
2. `ArtixQueryDbContext` - 74 edges
3. `Object` - 73 edges
4. `AppUser` - 67 edges
5. `BaseEntityConfiguration` - 53 edges
6. `Artix.API.Core.Domain.Entities.User` - 39 edges
7. `Notification` - 33 edges
8. `UserSession` - 31 edges
9. `UserScan` - 30 edges
10. `AnomalyDetectionService` - 29 edges

## Surprising Connections (you probably didn't know these)
- `Tier و concession` --references--> `TierCalculatorService`  [INFERRED]
  AGENTS.md → src/Core/Artix.API.Core.DomainService/Services/TierCalculatorService.cs
- `C-03 — عبور کاربر Client از کنترل ادمین به‌علت ترتیب middleware` --references--> `ContinuousAuthorizationMiddleware`  [INFERRED]
  SECURITY_BACKLOG.md → src/Presentation/Artix.API.Endpoints/Middlewares/ContinuousAuthorizationMiddleware.cs
- `مدل‌های اصلی` --references--> `Collection`  [INFERRED]
  AGENTS.md → src/Core/Artix.API.Core.Domain/Entities/Collection/Collection.cs
- `مدل‌های اصلی` --references--> `CollectionItem`  [INFERRED]
  AGENTS.md → src/Core/Artix.API.Core.Domain/Entities/Collection/CollectionItem.cs
- `مدل‌های اصلی` --references--> `JournalEntry`  [INFERRED]
  AGENTS.md → src/Core/Artix.API.Core.Domain/Entities/JournalEntry/JournalEntry.cs

## Import Cycles
- None detected.

## Communities (231 total, 31 thin omitted)

### Community 0 - "primitives"
Cohesion: 0.04
Nodes (29): Artix.API.Core.ApplicationService.Features.Museums.Client.Queries.GetObjects, Artix.API.Core.ApplicationService.Features.Users.Client.Queries.GetUserProfile, Artix.API.Core.ApplicationService.Features.Users.Admin.Queries.GetUserProfile, Artix.API.Core.Contract.Primitives.DomainServices.TierCalculator, Artix.API.Core.ApplicationService.Features.Objects.Client.Commands.AddToUserCollection, Artix.API.Core.ApplicationService.Features.Museums.Admin.Queries.GetObjects, Artix.API.Core.ApplicationService.Features.Objects.Client.Queries.GetPaginateObjects, Artix.API.Core.ApplicationService.Features.Users.Admin.Queries.GetLogin (+21 more)

### Community 1 - "ArtixCommandDbContext"
Cohesion: 0.05
Nodes (37): ArtixCommandDbContext, AdminAuditEntries, AppRoles, AppUsers, AppVersions, CollectionItems, Collections, Files (+29 more)

### Community 3 - "MigrationBuilder"
Cohesion: 0.09
Nodes (9): Init, OutboxMessageStatusToEnumType, AddFileForObjectInformations, ModifyObjectTable, ModifyQrCodeFieldInObjectTable, FixUserTables, update_user_session, AddAdminAuditEntry (+1 more)

### Community 4 - "Guid"
Cohesion: 0.06
Nodes (7): IFileStorage, IUploadRepository, IUploadService, ICommandRepository, FileSystemStorage, InMemoryUploadRepository, UploadService

### Community 5 - "VoiceTrack"
Cohesion: 0.14
Nodes (13): VoiceTrack, Artist, IsFree, Object, ObjectId, Season, SeasonId, Title (+5 more)

### Community 6 - "microsoft_extensions_logging"
Cohesion: 0.06
Nodes (13): Artix.API.Infra.RabbitMQ.Services, Artix.API.Infra.RabbitMQ, Artix.API.Infra.FileService.Services, Artix.API.Infra.FileService, Artix.API.Infra.RabbitMQ.Services.Notification, Artix.API.Infra.Redis.Services, Artix.API.Infra.Sql.Repositories.Features.Files, Artix.API.Infra.Redis (+5 more)

### Community 7 - "UserSession"
Cohesion: 0.10
Nodes (19): UserSession, DeviceId, Fingerprint, IpAddress, IsActive, JwtId, LastRefreshAt, LastSeenAt (+11 more)

### Community 8 - "ArtixCommandDbContext"
Cohesion: 0.09
Nodes (7): ArtixCommandDbContextFactory, DomainEventInterceptor, IChangeInterceptor, TimestampInterceptor, CommandRepository, UnitOfWork, AdminAuditWriter

### Community 9 - "EntityTypeBuilder"
Cohesion: 0.09
Nodes (21): Category, Description, Name, ObjectTypes, BaseEntityConfiguration, CollectionReadConfiguration, FileReadConfiguration, CategoryReadConfiguration (+13 more)

### Community 10 - "AppUser"
Cohesion: 0.07
Nodes (25): AppUser, BanExpiration, BanReason, BusinessId, Collections, CreatedAt, DisplayName, FirstName (+17 more)

### Community 11 - "common"
Cohesion: 0.06
Nodes (5): Artix.API.Core.Domain.Entities.TierConfig, Artix.API.Core.Domain.Entities.Season, Artix.API.Core.Domain.Entities.Collection, Artix.API.Core.Domain.Entities.Quiz, MuseumReadConfiguration

### Community 12 - "AnomalyDetectionService"
Cohesion: 0.07
Nodes (9): Artix.API.Core.Contract.Primitives.Infra.Audit, IAdminAuditWriter, IAnomalyDetectionService, AnomalyDetectionService, LocationData, City, Country, Latitude (+1 more)

### Community 13 - "microsoft_entityframeworkcore"
Cohesion: 0.10
Nodes (12): Artix.API.Infra.Sql.Data.Config.Write.Season, Artix.API.Core.Domain.Entities.User, Artix.API.Infra.Sql.Data.Config.Read.User, Artix.API.Infra.Sql.Data.Config.Write, Artix.API.Infra.Sql.Data.Config.Write.File, Artix.API.Infra.Sql.Data.Config.Write.Collection, Artix.API.Infra.Sql.Data.Config.Read.Collection, Artix.API.Infra.Sql.Data.Config.Read.Season (+4 more)

### Community 14 - "Artix.API.Core.Domain.Entities.Museum"
Cohesion: 0.18
Nodes (3): Artix.API.Infra.Sql.Data.Config.Write.Museum, Artix.API.Core.Domain.Entities.Museum, Artix.API.Infra.Sql.Data.Config.Read.Museum

### Community 15 - "Notification"
Cohesion: 0.10
Nodes (17): Notification, Body, CreatedAt, ExpirationDate, FailedAttempts, IsBroadcast, LastErrorMessage, Metadata (+9 more)

### Community 16 - "Artix.API.Core.Contract.Primitives.Infra.Redis"
Cohesion: 0.07
Nodes (12): Artix.API.Core.ApplicationService.Features.Objects.Client.Queries.GetObjectDetailsById, Artix.API.Core.ApplicationService.Features.Museums.Client.Queries.GetDetailsById, Artix.API.Core.ApplicationService.Features.Objects.Client.Queries.GetUserRecentObjectsVisits, Artix.API.Core.Contract.Primitives.Infra.Redis, Artix.API.Core.Contract.Features.Caches.Objects, Artix.API.Core.ApplicationService.Primitives, Artix.API.Core.ApplicationService.Features.Museums.Client.Queries.GetUserRecentMuseumsVisits, IEventDeduplicationStore (+4 more)

### Community 17 - "exceptions"
Cohesion: 0.05
Nodes (11): Artix.API.Infra.Sql.Data.Seed, Artix.API.Infra.Sql.Repositories.Features.Tiers, Artix.API.Infra.Sql.Repositories.Features.Versions, Artix.API.Infra.Sql.Repositories.Features.Objects, Artix.API.Infra.Sql.Repositories.Features.OTPs, Artix.API.Infra.Sql.Repositories.Features.Notifications, Artix.API.Infra.Sql.Services, Artix.API.Infra.Sql.Repositories.Features.Museums (+3 more)

### Community 19 - "AppUser"
Cohesion: 0.05
Nodes (8): RegisterCommandHandler, CommandHandlerBase, QueryHandlerBase, IAuthenticationService, ISessionStore, OtpService, AuthenticationService, JwtTokenGenerator

### Community 20 - "primitives_handlers"
Cohesion: 0.07
Nodes (22): Artix.API.Core.Contract.Features.Objects.Client.Commands.AddToUserCollection, Artix.API.Core.Contract.Features.Objects.Admin.Commands.Remove, Artix.API.Core.Contract.Features.Users.Client.Commands.DeleteProfile, Artix.API.Core.Contract.Features.Notifications.Commands.AddUserNotification, Artix.API.Core.Contract.Features.Objects.Admin.Commands.Upgrade, Artix.API.Core.Contract.Features.Users.Client.Commands.InitiateOTPAuth, Artix.API.Core.Contract.Features.Users.Admin.Commands.Register, Artix.API.Core.Contract.Features.Objects.Client.Commands.Scan (+14 more)

### Community 21 - "CancellationToken"
Cohesion: 0.06
Nodes (4): IBruteForceProtectionService, IMessageRelayService, IRepository, RedisRateLimiter

### Community 22 - "Object"
Cohesion: 0.05
Nodes (20): Object, Description, IsHidden, IsSpecial, JournalEntries, MarketplaceItems, MuseumObjects, Name (+12 more)

### Community 23 - "MongoCommandContext"
Cohesion: 0.13
Nodes (8): BaseEntity, BusinessId, CreatedAt, Id, IsDeleted, ModifiedAt, MongoCommandContext, MongoCommandRepository

### Community 24 - "microsoft_aspnetcore_http"
Cohesion: 0.07
Nodes (8): Artix.API.Infra.Identity.Services.TokenService, Artix.API.Endpoints.Middlewares, Artix.API.Core.ApplicationService.Features.Users.Admin.Commands.Register, Artix.API.Endpoints.Attributes, Artix.API.Tests.Unit.Authorization, Artix.API.Infra.Identity, Artix.API.Infra.Identity.Services, Artix.API.Infra.Identity.Services.TokenProvider

### Community 25 - ".AddRabbitMqService"
Cohesion: 0.06
Nodes (10): Artix.API.Core.Contract.Primitives.Infra.File, ICompressionWorker, IDistributedLockService, ILockScope, Acquired, NotificationProducer, NotificationService, RabbitMqEventPublisher (+2 more)

### Community 26 - "Artix.API.Core.Domain.Entities.Notification.Enums"
Cohesion: 0.08
Nodes (22): Artix.API.Core.Domain.Entities.Notification, Artix.API.Infra.Sql.Data.Config.Read.Notification, Artix.API.Infra.Sql.Data.Config.Write.Notification, Artix.API.Core.Domain.Entities.Notification.Enums, DeliveryStatus, Delivered, Failed, Pending (+14 more)

### Community 27 - "Museum"
Cohesion: 0.08
Nodes (14): OtpSessionData, Museum, Description, IsActive, MuseumImages, MuseumObjects, Name, Slug (+6 more)

### Community 28 - "MongoTimestampInterceptor"
Cohesion: 0.21
Nodes (6): Artix.API.Infra.Mongo.Data.Interceptors, Counter, Id, SequenceValue, IMongoInterceptor, MongoTimestampInterceptor

### Community 29 - ".Handle"
Cohesion: 0.12
Nodes (7): Artix.API.Core.ApplicationService.CacheKeys, CacheKeys, GetMuseumDetailsByIdQueryHandler, GetObjectDetailsByIdQueryHandler, GetUserRecentObjectsVisitsQueryHandler, ICacheRepository, ObjectQueryRepository

### Community 30 - "ObjectBuilder"
Cohesion: 0.13
Nodes (8): ObjectBuilder, ObjectHistoricalPeriod, HistoricalPeriod, HistoricalPeriodId, Object, ObjectId, ObjectHistoricalPeriodReadConfiguration, ObjectHistoricalPeriodWriteConfiguration

### Community 31 - "AppUserBuilder"
Cohesion: 0.23
Nodes (4): Artix.API.Core.Contract.Features.Users.Client.Commands.ModifyProfile, ModifyProfileHandler, ClientModifyProfileCommand, AppUserBuilder

### Community 32 - "IDomainEvent"
Cohesion: 0.10
Nodes (11): Artix.API.Core.Domain.Entities.Museum.Events, Artix.API.Core.Domain.Entities.Object.Events, AggregateRoot, DomainEvents, MuseumCreatedEvent, FirstUserScanEvent, RepeatUserScanEvent, QuizCreatedEvent (+3 more)

### Community 33 - "ClientPaginateObjectsDto"
Cohesion: 0.16
Nodes (11): Artix.API.Core.Domain.Entities.Object.ValueObjects, GetObjectDetailsByIdQueryHandler, GetPaginateObjectsQueryHandler, ClientPaginateObjectsDto, HistoricalPeriodDto, TypeDto, GetClientPaginateObjectsQuery, HistoricalDate (+3 more)

### Community 35 - "CreateNewObjectCommandHandler.cs"
Cohesion: 0.12
Nodes (6): Artix.API.Core.ApplicationService.Features.Museums.Admin.Commands.CreateNewMuseum, Artix.API.Core.ApplicationService.Features.Objects.Admin.Commands.Upgrade, Artix.API.Core.ApplicationService.Features.Objects.Admin.Commands.CreateNewObject, Artix.API.Core.Contract.Features.Files.Commands, Artix.API.Core.ApplicationService.Features.Users.Client.Commands.ModifyProfile, Artix.API.Core.Contract.Features.Museums

### Community 36 - "IEntityTypeConfiguration"
Cohesion: 0.09
Nodes (15): ObjectType, Category, Object, ObjectId, TypeId, MuseumObjectCategoryReadConfiguration, ObjectTypeReadConfiguration, AppUserTokenReadConfiguration (+7 more)

### Community 37 - ".Handle"
Cohesion: 0.22
Nodes (7): Artix.API.Core.Contract.Primitives.Infra.Identity.Authentication.Admin.Login, Artix.API.Core.Contract.Features.Users.Admin.Queries.GetLogin, GetLoginQueryHandler, AdminLoginDto, GetAdminLoginQuery, AdminLoginRequest, AdminLoginResponse

### Community 38 - "IActionResult"
Cohesion: 0.17
Nodes (6): MuseumController, ObjectController, UserController, MuseumController, ObjectController, UploadController

### Community 39 - "Artix.API.Infra.Sql/DependencyInjection.cs"
Cohesion: 0.12
Nodes (10): Artix.API.Core.Contract.Configs.Mongo, Artix.API.Core.Contract.Primitives.Repositories, Artix.API.Infra.Mongo, Artix.API.Infra.Sql, MongoDbSettings, ConnectionString, DatabaseName, IQueryRepository (+2 more)

### Community 40 - "XpRulesServiceTests"
Cohesion: 0.20
Nodes (3): Artix.API.Core.Contract.Primitives.DomainServices.XPRules, IXpRulesService, XpRulesServiceTests

### Community 42 - "NotificationMessage"
Cohesion: 0.14
Nodes (6): Artix.API.Core.Contract.Primitives.DomainServices.Notification, AddUserNotificationCommand, INotificationServiceProvider, INotificationService, NotificationMessage, NotificationServiceProvider

### Community 43 - ".AddRedis"
Cohesion: 0.05
Nodes (11): IFeatureToggleService, ILeaderboardService, XpRulesService, RedisEventDeduplicationStore, RedisFeatureToggleService, RedisJobQueueService, RedisLeaderboardService, RedisLockService (+3 more)

### Community 44 - "IEnumerable"
Cohesion: 0.21
Nodes (5): GetAllMuseumsQueryHandler, GetMuseumJournalEntriesQueryHandler, GetMuseumObjectsQueryHandler, MuseumQueries, MuseumQueryRepository

### Community 45 - "FileEntity"
Cohesion: 0.11
Nodes (13): FileEntity, FileName, FilePath, FileSize, MimeType, MuseumImages, ObjectGeneralInformation, ObjectImages (+5 more)

### Community 46 - "Quiz"
Cohesion: 0.14
Nodes (13): Quiz, BonusXP, Deadline, Description, IsSeasonal, Priority, RelatedFeature, RelatedObjectIds (+5 more)

### Community 47 - "ClientPaginateLoginHistoryDto"
Cohesion: 0.16
Nodes (7): Artix.API.Core.Contract.Features.Users.Admin.Queries.GetPaginateUsers, Artix.API.Core.Contract.Features.Objects.Client.Queries.GetPaginateObjects, Artix.API.Core.Contract.Features.Museums.Admin.Queries.GetPaginateObjects, AdminMuseumObjectDto, GetAdminMuseumObjectsQuery, AdminPaginateUsersDto, GetAdminPaginateUsersQuery

### Community 48 - ".Handle"
Cohesion: 0.18
Nodes (5): Artix.API.Core.Contract.Features.Collections.Client.Queries.GetUserCollection, GetAllCollectionsByUserIdQueryHandler, GetUserCollectionsQuery, UserCollectionDto, CollectionQueryRepository

### Community 49 - "DateTime"
Cohesion: 0.18
Nodes (4): LastLocationData, Ip, Timestamp, FixUserSession

### Community 50 - "Object"
Cohesion: 0.09
Nodes (16): Collection, Description, IsPublic, Items, Name, User, UserId, CollectionItem (+8 more)

### Community 51 - "مدل‌های اصلی"
Cohesion: 0.11
Nodes (17): مدل‌های اصلی, ObjectSaleType, Free, MemberShip, Tokenized, QuizObject, Object, ObjectId (+9 more)

### Community 52 - "OtpService.cs"
Cohesion: 0.11
Nodes (4): Artix.API.Core.ApplicationService.Features.Users.Client.Commands.InitiateOTPAuth, Artix.API.Core.ApplicationService.Features.Users.Client.Queries.GetVerifyOTPAuth, Artix.API.Core.DomainService, Artix.API.Core.DomainService.Services

### Community 53 - "QueryHandlerBase"
Cohesion: 0.18
Nodes (5): GetMuseumObjectsQueryHandler, GetUserProfileQueryHandler, GetPaginateLoginHistoryQueryHandler, ICommandHandler, IQueryHandler

### Community 54 - "ILogger"
Cohesion: 0.08
Nodes (8): IFileCompressor, CompressionWorker, NotificationConsumerHostedService, NotificationHub, NotificationOutboxProcessor, OutboxProcessor, RabbitMqConnectionFactory, SqlMigration

### Community 55 - "core_domain_entities_common"
Cohesion: 0.16
Nodes (4): Artix.API.Infra.Sql.Primitives, Artix.API.Infra.Mongo.Primitives, Artix.API.Infra.Sql.Repositories, Artix.API.Infra.Mongo.Data.DbContext

### Community 57 - "CommandHandlerBase"
Cohesion: 0.07
Nodes (15): AddObjectToCollectionCommandHandler, CreateNewMuseumCommandHandler, RemoveMuseumCommandHandler, CreateNewObjectCommandHandler, UpgradeObjectCommandHandler, AddObjectToUserCollectionCommandHandler, ScanObjectCommandHandler, IMuseumCommandRepository (+7 more)

### Community 58 - ".Handle"
Cohesion: 0.13
Nodes (8): Artix.API.Core.Contract.Primitives.Infra.Identity.Authentication.Admin.Logout, Artix.API.Core.Contract.Primitives.Infra.Identity.Authentication.Client.Logout, GetLogoutQueryHandler, GetLogoutQueryHandler, AdminLogoutRequest, AdminLogoutResponse, ClientLogoutRequest, ClientLogoutResponse

### Community 59 - "ApiVersionCheckMiddleware"
Cohesion: 0.21
Nodes (3): ApiKeyAuthenticationMiddleware, ApiVersionCheckMiddleware, RateLimitingMiddleware

### Community 60 - "IMuseumQueryRepository"
Cohesion: 0.17
Nodes (3): IMuseumQueryRepository, IQuizQueryRepository, QueryRepository

### Community 62 - "Artix API — Security Vulnerability Backlog"
Cohesion: 0.17
Nodes (11): Artix API — Security Vulnerability Backlog, C-01 — تصاحب حساب با OTP ثابت, C-02 — ثبت‌نام عمومی کاربر با نقش Admin, C-03 — عبور کاربر Client از کنترل ادمین به‌علت ترتیب middleware, C-04 — secret و credential در مخزن, C-05 — آپلود عمومی و سرو فایل‌های کنترل‌نشده؛ زنجیره Disk DoS و Stored XSS, Critical, اقدام فوری پیشنهادی (+3 more)

### Community 63 - "UserQuiz"
Cohesion: 0.17
Nodes (8): UserQuiz, CompletedAt, Priority, Progress, QuestId, Status, User, UserId

### Community 64 - "FileEntity"
Cohesion: 0.11
Nodes (10): ObjectGeneralInformation, FileEntity, FileId, Object, ObjectId, ObjectSpecialInformation, FileEntity, FileId (+2 more)

### Community 65 - "Result"
Cohesion: 0.16
Nodes (6): GetReNewAccessTokenQueryHandler, GetUserProfileQueryHandler, Result, Data, ErrorMessage, IsSuccess

### Community 66 - "UserXp"
Cohesion: 0.18
Nodes (7): UserXp, LastUpdated, TotalXp, User, UserId, UserXpReadConfiguration, UserXpWriteConfiguration

### Community 67 - "HistoricalPeriod"
Cohesion: 0.16
Nodes (10): HistoricalPeriod, Description, EndDate, Name, ObjectHistoricalPeriods, StartDate, HistoricalDateConverter, HistoricalPeriodReadConfiguration (+2 more)

### Community 68 - "UserScan"
Cohesion: 0.13
Nodes (12): Tier و concession, اسکن و XP, رفتار پیاده‌شده و قواعد دامنه, UserScan, AcquiredAt, InCollection, IsUpgraded, Object (+4 more)

### Community 69 - ".AddArtixServices"
Cohesion: 0.15
Nodes (3): Artix.API.Core.Contract.Primitives.CircuitBreaker, PollyPolicies, HostingExtension

### Community 72 - "GetVerifyOTPAuthQuery.cs"
Cohesion: 0.40
Nodes (3): Artix.API.Core.Contract.Features.Users.Client.Queries.GetVerifyOTPAuth, GetVerifyOTPAuthQuery, VerifyOTPAuthDto

### Community 73 - "AppVersion"
Cohesion: 0.14
Nodes (10): AppVersion, Description, IsRequired, Major, Minor, MinSupported, Patch, VersionString (+2 more)

### Community 74 - "SeasonTask"
Cohesion: 0.20
Nodes (8): SeasonTask, Description, IsPro, Season, SeasonId, XpReward, SeasonTaskReadConfiguration, SeasonTaskWriteConfiguration

### Community 75 - "UploadSession"
Cohesion: 0.12
Nodes (13): UploadSession, ChunkSize, Completed, CreatedAt, FileName, Id, IsCompressed, PhysicalFilePath (+5 more)

### Community 76 - "IntegrationError"
Cohesion: 0.20
Nodes (8): HttpIntegrationException, Error, HttpResponseMessageExtensions, IntegrationError, RawContent, ReasonPhrase, StatusCode, Url

### Community 77 - "MongoUnitOfWork"
Cohesion: 0.14
Nodes (3): IUnitOfWork, MongoQueryRepository, MongoUnitOfWork

### Community 78 - "Artix.API.Core.DomainService.csproj"
Cohesion: 0.12
Nodes (16): Microsoft.AspNetCore.SignalR, Microsoft.Extensions.Hosting.Abstractions, RabbitMQ.Client, net9.0, Microsoft.NET.Sdk, net9.0, StackExchange.Redis, Microsoft.NET.Sdk (+8 more)

### Community 80 - "UserMuseumKey"
Cohesion: 0.14
Nodes (11): UserMuseumKey, IsShared, IsUnlocked, Museum, MuseumId, ShareCount, UnlockedAt, User (+3 more)

### Community 81 - "http"
Cohesion: 0.13
Nodes (15): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, applicationUrl, commandName (+7 more)

### Community 82 - "ArtixQueryDbContext"
Cohesion: 0.05
Nodes (33): ArtixQueryDbContext, AppRoles, AppUsers, AppVersions, CollectionItems, Collections, Files, Friendships (+25 more)

### Community 83 - "PaginatedResult"
Cohesion: 0.22
Nodes (6): GetPaginateMuseumsQueryHandler, GetPaginatedUsersQueryHandler, PaginatedResult, HasNextPage, HasPreviousPage, TotalPages

### Community 84 - "IServiceCollection"
Cohesion: 0.09
Nodes (4): Artix.API.Endpoints.Extensions, Artix.API.Core.Contract, DependencyInjection, LowercaseParameterTransformer

### Community 85 - ".AddElasticsearch"
Cohesion: 0.16
Nodes (7): Artix.API.WebService.Extensions, ElasticsearchExtensions, ElasticsearchStatus, Index, IsValid, Settings, Uri

### Community 86 - "NotificationHandlerBase"
Cohesion: 0.15
Nodes (4): Artix.API.Core.Contract.Primitives.Infra.RabbitMQ, NotificationHandlerBase, IEventPublisher, INotificationProducer

### Community 87 - "Artix.API.WebService.csproj"
Cohesion: 0.11
Nodes (17): Elastic.Serilog.Sinks, Elasticsearch.Net, Microsoft.Extensions.Http.Polly, NEST, Serilog, Serilog.AspNetCore, Serilog.Settings.Configuration, Serilog.Sinks.Console (+9 more)

### Community 88 - "TierConfig"
Cohesion: 0.14
Nodes (13): TierConfig, MinDaysSinceAcquired, MinScanCount, Multiplier, Priority, RequiredActiveStreak, RequiredCoOpKey, RequiredInCollection (+5 more)

### Community 90 - "GetAllTiersQuery"
Cohesion: 0.22
Nodes (6): Artix.API.Core.Contract.Features.Objects.Admin.Commands.CreateNewObject, Artix.API.Core.Domain.Entities.Object.Enums, Artix.API.Core.Contract.Features.Tiers.Client.Queries.GetAll, AdminCreateNewObjectCommand, AllTierDto, GetAllTiersQuery

### Community 92 - "UserNotification"
Cohesion: 0.12
Nodes (11): UserNotification, DeliveredAt, DeliveryStatus, IsRead, Notification, NotificationId, ReadAt, User (+3 more)

### Community 93 - "UserSeasonProgress"
Cohesion: 0.16
Nodes (9): UserSeasonProgress, LastUpdated, Season, SeasonId, TotalXp, User, UserId, UserSeasonProgressReadConfiguration (+1 more)

### Community 96 - "Fact"
Cohesion: 0.12
Nodes (7): Artix.API.Tests.Unit, Artix.API.Tests.Integration, Artix.API.Tests.EndToEnd, UnitTest1, UnitTest1, AdminPolicyTests, UnitTest1

### Community 97 - "List"
Cohesion: 0.14
Nodes (10): Artix.API.Core.Contract.Features.Objects.Admin.Queries.GetObjectDetailsById, AdminObjectDetailsByIdDto, GetAdminObjectDetailsByIdQuery, RecentObjectDto, GeneralInformationUrl, HistoricalPeriod, ImageUrl, Model3DUrl (+2 more)

### Community 98 - "ClientObjectDetailsByIdDto"
Cohesion: 0.22
Nodes (6): Artix.API.Core.Contract.Features.Objects.Client.Queries.GetObjectDetailsById, Artix.API.Core.Contract.Features.Objects.Client.Queries.GetUserRecentObjectsVisits, ClientObjectDetailsByIdDto, GetClientObjectDetailsByIdQuery, ClientUserRecentObjectsVisitDto, GetClientUserRecentObjectsVisitQuery

### Community 99 - "FileSettings"
Cohesion: 0.15
Nodes (12): Artix.API.Core.Contract.Configs.FileSettings, FileSettings, Allowed3DMimeTypes, AllowedImageMimeTypes, AllowedReadmeMimeTypes, AllowGzipForOtherFiles, BaseUrl, ExtensionsToSkip (+4 more)

### Community 100 - ".AddDomainServiceServices"
Cohesion: 0.19
Nodes (3): FirstUserScanEventHandler, RepeatUserScanEventHandler, DependencyInjection

### Community 101 - "primitives_repositories"
Cohesion: 0.18
Nodes (6): Artix.API.Core.Contract.Features.Notifications.Commands, Artix.API.Core.Contract.Features.Objects, IFileCommandRepository, INotificationCommandRepository, IObjectCommandRepository, IObjectQueryRepository

### Community 102 - ".Handle"
Cohesion: 0.19
Nodes (5): Artix.API.Core.Contract.Primitives.DomainServices.OTP.Init, InitiateOTPAuthHandler, GetVerifyOTPAuthHandler, InitOTPRequest, InitOTPResult

### Community 104 - "Artix.API.Core.Contract.csproj"
Cohesion: 0.20
Nodes (9): FluentValidation, FluentValidation.DependencyInjectionExtensions, MediatR, Scrutor, net9.0, Polly, Polly.Extensions.Http, StackExchange.Redis (+1 more)

### Community 105 - "JournalEntry"
Cohesion: 0.20
Nodes (9): JournalEntry, Notes, Object, ObjectId, SketchUrl, Title, UserJournalEntries, JournalEntryReadConfiguration (+1 more)

### Community 106 - "MuseumImage"
Cohesion: 0.20
Nodes (7): MuseumImage, FileEntity, FileId, Museum, MuseumId, MuseumImageReadConfiguration, MuseumImageWriteConfiguration

### Community 107 - "MuseumObject"
Cohesion: 0.20
Nodes (7): MuseumObject, Museum, MuseumId, Object, ObjectId, MuseumObjectReadConfiguration, MuseumObjectWriteConfiguration

### Community 108 - "ObjectModel"
Cohesion: 0.20
Nodes (7): ObjectModel, FileEntity, FileId, Object, ObjectId, ObjectModelReadConfiguration, ObjectModelWriteConfiguration

### Community 109 - "ObjectImage"
Cohesion: 0.20
Nodes (7): ObjectImage, FileEntity, FileId, Object, ObjectId, ObjectImageReadConfiguration, ObjectImageWriteConfiguration

### Community 110 - "MarketplaceItem"
Cohesion: 0.20
Nodes (10): MarketplaceItem, IsSold, ListedAt, Object, ObjectId, PricePoints, Seller, SellerId (+2 more)

### Community 111 - "Artix.API.sln"
Cohesion: 0.15
Nodes (5): Swashbuckle.AspNetCore.SwaggerGen, net9.0, Microsoft.EntityFrameworkCore, Microsoft.EntityFrameworkCore.Tools, Microsoft.NET.Sdk

### Community 112 - "UserSessionService"
Cohesion: 0.11
Nodes (3): IUserSessionService, ITokenRevocationStore, UserSessionService

### Community 113 - "Artix.API.Utils"
Cohesion: 0.20
Nodes (4): Artix.API.Utils, DependencyInjection, FileNameHelper, ListExtensions

### Community 114 - "ClientType"
Cohesion: 0.10
Nodes (19): Artix.API.Core.Domain.Entities.User.Enums, ClientType, Emerald, Pro, Ruby, Turquoise, Role, Admin (+11 more)

### Community 115 - "IQuery"
Cohesion: 0.17
Nodes (7): Artix.API.Core.Contract.Features.Users.Admin.Queries.GetLogout, AdminLogoutDto, GetAdminLogoutQuery, PaginationQuery, SortDirection, Asc, Desc

### Community 116 - "AdminAuditEntry"
Cohesion: 0.20
Nodes (8): Artix.API.Core.Domain.Entities.Audit, AdminAuditEntry, IpAddress, Method, Path, Roles, StatusCode, UserId

### Community 117 - "AuthenticationSettings"
Cohesion: 0.20
Nodes (9): Artix.API.Core.Contract.Configs.Authentication, AuthenticationSettings, AccessTokenExpireSeconds, Audience, Issuer, IssuerSigningKey, RefreshTokenExpireDays, RefreshTokenLength (+1 more)

### Community 120 - "Artix.API.Infra.Sql.csproj"
Cohesion: 0.17
Nodes (11): EFCore.BulkExtensions, Microsoft.EntityFrameworkCore.Proxies, Microsoft.EntityFrameworkCore.SqlServer, Microsoft.Extensions.Configuration.EnvironmentVariables, Newtonsoft.Json, net9.0, Microsoft.AspNetCore.Identity.EntityFrameworkCore, Microsoft.EntityFrameworkCore (+3 more)

### Community 121 - "Artix.API.Tests.Integration.csproj"
Cohesion: 0.17
Nodes (11): Microsoft.EntityFrameworkCore.InMemory, Moq, NUnit, Testcontainers.MsSql, Testcontainers.Redis, net9.0, coverlet.collector, Microsoft.NET.Test.Sdk (+3 more)

### Community 122 - "FileWatcherService"
Cohesion: 0.14
Nodes (3): IBackgroundJobScheduler, FileCompressor, FileWatcherService

### Community 123 - "RateLimitAttribute"
Cohesion: 0.25
Nodes (6): RateLimitAttribute, KeyPrefix, Limit, UseClientIp, UseUserIdentifier, WindowSeconds

### Community 124 - "mediatr"
Cohesion: 0.05
Nodes (13): Artix.API.Endpoints.Controllers.Common, Artix.API.Endpoints.Controllers.Shared, Artix.API.Endpoints.Controllers.Client, Artix.API.Core.Contract.Primitives.Handlers, Artix.API.Endpoints.Controllers.AdminPanel, Artix.API.Infra.Sql.Data.CompiledQueries.Museums, Artix.API.Endpoints.Filters, ICommand (+5 more)

### Community 126 - "Artix.API.Core.ApplicationService/DependencyInjection.cs"
Cohesion: 0.20
Nodes (4): Artix.API.Core.Contract.Primitives.Validations, Artix.API.Core.ApplicationService, DependencyInjection, FluentValidationBehavior

### Community 127 - "QuizCompletedEvent"
Cohesion: 0.25
Nodes (7): Artix.API.Core.Domain.DomainEvents, Artix.API.Core.Domain.Entities.Quiz.Events, IDomainEvent, QuizCompletedEvent, QuestId, UserId, XPReward

### Community 128 - "ClientPaginateLoginHistoryDto"
Cohesion: 0.25
Nodes (7): Artix.API.Core.Contract.Features.Users.Client.Queries.GetPaginateLoginHistories, ClientPaginateLoginHistoryDto, Date, IpAddress, IsActive, UserAgent, GetClientPaginateLoginHistoryQuery

### Community 129 - "AnomalyDetectionResult"
Cohesion: 0.22
Nodes (6): AnomalyDetectionResult, AnomalyScore, DetectedAnomalies, IsAnomalous, Metadata, RequiredAction

### Community 130 - "OutboxMessage"
Cohesion: 0.13
Nodes (11): Artix.API.Core.Domain.Entities.OTP, Artix.API.Core.Domain.Persistence, OutboxMessage, CreatedAt, Data, Error, Id, ProcessedAt (+3 more)

### Community 132 - "UserStrike"
Cohesion: 0.14
Nodes (10): UserStrike, FuelCount, IsActive, LastInteraction, StrikeCount, StrikeStart, User, UserId (+2 more)

### Community 134 - "ApplicationServiceNotFoundException"
Cohesion: 0.15
Nodes (5): Artix.API.Core.Domain.Exceptions, Artix.API.Core.ApplicationService.Exceptions, ApplicationServiceNotFoundException, TooManyRequestsException, DomainException

### Community 135 - "BruteForceCheckResult"
Cohesion: 0.22
Nodes (6): BruteForceCheckResult, IsAllowed, LockoutUntil, RemainingAttempts, SuggestedAction, TotalAttempts

### Community 136 - "IVersionQueryRepository.cs"
Cohesion: 0.25
Nodes (4): Artix.API.Core.Contract.Features.Versions.Commands, Artix.API.Core.Contract.Features.Versions.Queries, IVersionCommandRepository, IVersionQueryRepository

### Community 137 - "Friendship"
Cohesion: 0.16
Nodes (8): Friendship, CreatedAt, Friend, FriendId, User, UserId, FriendshipReadConfiguration, FriendshipWriteConfiguration

### Community 139 - "ElasticsearchSettings"
Cohesion: 0.25
Nodes (7): Artix.API.Core.Contract.Configs.Elasticsearch, ElasticsearchSettings, IndexFormat, Password, RequestTimeoutInMinutes, Uri, Username

### Community 142 - "AnomalyScore"
Cohesion: 0.25
Nodes (7): AnomalyScore, BehaviorTrust, DeviceTrust, LocationTrust, OverallScore, RiskLevel, TimeTrust

### Community 143 - "GetUserRecentMuseumsVisitsQueryHandler"
Cohesion: 0.48
Nodes (4): Artix.API.Core.Contract.Features.Museums.Client.Queries.GetUserRecentVisits, GetUserRecentMuseumsVisitsQueryHandler, ClientUserRecentMuseumsVisitDto, GetClientUserRecentMuseumsVisitQuery

### Community 144 - "High"
Cohesion: 0.18
Nodes (11): H-01 — IDOR روی upload session و chunkها, H-02 — نبود سقف اندازه و اعتبارسنجی chunk, H-03 — refresh token به‌صورت plaintext و انقضای مؤثرنشده, H-04 — rotation refresh token در برابر هم‌زمانی اتمیک نیست, H-05 — اعتماد به `X-Forwarded-For` قابل جعل, H-06 — rate limit ارسال OTP خاموش است, H-07 — endpointهای خواندنی Admin بدون authorization, H-08 — اعتبارسنجی ورودی و محدودیت صفحه‌بندی ناکافی (+3 more)

### Community 145 - "TierConfig"
Cohesion: 0.38
Nodes (3): TierConfigReadConfiguration, TierConfigWriteConfiguration, TierQueryRepository

### Community 146 - "QuizStatus"
Cohesion: 0.29
Nodes (6): Artix.API.Core.Domain.Entities.Quest.Enums, QuizStatus, Completed, Failed, InProgress, Pending

### Community 147 - "RabbitMqOptions"
Cohesion: 0.29
Nodes (6): Artix.API.Core.Contract.Configs.RabbitMQ, RabbitMqOptions, Host, Password, Port, Username

### Community 148 - "Medium"
Cohesion: 0.29
Nodes (7): M-01 — Swagger و schema بدون محدودیت محیطی, M-02 — فایل‌های public با cache یک‌ساله و بدون کنترل دسترسی, M-03 — اعتماد به extension به‌جای magic bytes/MIME واقعی, M-04 — مسیر storage قابل تنظیم خارج از sandbox, M-05 — اطلاعات حساس در logها, M-06 — وابستگی geo lookup به HTTP بدون proxy trust و fail-closed, Medium

### Community 149 - ".Handle"
Cohesion: 0.48
Nodes (4): Artix.API.Core.Contract.Features.Versions.Queries.GetLast, GetLastVersionQueryHandler, GetLastVersionQuery, LastVersionDto

### Community 150 - "OutboxMessageStatus"
Cohesion: 0.29
Nodes (6): Artix.API.Core.Domain.Persistence.Enums, OutboxMessageStatus, Dead, Failed, Pending, Processed

### Community 151 - "AppRole"
Cohesion: 0.38
Nodes (3): AppRole, AppRoleReadConfiguration, AppRoleWriteConfiguration

### Community 153 - "VoiceTrack"
Cohesion: 0.16
Nodes (10): Artix.API.Infra.Sql.Data.Config.Read.Voice, Artix.API.Infra.Sql.Data.Config.Write.Voice, Artix.API.Core.Domain.Entities.Voice, VoiceTrackFile, FileEntity, FileId, VoiceTrack, VoiceTrackId (+2 more)

### Community 154 - "JwtTokenResult"
Cohesion: 0.12
Nodes (11): Artix.API.Core.Contract.Primitives.Infra.Identity, IDeviceFingerprintService, IJwtTokenGenerator, ITokenService, JwtTokenResult, AccessToken, AccessTokenExpiresAt, Jti (+3 more)

### Community 155 - "ErrorResponse"
Cohesion: 0.29
Nodes (6): ErrorResponse, Error, Exception, Path, Stack, Status

### Community 156 - "Artix.API.Core.Domain.csproj"
Cohesion: 0.18
Nodes (9): Microsoft.AspNetCore.Authentication, MongoDB.Bson, net9.0, Microsoft.AspNetCore.Identity.EntityFrameworkCore, Microsoft.NET.Sdk, net9.0, Microsoft.NET.Sdk, net9.0 (+1 more)

### Community 158 - "UserImage"
Cohesion: 0.20
Nodes (7): UserImage, AppUser, FileEntity, FileId, UserId, UserImageReadConfiguration, UserImageWriteConfiguration

### Community 159 - ".Handle"
Cohesion: 0.21
Nodes (6): Artix.API.Core.Contract.Features.Quizzes.Client.Queries.GetShuffledQuizzes, GetShuffledQuizzesQueryHandler, ClientShuffledQuizzesDto, ShuffledQuizzesActionDto, GetClientShuffledQuizzesQuery, QuizQueryRepository

### Community 160 - "VerifyOTPResult"
Cohesion: 0.40
Nodes (3): Artix.API.Core.Contract.Primitives.DomainServices.OTP.Verify, VerifyOTPRequest, VerifyOTPResult

### Community 161 - "RedisOptions"
Cohesion: 0.33
Nodes (5): Artix.API.Core.Contract.Configs.Redis, RedisOptions, Host, Password, Port

### Community 162 - "QuizAction"
Cohesion: 0.33
Nodes (5): Artix.API.Core.Domain.Entities.Quiz.ValueObjects, QuizAction, ActionType, Details, RequiredCount

### Community 163 - "UserProfileDto"
Cohesion: 0.47
Nodes (4): Artix.API.Core.Contract.Features.Users.Admin.Queries.GetUserProfile, GetUserProfileQuery, UserClaim, UserProfileDto

### Community 164 - "JournalEntryReadConfiguration.cs"
Cohesion: 0.40
Nodes (3): Artix.API.Core.Domain.Entities.JournalEntry, Artix.API.Infra.Sql.Data.Config.Read.JournalEntry, Artix.API.Infra.Sql.Data.Config.Write.JournalEntry

### Community 165 - "Artix.API.Endpoints/DependencyInjection.cs"
Cohesion: 0.07
Nodes (10): Artix.API.Core.Contract.Primitives.Infra.Redis.Caches.Museums, Artix.API.Core.Contract.Primitives.Infra.Redis.Caches, Artix.API.Endpoints, RecentMuseumDto, ImageUrl, Name, ObjectCount, RecentBaseEntity (+2 more)

### Community 166 - "Artix.API.Tests.EndToEnd.csproj"
Cohesion: 0.25
Nodes (7): RestSharp, net9.0, coverlet.collector, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio, Microsoft.NET.Sdk

### Community 168 - "ArtixCommandDbContext.cs"
Cohesion: 0.07
Nodes (9): Artix.API.Infra.Sql.Data.Config.Write.OutboxMessages, Artix.API.Infra.Sql.Data.Config.Write.TierConfig, Artix.API.Infra.Sql.Data.Config.Write.Audit, Artix.API.Infra.Sql.Data.Interceptors, Artix.API.Infra.Sql.Data.Config.Read.TierConfig, Artix.API.Infra.Sql.Data.Config.Read.OTP, Artix.API.Infra.Sql.Data.Config.Write.OTP, Artix.API.Infra.Sql.Data (+1 more)

### Community 169 - "GetReNewAccessTokenQuery.cs"
Cohesion: 0.50
Nodes (3): Artix.API.Core.Contract.Features.Users.Client.Queries.GetReNewAccessToken, GetReNewAccessTokenQuery, ReNewAccessTokenDto

### Community 170 - "Artix API — Agent Guide"
Cohesion: 0.29
Nodes (6): Artix API — Agent Guide, Graphify, تست و بررسی, شناخت محصول, قواعد تغییر, معماری و محل تغییر

### Community 171 - "PurposeType"
Cohesion: 0.40
Nodes (4): Artix.API.Core.Domain.Entities.OTP.Enums, PurposeType, Login, Registration

### Community 172 - "Artix.API.Infra.Mongo.csproj"
Cohesion: 0.33
Nodes (5): Microsoft.Extensions.Configuration.Abstractions, Microsoft.Extensions.Configuration.Binder, MongoDB.Driver, net9.0, Microsoft.NET.Sdk

### Community 173 - "GetLogoutQuery.cs"
Cohesion: 0.50
Nodes (3): Artix.API.Core.Contract.Features.Users.Client.Queries.GetLogout, GetLogoutQuery, LogoutDto

### Community 174 - "ClientMuseumDetailsByIdDto"
Cohesion: 0.50
Nodes (3): Artix.API.Core.Contract.Features.Museums.Client.Queries.GetDetailByIds, ClientMuseumDetailsByIdDto, GetClientMuseumDetailsByIdQuery

### Community 175 - "AuthenticationApiSettings"
Cohesion: 0.40
Nodes (4): Artix.API.Core.Contract.Configs.AuthenticationApi, AuthenticationApiSettings, ApiKey, RequireApiKeyInProduction

### Community 176 - "GetCollectionsByUserIdQuery"
Cohesion: 0.50
Nodes (3): Artix.API.Core.Contract.Features.Collections.Client.Queries.GetCollectionByUserId, CollectionsByUserIdDto, GetCollectionsByUserIdQuery

### Community 177 - "GetLatestOTPByPhoneNumberQuery"
Cohesion: 0.50
Nodes (3): Artix.API.Core.Contract.Features.OTPs.Queries.GetLatestByPhoneNumber, GetLatestOTPByPhoneNumberQuery, LatestOTPByPhoneNumberDto

### Community 178 - "GetMuseumJournalEntriesQuery"
Cohesion: 0.50
Nodes (3): Artix.API.Core.Contract.Features.Museums.Client.Queries.GetJournalEntries, GetMuseumJournalEntriesQuery, MuseumJournalEntryDto

### Community 179 - "GetMuseumKeyStatusQuery"
Cohesion: 0.50
Nodes (3): Artix.API.Core.Contract.Features.Museums.Client.Queries.GetKeyStatus, GetMuseumKeyStatusQuery, MuseumKeyStatusDto

### Community 180 - "IOTPQueryRepository.cs"
Cohesion: 0.25
Nodes (4): Artix.API.Core.Contract.Features.OTPs.Queries, Artix.API.Core.Contract.Features.OTPs.Commands, IOTPCommandRepository, IOTPQueryRepository

### Community 181 - "GetClientMuseumObjectsQuery"
Cohesion: 0.50
Nodes (3): Artix.API.Core.Contract.Features.Museums.Client.Queries.GetObjects, ClientMuseumObjectDto, GetClientMuseumObjectsQuery

### Community 182 - "ClientAllMuseumsDto"
Cohesion: 0.50
Nodes (3): Artix.API.Core.Contract.Features.Museums.Client.Queries.GetAll, ClientAllMuseumsDto, GetClientAllMuseumsQuery

### Community 184 - "DeviceTrustScore"
Cohesion: 0.40
Nodes (5): DeviceTrustScore, Anomalies, OverallScore, Reasons, Score

### Community 185 - "Artix.API.Tests.Unit.csproj"
Cohesion: 0.17
Nodes (10): Microsoft.AspNetCore.Authentication.JwtBearer, net9.0, Microsoft.AspNetCore.Identity.EntityFrameworkCore, Microsoft.NET.Sdk, net9.0, coverlet.collector, Microsoft.NET.Test.Sdk, xunit (+2 more)

### Community 187 - "BaseEntity.cs"
Cohesion: 0.15
Nodes (4): Artix.API.Core.Domain.Entities.Version, Artix.API.Infra.Sql.Data.Config.Write.Version, Artix.API.Infra.Sql.Data.Config.Read.Version, Artix.API.Core.Domain.Entities.Common

### Community 188 - "AdminPaginatedMuseumsDto"
Cohesion: 0.50
Nodes (3): Artix.API.Core.Contract.Features.Museums.Admin.Queries.GetPaginateMuseums, AdminPaginatedMuseumsDto, GetAdminPaginateMuseumsQuery

### Community 191 - "Application services — verified context and upkeep"
Cohesion: 0.25
Nodes (7): Application services — verified context and upkeep, Architectural cautions, Evidence / discovery anchors, Investigation checklist, Memory update protocol, Observed role, Project path

### Community 192 - "Application contracts — verified context and upkeep"
Cohesion: 0.25
Nodes (7): Application contracts — verified context and upkeep, Architectural cautions, Evidence / discovery anchors, Investigation checklist, Memory update protocol, Observed role, Project path

### Community 193 - "Season"
Cohesion: 0.25
Nodes (7): Season, EndDate, IsActive, Name, SeasonTasks, StartDate, UserSeasonProgresses

### Community 194 - "Domain model — verified context and upkeep"
Cohesion: 0.25
Nodes (7): Architectural cautions, Domain model — verified context and upkeep, Evidence / discovery anchors, Investigation checklist, Memory update protocol, Observed role, Project path

### Community 195 - "Domain services — verified context and upkeep"
Cohesion: 0.25
Nodes (7): Architectural cautions, Domain services — verified context and upkeep, Evidence / discovery anchors, Investigation checklist, Memory update protocol, Observed role, Project path

### Community 196 - "File storage — verified context and upkeep"
Cohesion: 0.25
Nodes (7): Architectural cautions, Evidence / discovery anchors, File storage — verified context and upkeep, Investigation checklist, Memory update protocol, Observed role, Project path

### Community 197 - "Identity and sessions — verified context and upkeep"
Cohesion: 0.25
Nodes (7): Architectural cautions, Evidence / discovery anchors, Identity and sessions — verified context and upkeep, Investigation checklist, Memory update protocol, Observed role, Project path

### Community 198 - "Mongo persistence — verified context and upkeep"
Cohesion: 0.25
Nodes (7): Architectural cautions, Evidence / discovery anchors, Investigation checklist, Memory update protocol, Mongo persistence — verified context and upkeep, Observed role, Project path

### Community 199 - "Messaging and notifications — verified context and upkeep"
Cohesion: 0.25
Nodes (7): Architectural cautions, Evidence / discovery anchors, Investigation checklist, Memory update protocol, Messaging and notifications — verified context and upkeep, Observed role, Project path

### Community 200 - "Redis integration — verified context and upkeep"
Cohesion: 0.25
Nodes (7): Architectural cautions, Evidence / discovery anchors, Investigation checklist, Memory update protocol, Observed role, Project path, Redis integration — verified context and upkeep

### Community 201 - "SQL persistence — verified context and upkeep"
Cohesion: 0.25
Nodes (7): Architectural cautions, Evidence / discovery anchors, Investigation checklist, Memory update protocol, Observed role, Project path, SQL persistence — verified context and upkeep

### Community 202 - "HTTP contracts and endpoints — verified context and upkeep"
Cohesion: 0.25
Nodes (7): Architectural cautions, Evidence / discovery anchors, HTTP contracts and endpoints — verified context and upkeep, Investigation checklist, Memory update protocol, Observed role, Project path

### Community 203 - "Composition root / runtime host — verified context and upkeep"
Cohesion: 0.25
Nodes (7): Architectural cautions, Composition root / runtime host — verified context and upkeep, Evidence / discovery anchors, Investigation checklist, Memory update protocol, Observed role, Project path

### Community 204 - "Shared utilities — verified context and upkeep"
Cohesion: 0.25
Nodes (7): Architectural cautions, Evidence / discovery anchors, Investigation checklist, Memory update protocol, Observed role, Project path, Shared utilities — verified context and upkeep

### Community 205 - "End-to-end tests — verified context and upkeep"
Cohesion: 0.25
Nodes (7): Architectural cautions, End-to-end tests — verified context and upkeep, Evidence / discovery anchors, Investigation checklist, Memory update protocol, Observed role, Project path

### Community 206 - "Integration tests — verified context and upkeep"
Cohesion: 0.25
Nodes (7): Architectural cautions, Evidence / discovery anchors, Integration tests — verified context and upkeep, Investigation checklist, Memory update protocol, Observed role, Project path

### Community 207 - "Unit tests — verified context and upkeep"
Cohesion: 0.25
Nodes (7): Architectural cautions, Evidence / discovery anchors, Investigation checklist, Memory update protocol, Observed role, Project path, Unit tests — verified context and upkeep

### Community 208 - "Artix Admin Roles & Access Control Backlog"
Cohesion: 0.29
Nodes (6): Artix Admin Roles & Access Control Backlog, تصمیم‌های لازم قبل از کدنویسی, فاز دوم, فاز سوم, هدف, کارهای فاز اول

### Community 211 - "ICollectionQueryRepository.cs"
Cohesion: 0.33
Nodes (3): Artix.API.Core.Contract.Features.Collections, ICollectionCommandRepository, ICollectionQueryRepository

### Community 213 - "Repository architecture memory (observed, not instructions)"
Cohesion: 0.29
Nodes (6): Critical architectural nuance, Functional structure, Maintenance, Operational facts, Repository architecture memory (observed, not instructions), Topology

### Community 216 - "Application services — scoped Codex instructions"
Cohesion: 0.33
Nodes (5): Application services — scoped Codex instructions, Change checklist, Completion, Local engineering rules, Ownership

### Community 217 - "Application contracts — scoped Codex instructions"
Cohesion: 0.33
Nodes (5): Application contracts — scoped Codex instructions, Change checklist, Completion, Local engineering rules, Ownership

### Community 218 - "Domain model — scoped Codex instructions"
Cohesion: 0.33
Nodes (5): Change checklist, Completion, Domain model — scoped Codex instructions, Local engineering rules, Ownership

### Community 221 - "Domain services — scoped Codex instructions"
Cohesion: 0.33
Nodes (5): Change checklist, Completion, Domain services — scoped Codex instructions, Local engineering rules, Ownership

### Community 222 - "File storage — scoped Codex instructions"
Cohesion: 0.33
Nodes (5): Change checklist, Completion, File storage — scoped Codex instructions, Local engineering rules, Ownership

### Community 223 - "Identity and sessions — scoped Codex instructions"
Cohesion: 0.33
Nodes (5): Change checklist, Completion, Identity and sessions — scoped Codex instructions, Local engineering rules, Ownership

### Community 224 - "Mongo persistence — scoped Codex instructions"
Cohesion: 0.33
Nodes (5): Change checklist, Completion, Local engineering rules, Mongo persistence — scoped Codex instructions, Ownership

### Community 225 - "Messaging and notifications — scoped Codex instructions"
Cohesion: 0.33
Nodes (5): Change checklist, Completion, Local engineering rules, Messaging and notifications — scoped Codex instructions, Ownership

### Community 226 - "Redis integration — scoped Codex instructions"
Cohesion: 0.33
Nodes (5): Change checklist, Completion, Local engineering rules, Ownership, Redis integration — scoped Codex instructions

### Community 227 - "SQL persistence — scoped Codex instructions"
Cohesion: 0.33
Nodes (5): Change checklist, Completion, Local engineering rules, Ownership, SQL persistence — scoped Codex instructions

### Community 228 - "HTTP contracts and endpoints — scoped Codex instructions"
Cohesion: 0.33
Nodes (5): Change checklist, Completion, HTTP contracts and endpoints — scoped Codex instructions, Local engineering rules, Ownership

### Community 229 - "Composition root / runtime host — scoped Codex instructions"
Cohesion: 0.33
Nodes (5): Change checklist, Completion, Composition root / runtime host — scoped Codex instructions, Local engineering rules, Ownership

### Community 230 - "Shared utilities — scoped Codex instructions"
Cohesion: 0.33
Nodes (5): Change checklist, Completion, Local engineering rules, Ownership, Shared utilities — scoped Codex instructions

### Community 231 - "End-to-end tests — scoped Codex instructions"
Cohesion: 0.33
Nodes (5): Change checklist, Completion, End-to-end tests — scoped Codex instructions, Local engineering rules, Ownership

### Community 232 - "Integration tests — scoped Codex instructions"
Cohesion: 0.33
Nodes (5): Change checklist, Completion, Integration tests — scoped Codex instructions, Local engineering rules, Ownership

### Community 233 - "Unit tests — scoped Codex instructions"
Cohesion: 0.33
Nodes (5): Change checklist, Completion, Local engineering rules, Ownership, Unit tests — scoped Codex instructions

### Community 236 - "ClientUserProfileDto"
Cohesion: 0.50
Nodes (3): Artix.API.Core.Contract.Features.Users.Client.Queries.GetUserProfile, ClientUserProfileDto, GetClientUserProfileQuery

### Community 239 - "Artix.API.Core.Domain.Entities.Object"
Cohesion: 0.12
Nodes (3): Artix.API.Core.Domain.Entities.Object, Artix.API.Infra.Sql.Data.Config.Write.Object, Artix.API.Infra.Sql.Data.Config.Read.Object

## Knowledge Gaps
- **641 isolated node(s):** `net9.0`, `Microsoft.NET.Sdk`, `net9.0`, `Polly`, `Polly.Extensions.Http` (+636 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 1411 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **31 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `ArtixCommandDbContext` connect `ArtixCommandDbContext` to `MigrationBuilder`, `UserStrike`, `UserSession`, `ArtixCommandDbContext`, `EntityTypeBuilder`, `AppUser`, `Friendship`, `TierConfig`, `AppRole`, `Museum`, `IEntityTypeConfiguration`, `ArtixCommandDbContext.cs`, `.AddSqlServices`, `Artix API — Agent Guide`, `IEnumerable`, `FileEntity`, `Object`, `مدل‌های اصلی`, `CommandHandlerBase`, `ApiVersionCheckMiddleware`, `FileEntity`, `Season`, `UserXp`, `HistoricalPeriod`, `UserScan`, `SeasonTask`, `UserMuseumKey`, `ArtixQueryDbContext`, `UserSeasonProgress`, `JournalEntry`, `MuseumImage`, `MuseumObject`, `MarketplaceItem`, `DbContextOptionsBuilder`, `UserSessionService`, `TokenService`, `Repository`?**
  _High betweenness centrality (0.075) - this node is a cross-community bridge._
- **What connects `net9.0`, `Microsoft.NET.Sdk`, `net9.0` to the rest of the system?**
  _641 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `primitives` be split into smaller, more focused modules?**
  _Cohesion score 0.03881578947368421 - nodes in this community are weakly interconnected._
- **Why does `AppUser` connect `AppUser` to `ArtixCommandDbContext`, `Guid`, `UserStrike`, `UserSession`, `EntityTypeBuilder`, `Friendship`, `AppUser`, `Object`, `UserImage`, `AppUserBuilder`, `IDomainEvent`, `XpRulesServiceTests`, `DateTime`, `مدل‌های اصلی`, `UserXp`, `UserScan`, `UserMuseumKey`, `PaginatedResult`, `UserSeasonProgress`, `List`, `MarketplaceItem`, `Artix.API.Core.Domain.Entities.Object`, `TokenService`?**
  _High betweenness centrality (0.061) - this node is a cross-community bridge._
- **Should `ArtixCommandDbContext` be split into smaller, more focused modules?**
  _Cohesion score 0.047619047619047616 - nodes in this community are weakly interconnected._
- **Why does `Object` connect `Object` to `IDomainEvent`, `List`, `FileEntity`, `IEntityTypeConfiguration`, `primitives_repositories`, `XpRulesServiceTests.cs`, `XpRulesServiceTests`, `common`, `ObjectModel`, `ObjectImage`, `Object`, `PaginatedResult`, `TierConfig`, `Museum`, `mediatr`, `ObjectBuilder`?**
  _High betweenness centrality (0.041) - this node is a cross-community bridge._
- **Should `CancellationToken` be split into smaller, more focused modules?**
  _Cohesion score 0.08333333333333333 - nodes in this community are weakly interconnected._
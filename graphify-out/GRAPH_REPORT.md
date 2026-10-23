# Graph Report - Artix.API  (2026-10-23)

## Corpus Check
- 503 files · ~83,786 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 31 file(s) not represented in the graph (top: (none) 22, .hiv 3, .props 2)

## Summary
- 3487 nodes · 7802 edges · 195 communities (158 shown, 37 thin omitted)
- Extraction: 97% EXTRACTED · 3% INFERRED · 0% AMBIGUOUS · INFERRED: 263 edges (avg confidence: 0.86)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `b546a72f`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- primitives
- ArtixCommandDbContext
- CancellationToken
- MigrationBuilder
- Guid
- Client/ObjectController.cs
- microsoft_extensions_logging
- UserSession
- Repository
- EntityTypeBuilder
- AppUser
- common
- AnomalyDetectionService
- microsoft_entityframeworkcore
- dbcontexts
- Notification
- Artix.API.Core.Contract.Primitives.Infra.Redis
- ArtixCommandDbContext.cs
- AuthorizeCheckOperationFilter.cs
- OtpService
- primitives_handlers
- Task
- Object
- MongoCommandContext
- microsoft_aspnetcore_http
- .AddRabbitMqService
- Artix.API.Core.Domain.Entities.Notification.Enums
- Museum
- IRedisConnectionFactory
- .Handle
- ObjectBuilder
- AppUserBuilder
- IDomainEvent
- ClientPaginateObjectsDto
- ContinuousAuthorizationMiddleware
- CreateNewObjectCommandHandler.cs
- IEntityTypeConfiguration
- GetLoginQueryHandler.cs
- IActionResult
- Artix.API.Endpoints/DependencyInjection.cs
- XpRulesServiceTests
- .AddSqlServices
- .HandleEventAsync
- XpRulesService
- IEnumerable
- FileEntity
- Quiz
- ClientPaginateLoginHistoryDto
- .Handle
- .AddRedis
- Object
- مدل‌های اصلی
- OtpService.cs
- QueryHandlerBase
- ILogger
- UserXp
- .Handle
- microsoft_entityframeworkcore_metadata_builders
- .Handle
- ApiVersionCheckMiddleware
- .Handle
- HostingExtension.cs
- Artix.API.Core.Domain.Entities.Museum
- UserQuiz
- FileEntity
- Result
- Artix.API.Core.Domain.Entities.Voice
- HistoricalPeriod
- UserScan
- .AddArtixServices
- BruteForceProtectionService
- MuseumQueryRepository.cs
- GetVerifyOTPAuthQuery.cs
- AppVersion
- SqlDataSeeder
- UploadSession
- IntegrationError
- MongoUnitOfWork
- Artix.API.Core.DomainService.csproj
- IRepository
- UserMuseumKey
- http
- ArtixQueryDbContext
- PaginatedResult
- .AddFileService
- .AddElasticsearch
- NotificationHandlerBase
- Artix.API.WebService.csproj
- TierConfig
- AppUser
- GetAllTiersQuery
- system
- Category
- UserSeasonProgress
- .AddIdentityService
- MongoDataSeeder
- Fact
- List
- OutboxProcessor.cs
- FileSettings
- .AddDomainServiceServices
- primitives_repositories
- VerifyOTPResult
- UserJournalEntry
- Artix.API.Core.Contract.csproj
- JournalEntry
- MuseumImage
- MuseumObject
- ObjectModel
- ObjectImage
- MarketplaceItem
- UserSessionService
- Artix.API.Utils
- ClientType
- IQuery
- Exception
- AuthenticationSettings
- IQuery.cs
- .FindAsync
- Artix.API.Infra.Sql.csproj
- Artix.API.Tests.Integration.csproj
- CompressionWorker
- RateLimitAttribute
- AdminPanel/MuseumController.cs
- SeasonReadConfiguration.cs
- Artix.API.Core.ApplicationService/DependencyInjection.cs
- QuizCompletedEvent
- FileWatcherService
- AnomalyDetectionResult
- VoiceTrackFile
- mediatr
- UserStrike
- XpRulesServiceTests.cs
- ApplicationServiceNotFoundException
- Artix.API.Infra.RabbitMQ.csproj
- LowercaseParameterTransformer
- Friendship
- Client/Queries/GetLogout/GetLogoutQueryHandler.cs
- ElasticsearchSettings
- .AddMongoServices
- GeoFencingMiddleware
- AnomalyScore
- IMuseumQueryRepository
- DateTime
- ApiKeyAuthenticationMiddleware
- QuizStatus
- RabbitMqOptions
- MongoTimestampInterceptor
- .Handle
- OutboxMessageStatus
- AppRole
- TierConfig
- VoiceTrack
- JwtTokenResult
- ErrorResponse
- Artix.API.Core.Domain.csproj
- .IsMatch
- UserImage
- .Handle
- BaseEntity.cs
- RedisOptions
- QuizAction
- UserProfileDto
- ObjectHistoricalPeriod
- RecentMuseumDto
- Artix.API.Tests.EndToEnd.csproj
- BruteForceCheckResult
- OtpReadConfiguration.cs
- GetReNewAccessTokenQuery.cs
- Artix API — Agent Guide
- PurposeType
- Artix.API.Infra.Mongo.csproj
- GetLogoutQuery.cs
- ClientMuseumDetailsByIdDto
- AuthenticationApiSettings
- Artix.API.Core.Domain.Entities.File
- GetLatestOTPByPhoneNumberQuery
- GetMuseumJournalEntriesQuery
- GetMuseumKeyStatusQuery
- GetCollectionsByUserIdQuery
- GetClientMuseumObjectsQuery
- ClientAllMuseumsDto
- HttpContext
- DeviceTrustScore
- Artix.API.Tests.Unit.csproj
- Artix.API.Endpoints.csproj
- BaseEntity
- AdminPaginatedMuseumsDto
- namespace_optimizer.sh
- copy_and_transform.sh
- Artix.API.Core.Contract.Primitives.DomainServices.OTP.Init
- JournalEntryReadConfiguration.cs
- ITierCalculatorService.cs
- TODO.md

## God Nodes (most connected - your core abstractions)
1. `ArtixCommandDbContext` - 84 edges
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
- `مدل‌های اصلی` --references--> `Collection`  [INFERRED]
  AGENTS.md → src/Core/Artix.API.Core.Domain/Entities/Collection/Collection.cs
- `مدل‌های اصلی` --references--> `CollectionItem`  [INFERRED]
  AGENTS.md → src/Core/Artix.API.Core.Domain/Entities/Collection/CollectionItem.cs
- `مدل‌های اصلی` --references--> `JournalEntry`  [INFERRED]
  AGENTS.md → src/Core/Artix.API.Core.Domain/Entities/JournalEntry/JournalEntry.cs
- `مدل‌های اصلی` --references--> `Museum`  [INFERRED]
  AGENTS.md → src/Core/Artix.API.Core.Domain/Entities/Museum/Museum.cs

## Import Cycles
- None detected.

## Communities (195 total, 37 thin omitted)

### Community 0 - "primitives"
Cohesion: 0.04
Nodes (28): Artix.API.Core.ApplicationService.Features.Museums.Client.Queries.GetObjects, Artix.API.Core.ApplicationService.Features.Users.Client.Queries.GetUserProfile, Artix.API.Core.ApplicationService.Features.Users.Admin.Queries.GetUserProfile, Artix.API.Core.ApplicationService.Features.Objects.Client.Commands.AddToUserCollection, Artix.API.Core.ApplicationService.Features.Museums.Client.Queries.GetDetailsById, Artix.API.Core.ApplicationService.Features.Museums.Admin.Queries.GetObjects, Artix.API.Core.ApplicationService.Features.Objects.Client.Queries.GetPaginateObjects, Artix.API.Infra.Mongo.Repositories.Features.Quiz (+20 more)

### Community 1 - "ArtixCommandDbContext"
Cohesion: 0.05
Nodes (36): ArtixCommandDbContext, AppRoles, AppUsers, AppVersions, CollectionItems, Collections, Files, Friendships (+28 more)

### Community 2 - "CancellationToken"
Cohesion: 0.07
Nodes (3): IUserSessionService, IAnomalyDetectionService, MongoCommandRepository

### Community 3 - "MigrationBuilder"
Cohesion: 0.10
Nodes (8): Init, OutboxMessageStatusToEnumType, AddFileForObjectInformations, ModifyObjectTable, ModifyQrCodeFieldInObjectTable, FixUserTables, update_user_session, ArtixCommandDbContextModelSnapshot

### Community 4 - "Guid"
Cohesion: 0.05
Nodes (7): CreateNewMuseumCommandHandler, IFileStorage, IUploadRepository, IUploadService, ICommandRepository, FileSystemStorage, InMemoryUploadRepository

### Community 6 - "microsoft_extensions_logging"
Cohesion: 0.11
Nodes (8): Artix.API.Infra.Redis.Services, Artix.API.Infra.Mongo.Data.Seed, Artix.API.Infra.Redis, Artix.API.Infra.Identity.Services.SessionService, IRedisConnectionFactory, Connection, RedisConnectionFactory, Connection

### Community 7 - "UserSession"
Cohesion: 0.10
Nodes (19): UserSession, DeviceId, Fingerprint, IpAddress, IsActive, JwtId, LastRefreshAt, LastSeenAt (+11 more)

### Community 8 - "Repository"
Cohesion: 0.06
Nodes (16): OutboxMessage, CreatedAt, Data, Error, Id, ProcessedAt, RetryCount, Status (+8 more)

### Community 9 - "EntityTypeBuilder"
Cohesion: 0.08
Nodes (20): BaseEntityConfiguration, CollectionReadConfiguration, MuseumReadConfiguration, NotificationReadConfiguration, UserNotificationReadConfiguration, TypeReadConfiguration, OtpReadConfiguration, SeasonReadConfiguration (+12 more)

### Community 10 - "AppUser"
Cohesion: 0.06
Nodes (26): AppUser, BanExpiration, BanReason, BusinessId, Collections, CreatedAt, DisplayName, FirstName (+18 more)

### Community 11 - "common"
Cohesion: 0.06
Nodes (5): Artix.API.Core.Domain.Entities.TierConfig, Artix.API.Core.Domain.Entities.OTP, Artix.API.Core.Domain.Entities.Season, Artix.API.Core.Domain.Entities.Collection, Artix.API.Core.Domain.Persistence

### Community 12 - "AnomalyDetectionService"
Cohesion: 0.09
Nodes (6): AnomalyDetectionService, LocationData, City, Country, Latitude, Longitude

### Community 13 - "microsoft_entityframeworkcore"
Cohesion: 0.16
Nodes (4): Artix.API.Core.Domain.Entities.User, Artix.API.Infra.Sql.Data.Config.Read.User, Artix.API.Infra.Sql.Data.Config.Write, Artix.API.Infra.Sql.Data.Config.Write.User

### Community 14 - "dbcontexts"
Cohesion: 0.13
Nodes (5): Artix.API.Infra.Sql.Data.Config.Write.OutboxMessages, Artix.API.Infra.Sql.Data.Seed, Artix.API.Infra.Sql.Data.Interceptors, Artix.API.Infra.Sql.Data, SqlDataRemover

### Community 15 - "Notification"
Cohesion: 0.05
Nodes (30): Artix.API.Core.Contract.Primitives.Infra.RabbitMQ, IEventPublisher, INotificationProducer, INotificationService, NotificationMessage, Notification, Body, CreatedAt (+22 more)

### Community 16 - "Artix.API.Core.Contract.Primitives.Infra.Redis"
Cohesion: 0.08
Nodes (8): Artix.API.Core.Contract.Primitives.Infra.Redis.Caches.Museums, Artix.API.Core.ApplicationService.Features.Objects.Client.Queries.GetObjectDetailsById, Artix.API.Core.ApplicationService.Features.Objects.Client.Queries.GetUserRecentObjectsVisits, Artix.API.Core.Contract.Primitives.Infra.Redis, Artix.API.Core.Contract.Features.Caches.Objects, Artix.API.Core.ApplicationService.Features.Museums.Client.Queries.GetUserRecentMuseumsVisits, IDistributedLockService, IEventDeduplicationStore

### Community 17 - "ArtixCommandDbContext.cs"
Cohesion: 0.05
Nodes (13): Artix.API.Infra.Sql.Repositories.Features.Tiers, Artix.API.Infra.Sql.Repositories.Features.Versions, Artix.API.Infra.Sql.Repositories.Features.Objects, Artix.API.Infra.Sql.Repositories.Features.OTPs, Artix.API.Infra.Sql.Repositories.Features.Notifications, Artix.API.Infra.Sql.Repositories.Features.Collections, CollectionCommandRepository, CollectionQueryRepository (+5 more)

### Community 18 - "AuthorizeCheckOperationFilter.cs"
Cohesion: 0.14
Nodes (3): Artix.API.Endpoints.Filters, AuthorizeCheckOperationFilter, DomainEventExtensions

### Community 19 - "OtpService"
Cohesion: 0.10
Nodes (4): Artix.API.Core.Contract.Primitives.DomainServices.OTP, IOtpService, ISessionStore, OtpService

### Community 20 - "primitives_handlers"
Cohesion: 0.08
Nodes (21): Artix.API.Core.Contract.Features.Objects.Client.Commands.AddToUserCollection, Artix.API.Core.Contract.Features.Objects.Admin.Commands.Remove, Artix.API.Core.Contract.Features.Users.Client.Commands.DeleteProfile, Artix.API.Core.Contract.Features.Objects.Admin.Commands.Upgrade, Artix.API.Core.Contract.Features.Users.Client.Commands.InitiateOTPAuth, Artix.API.Core.Contract.Features.Users.Admin.Commands.Register, Artix.API.Core.Contract.Features.Objects.Client.Commands.Scan, Artix.API.Core.Contract.Features.Museums.Admin.Commands.CreateNew (+13 more)

### Community 21 - "Task"
Cohesion: 0.10
Nodes (3): IBackgroundJobScheduler, IBruteForceProtectionService, RedisRateLimiter

### Community 22 - "Object"
Cohesion: 0.06
Nodes (20): Object, Description, IsHidden, IsSpecial, JournalEntries, MarketplaceItems, MuseumObjects, Name (+12 more)

### Community 24 - "microsoft_aspnetcore_http"
Cohesion: 0.07
Nodes (8): Artix.API.Infra.Identity.Services.TokenService, Artix.API.Endpoints.Middlewares, Artix.API.Core.ApplicationService.Features.Users.Admin.Commands.Register, Artix.API.Endpoints.Attributes, Artix.API.Infra.Identity, Artix.API.Core.ApplicationService.Primitives, Artix.API.Infra.Identity.Services, Artix.API.Infra.Identity.Services.TokenProvider

### Community 25 - ".AddRabbitMqService"
Cohesion: 0.07
Nodes (9): ICompressionWorker, ILockScope, Acquired, NotificationProducer, NotificationService, RabbitMqEventPublisher, RedisLockScope, Acquired (+1 more)

### Community 26 - "Artix.API.Core.Domain.Entities.Notification.Enums"
Cohesion: 0.08
Nodes (22): Artix.API.Core.Domain.Entities.Notification, Artix.API.Infra.Sql.Data.Config.Read.Notification, Artix.API.Infra.Sql.Data.Config.Write.Notification, Artix.API.Core.Domain.Entities.Notification.Enums, DeliveryStatus, Delivered, Failed, Pending (+14 more)

### Community 27 - "Museum"
Cohesion: 0.09
Nodes (13): Museum, Description, IsActive, MuseumImages, MuseumObjects, Name, Slug, OTP (+5 more)

### Community 28 - "IRedisConnectionFactory"
Cohesion: 0.07
Nodes (7): IMessageRelayService, RedisCacheRepository, RedisEventDeduplicationStore, RedisJobQueueService, RedisMessageRelayService, RedisSessionStore, RedisTokenRevocationStore

### Community 29 - ".Handle"
Cohesion: 0.12
Nodes (9): Artix.API.Core.Contract.Features.Museums.Client.Queries.GetUserRecentVisits, Artix.API.Core.ApplicationService.CacheKeys, CacheKeys, GetMuseumDetailsByIdQueryHandler, GetUserRecentMuseumsVisitsQueryHandler, GetUserRecentObjectsVisitsQueryHandler, ClientUserRecentMuseumsVisitDto, GetClientUserRecentMuseumsVisitQuery (+1 more)

### Community 31 - "AppUserBuilder"
Cohesion: 0.26
Nodes (3): ModifyProfileHandler, ClientModifyProfileCommand, AppUserBuilder

### Community 32 - "IDomainEvent"
Cohesion: 0.12
Nodes (9): Artix.API.Core.Domain.Entities.Museum.Events, Artix.API.Core.Domain.Entities.Object.Events, MuseumCreatedEvent, FirstUserScanEvent, RepeatUserScanEvent, QuizCreatedEvent, QuestId, DomainEventNotification (+1 more)

### Community 33 - "ClientPaginateObjectsDto"
Cohesion: 0.16
Nodes (11): Artix.API.Core.Domain.Entities.Object.ValueObjects, GetObjectDetailsByIdQueryHandler, GetPaginateObjectsQueryHandler, ClientPaginateObjectsDto, HistoricalPeriodDto, TypeDto, GetClientPaginateObjectsQuery, HistoricalDate (+3 more)

### Community 35 - "CreateNewObjectCommandHandler.cs"
Cohesion: 0.11
Nodes (8): Artix.API.Core.ApplicationService.Features.Museums.Admin.Commands.CreateNewMuseum, Artix.API.Core.Contract.Features.Users.Client.Commands.ModifyProfile, Artix.API.Core.ApplicationService.Features.Objects.Admin.Commands.Upgrade, Artix.API.Core.ApplicationService.Features.Objects.Admin.Commands.CreateNewObject, Artix.API.Core.Contract.Features.Files.Commands, Artix.API.Core.ApplicationService.Features.Users.Client.Commands.ModifyProfile, Artix.API.Core.Contract.Primitives.Infra.File, IFileCommandRepository

### Community 36 - "IEntityTypeConfiguration"
Cohesion: 0.10
Nodes (14): ObjectType, Category, Object, ObjectId, TypeId, MuseumObjectCategoryReadConfiguration, ObjectTypeReadConfiguration, AppUserTokenReadConfiguration (+6 more)

### Community 37 - "GetLoginQueryHandler.cs"
Cohesion: 0.12
Nodes (10): Artix.API.Core.Contract.Primitives.Infra.Identity.Authentication.Admin.Login, Artix.API.Core.ApplicationService.Features.Users.Admin.Queries.GetLogin, Artix.API.Core.Contract.Features.Users.Admin.Queries.GetLogin, Artix.API.Core.Contract.Primitives.Infra.Identity.Authentication, GetLoginQueryHandler, AdminLoginDto, GetAdminLoginQuery, AdminLoginRequest (+2 more)

### Community 38 - "IActionResult"
Cohesion: 0.06
Nodes (22): AddObjectToCollectionCommandHandler, RemoveMuseumCommandHandler, CreateNewObjectCommandHandler, RemoveObjectCommandHandler, UpgradeObjectCommandHandler, AddObjectToUserCollectionCommandHandler, ScanObjectCommandHandler, InitiateOTPAuthHandler (+14 more)

### Community 39 - "Artix.API.Endpoints/DependencyInjection.cs"
Cohesion: 0.06
Nodes (12): Artix.API.Infra.Sql.Primitives, Artix.API.Infra.Mongo.Primitives, Artix.API.Infra.Sql.Repositories, Artix.API.Core.Contract.Primitives.Repositories, Artix.API.Infra.Mongo.Data.DbContext, Artix.API.Infra.Mongo, Artix.API.Infra.Mongo.Data.Interceptors, Artix.API.Infra.Sql (+4 more)

### Community 40 - "XpRulesServiceTests"
Cohesion: 0.22
Nodes (3): Artix.API.Core.Contract.Primitives.DomainServices.XPRules, IXpRulesService, XpRulesServiceTests

### Community 42 - ".HandleEventAsync"
Cohesion: 0.22
Nodes (3): Artix.API.Core.Contract.Primitives.DomainServices.Notification, AddUserNotificationCommand, INotificationServiceProvider

### Community 43 - "XpRulesService"
Cohesion: 0.12
Nodes (5): IFeatureToggleService, ILeaderboardService, XpRulesService, RedisFeatureToggleService, RedisLeaderboardService

### Community 44 - "IEnumerable"
Cohesion: 0.22
Nodes (4): GetAllMuseumsQueryHandler, GetMuseumJournalEntriesQueryHandler, GetMuseumObjectsQueryHandler, MuseumQueryRepository

### Community 45 - "FileEntity"
Cohesion: 0.10
Nodes (15): FileEntity, FileName, FilePath, FileSize, MimeType, MuseumImages, ObjectGeneralInformation, ObjectImages (+7 more)

### Community 46 - "Quiz"
Cohesion: 0.13
Nodes (13): Quiz, BonusXP, Deadline, Description, IsSeasonal, Priority, RelatedFeature, RelatedObjectIds (+5 more)

### Community 47 - "ClientPaginateLoginHistoryDto"
Cohesion: 0.10
Nodes (14): Artix.API.Core.Contract.Features.Users.Client.Queries.GetPaginateLoginHistories, Artix.API.Core.Contract.Features.Users.Admin.Queries.GetPaginateUsers, Artix.API.Core.Contract.Features.Objects.Client.Queries.GetPaginateObjects, Artix.API.Core.Contract.Features.Museums.Admin.Queries.GetPaginateObjects, AdminMuseumObjectDto, GetAdminMuseumObjectsQuery, AdminPaginateUsersDto, GetAdminPaginateUsersQuery (+6 more)

### Community 48 - ".Handle"
Cohesion: 0.21
Nodes (4): Artix.API.Core.Contract.Features.Collections.Client.Queries.GetUserCollection, GetAllCollectionsByUserIdQueryHandler, GetUserCollectionsQuery, UserCollectionDto

### Community 50 - "Object"
Cohesion: 0.09
Nodes (16): Collection, Description, IsPublic, Items, Name, User, UserId, CollectionItem (+8 more)

### Community 51 - "مدل‌های اصلی"
Cohesion: 0.08
Nodes (23): مدل‌های اصلی, ObjectSaleType, Free, MemberShip, Tokenized, QuizObject, Object, ObjectId (+15 more)

### Community 52 - "OtpService.cs"
Cohesion: 0.06
Nodes (9): Artix.API.Infra.FileService, Artix.API.Core.Contract.Features.Notifications.Commands.AddUserNotification, Artix.API.Core.ApplicationService.Features.Users.Client.Commands.InitiateOTPAuth, Artix.API.Core.ApplicationService.Features.Users.Client.Queries.GetVerifyOTPAuth, Artix.API.Core.DomainService, Artix.API.Core.ApplicationService.Features.Objects.Client.Events, Artix.API.Core.DomainService.Services, DependencyInjection (+1 more)

### Community 53 - "QueryHandlerBase"
Cohesion: 0.19
Nodes (4): GetMuseumObjectsQueryHandler, GetUserProfileQueryHandler, GetPaginateLoginHistoryQueryHandler, IQueryHandler

### Community 54 - "ILogger"
Cohesion: 0.11
Nodes (5): NotificationConsumerHostedService, NotificationHub, NotificationOutboxProcessor, OutboxProcessor, RabbitMqConnectionFactory

### Community 55 - "UserXp"
Cohesion: 0.18
Nodes (7): UserXp, LastUpdated, TotalXp, User, UserId, UserXpReadConfiguration, UserXpWriteConfiguration

### Community 57 - "microsoft_entityframeworkcore_metadata_builders"
Cohesion: 0.11
Nodes (8): Artix.API.Infra.Sql.Data.Config.Write.TierConfig, Artix.API.Core.Domain.Entities.Object, Artix.API.Infra.Sql.Data.Config.Write.Object, Artix.API.Infra.Sql.Data.Config.Write.Collection, Artix.API.Infra.Sql.Data.Config.Read.TierConfig, Artix.API.Infra.Sql.Data.Config.Read.Collection, Artix.API.Infra.Sql.Data.Config.Read.Object, Artix.API.Infra.Sql.Data.Config

### Community 58 - ".Handle"
Cohesion: 0.13
Nodes (8): Artix.API.Core.Contract.Primitives.Infra.Identity.Authentication.Admin.Logout, Artix.API.Core.Contract.Primitives.Infra.Identity.Authentication.Client.Logout, GetLogoutQueryHandler, GetLogoutQueryHandler, AdminLogoutRequest, AdminLogoutResponse, ClientLogoutRequest, ClientLogoutResponse

### Community 62 - "Artix.API.Core.Domain.Entities.Museum"
Cohesion: 0.17
Nodes (3): Artix.API.Infra.Sql.Data.Config.Write.Museum, Artix.API.Core.Domain.Entities.Museum, Artix.API.Infra.Sql.Data.Config.Read.Museum

### Community 63 - "UserQuiz"
Cohesion: 0.12
Nodes (9): Artix.API.Core.Domain.Entities.Quiz, UserQuiz, CompletedAt, Priority, Progress, QuestId, Status, User (+1 more)

### Community 64 - "FileEntity"
Cohesion: 0.10
Nodes (11): ObjectGeneralInformation, FileEntity, FileId, Object, ObjectId, ObjectSpecialInformation, FileEntity, FileId (+3 more)

### Community 65 - "Result"
Cohesion: 0.17
Nodes (6): GetReNewAccessTokenQueryHandler, GetUserProfileQueryHandler, Result, Data, ErrorMessage, IsSuccess

### Community 66 - "Artix.API.Core.Domain.Entities.Voice"
Cohesion: 0.32
Nodes (3): Artix.API.Infra.Sql.Data.Config.Read.Voice, Artix.API.Infra.Sql.Data.Config.Write.Voice, Artix.API.Core.Domain.Entities.Voice

### Community 67 - "HistoricalPeriod"
Cohesion: 0.16
Nodes (10): HistoricalPeriod, Description, EndDate, Name, ObjectHistoricalPeriods, StartDate, HistoricalDateConverter, HistoricalPeriodReadConfiguration (+2 more)

### Community 68 - "UserScan"
Cohesion: 0.13
Nodes (12): Tier و concession, اسکن و XP, رفتار پیاده‌شده و قواعد دامنه, UserScan, AcquiredAt, InCollection, IsUpgraded, Object (+4 more)

### Community 69 - ".AddArtixServices"
Cohesion: 0.13
Nodes (4): Artix.API.Core.Contract.Primitives.CircuitBreaker, PollyPolicies, CustomerApiClient, HostingExtension

### Community 71 - "MuseumQueryRepository.cs"
Cohesion: 0.09
Nodes (4): Artix.API.Infra.RabbitMQ.Services, Artix.API.Infra.FileService.Services, Artix.API.Infra.Sql.Repositories.Features.Museums, Artix.API.Infra.Sql.Repositories.Features.Files

### Community 72 - "GetVerifyOTPAuthQuery.cs"
Cohesion: 0.33
Nodes (4): Artix.API.Core.Contract.Features.Users.Client.Queries.GetVerifyOTPAuth, GetVerifyOTPAuthQuery, OtpSessionData, VerifyOTPAuthDto

### Community 73 - "AppVersion"
Cohesion: 0.14
Nodes (10): AppVersion, Description, IsRequired, Major, Minor, MinSupported, Patch, VersionString (+2 more)

### Community 75 - "UploadSession"
Cohesion: 0.12
Nodes (13): UploadSession, ChunkSize, Completed, CreatedAt, FileName, Id, IsCompressed, PhysicalFilePath (+5 more)

### Community 76 - "IntegrationError"
Cohesion: 0.20
Nodes (8): HttpIntegrationException, Error, HttpResponseMessageExtensions, IntegrationError, RawContent, ReasonPhrase, StatusCode, Url

### Community 78 - "Artix.API.Core.DomainService.csproj"
Cohesion: 0.13
Nodes (15): Microsoft.AspNetCore.Authentication.JwtBearer, net9.0, Microsoft.NET.Sdk, net9.0, StackExchange.Redis, Microsoft.NET.Sdk, src\Infra\Artix.API.Infra.File\Artix.API.Infra.File, net9.0 (+7 more)

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
Cohesion: 0.25
Nodes (6): GetPaginateMuseumsQueryHandler, GetPaginatedUsersQueryHandler, PaginatedResult, HasNextPage, HasPreviousPage, TotalPages

### Community 85 - ".AddElasticsearch"
Cohesion: 0.16
Nodes (7): Artix.API.WebService.Extensions, ElasticsearchExtensions, ElasticsearchStatus, Index, IsValid, Settings, Uri

### Community 87 - "Artix.API.WebService.csproj"
Cohesion: 0.12
Nodes (15): Elastic.Serilog.Sinks, Elasticsearch.Net, Microsoft.Extensions.Http.Polly, NEST, Serilog, Serilog.AspNetCore, Serilog.Settings.Configuration, Serilog.Sinks.Console (+7 more)

### Community 88 - "TierConfig"
Cohesion: 0.14
Nodes (13): TierConfig, MinDaysSinceAcquired, MinScanCount, Multiplier, Priority, RequiredActiveStreak, RequiredCoOpKey, RequiredInCollection (+5 more)

### Community 89 - "AppUser"
Cohesion: 0.08
Nodes (5): CommandHandlerBase, QueryHandlerBase, AuthenticationService, JwtTokenGenerator, TokenService

### Community 90 - "GetAllTiersQuery"
Cohesion: 0.22
Nodes (6): Artix.API.Core.Contract.Features.Objects.Admin.Commands.CreateNewObject, Artix.API.Core.Domain.Entities.Object.Enums, Artix.API.Core.Contract.Features.Tiers.Client.Queries.GetAll, AdminCreateNewObjectCommand, AllTierDto, GetAllTiersQuery

### Community 92 - "Category"
Cohesion: 0.14
Nodes (7): Category, Description, Name, ObjectTypes, CategoryReadConfiguration, CategoryWriteConfiguration, TypeWriteConfiguration

### Community 93 - "UserSeasonProgress"
Cohesion: 0.16
Nodes (9): UserSeasonProgress, LastUpdated, Season, SeasonId, TotalXp, User, UserId, UserSeasonProgressReadConfiguration (+1 more)

### Community 96 - "Fact"
Cohesion: 0.15
Nodes (6): Artix.API.Tests.Unit, Artix.API.Tests.Integration, Artix.API.Tests.EndToEnd, UnitTest1, UnitTest1, UnitTest1

### Community 97 - "List"
Cohesion: 0.09
Nodes (16): Artix.API.Core.Contract.Features.Objects.Admin.Queries.GetObjectDetailsById, Artix.API.Core.Contract.Features.Objects.Client.Queries.GetObjectDetailsById, Artix.API.Core.Contract.Features.Objects.Client.Queries.GetUserRecentObjectsVisits, AdminObjectDetailsByIdDto, GetAdminObjectDetailsByIdQuery, ClientObjectDetailsByIdDto, GetClientObjectDetailsByIdQuery, ClientUserRecentObjectsVisitDto (+8 more)

### Community 98 - "OutboxProcessor.cs"
Cohesion: 0.15
Nodes (4): Artix.API.Infra.RabbitMQ, Artix.API.Infra.RabbitMQ.Services.Notification, Artix.API.Infra.RabbitMQ.Services.Outbox, DependencyInjection

### Community 99 - "FileSettings"
Cohesion: 0.15
Nodes (12): Artix.API.Core.Contract.Configs.FileSettings, FileSettings, Allowed3DMimeTypes, AllowedImageMimeTypes, AllowedReadmeMimeTypes, AllowGzipForOtherFiles, BaseUrl, ExtensionsToSkip (+4 more)

### Community 101 - "primitives_repositories"
Cohesion: 0.05
Nodes (17): Artix.API.Core.Contract.Features.Notifications.Commands, Artix.API.Core.Contract.Features.Versions.Commands, Artix.API.Core.Contract.Features.OTPs.Queries, Artix.API.Core.Contract.Features.Collections, Artix.API.Core.Contract.Features.OTPs.Commands, Artix.API.Core.Contract.Features.Quizzes, Artix.API.Core.Contract.Features.Museums, Artix.API.Core.Contract.Features.Objects (+9 more)

### Community 102 - "VerifyOTPResult"
Cohesion: 0.40
Nodes (3): Artix.API.Core.Contract.Primitives.DomainServices.OTP.Verify, VerifyOTPRequest, VerifyOTPResult

### Community 103 - "UserJournalEntry"
Cohesion: 0.21
Nodes (8): UserJournalEntry, JournalEntry, JournalEntryId, UnlockedAt, User, UserId, UserJournalEntryReadConfiguration, UserJournalEntryWriteConfiguration

### Community 104 - "Artix.API.Core.Contract.csproj"
Cohesion: 0.20
Nodes (9): FluentValidation, FluentValidation.DependencyInjectionExtensions, MediatR, Scrutor, net9.0, Polly, Polly.Extensions.Http, StackExchange.Redis (+1 more)

### Community 105 - "JournalEntry"
Cohesion: 0.14
Nodes (11): AggregateRoot, DomainEvents, JournalEntry, Notes, Object, ObjectId, SketchUrl, Title (+3 more)

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

### Community 112 - "UserSessionService"
Cohesion: 0.14
Nodes (3): ITokenRevocationStore, UserSessionService, CommandRepository

### Community 113 - "Artix.API.Utils"
Cohesion: 0.22
Nodes (4): Artix.API.Utils, DependencyInjection, FileNameHelper, ListExtensions

### Community 114 - "ClientType"
Cohesion: 0.18
Nodes (9): Artix.API.Core.Domain.Entities.User.Enums, ClientType, Emerald, Pro, Ruby, Turquoise, Role, Admin (+1 more)

### Community 115 - "IQuery"
Cohesion: 0.12
Nodes (11): Artix.API.Core.Contract.Features.Users.Admin.Queries.GetLogout, Artix.API.Core.Contract.Features.Users.Client.Queries.GetUserProfile, DPG.Core.Contract.Primitives.Models, AdminLogoutDto, GetAdminLogoutQuery, ClientUserProfileDto, GetClientUserProfileQuery, PaginationQuery (+3 more)

### Community 117 - "AuthenticationSettings"
Cohesion: 0.20
Nodes (9): Artix.API.Core.Contract.Configs.Authentication, AuthenticationSettings, AccessTokenExpireSeconds, Audience, Issuer, IssuerSigningKey, RefreshTokenExpireDays, RefreshTokenLength (+1 more)

### Community 118 - "IQuery.cs"
Cohesion: 0.24
Nodes (5): Artix.API.Core.Contract.Primitives.Handlers, ICommand, ICommandHandler, IQuery, IResponse

### Community 120 - "Artix.API.Infra.Sql.csproj"
Cohesion: 0.17
Nodes (11): EFCore.BulkExtensions, Microsoft.EntityFrameworkCore.Design, Microsoft.EntityFrameworkCore.Proxies, Microsoft.EntityFrameworkCore.SqlServer, Microsoft.Extensions.Configuration.EnvironmentVariables, Newtonsoft.Json, net9.0, Microsoft.AspNetCore.Identity.EntityFrameworkCore (+3 more)

### Community 121 - "Artix.API.Tests.Integration.csproj"
Cohesion: 0.17
Nodes (11): Microsoft.EntityFrameworkCore.InMemory, Moq, NUnit, Testcontainers.MsSql, Testcontainers.Redis, net9.0, coverlet.collector, Microsoft.NET.Test.Sdk (+3 more)

### Community 122 - "CompressionWorker"
Cohesion: 0.17
Nodes (3): IFileCompressor, CompressionWorker, FileCompressor

### Community 123 - "RateLimitAttribute"
Cohesion: 0.25
Nodes (6): RateLimitAttribute, KeyPrefix, Limit, UseClientIp, UseUserIdentifier, WindowSeconds

### Community 126 - "Artix.API.Core.ApplicationService/DependencyInjection.cs"
Cohesion: 0.20
Nodes (4): Artix.API.Core.Contract.Primitives.Validations, Artix.API.Core.ApplicationService, DependencyInjection, FluentValidationBehavior

### Community 127 - "QuizCompletedEvent"
Cohesion: 0.25
Nodes (7): Artix.API.Core.Domain.DomainEvents, Artix.API.Core.Domain.Entities.Quiz.Events, IDomainEvent, QuizCompletedEvent, QuestId, UserId, XPReward

### Community 129 - "AnomalyDetectionResult"
Cohesion: 0.22
Nodes (6): AnomalyDetectionResult, AnomalyScore, DetectedAnomalies, IsAnomalous, Metadata, RequiredAction

### Community 130 - "VoiceTrackFile"
Cohesion: 0.28
Nodes (7): VoiceTrackFile, FileEntity, FileId, VoiceTrack, VoiceTrackId, VoiceTrackFileReadConfiguration, VoiceTrackFileWriteConfiguration

### Community 131 - "mediatr"
Cohesion: 0.18
Nodes (5): Artix.API.Endpoints.Controllers.Common, Artix.API.Endpoints.Controllers.Shared, AdminBaseController, ClientBaseController, SharedController

### Community 132 - "UserStrike"
Cohesion: 0.14
Nodes (10): UserStrike, FuelCount, IsActive, LastInteraction, StrikeCount, StrikeStart, User, UserId (+2 more)

### Community 134 - "ApplicationServiceNotFoundException"
Cohesion: 0.25
Nodes (3): Artix.API.Core.ApplicationService.Exceptions, ApplicationServiceNotFoundException, TooManyRequestsException

### Community 135 - "Artix.API.Infra.RabbitMQ.csproj"
Cohesion: 0.33
Nodes (5): Microsoft.AspNetCore.SignalR, Microsoft.Extensions.Hosting.Abstractions, RabbitMQ.Client, net9.0, Microsoft.NET.Sdk

### Community 137 - "Friendship"
Cohesion: 0.16
Nodes (8): Friendship, CreatedAt, Friend, FriendId, User, UserId, FriendshipReadConfiguration, FriendshipWriteConfiguration

### Community 139 - "ElasticsearchSettings"
Cohesion: 0.25
Nodes (7): Artix.API.Core.Contract.Configs.Elasticsearch, ElasticsearchSettings, IndexFormat, Password, RequestTimeoutInMinutes, Uri, Username

### Community 140 - ".AddMongoServices"
Cohesion: 0.15
Nodes (5): Artix.API.Core.Contract.Configs.Mongo, MongoDbSettings, ConnectionString, DatabaseName, MongoQueryRepository

### Community 142 - "AnomalyScore"
Cohesion: 0.25
Nodes (7): AnomalyScore, BehaviorTrust, DeviceTrust, LocationTrust, OverallScore, RiskLevel, TimeTrust

### Community 143 - "IMuseumQueryRepository"
Cohesion: 0.18
Nodes (4): IMuseumQueryRepository, IQuizQueryRepository, MuseumQueries, QueryRepository

### Community 144 - "DateTime"
Cohesion: 0.18
Nodes (4): LastLocationData, Ip, Timestamp, FixUserSession

### Community 145 - "ApiKeyAuthenticationMiddleware"
Cohesion: 0.15
Nodes (3): DependencyInjection, ApiKeyAuthenticationMiddleware, RateLimitingMiddleware

### Community 146 - "QuizStatus"
Cohesion: 0.29
Nodes (6): Artix.API.Core.Domain.Entities.Quest.Enums, QuizStatus, Completed, Failed, InProgress, Pending

### Community 147 - "RabbitMqOptions"
Cohesion: 0.29
Nodes (6): Artix.API.Core.Contract.Configs.RabbitMQ, RabbitMqOptions, Host, Password, Port, Username

### Community 148 - "MongoTimestampInterceptor"
Cohesion: 0.18
Nodes (5): Counter, Id, SequenceValue, IMongoInterceptor, MongoTimestampInterceptor

### Community 149 - ".Handle"
Cohesion: 0.48
Nodes (4): Artix.API.Core.Contract.Features.Versions.Queries.GetLast, GetLastVersionQueryHandler, GetLastVersionQuery, LastVersionDto

### Community 150 - "OutboxMessageStatus"
Cohesion: 0.29
Nodes (6): Artix.API.Core.Domain.Persistence.Enums, OutboxMessageStatus, Dead, Failed, Pending, Processed

### Community 151 - "AppRole"
Cohesion: 0.47
Nodes (3): AppRole, AppRoleReadConfiguration, AppRoleWriteConfiguration

### Community 152 - "TierConfig"
Cohesion: 0.24
Nodes (4): Artix.API.Core.Contract.Features.Tiers, ITierQueryRepository, TierConfigReadConfiguration, TierConfigWriteConfiguration

### Community 153 - "VoiceTrack"
Cohesion: 0.18
Nodes (11): VoiceTrack, Artist, IsFree, Object, ObjectId, Season, SeasonId, Title (+3 more)

### Community 154 - "JwtTokenResult"
Cohesion: 0.13
Nodes (10): Artix.API.Core.Contract.Primitives.Infra.Identity, IDeviceFingerprintService, ITokenService, JwtTokenResult, AccessToken, AccessTokenExpiresAt, Jti, RefreshToken (+2 more)

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

### Community 160 - "BaseEntity.cs"
Cohesion: 0.20
Nodes (4): Artix.API.Core.Domain.Entities.Version, Artix.API.Infra.Sql.Data.Config.Write.Version, Artix.API.Infra.Sql.Data.Config.Read.Version, Artix.API.Core.Domain.Entities.Common

### Community 161 - "RedisOptions"
Cohesion: 0.33
Nodes (5): Artix.API.Core.Contract.Configs.Redis, RedisOptions, Host, Password, Port

### Community 162 - "QuizAction"
Cohesion: 0.33
Nodes (5): Artix.API.Core.Domain.Entities.Quiz.ValueObjects, QuizAction, ActionType, Details, RequiredCount

### Community 163 - "UserProfileDto"
Cohesion: 0.47
Nodes (4): Artix.API.Core.Contract.Features.Users.Admin.Queries.GetUserProfile, GetUserProfileQuery, UserClaim, UserProfileDto

### Community 164 - "ObjectHistoricalPeriod"
Cohesion: 0.24
Nodes (7): ObjectHistoricalPeriod, HistoricalPeriod, HistoricalPeriodId, Object, ObjectId, ObjectHistoricalPeriodReadConfiguration, ObjectHistoricalPeriodWriteConfiguration

### Community 165 - "RecentMuseumDto"
Cohesion: 0.22
Nodes (7): Artix.API.Core.Contract.Primitives.Infra.Redis.Caches, RecentMuseumDto, ImageUrl, Name, ObjectCount, RecentBaseEntity, Id

### Community 166 - "Artix.API.Tests.EndToEnd.csproj"
Cohesion: 0.25
Nodes (7): RestSharp, net9.0, coverlet.collector, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio, Microsoft.NET.Sdk

### Community 167 - "BruteForceCheckResult"
Cohesion: 0.22
Nodes (6): BruteForceCheckResult, IsAllowed, LockoutUntil, RemainingAttempts, SuggestedAction, TotalAttempts

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

### Community 176 - "Artix.API.Core.Domain.Entities.File"
Cohesion: 0.40
Nodes (3): Artix.API.Infra.Sql.Data.Config.Write.File, Artix.API.Infra.Sql.Data.Config.Read.File, Artix.API.Core.Domain.Entities.File

### Community 177 - "GetLatestOTPByPhoneNumberQuery"
Cohesion: 0.50
Nodes (3): Artix.API.Core.Contract.Features.OTPs.Queries.GetLatestByPhoneNumber, GetLatestOTPByPhoneNumberQuery, LatestOTPByPhoneNumberDto

### Community 178 - "GetMuseumJournalEntriesQuery"
Cohesion: 0.50
Nodes (3): Artix.API.Core.Contract.Features.Museums.Client.Queries.GetJournalEntries, GetMuseumJournalEntriesQuery, MuseumJournalEntryDto

### Community 179 - "GetMuseumKeyStatusQuery"
Cohesion: 0.50
Nodes (3): Artix.API.Core.Contract.Features.Museums.Client.Queries.GetKeyStatus, GetMuseumKeyStatusQuery, MuseumKeyStatusDto

### Community 180 - "GetCollectionsByUserIdQuery"
Cohesion: 0.50
Nodes (3): Artix.API.Core.Contract.Features.Collections.Client.Queries.GetCollectionByUserId, CollectionsByUserIdDto, GetCollectionsByUserIdQuery

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
Cohesion: 0.29
Nodes (6): net9.0, coverlet.collector, Microsoft.NET.Test.Sdk, xunit, xunit.runner.visualstudio, Microsoft.NET.Sdk

### Community 186 - "Artix.API.Endpoints.csproj"
Cohesion: 0.33
Nodes (5): Swashbuckle.AspNetCore.SwaggerGen, net9.0, Microsoft.EntityFrameworkCore, Microsoft.EntityFrameworkCore.Tools, Microsoft.NET.Sdk

### Community 187 - "BaseEntity"
Cohesion: 0.33
Nodes (6): BaseEntity, BusinessId, CreatedAt, Id, IsDeleted, ModifiedAt

### Community 188 - "AdminPaginatedMuseumsDto"
Cohesion: 0.50
Nodes (3): Artix.API.Core.Contract.Features.Museums.Admin.Queries.GetPaginateMuseums, AdminPaginatedMuseumsDto, GetAdminPaginateMuseumsQuery

### Community 191 - "Artix.API.Core.Contract.Primitives.DomainServices.OTP.Init"
Cohesion: 0.40
Nodes (3): Artix.API.Core.Contract.Primitives.DomainServices.OTP.Init, InitOTPRequest, InitOTPResult

### Community 192 - "JournalEntryReadConfiguration.cs"
Cohesion: 0.40
Nodes (3): Artix.API.Core.Domain.Entities.JournalEntry, Artix.API.Infra.Sql.Data.Config.Read.JournalEntry, Artix.API.Infra.Sql.Data.Config.Write.JournalEntry

## Knowledge Gaps
- **622 isolated node(s):** `net9.0`, `Microsoft.NET.Sdk`, `net9.0`, `Polly`, `Polly.Extensions.Http` (+617 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 1161 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **37 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `ArtixCommandDbContext` connect `ArtixCommandDbContext` to `MigrationBuilder`, `UserStrike`, `UserSession`, `Repository`, `EntityTypeBuilder`, `AppUser`, `Friendship`, `ArtixCommandDbContext.cs`, `AppRole`, `TierConfig`, `Museum`, `IEntityTypeConfiguration`, `.AddSqlServices`, `Artix API — Agent Guide`, `IEnumerable`, `FileEntity`, `Object`, `مدل‌های اصلی`, `UserXp`, `ApiVersionCheckMiddleware`, `FileEntity`, `HistoricalPeriod`, `UserScan`, `UserMuseumKey`, `ArtixQueryDbContext`, `UserSeasonProgress`, `primitives_repositories`, `UserJournalEntry`, `JournalEntry`, `MuseumImage`, `MuseumObject`, `MarketplaceItem`, `UserSessionService`?**
  _High betweenness centrality (0.087) - this node is a cross-community bridge._
- **What connects `net9.0`, `Microsoft.NET.Sdk`, `net9.0` to the rest of the system?**
  _622 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `primitives` be split into smaller, more focused modules?**
  _Cohesion score 0.03889270189887897 - nodes in this community are weakly interconnected._
- **Why does `Object` connect `Object` to `Guid`, `Client/ObjectController.cs`, `XpRulesServiceTests.cs`, `common`, `Museum`, `.IsMatch`, `ObjectBuilder`, `IDomainEvent`, `ObjectHistoricalPeriod`, `IEntityTypeConfiguration`, `XpRulesServiceTests`, `Object`, `FileEntity`, `TierConfig`, `Category`, `List`, `primitives_repositories`, `UserJournalEntry`, `JournalEntry`, `ObjectModel`, `ObjectImage`?**
  _High betweenness centrality (0.052) - this node is a cross-community bridge._
- **Should `ArtixCommandDbContext` be split into smaller, more focused modules?**
  _Cohesion score 0.047619047619047616 - nodes in this community are weakly interconnected._
- **Why does `AppUser` connect `AppUser` to `ArtixCommandDbContext`, `Guid`, `UserStrike`, `UserSession`, `EntityTypeBuilder`, `Friendship`, `common`, `DateTime`, `UserImage`, `AppUserBuilder`, `IDomainEvent`, `XpRulesServiceTests`, `مدل‌های اصلی`, `UserXp`, `UserScan`, `SqlDataSeeder`, `UserMuseumKey`, `UserSeasonProgress`, `List`, `UserJournalEntry`, `JournalEntry`, `MarketplaceItem`?**
  _High betweenness centrality (0.051) - this node is a cross-community bridge._
- **Should `CancellationToken` be split into smaller, more focused modules?**
  _Cohesion score 0.0660377358490566 - nodes in this community are weakly interconnected._
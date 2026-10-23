# Artix API — Security Vulnerability Backlog

تاریخ بررسی: 2026-10-23

این فهرست حاصل بازبینی استاتیک کل مسیرهای API، احراز هویت، فایل، ورودی، middleware و تنظیمات موجود در مخزن است. اولویت‌ها بر اساس اثر قابل‌انتظار و سهولت سوءاستفاده مرتب شده‌اند. مواردی که از روی کد قطعی هستند با **تأییدشده (Static)** مشخص شده‌اند؛ برای اثبات رفتاری در محیط اجرا هنوز تست end-to-end لازم است.

## وضعیت پس از اصلاحات اخیر

موارد زیر در کد فعلی پیاده‌سازی شده‌اند و باید با تست محیطی تأیید شوند:

| مورد | وضعیت | شواهد |
|---|---|---|
| RBAC و Role Claims استاندارد | اصلاح‌شده | `AdminPermission`, `AdminAuthorization`, `AspNetRoleClaims` |
| محدودسازی Museum Admin | اصلاح‌شده | claim `museum_scope` و فیلتر repository |
| Audit مدیریتی | اصلاح‌شده | `AdminAuditEntries` و migration `20261023131239_AddAdminAuditEntry` |
| گردش‌کار محتوای اثر | اصلاح‌شده | `ContentStatus` و migration `20261023135526_AddObjectContentWorkflow` |
| MFA برای عملیات حساس | کنترل پایه اضافه‌شده | claim `amr=mfa`; جریان صدور MFA هنوز لازم است |
| دسترسی اضطراری زمان‌دار | کنترل پایه اضافه‌شده | claim `emergency_until` |
| تست policyهای authorization | اضافه‌شده | `tests/Artix.API.Tests.Unit/Authorization` |

مواردی که هنوز **آسیب‌پذیری باز** هستند: OTP ثابت، ثبت‌نام عمومی Admin، آپلود عمومی و IDOR، refresh token plaintext/rotation، اعتماد به `X-Forwarded-For`، rate limit ناقص OTP، endpointهای خواندنی Admin، اعتبارسنجی ناکافی، security stamp غیرفعال و افشای خطای production. این موارد در بخش‌های C/H پایین‌تر باید قبل از انتشار بسته شوند.

## خلاصه اولویت

| اولویت | تعداد | وضعیت |
|---|---:|---|
| Critical | 5 | قبل از انتشار باید مسدود شوند |
| High | 10 | در اولین چرخه اصلاح امنیتی |
| Medium | 6 | پس از موارد بالا |
| نیازمند تست محیطی | 4 | الگوی پرریسک؛ اثر نهایی با اجرای امن تأیید شود |

---

## Critical

### C-01 — تصاحب حساب با OTP ثابت

- **CWE / شدت:** CWE-287, CWE-640 — Critical — **تأییدشده (Static)**
- **مکان:** `src/Core/Artix.API.Core.Domain/Entities/OTP/OTP.cs:37-42`
- **مسیر:** `POST /api/client/authentication/send-otp` سپس `POST /api/client/authentication/verify-otp`
- **شرح:** `GenerateOtpCode()` همیشه مقدار `123456` برمی‌گرداند. هر شخصی که شماره تلفن کاربر را بداند می‌تواند بدون دریافت SMS احراز هویت کند و JWT بگیرد.
- **بازسازی حداقلی:** برای شماره قربانی درخواست OTP بسازید؛ سپس همان شماره را با `{"otpCode":"123456"}` verify کنید.
- **اثر:** ورود/ثبت‌نام به نام هر شماره، صدور access و refresh token و دسترسی به داده و عملیات کاربر.
- **اصلاح:** تولید کد با `RandomNumberGenerator`/provider امن، ذخیره hash کد، TTL کوتاه، مصرف اتمیک، و rate limit مستقل برای هر شماره و IP. کد ثابت را از seed/test جدا کنید.

### C-02 — ثبت‌نام عمومی کاربر با نقش Admin

- **CWE / شدت:** CWE-269, CWE-862 — Critical — **تأییدشده (Static)**
- **مکان:** `src/Presentation/Artix.API.Endpoints/Controllers/AdminPanel/AuthenticationController.cs:20-27` و `src/Core/Artix.API.Core.ApplicationService/Features/Users/Admin/Commands/Register/RegisterCommandHandler.cs:25-70`
- **مسیر:** `POST /api/admin/authentication/register`
- **شرح:** endpoint هیچ `[Authorize]` یا سیاست bootstrap ندارد و handler هر درخواست را با نقش `Admin`، claimهای admin و مجوز write می‌سازد.
- **بازسازی حداقلی:** بدون توکن معتبر، نام کاربری و رمز قوی دلخواه را POST کنید؛ پاسخ موفق و کاربر Admin دریافت می‌شود.
- **اثر:** ایجاد اولین یا هر ادمین دلخواه و در اختیار گرفتن تمام عملیات مدیریتی.
- **اصلاح:** حذف endpoint از سطح عمومی؛ bootstrap یک‌باره خارج از HTTP یا policy مخصوص، ثبت دعوت/تأیید موجود، و الزام role/policy برای ثبت ادمین.

### C-03 — عبور کاربر Client از کنترل ادمین به‌علت ترتیب middleware

- **CWE / شدت:** CWE-862, CWE-863 — Critical — **تأییدشده (Static)**
- **مکان:** `src/Presentation/Artix.API.WebService/Program.cs:328-342` و `src/Presentation/Artix.API.Endpoints/DependencyInjection.cs:69-79`
- **شرح:** `UseCustomMiddlewares` (از جمله `ContinuousAuthorizationMiddleware`) قبل از `UseAuthentication` اجرا می‌شود؛ بنابراین middleware در آن مرحله `context.User` را ناشناس می‌بیند و بررسی permission را رد می‌کند. endpointهای مدیریتی نیز فقط `[Authorize]` دارند و policy/role `Admin` ندارند.
- **مسیرهای درگیر:** `POST/PATCH/DELETE /api/admin/**` که `[Authorize]` دارند.
- **اثر:** هر JWT معتبر Client می‌تواند پس از عبور از `[Authorize]` عملیات ایجاد/ویرایش/حذف مدیریتی انجام دهد (به‌خصوص در زنجیره C-02).
- **اصلاح:** ترتیب استاندارد `UseRouting → UseAuthentication → policy middleware → UseAuthorization → MapControllers`؛ روی controller ادمین `[Authorize(Roles="Admin")]` یا policy صریح بگذارید و permission را در policy متمرکز کنید.

### C-04 — secret و credential در مخزن

- **CWE / شدت:** CWE-798, CWE-312 — Critical — **تأییدشده (Static)**
- **مکان:** `src/Presentation/Artix.API.WebService/appsettings.Development.json:3-103`
- **موارد:** API key، SQL Server `sa` password، Elasticsearch password، RabbitMQ credential، Redis password، MongoDB credential و JWT signing key (`your-256-bit-secret-key-for-jwt-signing`).
- **اثر:** در صورت deploy/دسترسی به repository، دسترسی مستقیم به زیرساخت، جعل JWT، خواندن/تغییر داده و pivot داخلی.
- **اصلاح:** چرخش فوری همه secretها، حذف از git history، Secret Manager/Vault/مقادیر محیطی، fail-closed در production و کلید تصادفی حداقل 256 بیت.

### C-05 — آپلود عمومی و سرو فایل‌های کنترل‌نشده؛ زنجیره Disk DoS و Stored XSS

- **CWE / شدت:** CWE-434, CWE-400, CWE-79 — Critical — **تأییدشده (Static)**
- **مکان:** `src/Presentation/Artix.API.Endpoints/Controllers/Shared/UploadController.cs:20-58`، `src/Presentation/Artix.API.WebService/Program.cs:312-327`، `appsettings.Development.json:65-87`
- **شرح:** تمام عملیات upload بدون authorization است؛ نوع/محتوا در این controller بررسی نمی‌شود. فایل‌ها با `ServeUnknownFileTypes=true` و مسیر عمومی `/files` سرو می‌شوند و extensionهای `.html/.md/.txt/.pdf` مجازند.
- **اثر:** مهاجم بدون حساب می‌تواند دیسک را پر کند، فایل HTML حاوی اسکریپت را روی origin معتبر قرار دهد یا فایل‌های آپلودی دیگر را در معرض عموم بگذارد. اگر فایل به آبجکت متصل شود، زنجیره با endpointهای admin شدیدتر می‌شود.
- **اصلاح:** احراز هویت و مالکیت session، allowlist واقعی MIME با magic bytes، ذخیره خارج از web root، `Content-Disposition: attachment` برای فایل غیرتصویری، حذف HTML از allowlist یا sanitize، quota و cleanup.

---

## High

### H-01 — IDOR روی upload session و chunkها

- **CWE / شدت:** CWE-639 — High — **تأییدشده (Static)**
- **مکان:** `UploadController.cs:28-58`، `UploadService.cs:49-93`
- **شرح:** session به user/device متصل نیست. هر دارنده GUID می‌تواند chunk بنویسد، وضعیت بخواند یا merge کند؛ همچنین handlerهای admin فقط `uploadId` را می‌پذیرند.
- **اثر:** دستکاری/تکمیل فایل کاربر دیگر، افشای نام/اندازه/chunkها و attach کردن فایل بیگانه به رکورد مدیریتی.
- **اصلاح:** `OwnerUserId` و purpose را در session نگه دارید و در هر operation با کاربر جاری و resource هدف مقایسه کنید؛ sessionهای تمام‌شده/منقضی را حذف کنید.

### H-02 — نبود سقف اندازه و اعتبارسنجی chunk

- **CWE / شدت:** CWE-400, CWE-20 — High — **تأییدشده (Static)**
- **مکان:** `UploadController.cs:20-25` و `UploadService.cs:23-40,49-56`
- **شرح:** `totalSize`, `chunkSize` و `index` محدود یا validate نمی‌شوند؛ `chunkSize=0`، مقدار منفی، index خارج از بازه و body بسیار بزرگ پذیرفته می‌شود.
- **اثر:** خطای محاسباتی، ساخت sessionهای مخرب، مصرف دیسک/حافظه و DoS. با C-05 از شبکه بدون احراز هویت قابل سوءاستفاده است.
- **اصلاح:** بازه ثابت برای total/chunk، `index ∈ [0,totalChunks)`, سقف کل و per-session، تطبیق طول body با chunk، timeout و cleanup.

### H-03 — refresh token به‌صورت plaintext و انقضای مؤثرنشده

- **CWE / شدت:** CWE-256, CWE-613 — High — **تأییدشده (Static)**
- **مکان:** `src/Infra/Artix.API.Infra.Identity/Services/TokenService/TokenService.cs:30-60,73-83`
- **شرح:** query با `IdentityUserToken.Value == refreshToken` انجام می‌شود؛ در حالی که generator hash را هم تولید می‌کند. `GetTokenCreationDate` همیشه null است و در نبود metadata، انقضای واقعی قابل محاسبه نیست.
- **اثر:** نشت DB یا log به replay مستقیم refresh token منجر می‌شود؛ token ممکن است تا زمان نامعلوم معتبر بماند.
- **اصلاح:** فقط hash مقایسه‌شده با `CryptographicOperations.FixedTimeEquals`، ستون‌های `CreatedAt/ExpiresAt/RevokedAt`, rotation اتمیک و reuse detection.

### H-04 — rotation refresh token در برابر هم‌زمانی اتمیک نیست

- **CWE / شدت:** CWE-362, CWE-613 — High — **تأییدشده (Static)**
- **مکان:** `TokenService.cs:52-70`
- **شرح:** حذف token قدیمی و صدور token جدید در transaction/conditional update واحد نیست. دو درخواست هم‌زمان می‌توانند هر دو قبل از حذف، token را معتبر ببینند.
- **اثر:** replay موازی refresh token و چند session معتبر از یک credential سرقت‌شده.
- **اصلاح:** row-version/compare-and-delete در transaction، unique family state و revoke کل family هنگام reuse.

### H-05 — اعتماد به `X-Forwarded-For` قابل جعل

- **CWE / شدت:** CWE-345, CWE-770 — High — **تأییدشده (Static)**
- **مکان:** `RateLimitAttribute.cs:34-50`، `GeoFencingMiddleware.cs:183-195`، `ContinuousAuthorizationMiddleware.cs:495-510`، در حالی‌که `UseForwardedHeaders()` بعداً در `Program.cs:337` آمده است.
- **شرح:** client می‌تواند IP دلخواه در header بفرستد و کلید rate limit/geo/device-location را تغییر دهد.
- **اثر:** دور زدن محدودیت OTP/scan، جعل کشور و خراب کردن anomaly detection.
- **اصلاح:** فقط `ForwardedHeadersMiddleware` با proxyهای trusted و شبکه مشخص؛ در غیر این صورت فقط `RemoteIpAddress`; header ورودی را از اینترنت نپذیرید.

### H-06 — rate limit ارسال OTP خاموش است

- **CWE / شدت:** CWE-799, CWE-307 — High — **تأییدشده (Static)**
- **مکان:** `src/Presentation/Artix.API.Endpoints/Controllers/Client/AuthenticationController.cs:23-26`
- **شرح:** attribute ارسال OTP کامنت شده است؛ تنها verify محدودیت IP دارد و آن هم با H-05 قابل جعل است.
- **اثر:** SMS/OTP flood، enumeration شماره‌ها، هزینه provider و brute force عملی‌تر.
- **اصلاح:** محدودیت چندبعدی شماره/IP/device/provider، backoff، پاسخ یکنواخت و quota روزانه.

### H-07 — endpointهای خواندنی Admin بدون authorization

- **CWE / شدت:** CWE-862 — High — **تأییدشده (Static)**
- **مکان:** `AdminPanel/MuseumController.cs:21-35` و `AdminPanel/ObjectController.cs:31-37`
- **مسیرها:** `GET /api/admin/museum/all`, `GET /api/admin/museum/objects`, `GET /api/admin/object/by-id`
- **اثر:** هر کاربر ناشناس می‌تواند داده‌های مدیریتی، hidden/special و جزئیات داخلی را enumerate کند.
- **اصلاح:** role/policy ادمین روی controller یا action و projection حداقلی برای admin.

### H-08 — اعتبارسنجی ورودی و محدودیت صفحه‌بندی ناکافی

- **CWE / شدت:** CWE-20, CWE-400 — High — **تأییدشده (Static)**
- **مکان:** handlerهای OTP، admin register، profile و pagination؛ فقط یک validator در کل featureها شناسایی شد و چند handler صریحاً TODO validation دارند.
- **اثر:** payloadهای بزرگ/صفحه‌های بسیار بزرگ، خطاهای پرهزینه، داده‌های نامعتبر و افزایش سطح حمله. در profile نیز base64 تصویر بدون سقف آماده decode است.
- **اصلاح:** validator برای همه command/query، سقف body/base64/page size، نرمال‌سازی phone/email/name و reject زودهنگام.

### H-09 — JWT پس از تغییر وضعیت کاربر/پسورد الزاماً باطل نمی‌شود

- **CWE / شدت:** CWE-613 — High — **تأییدشده (Static)**
- **مکان:** `Infra.Identity/DependencyInjection.cs:162+` (اعتبارسنجی‌های security stamp/session کامنت شده) و `JwtTokenGenerator.cs:84-99`
- **شرح:** stamp داخل claim صادر می‌شود اما OnTokenValidated بررسی stamp، user، ban، session و revocation را انجام نمی‌دهد. middleware سفارشی هم فقط JTI را از token می‌خواند و برای tokenهای revoke نشده ادامه می‌دهد.
- **اثر:** token سرقت‌شده تا پایان عمر access token قابل استفاده است و تغییر password/ban لزوماً آن را قطع نمی‌کند.
- **اصلاح:** اعتبارسنجی server-side کاربر، security stamp، session و revocation در handler استاندارد JWT؛ باطل‌سازی همه sessionها هنگام reset/delete/ban.

### H-10 — افشای خطای داخلی در پاسخ production

- **CWE / شدت:** CWE-209 — High — **تأییدشده (Static)**
- **مکان:** `src/Presentation/Artix.API.WebService/Program.cs:218-249`
- **شرح:** در production هم `Error = ex.Message`, `Exception = ex.GetType().Name`, و `Path` به client برگردانده می‌شود. بسیاری از exceptionها شامل نام entity، مسیر فایل و جزئیات persistence هستند.
- **اثر:** افشای ساختار داخلی، مسیرهای filesystem، نام منابع و کمک به exploit chaining.
- **اصلاح:** correlation id عمومی و پیام ثابت؛ جزئیات فقط log امن با redaction.

---

## Medium

### M-01 — Swagger و schema بدون محدودیت محیطی

`Program.cs:331-335` همیشه Swagger را فعال می‌کند. در production می‌تواند endpointها، مدل‌ها و نام عملیات حساس را آشکار کند. فقط در محیط مجاز فعال و پشت auth/network policy قرار گیرد.

### M-02 — فایل‌های public با cache یک‌ساله و بدون کنترل دسترسی

`Program.cs:289-327` برای `/files` `public,max-age=31536000` می‌گذارد. حذف/تعویض فایل یا revoke دسترسی تا پایان cache کلاینت/CDN باقی می‌ماند. برای محتوای حساس private storage و cache کوتاه/نسخه‌گذاری استفاده شود.

### M-03 — اعتماد به extension به‌جای magic bytes/MIME واقعی

`CreateNewObjectCommandHandler.cs:65-190` و `CreateNewMuseumCommandHandler.cs:45-66` فقط extension را با allowlist مقایسه می‌کنند. فایل polyglot یا محتوای نامتناسب پذیرفته می‌شود. signature check، re-encode تصاویر و اسکن محتوا اضافه شود.

### M-04 — مسیر storage قابل تنظیم خارج از sandbox

`Program.cs:104-130` مقدار `FileSettings:StoragePath` مطلق یا `~` را بدون محدود کردن به root مجاز می‌پذیرد. misconfiguration یا secret/config compromise می‌تواند برنامه را به overwrite/read مسیرهای حساس نزدیک کند. مسیر را به root ثابت و canonicalized محدود کنید.

### M-05 — اطلاعات حساس در logها

نام فایل، IP، user-agent، user id و مسیرهای فایل در log ثبت می‌شوند؛ نمونه‌ها در `UploadService`, `GeoFencingMiddleware`, `ContinuousAuthorizationMiddleware` و handlerهای profile است. retention، access control و redaction برای PII/credential اعمال شود.

### M-06 — وابستگی geo lookup به HTTP بدون proxy trust و fail-closed

`GeoFencingMiddleware.cs:148-177` برای هر IP ناشناس به سرویس بیرونی HTTP متصل می‌شود و در خطا `UNKNOWN` برمی‌گرداند، در حالی‌که whitelist فقط log می‌شود و block نمی‌کند. این کنترل امنیتی قابل اتکا نیست و availability/حریم خصوصی IP را به third party وابسته می‌کند. از provider داخلی/HTTPS و تصمیم fail-closed مشخص استفاده شود.

---

## نیازمند تست محیطی (الگوهای پرریسک)

1. **Race condition در scan/XP و marketplace:** کد دامنه قفل/دِدوپ را ادعا می‌کند، اما باید با درخواست‌های هم‌زمان و Redis واقعی verify شود که افزایش XP/خرید دوباره انجام نمی‌شود.
2. **CORS و credential behavior:** policy فعلی originهای مشخص دارد؛ با تست preflight و originهای نزدیک (`null`, subdomain, mixed case) تأیید شود که reflection ناخواسته رخ نمی‌دهد.
3. **دسترسی فایل و traversal پس از URL decoding:** middleware دستی فقط `..` خام را چک می‌کند. با مسیرهای encoded و separatorهای platform در محیط واقعی تست غیرمخرب انجام شود.
4. **مدیریت cleanup و persistence آپلود:** repository در حافظه است؛ restart چند instance، orphan chunk و race بین complete/chunk باید با deployment واقعی بررسی شود.

---

## اقدام فوری پیشنهادی

1. همه credentialها و signing keyهای فهرست C-04 را rotate و از history حذف کنید.
2. endpoint ثبت Admin و تمام upload endpointها را فوراً از دسترسی عمومی خارج کنید.
3. OTP ثابت را حذف کنید و rate limit شماره‌محور اضافه کنید.
4. ترتیب middleware و role policy ادمین را اصلاح کنید.
5. تست‌های regression برای C-01 تا C-05 و H-01 تا H-06 اضافه و با build/integration environment اجرا کنید.

## محدودیت بررسی

این فایل گزارش static review است؛ exploit فعال روی production، اسکن مخرب، دسترسی به DB/Redis/RabbitMQ واقعی و تست providerهای بیرونی انجام نشده است. موارد «تأییدشده (Static)» بر اساس مسیر کد قابل بازتولید هستند، اما پاسخ واقعی deployment باید با تست کنترل‌شده تکمیل شود.

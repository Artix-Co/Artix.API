# BACKLOGSECURITY — Artix API

تاریخ بررسی: 2026-10-24
شاخه: `security/2026-10-24-full-assessment`
روش: بازبینی ایستای کل فایل‌های tracked زیر `src` و `tests`، جست‌وجوی مسیرهای ورودی و اعتماد، سپس خواندن خط‌به‌خط فایل‌های احراز هویت، session، upload، middleware، پیکربندی و serialization. هیچ درخواست شبکه یا exploit مخرب اجرا نشد.

## خلاصه اولویت

| اولویت | تعداد | اقدام |
|---|---:|---|
| Critical | 1 | قبل از انتشار کلید را حذف و rotate کنید. |
| High | 4 | در چرخهٔ بعدی امنیتی fix و regression test اضافه کنید. |
| Medium | 4 | قبل از production با تصمیم معماری و تست محیطی ببندید. |
| نیازمند تست محیطی | 5 | با محیط staging و نقش‌های واقعی تأیید کنید. |

## Critical

### ✅ C-01 — کلید امضای JWT در فایل پیکربندی commit شده است

- **CWE:** CWE-798, CWE-321
- **Evidence:** `src/Presentation/Artix.API.WebService/appsettings.json:39-46`
- **Observed:** مقدار ثابت `Authentication:IssuerSigningKey` در repository وجود دارد.
- **Impact:** هر دارندهٔ repository می‌تواند access token معتبر بسازد و به همهٔ مسیرهای JWT دسترسی بگیرد. کلید فعلی باید افشاشده فرض شود.
- **Fix:** کلید را فوراً rotate کنید؛ از secret manager/environment با حداقل 256 بیت تصادفی استفاده کنید؛ startup در production را برای مقدار placeholder یا مقدار commit شده fail کنید؛ history و artifactهای قبلی را پاک‌سازی و همهٔ sessionها را revoke کنید.
- **Retest:** token ساختگی با کلید قبلی رد شود و startup بدون secret امن fail-closed شود.
- **Status:** کلید از `appsettings.json` حذف شد و startup کلید خالی، placeholder یا کوتاه‌تر از ۳۲ کاراکتر را رد می‌کند. چرخش عملیاتی کلید و پاک‌سازی history هنوز باید خارج از کد انجام شود.

## High

### ✅ H-01 — OTP در Redis و entity به‌صورت plaintext نگه‌داری می‌شود

- **CWE:** CWE-256, CWE-522
- **Evidence:** `src/Core/Artix.API.Core.Domain/Entities/OTP/OTP.cs:22-29`; `src/Core/Artix.API.Core.DomainService/Services/OtpService.cs:61-68,196-205`
- **Observed:** hash در `OTP.Code` ذخیره می‌شود، اما `CodeForDelivery` و `OtpSessionData.Code` همان کد را نگه می‌دارند. کلید Redis فقط شماره تلفن است.
- **Impact:** دسترسی read به DB یا Redis برای تصاحب login/registration کافی است. OTPهای فعال قابل بازیابی هستند.
- **Fix:** فقط hash را در DB/Redis نگه دارید؛ کد ارسال را مستقیم به provider بدهید؛ session را با nonce تصادفی و شناسهٔ challenge bind کنید؛ TTL و حذف اتمیک را حفظ کنید.
- **Retest:** dump کنترل‌شدهٔ DB/Redis هیچ OTP قابل استفاده‌ای نداشته باشد و verify بدون challenge رد شود.
- **Status:** entity دیگر کد تحویل را نگه نمی‌دارد و Redis فقط purpose و تعداد تلاش را ذخیره می‌کند؛ اعتبارسنجی کد فقط با hash ذخیره‌شده در DB انجام می‌شود.

### H-02 — مصرف OTP در برابر درخواست هم‌زمان اتمیک نیست

- **CWE:** CWE-362, CWE-613
- **Evidence:** `src/Core/Artix.API.Core.DomainService/Services/OtpService.cs:73-110`
- **Observed:** چند درخواست می‌توانند session را بخوانند و کد را برابر ببینند؛ سپس token صادر می‌شود و بعد `MarkAsUsed` انجام می‌شود. قفل یا compare-and-delete برای challenge وجود ندارد.
- **Impact:** replay هم‌زمان می‌تواند بیش از یک جفت token صادر کند.
- **Fix:** یک عملیات atomic در Redis/DB برای consume challenge انجام دهید؛ فقط برندهٔ consume مجاز به صدور token باشد؛ صدور token و علامت مصرف را با invariant قابل آزمون هماهنگ کنید.
- **Retest:** دو verify هم‌زمان با یک challenge، دقیقاً یک پاسخ موفق داشته باشند.

### H-03 — endpointهای health جزئیات backend را عمومی برمی‌گردانند

- **CWE:** CWE-200
- **Evidence:** `src/Presentation/Artix.API.WebService/Program.cs:361-375`; `src/Presentation/Artix.API.WebService/Extensions/HealthCheckExtensions.cs:46-69`
- **Observed:** `/health`, `/health/ready` و `/health/live` بدون authorization map می‌شوند. payload نام check، مدت، description و `Exception.Message` را برمی‌گرداند.
- **Impact:** وضعیت SQL Server، MongoDB، Redis و RabbitMQ و متن خطای داخلی برای anonymous افشا می‌شود و reconnaissance و تنظیم حمله را ساده می‌کند.
- **Fix:** فقط liveness عمومی و حداقلی باشد؛ readiness و جزئیات backend را به شبکهٔ داخلی یا policy محدود کنید؛ `error` و exception متن را از پاسخ عمومی حذف کنید.
- **Retest:** anonymous فقط status حداقلی بگیرد و هیچ نام سرویس/پیام خطا نبیند.

### H-04 — پاسخ خطای production هنوز متن و نوع exception را افشا می‌کند

- **CWE:** CWE-209
- **Evidence:** `src/Presentation/Artix.API.WebService/Program.cs:225-258`
- **Observed:** قبل از شاخهٔ production، `baseResponse.Error = ex.Message` و `Exception = ex.GetType().Name` ساخته می‌شود و در پاسخ JSON نوشته می‌شود. فقط stack trace به Development محدود شده است.
- **Impact:** خطاهای persistence، مسیرهای داخلی و نوع component به client نشت می‌کند.
- **Fix:** در production فقط پیام ثابت و correlation id برگردانید؛ جزئیات را فقط در log محافظت‌شده و redacted ثبت کنید.
- **Retest:** یک exception کنترل‌شده در production-like host فقط پیام ثابت برگرداند.

## Medium

### M-01 — allowlist فایل در upload به‌کار نمی‌رود و extension ناشناخته پذیرفته می‌شود

- **CWE:** CWE-434, CWE-79
- **Evidence:** `src/Infra/Artix.API.Infra.FileService/Services/UploadService.cs:97-140`; `src/Core/Artix.API.Core.Contract/Configs/FileSettings/FileSettings.cs:11-17`; `src/Presentation/Artix.API.WebService/Program.cs:331-342`
- **Observed:** `_fileSettings` خوانده می‌شود ولی allowlist آن استفاده نمی‌شود. `HasExpectedSignatureAsync` برای extensionهای خارج از چهار نوع، `true` برمی‌گرداند. فایل‌های شناخته‌شده زیر `/files` static serve می‌شوند.
- **Impact:** فایل HTML یا محتوای فعال می‌تواند توسط کاربر مجاز upload و روی origin برنامه اجرا شود.
- **Fix:** allowlist server-side بر اساس purpose، MIME واقعی و magic bytes؛ فایل فعال را از web root خارج کنید یا با `Content-Disposition: attachment` و origin جدا serve کنید.
- **Retest:** `.html`, `.svg`, `.js` و extension ناشناخته رد یا به‌صورت attachment بی‌خطر تحویل شوند.

### M-02 — sessionهای upload انقضا و cleanup ندارند

- **CWE:** CWE-400, CWE-459
- **Evidence:** `src/Core/Artix.API.Core.Domain/Entities/File/UploadSession.cs:5-27`; `src/Infra/Artix.API.Infra.FileService/Services/InMemoryUploadRepository.cs:7-25`; `src/Infra/Artix.API.Infra.FileService/Services/FileSystemStorage.cs:43-66,151-156`
- **Observed:** `CreatedAt` وجود دارد، اما repository هیچ expiry/delete ندارد و temp folder فقط هنگام merge حذف می‌شود.
- **Impact:** initهای رهاشده و chunkهای نیمه‌کاره می‌توانند دیسک/حافظه را پر کنند؛ restart نیز state را از بین می‌برد.
- **Fix:** TTL و quota per user، job cleanup، حذف session و temp folder در failure، و repository پایدار با status اتمیک.
- **Retest:** session منقضی‌شده و folder آن خودکار حذف شود و quota قابل دور زدن نباشد.

### M-03 — geo-fencing برای مسیرهای حساس فقط log می‌کند

- **CWE:** CWE-693
- **Evidence:** `src/Presentation/Artix.API.Endpoints/Middlewares/GeoFencingMiddleware.cs:81-101`
- **Observed:** وقتی country در allowlist نیست، کد warning می‌نویسد و به `_next` ادامه می‌دهد؛ پاسخ 403 در comment باقی مانده است.
- **Impact:** اگر geo-fencing کنترل امنیتی فرض شده باشد، `/api/admin` و مسیرهای حساس از کشور غیرمجاز همچنان قابل دسترسی هستند.
- **Fix:** geo را به‌عنوان defense-in-depth تعریف کنید و تصمیم deny را در authorization اصلی enforce کنید؛ در صورت نیاز policy fail-closed و proxy trust مشخص اضافه کنید.
- **Retest:** matrix کشور/نقش نشان دهد تصمیم authorization مستقل از header و client است.

### M-04 — lookup جغرافیایی به سرویس HTTP ثالث وابسته است

- **CWE:** CWE-359, CWE-400
- **Evidence:** `src/Presentation/Artix.API.Endpoints/Middlewares/GeoFencingMiddleware.cs:159-185`
- **Observed:** برای IP کاربر، هر cache miss به `https://ip-api.com/json/{ip}` درخواست می‌رود؛ parser دستی است و provider خارج از کنترل برنامه است.
- **Impact:** IP کاربر به ثالث افشا می‌شود و latency/availability درخواست به provider وابسته می‌ماند.
- **Fix:** MaxMind/GeoIP محلی یا provider قراردادی با TLS و timeout مشخص؛ IP را minimize کنید؛ failure را برای مسیر امنیتی صریح و قابل مانیتور کنید.
- **Retest:** قطع provider فقط رفتار fail-closed تعریف‌شده را ایجاد کند و IP به log/پاسخ نشت نکند.

## نیازمند تست محیطی

1. **CORS و credential:** `HostingExtension.cs:95-107` از `AllowCredentials` همراه `AllowAnyHeader/AllowAnyMethod` استفاده می‌کند. originهای production باید از configuration واقعی بررسی شوند.
2. **Forwarded headers:** فقط `Program.cs:344` دیده شد و تنظیم `KnownProxies/KnownNetworks` در کد یافت نشد. در staging پشت proxy بررسی کنید که IP و scheme قابل جعل یا اشتباه نباشد.
3. **Admin authorization:** policyهای `AdminPermission` روی همهٔ controllerها باید با matrix نقش/permission و object scope تست شوند؛ بازبینی ایستا وجود policy را تأیید می‌کند، نه تصمیم runtime را.
4. **Refresh-token rotation:** کد hash، expiry و revocation دارد؛ race واقعی در SQL isolation و دو درخواست هم‌زمان باید با integration test تأیید شود.
5. **Dependency versions:** `Directory.Packages.props` و lock/artifactهای restore باید با advisory database سازمانی یا `dotnet list package --vulnerable` بررسی شوند؛ این بررسی شبکه‌ای در این گزارش اجرا نشد.

## کنترل‌های تأییدشده در بازبینی ایستا

- OTP تصادفی با `RandomNumberGenerator.GetInt32` تولید می‌شود: `OTP.cs:41-44`.
- rate limit برای send/verify OTP فعال است: `AuthenticationController.cs:23-35`.
- مالکیت upload در initiate/chunk/status/merge بررسی می‌شود: `UploadService.cs:61-69,75-80,142-154`.
- اندازهٔ کل upload و chunk در service محدود شده است: `UploadService.cs:33-38`.
- JWT issuer، audience، lifetime و signing key validation در Identity ثبت شده است: `Infra.Identity/DependencyInjection.cs:145-172`.

## روش retest و پذیرش

هر finding پس از fix باید regression test داشته باشد. برای C-01، H-01 و H-02 ابتدا secret/sessionهای موجود revoke شوند. تست‌های unit، integration و end-to-end با نقش anonymous، client، admin و دو درخواست هم‌زمان اجرا شوند. هیچ findingی در این گزارش به‌معنای exploit تأییدشده در production نیست؛ موارد static باید در staging کنترل‌شده retest شوند.

## محدودیت‌ها

این گزارش source review است. اسکن active، fuzzing، بررسی deployment واقعی، secret rotation، advisory lookup شبکه‌ای، و تست DB/Redis/RabbitMQ واقعی انجام نشد. severity بر اساس مسیر کد و اثر محتمل تعیین شده است و باید با محیط استقرار نهایی بازبینی شود.

## وضعیت اعتبارسنجی این شاخه

- `dotnet build Artix.API.sln --no-restore` اجرا شد اما fail شد: `GeoFencingMiddleware.cs:61` و `:85` خطای `CS1525 Invalid expression term ')'` دارند.
- `dotnet test tests/Artix.API.Tests.Unit/Artix.API.Tests.Unit.csproj --no-restore` به‌دلیل خطای اجرای test host abort شد؛ نتیجهٔ سبز گزارش نمی‌شود.
- Graphify در workspace موجود نیست؛ `graphify update .` قابل اجرا نبود.

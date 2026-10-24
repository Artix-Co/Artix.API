# Artix API — Agent Guide

این فایل قرارداد کاری عامل‌های کدنویس در این مخزن است. قبل از تغییر، بخش مربوط به دامنه و مسیر اجرای تغییر را بخوان؛ رفتار فعلی کد بر ایده‌های محصول مقدم است.

## شناخت محصول

Artix یک تجربهٔ فرهنگی/بازی‌وار برای بازدید از موزه‌هاست. کاربر با اسکن QR و آبجکت‌های موزه، انجام تسک‌ها و ایونت‌های فصلی، XP و آیتم جمع می‌کند؛ آبجکت‌ها می‌توانند در کلکسیون، ژورنال یا بازار معاملات استفاده شوند. هدف محصول تشویق بازدید حضوری، تکمیل موزه، بازگشت دوباره، ارتباط اجتماعی و پیشرفت منصفانه است.

سناریوهای محصول:

- **QR Hunt / کاوشگر:** QRهای متعدد در موزه ممکن است پوچ باشند؛ اسکن‌ها قطعات یا آبجکت ویژه را آشکار می‌کنند و تکمیل مسیر پاداش دارد.
- **ژورنال:** آبجکت ویژه می‌تواند یک مدخل آرت‌بوک شامل اسکچ و توضیح اختصاصی بسازد.
- **Co-Up:** تکمیل آبجکت‌های یک موزه کلید آن موزه را می‌دهد. دوست‌ها می‌توانند با کلیدهای خود بخشی از دسترسی موزه‌های دیگر را برای هم باز کنند.
- **آخرین کوییز:** پس از تکمیل آبجکت‌های موزه، سرنخ و اسکن‌های محدود برای یافتن آبجکت نهایی ارائه می‌شود؛ موفقیت می‌تواند آن را به golden level برساند و شکست کامل هم باید پاداش تلاش داشته باشد.
- **ارتقای کارت:** اسکن یا دریافت تکراری، آبجکت را قابل ارتقا و باارزش‌تر می‌کند.
- **Adventure/Season Path:** هر فصل مسیر رایگان و اشتراکی دارد و با XP، تسک و پاداش‌هایی مثل آیتم، بنر، آواتار و موسیقی پیش می‌رود.
- **بازار:** آبجکت قابل مبادله با واحد پول داخلی فهرست می‌شود تا کاربر بتواند از تکراری‌ها استفاده کند؛ انتقال‌های حمایتی باید محدود و منصفانه باشند.
- **Strike:** شعلهٔ فعالیت برای یک بازهٔ زمانی با بازدید یا تسک روشن می‌ماند؛ fuel مانند هیزم/زغال می‌تواند فرصت جبران بدهد و الزام روزانهٔ تنش‌زا ایجاد نشود.
- **رده‌بندی:** رتبه‌بندی دائمی کشوری/جهانی بر پایه XP است؛ لیگ‌های فصلی یا رقابت نابرابر اضافه نکن.

موارد بالا، به‌جز آنچه در بخش «رفتار پیاده‌شده» آمده، نیازمندی محصول‌اند و نباید بدون مدل، قرارداد و تست موجود فرض شوند.

## رفتار پیاده‌شده و قواعد دامنه

### اسکن و XP

- `UserScan` وضعیت رابطهٔ کاربر و آبجکت است: `ScanCount`، `AcquiredAt`، `IsUpgraded` و `InCollection`.
- اسکن اول در `XpRulesService.CalculateXpForFirstScanAsync` انجام می‌شود: `base = (IsSpecial ? 150 : 100) + (Object.Tier ?? 0) * 10` و XP نهایی `base × multiplier` است.
- اسکن تکراری در `CalculateXpForRepeatScanAsync`: `base = (isGoldenLevel ? 200 : 50) + (Object.Tier ?? 0) * 5` و سپس ضرب در multiplier. اسکن تکراریِ غیرارتقایافته ابتدا `Upgrade()` می‌شود؛ golden scan بعدی شمارنده را ثبت می‌کند.
- اگر feature flag `xp_double_event` مقدار `true` داشته باشد، multiplier دو برابر می‌شود.
- هر XP موفق هم به `UserXp.TotalXp` و leaderboard سراسری اضافه می‌شود؛ اگر `seasonId` وجود داشته باشد به `UserSeasonProgress.TotalXp` و leaderboard فصل نیز اضافه می‌شود.
- رویدادهای XP باید idempotent باشند: dedup key برای first scan برابر `xp:firstscan:{userId}:{objectId}` و برای repeat برابر `xp:repeatscan:{userId}:{objectId}` با پسوند `:golden` است. قفل توزیع‌شدهٔ `user:{userId}:xp` از افزایش هم‌زمان جلوگیری می‌کند.
- آبجکت، کاربر و `UserScan` لازم باید وجود داشته باشند؛ خطای نبودن هرکدام را بی‌صدا نادیده نگیر.

### Tier و concession

`TierCalculatorService` کانفیگ‌های Tier را از repository می‌خواند و بر اساس اولویت نزولی اولین تطابق را انتخاب می‌کند. معیارهای موجود عبارت‌اند از حداقل تعداد اسکن، ارتقا، حضور در کالکشن، روزهای سپری‌شده، Strike فعال، ویژه بودن آبجکت، نوع فروش، نوع عضویت و کلید Co-Up.

اگر تطابق کامل نباشد، نزدیک‌ترین tier نگه داشته می‌شود و multiplier نسبی با سقف ۸۰٪ از فاصلهٔ multiplier اصلی محاسبه می‌شود. fuel می‌تواند شرط Strike فعال را به‌صورت concession پاس کند. این منطق را هنگام افزودن معیار جدید حفظ کن و multiplier را در چند handler تکرار نکن.

### مدل‌های اصلی

- `AppUser`: هویت و ریشهٔ ارتباط با XP، scan، کلید موزه، Strike، فصل، کالکشن، ژورنال و دوستی.
- `Museum`, `MuseumObject`, `Object`: محتوای موزه و QR/آبجکت؛ آبجکت می‌تواند special، hidden، tier و `ObjectSaleType` داشته باشد.
- `Collection`, `CollectionItem`, `JournalEntry`, `UserJournalEntry`: نمایش و تجربهٔ شخصی کاربر.
- `Season`, `SeasonTask`, `UserSeasonProgress`: فصل و پیشرفت کاربر.
- `Quiz`, `QuizObject`, `UserQuiz`: کوییز/تسک عمومی و وضعیت کاربر؛ `Quiz.RelatedFeature` می‌تواند به `QRHunts`, `CoUp`, `Strike` یا `LastQuiz` اشاره کند.
- `MarketplaceItem`: آبجکت، فروشنده، قیمت امتیازی، زمان فهرست‌شدن و وضعیت فروش.
- `UserMuseumKey`, `Friendship`, `UserStrike`, `UserXp`, `UserScan`: قابلیت‌های اجتماعی، استمرار و پیشرفت.

## معماری و محل تغییر

راه‌حل .NET چندلایه است:

- `Core.Domain`: entity، aggregate، value object، enum و invariantهای دامنه؛ این لایه نباید به EF، Redis، RabbitMQ یا HTTP وابسته شود.
- `Core.Contract`: قرارداد command/query، DTO، repository و interface سرویس‌ها.
- `Core.DomainService`: قواعد چند aggregate مانند XP، Tier، OTP و notification.
- `Core.ApplicationService`: handlerهای CQRS/MediatR به‌صورت vertical slice؛ command/query و validator را در feature خودش نگه دار.
- `Infra.Sql`: EF Core، `ArtixCommandDbContext` و `ArtixQueryDbContext`، configurationهای Write/Read، seed و interceptorها.
- `Infra.Mongo`: read/queryهای Mongo، مخصوصاً Quiz و داده‌های خواندنی مناسب document store.
- `Infra.Redis`: cache، rate limit، lock، dedup، leaderboard، session و anomaly detection.
- `Infra.RabbitMQ`: outbox، consumer و notification؛ انتشار پیام باید از مسیر outbox موجود انجام شود.
- `Infra.Identity`: ASP.NET Identity، JWT، session و device trust.
- `Presentation`: endpointها، middlewareها، versioning، API key، authorization، geo-fencing و rate limiting.

برای write از repository/command context و برای read از query repository/query context استفاده کن. دسترسی مستقیم endpoint به DbContext یا منطق دامنه در controller/handler ممنوع است. از الگوی موجود MediatR 12 استفاده کن: `IRequest`, `IRequestHandler<TRequest,TResponse>` و `IPipelineBehavior<TRequest,TResponse>`.

## قواعد تغییر

1. قبل از جست‌وجوی خام، اگر `graphify-out/graph.json` هست، `graphify query "<question>"` را اجرا کن؛ برای رابطهٔ دو نماد `graphify path` و برای مفهوم متمرکز `graphify explain` را به‌کار ببر. سپس فقط فایل‌ها و خطوطی را بخوان که graph یا خواندن قبلی مشخص کرده است.
2. قبل از تغییر یک تابع، callerها، handlerها، endpointها، repositoryها، تست‌ها، seed/config و exportهای آن را پیدا کن. تغییر را در همان الگوی اطراف نگه دار.
3. هر مسیر اعتماد ورودی را validate کن؛ مالکیت کاربر، وضعیت فروش، دسترسی موزه و محدودیت‌های اسکن را دور نزن.
4. عملیات XP، خرید/فروش، ارتقا و پاداش باید idempotent و در برابر retry/پردازش هم‌زمان امن باشند. خطاهای persistence یا پیام‌رسانی را مخفی نکن.
5. تاریخ‌های ذخیره‌شده و محاسبات زمانی UTC باشند؛ زمان فصل و Strike را با مرزهای مشخص و قابل تست محاسبه کن.
6. تغییرات read/write configuration، migration، seed و cache invalidation را هم‌زمان با مدل تغییر بده. از افزودن abstraction یا package جدید برای چند خط کد خودداری کن.
7. نام‌گذاری و ساختار پوشهٔ موجود را حفظ کن؛ feature جدید را در مسیر `Core.Contract` → `ApplicationService` → `Endpoints` و implementation زیرساخت مربوط قرار بده.
8. هر منطق غیرtrivial جدید، به‌ویژه XP، پول داخلی، parser، شرط دسترسی یا concurrency، حداقل یک تست معنادار داشته باشد.

## تست و بررسی

پس از تغییر، نزدیک‌ترین تست را اجرا و در صورت امکان کل solution را build کن:

```bash
dotnet test tests/Artix.API.Tests.Unit/Artix.API.Tests.Unit.csproj
dotnet test tests/Artix.API.Tests.Integration/Artix.API.Tests.Integration.csproj
dotnet test tests/Artix.API.Tests.EndToEnd/Artix.API.Tests.EndToEnd.csproj
dotnet build Artix.API.sln
graphify update .
```

برای تغییر XP یا Tier، تست‌های `tests/Artix.API.Tests.Integration/Core/DomainService/XPRules` را به‌عنوان مرجع رفتاری بخوان و سناریوهای first scan، repeat/golden، season، کاربر ناموجود، dedup و lock را پوشش بده. اگر محیط لازم (DB، Redis، RabbitMQ یا secret) موجود نیست، همان تست/محدودیت را دقیق گزارش کن.

## Graphify

این پروژه knowledge graph در `graphify-out/` دارد. فایل‌های dirty graphify مانع کار نیستند. فقط برای بررسی stale/غلط بودن graph یا با درخواست صریح کاربر graphify را کنار بگذار. پس از هر تغییر کد `graphify update .` را اجرا کن؛ بازنویسی بزرگ `graphify-out/graph.json` طبیعی است.

## ASD-STE100 writing rule

Apply ASD-STE100 to every new or changed code comment and summary.

- Use short sentences with one clear action.
- Use simple words and active voice.
- Use one term for one meaning.
- Avoid idioms, vague words, and unnecessary words.
- State the condition, action, and result.
- Write comments that explain why the code exists.
- Keep summaries factual and easy to scan.
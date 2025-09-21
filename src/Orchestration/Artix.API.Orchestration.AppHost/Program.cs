var builder = DistributedApplication.CreateBuilder(args);

// اضافه کردن Redis
var redis = builder.AddRedis("redis")
    .WithDataVolume("redis-data") // معادل ولوم redis_data
    .WithArgs("--requirepass", "Heli@ghar771379"); // رمز عبور Redis

// اضافه کردن RabbitMQ
var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithDataVolume("rabbitmq_data") // معادل ولوم rabbitmq_data
    .WithEnvironment("RABBITMQ_DEFAULT_USER", "admin")
    .WithEnvironment("RABBITMQ_DEFAULT_PASS", "admin")
    .WithEndpoint(port: 15672, targetPort: 15672, name: "management"); 

// اضافه کردن SQL Server
var sqlserver = builder.AddSqlServer("sqlserver")
    .WithDataVolume("mssql-data") // معادل ولوم mssql-data
    .WithEnvironment("SA_PASSWORD", "Hello&Run1234")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("MSSQL_PID", "Express");

// اضافه کردن اپلیکیشن‌ها (app1, app2, app3) با استفاده از Dockerfile
var app1 = builder.AddDockerfile("app1", "../src/Presentation/Artix.API.WebService", "Dockerfile")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:80")
    .WithEnvironment("DOTNET_RUNNING_IN_CONTAINER", "true")
    .WithEnvironment("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT", "0")
    .WithVolume("dataprotection-keys", "/app/dataprotection-keys") // معادل ولوم dataprotection_keys
    .WithVolume("file_storage", "/app/files") // معادل ولوم file_storage
    .WithReference(redis) // وابستگی به Redis
    .WithReference(rabbitmq) // وابستگی به RabbitMQ
    .WithReference(sqlserver) // وابستگی به SQL Server
    .WithHttpEndpoint(port: 80, targetPort: 80, name: "http");


var app2 = builder.AddDockerfile("app2", "../src/Presentation/Artix.API.WebService", "Dockerfile")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:80")
    .WithEnvironment("DOTNET_RUNNING_IN_CONTAINER", "true")
    .WithEnvironment("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT", "0")
    .WithVolume("dataprotection-keys", "/app/dataprotection-keys")
    .WithVolume("file_storage", "/app/files")
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithReference(sqlserver)
    .WithHttpEndpoint(port: 80, targetPort: 80, name: "http");


var app3 = builder.AddDockerfile("app3", "../src/Presentation/Artix.API.WebService", "Dockerfile")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:80")
    .WithEnvironment("DOTNET_RUNNING_IN_CONTAINER", "true")
    .WithEnvironment("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT", "0")
    .WithVolume("dataprotection-keys", "/app/dataprotection-keys")
    .WithVolume("file_storage", "/app/files")
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithReference(sqlserver)
    .WithHttpEndpoint(port: 80, targetPort: 80, name: "http");


// اضافه کردن nginx
var nginx = builder.AddContainer("nginx", "nginx:latest")
    .WithBindMount("../nginx.conf", "/etc/nginx/nginx.conf", isReadOnly: true) // مپ کردن nginx.conf
    .WithHttpEndpoint(port: 8080, targetPort: 80, name: "http"); // پورت 8080

builder.Build().Run();

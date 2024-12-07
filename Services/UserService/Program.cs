using Core.Data;
using Core.Mapping;
using UserService.Data;
using UserService.Data.Seeding;
using UserService.Services.Token;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
}).AddJsonTranscoding();
builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddMappingServices(typeof(Program).Assembly);

builder.Services.AddDataServices();

builder.Services.AddJwtTokenServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.ApplyMigrations<UserDbContext>();
    await app.SeedAdmin();
    await app.SeedRoles();
}

app.MapGrpcService<UserService.Grpc.UserGrpcService>();

app.Run();
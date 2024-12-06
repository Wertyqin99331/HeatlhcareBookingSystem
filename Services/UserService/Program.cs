using Core.Mapping;
using UserService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddMappingServices(typeof(Program).Assembly);

builder.Services.AddDataServices();

var app = builder.Build();

app.MapGrpcService<UserService.UserService.UserServiceBase>();

app.Run();
using AuthService.Registrations;
using Cagnaz.Family.Infrastructure.Registration;
using Cagnaz.Family.Infrastructure.Repositorys;
using ExceptionHandling.filters;
using ExceptionHandling.Middlewares;
using MQService.Registrations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(opt=>opt.Filters.Add<ValidationFilter>());
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositorys();
builder.Services.AddAuthenticationServices(builder.Configuration["AppSettings:TokenKey"]);
builder.Services.AddMQService<UnitOfWork>(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();




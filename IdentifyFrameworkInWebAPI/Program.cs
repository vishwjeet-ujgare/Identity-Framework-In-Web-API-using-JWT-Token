using IdentifyFrameworkInWebAPI.Data;
using IdentityFrameworkInWebAPI.Service.Models;
using IdentityFrameworkInWebAPI.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit;


var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
// Add services to the container.



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//For entity framwork
builder.Services.AddDbContext<IFWebAPIDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IFWebAPIDB")));


//registering identity 
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<IFWebAPIDBContext>()
    .AddDefaultTokenProviders();

//add config for require email 
builder.Services.Configure<IdentityOptions>(opts => opts.SignIn.RequireConfirmedEmail = true);

//For authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
});

//For Email Configuration
var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

builder.Services.AddScoped<IEmailService, EmailService>();






var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

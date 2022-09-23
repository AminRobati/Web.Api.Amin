using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Web.Api.Amin.Models.Contexts;
using Web.Api.Amin.Models.Services;
using Web.Api.Amin.Models.Services.Validator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{

    

    var security = new OpenApiSecurityScheme
    {
        Name = "JWT Auth",
        Description = "توکن خود را وارد کنید- دقت کنید فقط توکن را وارد کنید",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(security.Reference.Id, security);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { security , new string[]{ } }
                });

});
        


string connection = "Data Source=.;Initial Catalog=WebApi;Integrated Security=True;MultipleActiveResultSets=true";

builder.Services.AddEntityFrameworkSqlServer().AddDbContext<DataBaseContext>(option => option.UseSqlServer(connection));

builder.Services.AddScoped<TodoRepository, TodoRepository>();
builder.Services.AddScoped<CategoryRepository, CategoryRepository>();
builder.Services.AddScoped<UserRipository, UserRipository>();
builder.Services.AddScoped<UserTokenRipository, UserTokenRipository>();

builder.Services.AddApiVersioning(Options =>
{
    Options.AssumeDefaultVersionWhenUnspecified = true;
    Options.DefaultApiVersion = new ApiVersion(1, 0);
    Options.ReportApiVersions = true;
});


builder.Services.AddAuthentication(Options =>
{
    Options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
           .AddJwtBearer(configureOptions =>
           {
               configureOptions.TokenValidationParameters = new TokenValidationParameters()
               {
                   ValidIssuer = builder.Configuration["JWtConfig:issuer"],
                   ValidAudience = builder.Configuration["JWtConfig:audience"],
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWtConfig:Key"])),
                   ValidateIssuerSigningKey = true,
                   ValidateLifetime = true,
               };
               configureOptions.SaveToken = true; // HttpContext.GetTokenAsunc();
               configureOptions.Events = new JwtBearerEvents
               {
                   OnAuthenticationFailed = context =>
                   {
                       //log 
                       //........
                       return Task.CompletedTask;
                   },
                   OnTokenValidated = context =>
                   {
                       //log

                       var tokenValidatorService = context.HttpContext.RequestServices.GetRequiredService<ITokenValidator>();
                       return tokenValidatorService.Execute(context);
                   },
                   OnChallenge = context =>
                   {
                       return Task.CompletedTask;

                   },
                   OnMessageReceived = context =>
                   {
                       return Task.CompletedTask;

                   },
                   OnForbidden = context =>
                   {
                       return Task.CompletedTask;

                   }
               };

           });


builder.Services.AddScoped<ITokenValidator, TokenValidate>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthorization();

app.MapControllers();

app.Run();

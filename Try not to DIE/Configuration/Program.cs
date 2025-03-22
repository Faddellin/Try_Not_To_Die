using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Try_not_to_DIE.Configuration;
using Try_not_to_DIE.DBContext;
using Try_not_to_DIE.Models.Comment;
using Try_not_to_DIE.Models.Consultation;
using Try_not_to_DIE.Models.Diagnosis;
using Try_not_to_DIE.Models.Doctor;
using Try_not_to_DIE.Models.EmailNotification;
using Try_not_to_DIE.Models.Icd10;
using Try_not_to_DIE.Models.Inspection;
using Try_not_to_DIE.Models.Patient;
using Try_not_to_DIE.Models.Speciality;
using Try_not_to_DIE.Services;
using Try_not_to_DIE.ServicesInterfaces;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<CommentRepository>();
builder.Services.AddScoped<ConsultationRepository>();
builder.Services.AddScoped<DiagnosRepository>();
builder.Services.AddScoped<DoctorRepository>();
builder.Services.AddScoped<Icd10Repository>();
builder.Services.AddScoped<InspectionRepository>();
builder.Services.AddScoped<PatientRepository>();
builder.Services.AddScoped<SpecialityRepository>();


builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<ConsultationService>();
builder.Services.AddScoped<DBCheckerService>();
builder.Services.AddScoped<DiagnosisService>();
builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<Icd10Service>();
builder.Services.AddScoped<InspectionService>();
builder.Services.AddScoped<PageService>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<SpecialityService>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<IJsonReaderService, JsonReaderService>();
builder.Services.AddScoped<UserService>();

//builder.Services.AddScoped<EmailSending>();

builder.Services.AddScoped<EmailSendingService>();

builder.Services.AddTransient<EmailJobFactory>();
builder.Services.AddScoped<GmailSender>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = configuration["AppConfig:AuthConfig:ISSUER"],
            ValidAudience = configuration["AppConfig:AuthConfig:AUDIENCE"],
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["AppConfig:AuthConfig:KEY"]))
        };
    });

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
builder.Services.AddSwaggerGen(o =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));


    var securityScheme = new OpenApiSecurityScheme()
        {
            Reference = new OpenApiReference
            {
                Type=ReferenceType.SecurityScheme,
                Id = JwtBearerDefaults.AuthenticationScheme
            },
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            Name = "Authentication",
        };

    o.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

    o.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            securityScheme,
            new List<string>()
        }
    });
});

builder.Services.Configure<AppConfig>(configuration.GetSection("AppConfig"));

builder.Services.AddDbContext<HospitalContext>(o =>
{
    o.UseNpgsql(configuration["DbConnectionString"]);
});
var app = builder.Build();


var serviceProvider = builder.Services.BuildServiceProvider();
using (var scope = serviceProvider.CreateScope())
{

    try
    {
        EmailSendingService.StartBackgroundTasks(serviceProvider);
    }
    catch (Exception)
    {
        throw;
    }
}

//WebHost.CreateDefaultBuilder(args).UseUrls("http://192.168.0.86:5002");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Notes.Application.Common.Mappings;
using Notes.Application.Interfaces;
using Notes.Persistance;
using Notes.Application;
using Notes.Persistance;
using Notes.Application.Notes.Queries.GetNoteList;
using Notes.WebApi;
using Notes.WebApi.Middleware;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

builder.Services.AddAutoMapper((IMapperConfigurationExpression config) =>
{
    config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
    config.AddProfile(new AssemblyMappingProfile(typeof(INotesDbContext).Assembly));
});

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetNoteListQuery).Assembly));

builder.Services.AddApplication();
builder.Services.AddPersistence(configuration);
builder.Services.AddControllers();
builder.Services.AddApiVersioning();
builder.Services.AddSwaggerGen(config =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    config.IncludeXmlComments(xmlPath);
});

builder.Services.AddCors(option =>
{
    option.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddAuthentication(config =>
    {
        config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "http://localhost:5054";
        options.Audience = "NotesWebAPI";
        options.RequireHttpsMetadata = false;
    });
builder.Services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var context = serviceProvider.GetRequiredService<NotesDbContext>();
        DbInitializer.Initialize(context);
    }
    catch(Exception ex)
    {

    }
}

IApiVersionDescriptionProvider provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseSwagger();
app.UseSwaggerUI(config =>
{
    foreach (var description in provider.ApiVersionDescriptions)
    {
        config.SwaggerEndpoint(
            $"/swagger/{description.GroupName}/swagger.json",
            description.GroupName.ToUpperInvariant());
        config.RoutePrefix = string.Empty;
    }
});
app.UseCustomExceptionHandler();
app.UseRouting();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseApiVersioning();
app.UseEndpoints(endoints =>
{
    endoints.MapControllers();
});

app.Run();

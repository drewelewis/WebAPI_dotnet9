using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Net;
using WebAPI_dotnet9;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IStorage, Storage>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () =>
{
    return Results.Redirect("/form");
});


app.MapGet("/form", () =>
{
    var htmlContent = @"
    <!DOCTYPE html>
    <html>
    <head>
        <title>Upload Page</title>
    </head>
    <body>
        <h1>Upload your files</h1>
        <form action='/upload_file' method='post' enctype='multipart/form-data'>
            <input type='file' name='files' multiple />
            <button type='submit'>Upload</button>
        </form>
    </body>
    </html>";

    return Results.Content(htmlContent, "text/html");
}).WithOpenApi();

app.MapGet("/uploads", (IStorage storage) =>
{
    return storage.GetAll();
}).WithOpenApi();

app.MapGet("/uploads/{guid}/status", (IStorage storage, Guid guid) =>
{
    return storage.Get(guid);
}).WithOpenApi();

//https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/Scenarios/Controllers/FireAndForgetController.cs
app.MapPost("/upload_file", [Microsoft.AspNetCore.Mvc.DisableRequestSizeLimit] (HttpContext http, HttpRequest request, IStorage storage) =>
{
    try
    {
        if (!request.HasFormContentType || !request.Form.Files.Any())
        {
            return Results.BadRequest("No files uploaded.");
        }
    }
    catch (InvalidDataException)
    {
        return Results.BadRequest("Max file size is limited to 2GB");
    }
    var file = request.Form.Files[0];
    var uploadItem = new UploadItem();
    uploadItem.Status = "Processing";
    uploadItem.FileName = file.FileName;
    storage.Add(uploadItem);

    Task.Run(async () =>
    {
        await UploadProcess.Upload (file);
        uploadItem.Status = "Complete";
        await storage.UpdateAsync(uploadItem);
    });

    var statusURL = "/uploads/" + uploadItem.Guid + "/status";

    http.Response.Headers.Append("location", statusURL);
    var response = Results.Accepted();



    return response;
}).WithOpenApi();

//app.MapPost("/upload_sync", [Microsoft.AspNetCore.Mvc.DisableRequestSizeLimit] async (HttpRequest request) =>
//{
//    if (!request.HasFormContentType || !request.Form.Files.Any())
//    {
//        return Results.BadRequest("No files uploaded.");
//    }

//    var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
//    Directory.CreateDirectory(uploadsDirectory);

//    foreach (var file in request.Form.Files)
//    {
//        var filePath = Path.Combine(uploadsDirectory, file.FileName);

//        using (var stream = new FileStream(filePath, FileMode.Create))
//        {
//            await file.CopyToAsync(stream);
//        }
//    }

//    return Results.Ok("Files uploaded successfully.");
//}).WithOpenApi();



app.Run();





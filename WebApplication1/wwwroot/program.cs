
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseStaticFiles();

app.MapGet("/", async (HttpContext context) =>
{
    var htmlpage = await File.ReadAllTextAsync("wwwroot/index.html");
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(htmlpage);
});

app.MapPost("/upload", async (HttpContext context) =>
{
    var title = context.Request.Form["image-title"];
    var file = context.Request.Form.Files.GetFile("image-file");

    if (string.IsNullOrEmpty(title))
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync(@"<h1 style=""font-size: 45px; padding-top:20px; text-align: center; color: red;"">  Please enter title for the image !</h1>");
        return;
    }

    if (file == null || file.Length == 0)
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync(@"<h1 style=""font-size: 45px; padding-top:20px; text-align: center; color: red;"">  Please choose an image !</h1>");
        return;
    }

    var fileextension = Path.GetExtension(file.FileName).ToLower();
    if (fileextension != ".jpeg" && fileextension != ".png" && fileextension != ".gif")
    {
        context.Response.StatusCode = 400;
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync(@"<h1 style=""font-size: 45px; padding-top:20px; text-align: center; color: red;"">  Please choose correct image extension !</h1>");
        return;
    }

    var imageid = Guid.NewGuid().ToString();
    var imagename = $"{imageid}{fileextension}";
    var imagepath = Path.Combine("uploads", imagename);

    var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory() ,"uploads");
    if (!Directory.Exists(uploadsDirectory))
    {
        Directory.CreateDirectory(uploadsDirectory);
    }

    using (var stream = new FileStream(imagepath, FileMode.Create))
    {
        await file.CopyToAsync(stream);
    }

    var imagesJsonPath = Path.Combine(uploadsDirectory, "images.json");
    var imageList = new List<UploadedImage>();
    if (File.Exists(imagesJsonPath))
    {
        var jsonContent = await File.ReadAllTextAsync(imagesJsonPath);
        imageList = JsonSerializer.Deserialize<List<UploadedImage>>(jsonContent);
    }
    else
    {
        imageList = new List<UploadedImage>();
    }

    var uploadedImage = new UploadedImage { Id = imageid, Title = title, FileName = file.FileName, Extension = fileextension };
    imageList.Add(uploadedImage);

    var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
    var jsonContentUpdated = JsonSerializer.Serialize(imageList, options);

    await File.WriteAllTextAsync(imagesJsonPath, $"{jsonContentUpdated}{Environment.NewLine}");

    context.Response.Redirect($"/picture/{imageid}");
});

app.MapGet("/picture/{id}", async (HttpContext context) =>
{
    var id = context.Request.RouteValues["id"].ToString();
    var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
    var imagesJsonPath = Path.Combine(uploadsDirectory, "images.json");
    var jsonContent = await File.ReadAllTextAsync(imagesJsonPath);
    List<UploadedImage> imageList = JsonSerializer.Deserialize<List<UploadedImage>>(jsonContent);
    var uploadedImage = imageList.FirstOrDefault(img => img.Id == id);

    if (uploadedImage == null)
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Image not found");
        return;
    }
    var html = $@"
 <!DOCTYPE html>
        <html>
        <head>
            <title>{uploadedImage.Title}</title>
</head>
<style>
body {{
    margin:0;
    padding:0;
}}

h1{{
 margin:0;
 font-size: 75px;
    color:black;
    font-family: Andalus;
    text-align: center;
    text-shadow:2px 2px black;
  }}
.image-container {{
    
    display: flex;
    justify-content: center;
    align-content: center;
    width: 50%;
    
    margin-left:350px;

}}
img{{
    
    width: 65%;
    
}}
.btn2{{
 display: flex;
    justify-content: center;
    align-content: center;
padding-top:15px;
}}
button{{
    padding: 12px;
    background-color:green;
    color:white;
    font-size:12px;
    border:none;
    
}}
</style>
<body>
<h1>{uploadedImage.Title}</h1>

<div class='image-container'>
 <img src='/uploaded-image/{uploadedImage.Id}'  width='100'  alt='{uploadedImage.Title}' />

 </div>
 <div class='btn2'>
 <button type='submit' name='submit' id='back-btn'>Back to form </button>
</div>

</body>
<script>
const myButton = document.getElementById('back-btn');
myButton.addEventListener('click', () => {{
  window.location.href = '/';
}});
</script>
</html>
";
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(html);
});

app.MapGet("/uploaded-image/{id}", async (string id) => {
    var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
    var imagesJsonPath = Path.Combine(uploadsDirectory, "images.json");
    var jsonContent = await File.ReadAllTextAsync(imagesJsonPath);

    var Listofimages = JsonSerializer.Deserialize<List<UploadedImage>>(jsonContent);
    var doc = Listofimages.FirstOrDefault(img => img.Id == id);
    string imagePath = Path.Combine(uploadsDirectory, $"{doc.Id}{doc.Extension}");
    return Results.File(imagePath, "image/png");
});

app.Run();

public class UploadedImage
{
    public UploadedImage()
    {
    }
    public string Id { get; set; }
    public string Title { get; set; }
    public string FileName { get; set; }
    public string Extension { get; set; }
}
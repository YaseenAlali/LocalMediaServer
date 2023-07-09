using System.Net.Sockets;
using System.Net;
using HF =  WebAppTest1.HelperFunctions.General;
using FS = WebAppTest1.HelperFunctions.FileSystem;
using SF = WebAppTest1.HelperFunctions.Security;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

//Dynamic local URL
app.Urls.Add(HF.GetLocalIPAddress());



FS.GetFilesFromDirectory(@"D:\stuff\music");
SF.generateOTP(1,5);
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
/*FileProvider =  new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Content")),
                RequestPath = new PathString("/Admin")*/
app.UseStaticFiles(new StaticFileOptions { ServeUnknownFileTypes = true,
	FileProvider = new PhysicalFileProvider(@"D:\stuff\music"),
	RequestPath = new PathString("")
}) ;

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

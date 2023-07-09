using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace WebAppTest1 { 
public static class AudioStreamer
{
    public static IActionResult StreamAudio(string filePath, HttpRequest request)
    {
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, FileOptions.Asynchronous);
        var range = request.Headers["Range"].ToString();
        var response = new FileStreamResult(fileStream, new MediaTypeHeaderValue("audio/mp3"))
        {
            EnableRangeProcessing = true
        };
        if (!String.IsNullOrEmpty(range))
        {
            var totalLength = fileStream.Length;
            var rangeParams = range.Split('=')[1].Split('-');
            var start = long.Parse(rangeParams[0]);
            var end = rangeParams.Length > 1 ? long.Parse(rangeParams[1]) : totalLength - 1;
            var length = end - start + 1;
            response.FileDownloadName = Path.GetFileName(filePath);
            response.LastModified = System.IO.File.GetLastWriteTimeUtc(filePath);
            response.EnableRangeProcessing = true;
            request.ContentType = "audio/mp3";
            // request.Headers.
            request.HttpContext.Response.Headers.Add("Content-Range", $"bytes {start}-{end}/{totalLength}");
            request.HttpContext.Response.ContentLength = length;
            fileStream.Seek(start, SeekOrigin.Begin);
            request.HttpContext.Response.StatusCode = StatusCodes.Status206PartialContent;

            //response.ContentType = new MediaTypeHeaderValue("audio/mp3");
            //response.HttpContext.Response.Headers.Add("Content-Range", $"bytes {start}-{end}/{totalLength}");
            //response.ContentLength = length;
            
            //response.StatusCode = StatusCodes.Status206PartialContent;
            
        }
        return response;
    }
}
}
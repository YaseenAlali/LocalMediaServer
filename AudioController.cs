using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace WebAppTest1
{
    public class AudioController : Controller
    {
        [HttpGet]
        public IActionResult StreamAudio(string fileName)
        {
            //var filePath = Path.Combine("path/to/audio/files/", fileName);
            
            if (!System.IO.File.Exists(fileName))
            {
                return NotFound();
            }

            var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            var range = Request.Headers["Range"].ToString();

            if (!string.IsNullOrEmpty(range))
            {
                var totalSize = fileStream.Length;
                var rangeString = range.Replace("bytes=", "");
                var rangeArray = rangeString.Split('-');
                var start = long.Parse(rangeArray[0]);
                var end = rangeArray.Length > 1 ? long.Parse(rangeArray[1]) : totalSize - 1;
                var length = end - start + 1;

                Response.Headers.Add("Content-Range", $"bytes {start}-{end}/{totalSize}");
                Response.StatusCode = 206;
                Response.ContentLength = length;
                fileStream.Seek(start, SeekOrigin.Begin);
                return new FileStreamResult(fileStream, "audio/mpeg");
            }
            else
            {
                return File(fileStream, "audio/mpeg");
            }
        }

    }
}

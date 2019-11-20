using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Stocks.CustomExceptionMiddleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public void writeToLogFile(string str)
        {
            string date = DateTime.Now.ToString("d-M-yyyy");
            var LogName = "log "+date+".txt";
            var logPath = Directory.GetCurrentDirectory()+ "/Logs/" + LogName;
            StreamWriter logWriter= new System.IO.StreamWriter(logPath, append: true);
            logWriter.WriteLine(str);
            logWriter.Dispose();
            logWriter.Close();
        }

        public async Task Invoke(HttpContext context)
        {
            string TempLog = "" ;
            //First, get the incoming request
            var request = await FormatRequest(context.Request);
            TempLog += " request : ";
            TempLog += request;
            TempLog += " Date : ";
            TempLog += DateTime.Now.ToString();


            //Copy a pointer to the original response body stream
            var originalBodyStream = context.Response.Body;

            //Create a new memory stream...
            using (var responseBody = new MemoryStream())
            {
                //...and use that for the temporary response body
                context.Response.Body = responseBody;

                //Continue down the Middleware pipeline, eventually returning to this class
                await _next(context);

                //Format the response from the server
                var response = await FormatResponse(context.Response);
                TempLog += "\n response : ";
                TempLog += response;
                TempLog += "\n";
                writeToLogFile(TempLog);

                //TODO: Save log to chosen datastore

                //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            var body = request.Body;
            var header = request.Headers; 
            //This line allows us to set the reader for the request back at the beginning of its stream.
            request.EnableRewind();

            //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
            var bodyBuffer = new byte[Convert.ToInt32(request.ContentLength)];
            //...Then we copy the entire request stream into the new buffer.
            await request.Body.ReadAsync(bodyBuffer, 0, bodyBuffer.Length);
            //We convert the byte[] into a string using UTF8 encoding...
            var bodyAsText = Encoding.UTF8.GetString(bodyBuffer);
            var TokenAsText = request.Headers["Authorization"].ToString();
            //read token
            var stream = TokenAsText.Replace("Bearer ","");
            string UserID = "";
            if (stream != "")
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(stream);
                var tokenS = handler.ReadToken(stream) as JwtSecurityToken;
                UserID = tokenS.Claims.First().Value;
           
            }
                // var userId = tokenS.Claims 
            return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText} {UserID}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            //We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return $"{response.StatusCode}: {text}";
        }
    }
}

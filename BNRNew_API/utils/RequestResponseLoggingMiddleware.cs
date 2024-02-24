using BNRNew_API.Controllers.user.dto;
using BNRNew_API.CustomeAttribute;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace BNRNew_API.utils
{
    public class RequestResponseLoggingMiddleware<T> where T : class, new()
    {
        private readonly RequestDelegate _next;
        string jwtSecret;
        string _rightDataPath = "rights"; 
        string[] skipUrl;

        public RequestResponseLoggingMiddleware(RequestDelegate next,  string jwtSecret)
        {
            this._next = next;
            this.jwtSecret = jwtSecret;
        }

        public RequestResponseLoggingMiddleware(RequestDelegate next, string jwtSecret, string rightDataPath)
        {
            this._next = next;
            this.jwtSecret = jwtSecret;
            this._rightDataPath = rightDataPath;
        }

        public async Task Invoke(HttpContext context)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var token = context.Request.Headers["Authorization"].FirstOrDefault();

            if (String.IsNullOrEmpty(token))
            {
                token = context.Request.Query["Authorization"].ToString();  
            }
            context.Items.Add("token", token);
            context.Items.Add("secret", jwtSecret);


            var skipLogging = false;
            if(skipUrl!=null) {
                if (!context.Request.Path.HasValue)
                {
                    foreach(var temp in skipUrl)
                    {
                        if (context.Request.Path.Equals(temp, StringComparison.OrdinalIgnoreCase))
                        {
                            skipLogging = true;
                            break;
                        }
                    }
                }
            }

            //if (token != null) {
            string requestChain = DateTime.Now.Ticks.ToString();

            long? memberId = null;
            var jwtsecret = (string)context.Items["secret"];


            //CallContext.SetData("jwtsecret", jwtsecret);

            try
            {
                var session = JWTHelper.validateToken<T>(jwtsecret, token);

                if (session != null)
                {
                    //memberId = session.user_id;
                    context.Items.Add("session", session);

                    List<PropertyInfo> p = new List<PropertyInfo>(session.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(JWTRightAttribute))));
                    if (p != null && p.Count > 0)
                        context.Items.Add("rights", p[0].GetValue(session));
                }
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException e)
            {

            }


            //First, get the incoming request            
            var requeststr = await FormatRequest(context.Request);
            object? obj = null;
            try
            {
                if (requeststr != null)
                    obj = JsonSerializer.Deserialize <Dictionary<string, object>>(requeststr);
            }
            catch (Exception e)
            {

            }

            HttpLogData requestLog = null;

            if (!skipLogging) { 
                requestLog = new HttpLogData() {
                    chain = requestChain,
                    scheme = context.Request.Scheme,
                    host = context.Request.Host.Host + ":" + context.Request.Host.Port,
                    Path = context.Request.Path,
                    request_data = obj,
                    query_str = context.Request.QueryString.ToString(),
                    httpcode = 0
                };
            }

            var body = context.Response.Body;
            using (var newBody = new MemoryStream())
            {
                context.Response.Body = newBody;
                try
                {
                    await _next(context);

                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
                    context.Response.Body.Seek(0, SeekOrigin.Begin);

                    context.Response.Body = body;
                    await newBody.CopyToAsync(body);

                    try
                    {
                        if (text.IsNullOrEmpty())
                            obj = JsonSerializer.Deserialize<Object>(text);
                    }
                    catch (Exception e)
                    {

                    }

                    watch.Stop();
                    if (!skipLogging) { 
                        requestLog.httpcode = context.Response.StatusCode;
                        requestLog.response_data = obj;
                        requestLog.duration_ms = watch.ElapsedMilliseconds;
                    }
                    System.Console.WriteLine("\r\n" + JsonSerializer.Serialize(requestLog) + "\r\n");
                }
                catch (Exception e)
                {
                    watch.Stop();
                    BaseDtoResponse errResult = new BaseDtoResponse
                    {
                        message = ""
                    };

                    int httpCode = 500;
                    errResult.message = e.Message;

                    if (e is UnauthorizedAccessException) {
                        httpCode = 403;
                    }else if (e is BadHttpRequestException)
                    {
                        httpCode = 400;
                    }
                    else if (e.InnerException != null && e.InnerException is SqliteException sqliteException) {
                        if (sqliteException.SqliteErrorCode == 19) {
                            httpCode = 400;
                            errResult.message = sqliteException.Message;
                            errResult.message = errResult.message.Replace("'", "");
                            var match = Regex.Match(errResult.message, @"(?<errorCode>.*):(?<type>.*)constraint failed:(?<column>.*)");
                            if (match.Success)
                            {
                                var type = match.Groups["type"].Value;
                                var column = match.Groups["column"].Value;
                                type = type.Trim();
                                column = column.Trim();

                                if (type == "NOT NULL")
                                    errResult.message = "data " + column + " tidak boleh kosong";
                                else if (type == "UNIQUE")
                                    errResult.message = "data " + column + " telah ada sebelumnya, mohon isi dengan nilai lainnya (tidak boleh double)";
                            }
                            else
                            {
                                var match1 = Regex.Match(errResult.message, @"(?<errorCode>.*): FOREIGN KEY constraint failed");
                                if (match1.Success)
                                    errResult.message = "Data tidak dapat di hapus karena telah di gunakan sebagai referensi di tempat lain";
                            }
                        }
                        else
                        {
                            httpCode = 500;
                            errResult.message = sqliteException.Message;
                        }

                        requestLog.exception = e.StackTrace;
                        requestLog.inner_exception = e.InnerException != null ? e.InnerException.StackTrace : "";
                        requestLog.exception_msg = errResult.message;
                    }

                    requestLog.response_data = errResult;
                    requestLog.duration_ms = watch.ElapsedMilliseconds;
                    Console.WriteLine(e);


                    context.Response.StatusCode = httpCode;
                    context.Response.ContentType = "application/json";
                    var result1 = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(errResult));
                    await body.WriteAsync(result1, 0, result1.Length);
                }
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();

            string requeststr = "";
            using (var newBody = new MemoryStream())
            {
                await request.Body.CopyToAsync(newBody);

                requeststr = ReadStreamInChunks(newBody);
            }

            request.Body.Position = 0;

            return requeststr;
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;

            stream.Seek(0, SeekOrigin.Begin);

            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);

            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;

            do
            {
                readChunkLength = reader.ReadBlock(readChunk,
                                                   0,
                                                   readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            return textWriter.ToString();
        }


        public class HttpLogData
        {
            public string chain { get; set; }
            public string scheme { get; set; }
            public string host { get; set; }
            public string Path { get; set; }
            public string query_str { get; set; }
            public int httpcode { get; set; }
            public object request_data { get; set; }
            public object response_data { get; set; }
            public object exception { get; set; }
            public object inner_exception { get; set; }
            public object exception_msg { get; set; }
            public string request_content_type { get; set; }
            public string trace_id { get; set; }
            public long duration_ms { get; set; }
        }
    }

}

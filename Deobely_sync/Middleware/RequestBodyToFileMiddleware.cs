using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class RequestBodyLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestBodyLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Capture the request body
        string requestBody = await ReadRequestBodyAsync(context.Request);

        // Log the request body (you can customize the logging mechanism)
        LogRequestBody(requestBody);

        // Continue processing the request
        await _next(context);
    }

    private async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        // Read the request body and return it as a string
        request.EnableBuffering(); // Allows reading the body multiple times
        using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Seek(0, SeekOrigin.Begin); // Reset the request body stream
        return request.Path + "-" + body;
    }

    private void LogRequestBody(string requestBody)
    {
        // Customize the logging mechanism here (e.g., write to a log file, database, or console)
        string fileName = Path.Combine("Logs/", $"DB{DateTime.Now:yyyyMMdd}.txt");

        try
        {
            if (!File.Exists(fileName))
            {
                File.Create(fileName).Close();
            }
            // Write the request body to the file
            File.AppendAllText(fileName, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: {requestBody}\n");
            Console.WriteLine($"Request body saved to file: {fileName}");
        }
        catch (Exception ex)
        {
            // Handle any potential exceptions here (e.g., logging the exception)
            Console.WriteLine($"Error saving request body to file: {ex.Message}");
        }
    }
}
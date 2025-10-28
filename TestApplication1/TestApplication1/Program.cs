var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!" + "Worker Process Name: " + System.Diagnostics.Process.GetCurrentProcess().ProcessName + 
"Time: "+ System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime);

app.Run();

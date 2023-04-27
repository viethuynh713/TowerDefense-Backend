using Microsoft.AspNetCore.Hosting;
using Realtime;

var hosting = Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(WebApplicationBuilder =>
{
    WebApplicationBuilder.UseStartup<Startup>();
});

hosting.Build().Run();

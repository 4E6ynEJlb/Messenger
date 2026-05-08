using Microsoft.AspNetCore.Http.Features;
using UserAPI.Extensions;

namespace UserAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = 2L * 1024 * 1024 * 1024;
            });
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 2L * 1024 * 1024 * 1024;
                options.MemoryBufferThreshold = 1 * 1024 * 1024;
            });


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.ConfigureSwaggerGen();
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Documentation.xml"));
            });

            builder.Services.AddAuthBuilderExtension(builder.Configuration);

            var app = builder.Build();
            app.AddAuthAppExtension();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();


            app.MapControllers();

            app.Run();
        }
    }
}

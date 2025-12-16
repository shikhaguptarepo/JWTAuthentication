using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JWTAuthentication API", Version = "v1" });
});

builder.Services.AddDbContext<JWTAuthentication.Data.AppDbContext>(options =>
    options.UseSqlServer(JWTAuthentication.ConfigurationManager.AppSetting.GetConnectionString("DefaultConnection")));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = JWTAuthentication.ConfigurationManager.AppSetting["Jwt:Issuer"],
                    ValidAudience = JWTAuthentication.ConfigurationManager.AppSetting["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTAuthentication.ConfigurationManager.AppSetting["Jwt:Key"]))
                };
            });
var app = builder.Build();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy",
        builder =>
        {
            builder.AllowAnyHeader()
                   .AllowAnyMethod()
                   .WithOrigins("http://localhost:5173");
        });
}
);


        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
       
          
    if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
   
}
app.UseHttpsRedirection();
app.UseCors("ReactPolicy");
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

app.Run();




       


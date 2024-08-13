using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MoviesApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connetionString = builder.Configuration.GetConnectionString("DefualtConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connetionString));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors();

builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = "v1",
		Title = "TestApi",
		Description = "First Api test version 1 ",
		TermsOfService = new Uri("https://google.com"),  // Corrected URI
		Contact = new OpenApiContact
		{
			Name = "Abdelrahman",
			Email = "test@ayhaga.com"
		}
	});
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
	{ 
		Name= "Authorization",
		Type=SecuritySchemeType.ApiKey,
		Scheme= "Bearer",
		BearerFormat="JWT",
		In=ParameterLocation.Header,
	});
	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference=new OpenApiReference
				{
					Type=ReferenceType.SecurityScheme,
					Id="Bearer"
					
				},
				Name="Bearer",
				In=ParameterLocation.Header
			},
			new List<string>()
		}

	});

});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthorization();

app.MapControllers();

app.Run();

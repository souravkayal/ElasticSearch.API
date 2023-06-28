using Elasticsearch.Net;
using Nest;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Set up the connection settings
var uri = new Uri("http://localhost:9200"); // Replace with your Elasticsearch URL
var connectionPool = new SingleNodeConnectionPool(uri);
var connectionSettings = new ConnectionSettings(connectionPool);

// Set the username and password
var username = "elastic";
var password = "elastic";

connectionSettings.BasicAuthentication(username , password);

// Create the Elasticsearch client
var client = new ElasticClient(connectionSettings);

// Register the Elasticsearch client as a singleton service
builder.Services.AddSingleton<IElasticClient>(client);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

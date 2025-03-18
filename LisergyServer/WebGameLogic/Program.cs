using GameDataTest;
using PlayFab;
using WebGameLogic;
using WebGameLogic.Playfab;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IServerConfig, ServerConfig>();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.MapControllers();
app.UseHttpsRedirection();

var serverConfig = app.Services.GetRequiredService<IServerConfig>();
if (serverConfig != null)
{
    PlayFabSettings.staticSettings.TitleId = serverConfig.Title;
    PlayFabSettings.staticSettings.DeveloperSecretKey = serverConfig.TitleKey;

    var setup = new PlayfabSetup(TestSpecs.Generate());
    await setup.SetupPlayfab();
}
else
{
    // Log error or throw exception
    throw new InvalidOperationException("Server configuration could not be loaded.");
}

app.Run();

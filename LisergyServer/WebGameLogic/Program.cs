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

var serverConfig = app.Services.GetService(typeof(IServerConfig)) as IServerConfig;

PlayFabSettings.staticSettings.TitleId = serverConfig.Title;
PlayFabSettings.staticSettings.DeveloperSecretKey = serverConfig.TitleKey;

var setup = new PlayfabSetup(TestSpecs.Generate());
await setup.SetupPlayfab();

app.Run();

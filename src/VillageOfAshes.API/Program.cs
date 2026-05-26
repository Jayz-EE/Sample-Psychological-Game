using VillageOfAshes.Core.Services;
using VillageOfAshes.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register game services
builder.Services.AddSingleton<ITimeManager, TimeManager>();
builder.Services.AddSingleton<INightSimulationService, NightSimulationService>();
builder.Services.AddSingleton<ISuspicionCalculator, SuspicionCalculator>();
builder.Services.AddSingleton<IDialogueService, DialogueService>();
builder.Services.AddSingleton<IRumorService, RumorService>();
builder.Services.AddSingleton<ICouncilService, CouncilService>();
builder.Services.AddSingleton<IObservationService, ObservationService>();
builder.Services.AddSingleton<IBehaviorAnalysisService, BehaviorAnalysisService>();
builder.Services.AddSingleton<IGameProgressionService, GameProgressionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

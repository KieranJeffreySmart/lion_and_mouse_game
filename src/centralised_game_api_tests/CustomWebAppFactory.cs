using lion_and_mouse_game.Events;
using lion_and_mouse_game.LionContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace centralised_game_api_tests;

public class CustomWebApplicationFactory<TProgram>: WebApplicationFactory<TProgram> where TProgram : class
{
    CustomisableLionBehaviorCalculator lionBehaviorCalculator = new CustomisableLionBehaviorCalculator();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(s =>
        {
            s.AddSingleton<ILionBehaviorCalculator>(lionBehaviorCalculator);
        });
    }

    public void SetLionBehavior(Func<LionEngine, NewDayEvent, LionBehaviours> calculate)
    {
        lionBehaviorCalculator.CalculateFunc = calculate;
    }
}

internal class CustomisableLionBehaviorCalculator : ILionBehaviorCalculator
{
    public Func<LionEngine, NewDayEvent, LionBehaviours>? CalculateFunc { get; set; }
    public LionBehaviours Calculate(LionEngine lionEngine, NewDayEvent gameEvent)
    {
        return CalculateFunc != null ? CalculateFunc(lionEngine, gameEvent) : throw new NotImplementedException();
    }
}
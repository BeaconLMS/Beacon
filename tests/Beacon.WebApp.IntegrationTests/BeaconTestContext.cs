using BeaconUI.Core;

namespace Beacon.WebApp.IntegrationTests;

public abstract class BeaconTestContext : TestContext
{
    protected void SetupCoreServices()
    {
        Services.AddBeaconUI();
        JSInterop.SetupModule("./_content/Blazored.Modal/BlazoredModal.razor.js");
    }
}

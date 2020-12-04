namespace ESP32NodeMCU
{
  using System.Diagnostics;
  using System.Threading;
  using Windows.Devices.WiFi;

  public class Program
  {
    public static void Main()
    {
      WiFiAdapter[] adapters = WiFiAdapter.FindAllAdapters();
      if (adapters.Length == 1)
      {
        WiFiAdapter adapter = adapters[0];
        adapter.AvailableNetworksChanged += Adapter_AvailableNetworksChanged;
        adapter.ScanAsync(); 
      }

      Thread.Sleep(Timeout.Infinite);

      // Browse our samples repository: https://github.com/nanoframework/samples
      // Check our documentation online: https://docs.nanoframework.net/
      // Join our lively Discord community: https://discord.gg/gCyBu8T
    }

    private static void Adapter_AvailableNetworksChanged(WiFiAdapter sender, object e)
    {
      Debug.WriteLine(string.Empty);
      WiFiAvailableNetwork[] networks = sender.NetworkReport.AvailableNetworks;
      Debug.WriteLine($"Network scan complete, found {networks.Length} networks");
      foreach (var network in networks)
      {
        Debug.WriteLine(network.Ssid);
      }

      sender.ScanAsync();
    }
  }
}

// <copyright file="Program.cs" company="RolandoMagico">
// MIT License
//
// Copyright (c) 2020 by RolandoMagico
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

namespace ESP32NodeMCU
{
  using System.Diagnostics;
  using System.Threading;
  using Windows.Devices.WiFi;

  /// <summary>
  /// Entry class of the application.
  /// </summary>
  public class Program
  {
    /// <summary>
    /// Entry method of the application.
    /// </summary>
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

    /// <summary>
    /// Handles the <see cref="WiFiAdapter.AvailableNetworksChanged"/> event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments.</param>
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

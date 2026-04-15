using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Net.NetworkInformation;
using System.Timers;

namespace NetOpsConsole;

public class MonitorService(IHostApplicationLifetime hostApplicationLifetime) : IHostedService
{
    public List<Node> nodes;
    private static System.Timers.Timer aTimer;
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        hostApplicationLifetime.ApplicationStarted.Register(OnStarted);
        hostApplicationLifetime.ApplicationStopping.Register(OnStopping);
        hostApplicationLifetime.ApplicationStopped.Register(OnStopped);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private void ConstructNodes()
    {
        Console.WriteLine("Starting to Construct Nodes");

        try
        {
            var json = File.ReadAllText("nodes.json");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var data = JsonSerializer.Deserialize<NodeDataFile>(json, options);

            if (data != null)
            {
                nodes = data.Nodes;
            }

            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Generic error occured: {ex.Message}");
            throw;
        }
    }

    private void SetTimer()
    {
        aTimer = new(5000);
        aTimer.Elapsed += PingNodes;
        aTimer.AutoReset = true;
        aTimer.Enabled = true;
    }
    private void PingNodes(Object source, ElapsedEventArgs e)
    {
        Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
            e.SignalTime);
        using var ping = new Ping();
        foreach (var node in nodes)
        {
            try
            {
                var checkPing = ping.Send(node.Address);

                node.isReachable = checkPing.Status == IPStatus.Success;

                if (node.isReachable)
                {
                    node.Ping = checkPing.RoundtripTime;
                }
                
                Console.WriteLine($"Node id: {node.Id}, address: {node.Address}, reachable: {node.isReachable}, latency: {node.Ping} ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Generic error occured: {ex.Message}");
                throw;
            }
            
        }
    }
    private void OnStarted()
    {
        SetTimer();
        Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
        // Read from json to see what we have currently as a node
        ConstructNodes();

    }

    private void OnStopping()
    {
        throw new NotImplementedException();
    }

    private void OnStopped()
    {
        throw new NotImplementedException();
    }
}
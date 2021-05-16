using Medo.Net.Influx;
using System;
using System.Threading;

namespace Example {
    internal class App {

        public static Random Random = new();

        private static void Main(string[] args) {
            var client = InfluxClient.V2(
                serverUrl: new Uri("http://localhost:8086"),
                organization: "Test",
                bucket: "example",
                token: "LvgKoRh00PrHxxV9exx6pCcjjeRdkzi-wuusx6eoSmP_VVVKIFpw9YIBXmRlPG0pk71MhERe9FFRqBna4TTsJQ==",
                resolution: InfluxTimestampResolution.Miliseconds
                );

            // setup batching for shorter interval and smaller batch size
            client.MaxBatchInterval = 1;    // send batch every second
            client.MaxBatchSize = 5;       // batch up to 5 items together
            client.BatchRetryInterval = 1;  // retry after 1 second

            // attach to BatchFailed event
            client.BatchFailed += delegate (object? sender, InfluxBatchEventArgs e) {
                Console.WriteLine($"{e.Result.ErrorText} ({e.BatchSize})");
            };

            // attach to BatchSucceeded event
            client.BatchSucceeded += delegate (object? sender, InfluxBatchEventArgs e) {
                Console.Write(new string('+', e.BatchSize));
            };

            while (true) {  // just loop forever

                {  // Tick count - queue example
                    var measurement = new InfluxMeasurement("Tick");
                    measurement
                        .AddTag("MachineName", Environment.MachineName)
                        .AddTag("UserName", Environment.UserName)
                        .AddField("TickCount", Environment.TickCount)
                        .AddField("TickCount64", Environment.TickCount64);
                    client.Queue(measurement);
                }

                {  // GC - queue example
                    var measurement = new InfluxMeasurement("GC");
                    measurement.AddTag("MachineName", Environment.MachineName);
                    measurement.AddTag("UserName", Environment.UserName);
                    measurement.AddField("TotalMemory", GC.GetTotalMemory(false));
                    measurement.AddField("AllocatedBytes", GC.GetTotalAllocatedBytes(false));
                    client.Queue(measurement);
                }

                {  // random numbers - imediate sending example
                    var measurement = new InfluxMeasurement("Random");
                    measurement.Tags.Add("MachineName", Environment.MachineName);
                    measurement.Tags.Add("UserName", Environment.UserName);
                    measurement.Fields.Add("Random", Random.Next(0, 100));
                    var res = client.Send(measurement);
                    if (!res) { Console.WriteLine(res.ErrorText); }
                }

                Console.Write('.');
                Thread.Sleep(250);  // repeat every 250 ms or so
            }
        }

    }
}

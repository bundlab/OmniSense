using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OmniSense.Hardware;
using OmniSense.Intelligence;

namespace OmniSense.Gateway
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("======================================================");
            Console.WriteLine(" OmniSense .NET Gateway: IoT & Edge ML.NET Integration");
            Console.WriteLine("======================================================\n");

            using var sensorReader = new SensorReader(statusLedPin: 18);
            var detector = new AnomalyDetector();

            // 1. Gather baseline data for the ML model
            Console.WriteLine("[Gateway] Collecting baseline sensor stream for ML training...");
            List<float> baseline = new();
            for (int i = 0; i < 30; i++)
            {
                var payload = sensorReader.ReadTelemetry(injectSpike: false);
                baseline.Add(payload.Value);
                Thread.Sleep(50);
            }

            // 2. Train model
            detector.Train(baseline);
            Console.WriteLine("[Gateway] Anomaly detection model trained successfully.\n");

            // 3. Start monitoring telemetry loop
            Console.WriteLine("[Gateway] Starting live telemetry monitoring (Press Ctrl+C to exit)...");
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine($"{"Timestamp",-12} | {"Sensor Value",-12} | {"Status",-8} | {"P-Value",-8}");
            Console.WriteLine("------------------------------------------------------------------");

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) => { e.Cancel = true; cts.Cancel(); };

            int counter = 0;
            while (!cts.Token.IsCancellationRequested)
            {
                counter++;
                bool injectSpike = (counter % 7 == 0); // Inject test anomaly every 7th tick

                var telemetry = sensorReader.ReadTelemetry(injectSpike);
                var inference = detector.Evaluate(telemetry.Value);

                string statusStr = inference.IsAnomaly ? "ALERT" : "NORMAL";
                
                if (inference.IsAnomaly)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                Console.WriteLine($"{telemetry.Timestamp:HH:mm:ss.fff} | {telemetry.Value,12:F2} | {statusStr,-8} | {inference.PValue,8:F4}");
                Console.ResetColor();

                try
                {
                    await Task.Delay(1000, cts.Token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            Console.WriteLine("\n[Gateway] Shutting down gracefully.");
        }
    }
}
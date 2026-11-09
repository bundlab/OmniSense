using System;

namespace OmniSense.Core
{
    public class TelemetryPayload
    {
        public string DeviceId { get; set; } = "EdgeNode-01";
        public string SensorType { get; set; } = "Environment";
        public float Value { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class InferenceResult
    {
        public bool IsAnomaly { get; set; }
        public double RawScore { get; set; }
        public double PValue { get; set; }
        public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;
    }
}
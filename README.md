# OmniSense .NET 🚀

**OmniSense .NET** is a modular, high-performance edge AI and IoT telemetry gateway built from the ground up in C#. It bridges physical IoT hardware sensors with real-time on-device machine learning inference, enabling single-board computers (like the Raspberry Pi) to monitor telemetry streams and detect anomalies instantly without cloud dependency.

---

## 🏗️ Architecture & Project Structure

OmniSense is organized as a clean, multi-project solution designed for separation of concerns and testability:

```text
OmniSense/
├── OmniSense.Core/          # Shared domain contracts, models, and data types
├── OmniSense.Hardware/      # GPIO and sensor binding layer (System.Device.Gpio)
├── OmniSense.Intelligence/  # On-device ML.NET time-series anomaly detection engine
└── OmniSense.Gateway/       # Core orchestration runtime and execution loop
```

### Module Breakdown

* **`OmniSense.Core`**: Defines standard domain entities (`TelemetryPayload`, `InferenceResult`) utilized across hardware ingestion and ML pipelines.
* **`OmniSense.Hardware`**: Interfaces directly with physical hardware pins using `System.Device.Gpio`. Includes robust fallback logic to automatically switch to simulated sensor mode when executed on standard desktop operating systems (Linux, Windows, macOS).
* **`OmniSense.Intelligence`**: Powered by **ML.NET**, utilizing independent and identically distributed time-series spike detection (`DetectIidSpike`) to evaluate live sensor metrics and compute statistical anomaly confidence scores.
* **`OmniSense.Gateway`**: Coordinates hardware data acquisition, feeds streams into the on-device ML model, and logs real-time telemetry events and alerts to the console.

---

## 📦 Prerequisites & Dependencies

* **.NET 8 SDK** or higher installed on your system.
* Required NuGet packages (automatically resolved via project files):
* `Microsoft.ML` (v3.0.1)
* `Microsoft.ML.TimeSeries` (v3.0.1)
* `System.Device.Gpio` (v3.1.0+)



---

## ⚙️ Getting Started & Running

1. Clone or navigate to your solution root directory (`OmniSense/`).
2. Restore dependencies, build the solution, and run the gateway:

```bash
dotnet restore OmniSense.sln
dotnet build OmniSense.sln
dotnet run --project OmniSense.Gateway/OmniSense.Gateway.csproj
```

---

## 💻 Sample Output

When executed on a workstation, OmniSense automatically initializes in simulated hardware mode, trains its internal baseline model, and begins real-time telemetry streaming:

```text
======================================================
 OmniSense .NET Gateway: IoT & Edge ML.NET Integration
======================================================

[Hardware] GPIO hardware/libgpiod not available (GpiodException). Running in simulated hardware mode.
[Gateway] Collecting baseline sensor stream for ML training...
[Gateway] Anomaly detection model trained successfully.

[Gateway] Starting live telemetry monitoring (Press Ctrl+C to exit)...
------------------------------------------------------------------
Timestamp    | Sensor Value | Status   | P-Value 
------------------------------------------------------------------
10:49:03.250 |        23.74 | NORMAL   |   0.5000
10:49:04.323 |        23.62 | NORMAL   |   0.4513
10:49:05.363 |        39.44 | ALERT    |   0.0008
```

---

## 📄 License

This project is open-source and available under the [MIT License](LICENSE).
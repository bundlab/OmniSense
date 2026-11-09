using System;
using System.Device.Gpio;
using System.Threading;
using OmniSense.Core;

namespace OmniSense.Hardware
{
    public class SensorReader : IDisposable
    {
        private readonly GpioController? _gpioController;
        private readonly int _statusLedPin;
        private readonly Random _random = new();

        public SensorReader(int statusLedPin = 18)
        {
            _statusLedPin = statusLedPin;

            try
            {
                // Initializes GPIO controller on supported SBC platforms (e.g., Raspberry Pi)
                _gpioController = new GpioController();
                _gpioController.OpenPin(_statusLedPin, PinMode.Output);
                Console.WriteLine($"[Hardware] GPIO Controller initialized. Status LED on Pin {_statusLedPin}.");
            }
            catch (Exception ex)
            {
                // Graceful fallback for running/testing on desktop Linux/Windows/macOS without GPIO hardware or libgpiod
                Console.WriteLine($"[Hardware] GPIO hardware/libgpiod not available ({ex.GetType().Name}). Running in simulated hardware mode.");
                _gpioController = null;
            }
        }

        public TelemetryPayload ReadTelemetry(bool injectSpike = false)
        {
            ToggleLed(PinValue.High);
            Thread.Sleep(40);
            ToggleLed(PinValue.Low);

            float baseValue = 23.5f + (float)(_random.NextDouble() * 3.0 - 1.5);
            if (injectSpike)
            {
                baseValue += 16.0f; // Simulate temperature anomaly spike
            }

            return new TelemetryPayload
            {
                DeviceId = "EdgeNode-Pi01",
                SensorType = "Temperature",
                Value = baseValue,
                Timestamp = DateTime.UtcNow
            };
        }

        private void ToggleLed(PinValue value)
        {
            if (_gpioController != null)
            {
                try
                {
                    if (_gpioController.IsPinOpen(_statusLedPin))
                    {
                        _gpioController.Write(_statusLedPin, value);
                    }
                }
                catch
                {
                    // Suppress LED write errors on simulated hardware
                }
            }
        }

        public void Dispose()
        {
            try
            {
                _gpioController?.Dispose();
            }
            catch
            {
                // Suppress disposal errors on simulated hardware
            }
        }
    }
}
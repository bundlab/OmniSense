using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using OmniSense.Core;

namespace OmniSense.Intelligence
{
    public class MlInput
    {
        public float Value { get; set; }
    }

    public class MlPrediction
    {
        [VectorType(3)]
        public double[] Prediction { get; set; } = Array.Empty<double>();
    }

    public class AnomalyDetector
    {
        private readonly MLContext _mlContext;
        private TimeSeriesPredictionEngine<MlInput, MlPrediction>? _predictionEngine;

        public AnomalyDetector()
        {
            _mlContext = new MLContext(seed: 42);
        }

        public void Train(List<float> baselineValues)
        {
            var dataList = new List<MlInput>();
            foreach (var val in baselineValues)
            {
                dataList.Add(new MlInput { Value = val });
            }

            IDataView dataView = _mlContext.Data.LoadFromEnumerable(dataList);

            // Corrected to DetectIidSpike for independent/identically distributed time-series spike detection
            var pipeline = _mlContext.Transforms.DetectIidSpike(
                outputColumnName: nameof(MlPrediction.Prediction),
                inputColumnName: nameof(MlInput.Value),
                confidence: 95.0,
                pvalueHistoryLength: 10);

            var model = pipeline.Fit(dataView);
            _predictionEngine = model.CreateTimeSeriesEngine<MlInput, MlPrediction>(_mlContext);
        }

        public InferenceResult Evaluate(float value)
        {
            if (_predictionEngine == null)
            {
                throw new InvalidOperationException("Model has not been trained yet.");
            }

            var prediction = _predictionEngine.Predict(new MlInput { Value = value });

            return new InferenceResult
            {
                IsAnomaly = prediction.Prediction[0] == 1.0,
                RawScore = prediction.Prediction[1],
                PValue = prediction.Prediction[2],
                EvaluatedAt = DateTime.UtcNow
            };
        }
    }
}
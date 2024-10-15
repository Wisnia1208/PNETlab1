using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SensorDataProcessor
{
    private readonly ISensor _sensor;

    public SensorDataProcessor(ISensor sensor)
    {
        _sensor = sensor;
    }

    // Metoda obliczaj¹ca œredni¹ wartoœæ
    public double CalculateAverage(List<double?> values)
    {
        var validValues = values.Where(v => v.HasValue).Select(v => v.Value).ToList();
        if (validValues.Count == 0)
        {
            throw new InvalidOperationException("Brak wartoœci do obliczenia œredniej.");
        }
        return validValues.Average();
    }

    // Metoda obliczaj¹ca odchylenie standardowe
    public double CalculateStandardDeviation(List<double?> values)
    {
        var validValues = values.Where(v => v.HasValue).Select(v => v.Value).ToList();
        if (validValues.Count == 0)
        {
            throw new InvalidOperationException("Brak wartoœci do obliczenia odchylenia standardowego.");
        }
        double average = validValues.Average();
        double sumOfSquaresOfDifferences = validValues.Select(val => (val - average) * (val - average)).Sum();
        return Math.Sqrt(sumOfSquaresOfDifferences / validValues.Count);
    }

    // Metoda sortuj¹ca listê wartoœci
    public List<double?> SortValues(List<double?> values)
    {
        return values.OrderBy(v => v).ToList();
    }

    // Metoda zapisuj¹ca listê wartoœci do pliku
    public void SaveValuesToFile(List<double?> values, string fileName)
    {
        _sensor.SaveToFile(values, fileName);
    }

    // Metoda usuwaj¹ca wartoœci z okreœlonego zakresu
    public List<double?> RemoveValuesInRange(List<double?> values, double minValue, double maxValue)
    {
        return values.Where(v => !v.HasValue || v < minValue || v > maxValue).ToList();
    }
}

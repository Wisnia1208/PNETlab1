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

    // Metoda obliczaj�ca �redni� warto��
    public double CalculateAverage(List<double?> values)
    {
        var validValues = values.Where(v => v.HasValue).Select(v => v.Value).ToList();
        if (validValues.Count == 0)
        {
            throw new InvalidOperationException("Brak warto�ci do obliczenia �redniej.");
        }
        return validValues.Average();
    }

    // Metoda obliczaj�ca odchylenie standardowe
    public double CalculateStandardDeviation(List<double?> values)
    {
        var validValues = values.Where(v => v.HasValue).Select(v => v.Value).ToList();
        if (validValues.Count == 0)
        {
            throw new InvalidOperationException("Brak warto�ci do obliczenia odchylenia standardowego.");
        }
        double average = validValues.Average();
        double sumOfSquaresOfDifferences = validValues.Select(val => (val - average) * (val - average)).Sum();
        return Math.Sqrt(sumOfSquaresOfDifferences / validValues.Count);
    }

    // Metoda sortuj�ca list� warto�ci
    public List<double?> SortValues(List<double?> values)
    {
        return values.OrderBy(v => v).ToList();
    }

    // Metoda zapisuj�ca list� warto�ci do pliku
    public void SaveValuesToFile(List<double?> values, string fileName)
    {
        _sensor.SaveToFile(values, fileName);
    }

    // Metoda usuwaj�ca warto�ci z okre�lonego zakresu
    public List<double?> RemoveValuesInRange(List<double?> values, double minValue, double maxValue)
    {
        return values.Where(v => !v.HasValue || v < minValue || v > maxValue).ToList();
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class TemperatureSensor : ISensor
{
    private static Random random = new Random();

    // Metoda generująca pojedynczą wartość z przedziału [-100, 100]
    public double? GenerateSingleValue()
    {
        double value = random.NextDouble() * 200 - 100; // Generuje wartość z przedziału [-100, 100]
        if (value < -80)
        {
            return null; // Wartość poniżej -80 jest interpretowana jako błędny odczyt
        }
        return value;
    }

    // Metoda generująca N wartości zmiennoprzecinkowych
    public List<double?> GenerateMultipleValues(int n)
    {
        if (n <= 0)
        {
            throw new ArgumentException("Liczba wartości musi być większa od zera.");
        }

        List<double?> values = new List<double?>();
        for (int i = 0; i < n; i++)
        {
            values.Add(GenerateSingleValue());
        }
        return values;
    }

    // Przedefiniowanie metody ToString do wyświetlania wartości z dokładnością do dwóch miejsc po przecinku
    public string ToString(List<double?> values)
    {
        string result = "";
        foreach (double? value in values)
        {
            if (value.HasValue)
            {
                result += $"{value:0.00}\n";
            }
            else
            {
                result += "null\n";
            }
        }
        return result;
    }

    // Metoda zapisująca wartości do pliku
    public void SaveToFile(List<double?> values, string fileName)
    {
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            foreach (double? value in values)
            {
                if (value.HasValue)
                {
                    writer.WriteLine($"{value:0.00}");
                }
                else
                {
                    writer.WriteLine("null");
                }
            }
        }
    }

    // Metoda serializująca wartości do pliku JSON
    public void SerializeToFile(List<double?> values)
    {
        string fileName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".json";
        string jsonString = JsonSerializer.Serialize(values, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fileName, jsonString);
        Console.WriteLine($"Wartości zostały zapisane do pliku: {fileName}");
    }

    // Metoda deserializująca wartości z najnowszego pliku JSON
    public List<double?> DeserializeFromFile(bool deleteAfterRead)
    {
        string directoryPath = Directory.GetCurrentDirectory();
        var files = Directory.GetFiles(directoryPath, "*.json")
                             .OrderByDescending(f => new FileInfo(f).CreationTime)
                             .ToList();

        if (files.Count == 0)
        {
            throw new FileNotFoundException("Brak plików JSON w katalogu.");
        }

        string latestFile = files.First();
        string jsonString = File.ReadAllText(latestFile);
        List<double?>? values = JsonSerializer.Deserialize<List<double?>>(jsonString);
        if (values == null)
        {
            throw new InvalidOperationException("Deserializacja nie powiodła się.");
        }
        Console.WriteLine($"Wartości zostały odczytane z pliku: {latestFile}");

        if (deleteAfterRead)
        {
            try
            {
                File.Delete(latestFile);
                Console.WriteLine($"Plik {latestFile} został usunięty.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas usuwania pliku: {ex.Message}");
            }
        }

        return values;
    }
}

public class FileReader
{
    // Metoda odczytująca zadaną liczbę wartości zmiennoprzecinkowych z pliku
    public List<double?> ReadValuesFromFile(string fileName, int numberOfValues)
    {
        List<double?> values = new List<double?>();

        using (StreamReader reader = new StreamReader(fileName))
        {
            string line;
            int count = 0;
            while ((line = reader.ReadLine()) != null && count < numberOfValues)
            {
                if (double.TryParse(line, out double result))
                {
                    values.Add(result);
                }
                else
                {
                    values.Add(null);
                }
                count++;
            }
        }

        return values;
    }
}

public class Program
{
    public static void Main()
    {
        ISensor sensor = new TemperatureSensor();
        SensorDataProcessor processor = new SensorDataProcessor(sensor);

        Console.WriteLine("Podaj liczbę wartości do wygenerowania:");
        string? input = Console.ReadLine();
        if (input != null && int.TryParse(input, out int n) && n > 0)
        {
            List<double?> values = sensor.GenerateMultipleValues(n);
            Console.WriteLine("Wygenerowane wartości:");
            Console.WriteLine(sensor.ToString(values));

            // Zapisz wartości do pliku
            string fileName = "plikDane.txt";
            sensor.SaveToFile(values, fileName);
            Console.WriteLine($"Wartości zostały zapisane do pliku: {fileName}");

            // Serializuj wartości do pliku JSON
            sensor.SerializeToFile(values);

            // Odczytaj wartości z pliku
            FileReader fileReader = new FileReader();
            List<double?> readValues = fileReader.ReadValuesFromFile(fileName, n);
            Console.WriteLine("Odczytane wartości z pliku:");
            foreach (var value in readValues)
            {
                Console.WriteLine(value.HasValue ? $"{value:0.00}" : "null");
            }

            // Deserializuj wartości z najnowszego pliku JSON i usuń plik po odczytaniu
            List<double?> deserializedValues = sensor.DeserializeFromFile(deleteAfterRead: true);
            Console.WriteLine("Odczytane wartości z najnowszego pliku JSON:");
            foreach (var value in deserializedValues)
            {
                Console.WriteLine(value.HasValue ? $"{value:0.00}" : "null");
            }

            // Obliczanie średniej wartości
            double average = processor.CalculateAverage(deserializedValues);
            Console.WriteLine($"Średnia wartość: {average:0.00}");

            // Obliczanie odchylenia standardowego
            double standardDeviation = processor.CalculateStandardDeviation(deserializedValues);
            Console.WriteLine($"Odchylenie standardowe: {standardDeviation:0.00}");

            // Sortowanie wartości
            List<double?> sortedValues = processor.SortValues(deserializedValues);
            Console.WriteLine("Posortowane wartości:");
            foreach (var value in sortedValues)
            {
                Console.WriteLine(value.HasValue ? $"{value:0.00}" : "null");
            }

            // Zapisz posortowane wartości do pliku
            string sortedFileName = "posortowaneWartosci.txt";
            processor.SaveValuesToFile(sortedValues, sortedFileName);
            Console.WriteLine($"Posortowane wartości zostały zapisane do pliku: {sortedFileName}");

            // Usuwanie wartości z określonego zakresu
            Console.WriteLine("Podaj minimalną wartość do usunięcia:");
            double minValue = double.Parse(Console.ReadLine() ?? "0");
            Console.WriteLine("Podaj maksymalną wartość do usunięcia:");
            double maxValue = double.Parse(Console.ReadLine() ?? "0");

            List<double?> filteredValues = processor.RemoveValuesInRange(deserializedValues, minValue, maxValue);
            Console.WriteLine("Wartości po usunięciu zakresu:");
            foreach (var value in filteredValues)
            {
                Console.WriteLine(value.HasValue ? $"{value:0.00}" : "null");
            }

            // Zapisz przefiltrowane wartości do pliku
            string filteredFileName = "przefiltrowaneWartosci.txt";
            processor.SaveValuesToFile(filteredValues, filteredFileName);
            Console.WriteLine($"Przefiltrowane wartości zostały zapisane do pliku: {filteredFileName}");
        }
        else
        {
            Console.WriteLine("Nieprawidłowa liczba. Proszę podać liczbę całkowitą większą od zera.");
        }
    }
}


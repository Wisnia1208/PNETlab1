using System.Collections.Generic;

public interface ISensor
{
    double? GenerateSingleValue();
    List<double?> GenerateMultipleValues(int n);
    void SaveToFile(List<double?> values, string fileName);
    void SerializeToFile(List<double?> values);
    List<double?> DeserializeFromFile(bool deleteAfterRead);
    string ToString(List<double?> values);
}

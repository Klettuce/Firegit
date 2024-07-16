using System;

[Serializable]
public class DataModel
{
    public int Id { get; set; }
    public string StringValue1 { get; set; }
    public string StringValue2 { get; set; }
    public int IntValue { get; set; }

    public DataModel() { }

    public DataModel(int id, string stringValue1, string stringValue2, int intValue)
    {
        Id = id;
        StringValue1 = stringValue1;
        StringValue2 = stringValue2;
        IntValue = intValue;
    }
}
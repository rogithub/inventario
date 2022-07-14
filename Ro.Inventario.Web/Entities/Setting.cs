using Ro.Inventario.Web.Entities;

public class Setting
{
    public Setting()
    {
        Key = string.Empty;
        Value = string.Empty;
    }
    public Setting(string key, string value)
    {
        Key = key;
        Value = value;
    }
    public string Key { get; set; }
    public string Value { get; set; }

}
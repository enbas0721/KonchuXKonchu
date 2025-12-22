public enum InsectType
{
    Kabuto,
    Kuwagata
}

public enum ControlType
{
    HumanJoycon,
    CPU
}

[System.Serializable]
public class FighterConfig
{
    public InsectType insectType = InsectType.Kabuto;
    public ControlType controlType = ControlType.HumanJoycon;
}

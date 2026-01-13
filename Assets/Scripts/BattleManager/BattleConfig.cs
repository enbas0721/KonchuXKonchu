public static class BattleConfig
{
    public static FighterConfig Player1 = new FighterConfig();
    public static FighterConfig Player2 = new FighterConfig();

    public static void SetDefaults()
    {
        Player1.insectType = InsectType.Kabuto;
        Player1.controlType = ControlType.HumanJoycon;

        Player2.insectType = InsectType.Kabuto;
        Player2.controlType = ControlType.CPU;
    }

    public static void SetPlayer1Insect(InsectType type)
    {
        Player1.insectType = type;
        Player1.controlType = ControlType.HumanJoycon;
    }

    public static void SetPlayer2Insect(InsectType type)
    {
        Player2.insectType = type;
        // [TODO] 対戦機能をつける場合はここをCPU以外を選べるように
        Player2.controlType = ControlType.CPU;
    }
}

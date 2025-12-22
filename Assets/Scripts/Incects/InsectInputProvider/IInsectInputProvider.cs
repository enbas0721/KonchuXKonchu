public struct InsectInput
{
    public int AttackFlag; // 0/1/2 ‚ğ‚»‚Ì‚Ü‚Üg‚¤i‹““®‚ğ•Ï‚¦‚È‚¢‚½‚ßj
    public int DodgeFlag;  // 0/1/2
}

public interface IInsectInputProvider
{
    InsectInput GetInput();
}
public interface IOpponentAware
{
    void SetOpponent(InsectControllerBase opponent);
}


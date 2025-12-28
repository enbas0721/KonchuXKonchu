public struct InsectInput
{
    public bool Attack;
    public bool Dodge;
}

public interface IInsectInputProvider
{
    InsectInput GetInput();
    void SetEnabled(bool enabled);
}

public interface IOpponentAware
{
    void SetOpponent(InsectControllerBase opponent);
}


namespace Service.Engines
{
    public interface IConditionChecker
    {
        bool Check(string op, object left, object right, string type);
    }
}

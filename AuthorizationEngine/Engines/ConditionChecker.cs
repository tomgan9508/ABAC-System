using Service.Engines;
using System;

namespace AuthorizationEngine.Engines
{
    /// <summary>
    /// ConditionCheker class is used to check conditions in policies.
    /// Conditions are defined in policies as a string, integer or boolean comparison,
    /// and each comprise of an attribute name, operator and value.
    /// </summary>
    public class ConditionChecker : IConditionChecker
    {
        public bool Check(string op, object left, object right, string type)
        {
            switch (type.ToLower())
            {
                case "integer":
                    return CompareInt(op, left, right);
                case "string":
                    return CompareString(op, left, right);
                case "boolean":
                    return CompareBool(left, right);
                default:
                    throw new ArgumentException($"Invalid type for comparison {type}");
            }
        }

        private static bool CompareBool(object left, object right)
        {
            return Convert.ToBoolean(left) == Convert.ToBoolean(right);
        }

        private static bool CompareString(string op, object left, object right)
        {
            switch (op)
            {
                case "startsWith": return left.ToString().StartsWith(right.ToString());
                case "=": return left.ToString().Equals(right.ToString());
                default: throw new ArgumentException($"Invalid operator for string comparison {op}");
            }
        }

        private static bool CompareInt(string op, object left, object right)
        {
            int leftInt = Convert.ToInt32(left);
            int rightInt = Convert.ToInt32(right);

            switch (op)
            {
                case "=": return leftInt == rightInt;
                case ">": return leftInt > rightInt;
                case "<": return leftInt < rightInt;
                default: throw new ArgumentException($"Invalid operator for int comparison {op}");
            }
        }
    }
}

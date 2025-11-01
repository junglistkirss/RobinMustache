namespace Robin.Contracts.Expressions;

public static class BinaryOperatorExtensions
{
    private static readonly Dictionary<string, BinaryOperator> _fromSymbol;
    private static readonly Dictionary<BinaryOperator, string> _toSymbol;

    static BinaryOperatorExtensions()
    {
        _fromSymbol = [];
        _toSymbol = [];

        foreach (System.Reflection.FieldInfo field in typeof(BinaryOperator).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
        {
            BinaryOperator op = (BinaryOperator)field.GetValue(null)!;
            IEnumerable<SymbolAttribute> symbols = field.GetCustomAttributes(typeof(SymbolAttribute), false)
                               .Cast<SymbolAttribute>();

            foreach (SymbolAttribute s in symbols)
            {
                _fromSymbol[s.Text] = op;
                _toSymbol[op] = s.Text;
            }
        }
    }

    public static bool TryParse(this string symbol, out BinaryOperator op)
        => _fromSymbol.TryGetValue(symbol, out op);

    public static string GetSymbol(this BinaryOperator op)
        => _toSymbol.TryGetValue(op, out string? s) ? s : op.ToString();

    //public static implicit operator string(BinaryOperator op) => op.GetSymbol();
}


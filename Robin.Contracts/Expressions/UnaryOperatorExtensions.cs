namespace Robin.Contracts.Expressions;

public static class UnaryOperatorExtensions
{
    private static readonly Dictionary<string, UnaryOperator> _fromSymbol;
    private static readonly Dictionary<UnaryOperator, string> _toSymbol;

    static UnaryOperatorExtensions()
    {
        _fromSymbol = [];
        _toSymbol = [];

        foreach (System.Reflection.FieldInfo field in typeof(UnaryOperator).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
        {
            UnaryOperator op = (UnaryOperator)field.GetValue(null)!;
            IEnumerable<SymbolAttribute> symbols = field.GetCustomAttributes(typeof(SymbolAttribute), false)
                               .Cast<SymbolAttribute>();

            foreach (SymbolAttribute s in symbols)
            {
                _fromSymbol[s.Text] = op;
                _toSymbol[op] = s.Text;
            }
        }
    }

    public static bool TryParse(this string symbol, out UnaryOperator op)
        => _fromSymbol.TryGetValue(symbol, out op);

    public static string GetSymbol(this UnaryOperator op)
        => _toSymbol.TryGetValue(op, out string? s) ? s : op.ToString();

    //public static implicit operator string(UnaryOperator op) => op.GetSymbol();
}

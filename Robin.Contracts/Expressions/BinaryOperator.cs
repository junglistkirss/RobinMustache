namespace Robin.Contracts.Expressions;

public enum BinaryOperator
{
    [Symbol("+")] Add,
    [Symbol("-")] Subtract,
    [Symbol("*")] Multiply,
    [Symbol("/")] Divide,
    [Symbol("^")] Power,
    [Symbol("%")] Modulus,
    [Symbol("&")][Symbol("&&")] And,
    [Symbol("|")][Symbol("`||")] Or,
    [Symbol("==")] Equal,
    [Symbol("!=")]NotEqual,
    [Symbol(">")] GreaterThan,
    [Symbol("<")] LessThan,
    [Symbol(">=")] GreaterThanOrEqual,
    [Symbol("<=")] LessThanOrEqual
}


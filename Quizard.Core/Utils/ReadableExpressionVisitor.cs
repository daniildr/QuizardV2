using System.Linq.Expressions;
using System.Text;

namespace Quizard.Core.Utils;

/// <summary> Метод, для преобразования деревьев выражений LINQ в человеко-понятные строковые представления </summary>
public class ReadableExpressionVisitor : ExpressionVisitor
{
    private readonly StringBuilder _builder = new();

    /// <summary> Преобразует выражение в строку </summary>
    public string Translate(Expression expression)
    {
        Visit(expression);
        return _builder.ToString();
    }

    /// <inheritdoc />
    protected override Expression VisitBinary(BinaryExpression node)
    {
        _builder.Append('(');
        Visit(node.Left);
        _builder.Append(" " + GetOperator(node.NodeType) + " ");
        Visit(node.Right);
        _builder.Append(')');
        return node;
    }

    /// <inheritdoc />
    protected override Expression VisitConstant(ConstantExpression node)
    {
        if (node.Value == null)
            _builder.Append("null");
        else
            _builder.Append(node.Value);
        return node;
    }

    /// <inheritdoc />
    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression is { NodeType: ExpressionType.Parameter })
        {
            _builder.Append(node.Member.Name);
        }
        else
        {
            
            var value = Expression.Lambda(node).Compile().DynamicInvoke();
            _builder.Append(value);
        }
        return node;
    }

    /// <inheritdoc />
    protected override Expression VisitParameter(ParameterExpression node) => node;

    private static string GetOperator(ExpressionType nodeType)
    {
        return nodeType switch
        {
            ExpressionType.Equal => "==",
            ExpressionType.NotEqual => "!=",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            ExpressionType.AndAlso => "&&",
            ExpressionType.OrElse => "||",
            _ => nodeType.ToString()
        };
    }
}
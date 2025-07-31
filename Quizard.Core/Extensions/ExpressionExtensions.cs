using System.Linq.Expressions;
using Quizard.Core.Utils;

namespace Quizard.Core.Extensions;

/// <summary> Расширения для работы с выражениями LINQ. </summary>
public static class ExpressionExtensions
{
    /// <summary> Преобразует выражение-предикат в удобочитаемую строку </summary>
    /// <remarks>
    /// Например, для выражения:
    /// <code> c =&gt; c.Id == Id &amp;&amp; c.Type == type</code>
    /// результат будет:
    /// <code><code>"(Id == 123 &amp;&amp; Type == SomeType)"</code></code>
    /// (при условии, что Id равен 123, а type – SomeType)
    /// </remarks>
    /// <param name="predicate"> Предикат </param>
    /// <typeparam name="T"> Тип входного параметра предиката </typeparam>
    /// <returns> Удобочитаемая строка </returns>
    public static string ToReadableString<T>(this Expression<Func<T, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var visitor = new ReadableExpressionVisitor();
        var result = visitor.Translate(predicate.Body);
        return result;
    }
}
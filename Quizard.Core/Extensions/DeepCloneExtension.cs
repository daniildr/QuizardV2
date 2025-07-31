using System.Reflection;
using System.Runtime.CompilerServices;

namespace Quizard.Core.Extensions;

/// <summary> Расширение глубокого копирования </summary>
public static class DeepCloneExtension
{
    /// <summary> Метод расширения, выполняющий глубокое копирование объекта </summary>
    /// <param name="original"> Объект, который необходимо скопировать </param>
    /// <typeparam name="T"> Тип данных объекта </typeparam>
    /// <returns> Полностью скопированный объект </returns>
    public static T DeepClone<T>(this T original)
    {
        if (ReferenceEquals(original, null))
            return default!;

        var visited = new Dictionary<object, object>(ReferenceEqualityComparer.Instance);

        return (T)DeepCloneInternal(original, visited);
    }

    private static object DeepCloneInternal(object? original, IDictionary<object, object> visited)
    {
        ArgumentNullException.ThrowIfNull(original);
        var type = original.GetType();

        if (type.IsPrimitive || type.IsEnum || type == typeof(string) || type.IsValueType)
            return original;

        if (type.IsArray)
        {
            var array = (Array)original;
            var clone = Array.CreateInstance(type.GetElementType()!, array.Length);
            visited[original] = clone;
            for (var i = 0; i < array.Length; i++)
                clone.SetValue(DeepCloneInternal(array.GetValue(i)!, visited), i);
            return clone;
        }

        if (visited.TryGetValue(original, out var existing))
            return existing;

        var cloneObject = InvokeMemberwiseClone(original);
        visited[original] = cloneObject;

        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var fieldValue = field.GetValue(original);
            if (fieldValue == null)
                continue;

            var clonedFieldValue = DeepCloneInternal(fieldValue, visited);
            field.SetValue(cloneObject, clonedFieldValue);
        }

        return cloneObject;
    }


    private static object InvokeMemberwiseClone(object obj)
    {
        var method = typeof(object)
            .GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
        return method!.Invoke(obj, null)!;
    }

    /// <summary>
    /// Компаратор по ссылочному равенству, чтобы отличать равные по содержимому объекты.
    /// </summary>
    private class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public static ReferenceEqualityComparer Instance { get; } = new();

        public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);

        public int GetHashCode(object? obj)
            => obj is null 
                ? 0 
                : RuntimeHelpers.GetHashCode(obj);
    }
}
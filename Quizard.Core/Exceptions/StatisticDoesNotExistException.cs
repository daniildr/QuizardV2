using System.Linq.Expressions;
using Quizard.Core.Extensions;

namespace Quizard.Core.Exceptions;

/// <summary> Кастомное исключение, если записи статистики не найдена </summary>
/// <param name="predicate"></param>
public class StatisticDoesNotExistException<T>(Expression<Func<T, bool>> predicate) 
    : Exception($"Статистика {predicate.ToReadableString()} отсутствует");
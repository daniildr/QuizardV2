using Quizard.Core.Enums;

namespace Quizard.Core.Exceptions;

/// <summary> Исключение: товар закончился на складе магазина. </summary>
public class ProductIsOutOfStockException(ModifierType modifier) : Exception($"Предмет '{modifier}' закончился!");
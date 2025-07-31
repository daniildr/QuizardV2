using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NUlid;

namespace Quizard.Infrastructure.DataProvider.DbContext.ValueConverters;

/// <summary> Конвертер, который при сохранении переводит объект ULID в массив байт, а при чтении – обратно </summary>
public class UlidValueConverter() 
    : ValueConverter<Ulid, byte[]>(ulid => ulid.ToByteArray(), bytes => new Ulid(bytes));
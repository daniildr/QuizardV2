namespace Quizard.Core.Interfaces;

/// <summary> Интерфейс сервиса шифрования </summary>
public interface IAesEncryptor
{
    /// <summary> Метод для шифрования данных </summary>
    /// <param name="plainText"> Чистый текст </param>
    /// <returns> Зашифрованная строка </returns>
    public Task<string> EncryptAsync(string plainText);
    
    /// <summary> Метод для расшифрования данных </summary>
    /// <param name="cipherText"> Зашифрованный текст </param>
    /// <returns> Расшифрованная строка </returns>
    public Task<string> DecryptAsync(string cipherText);
}
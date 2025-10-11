using System.Security.Cryptography;

namespace FinCore.Core.Helpers;

public class CodeGenerate
{
    private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string GenerateCode(int length = 6)
    {
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);
        var result = new char[length];

        for (int i = 0; i < length; i++)
            result[i] = _chars[bytes[i] % _chars.Length];

        return new string(result);
    }
}
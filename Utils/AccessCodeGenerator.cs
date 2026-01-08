using System.Security.Cryptography;
using System.Text;

namespace SAT.API.Utils;

public static class AccessCodeGenerator
{
    private const string DefaultAlphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";

    public static string Generate(int length = 6, string? alphabet = null)
    {
        alphabet ??= DefaultAlphabet;
        if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));

        var bytes = RandomNumberGenerator.GetBytes(length);
        var sb = new StringBuilder(length);

        for (var i = 0; i < length; i++)
        {
            var idx = bytes[i] % alphabet.Length;
            sb.Append(alphabet[idx]);
        }

        return sb.ToString();
    }
}

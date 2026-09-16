using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Inmobiliaria.Helpers;

public static class SeguridadHelper
{    private const string SaltSecreto = "Inmobiliaria_ULP_2026_Salt_Secreto";

    public static string HashearClave(string clavePlana)
    {
        if (string.IsNullOrEmpty(clavePlana)) return "";

        byte[] salt = Encoding.UTF8.GetBytes(SaltSecreto);

        string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: clavePlana,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        return hash;
    }
}
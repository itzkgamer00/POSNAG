using System;
using System.Security.Cryptography;

namespace CapaNegocio.Seguridad
{
    /// <summary>
    /// Deriva y verifica hashes de contraseñas con PBKDF2-HMAC-SHA256.
    /// Formato almacenado: "PBKDF2.SHA256.{iteraciones}.{saltBase64}.{hashBase64}".
    /// </summary>
    public static class PasswordHasher
    {
        private const string Prefijo = "PBKDF2.SHA256";
        private const int Iteraciones = 100_000;
        private const int TamanoSaltBytes = 16;
        private const int TamanoHashBytes = 32;

        /// <summary>Genera el hash de una contraseña en texto plano.</summary>
        public static string Hash(string contrasena)
        {
            if (contrasena == null) throw new ArgumentNullException(nameof(contrasena));

            byte[] salt = new byte[TamanoSaltBytes];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            byte[] hash = Derivar(contrasena, salt, Iteraciones, TamanoHashBytes);

            return string.Join(".",
                Prefijo,
                Iteraciones.ToString(),
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash));
        }

        /// <summary>
        /// Verifica una contraseña contra un hash almacenado. Devuelve false ante
        /// cualquier hash con formato inválido o nulo, sin lanzar excepción.
        /// </summary>
        public static bool Verificar(string contrasena, string hashAlmacenado)
        {
            if (string.IsNullOrEmpty(contrasena) || string.IsNullOrEmpty(hashAlmacenado))
                return false;

            string[] partes = hashAlmacenado.Split('.');
            if (partes.Length != 5) return false;
            if (partes[0] + "." + partes[1] != Prefijo) return false;
            if (!int.TryParse(partes[2], out int iteraciones) || iteraciones < 1) return false;

            byte[] salt;
            byte[] esperado;
            try
            {
                salt = Convert.FromBase64String(partes[3]);
                esperado = Convert.FromBase64String(partes[4]);
            }
            catch (FormatException)
            {
                return false;
            }

            byte[] calculado = Derivar(contrasena, salt, iteraciones, esperado.Length);
            return ComparacionEnTiempoFijo(calculado, esperado);
        }

        private static byte[] Derivar(string contrasena, byte[] salt, int iteraciones, int longitud)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(contrasena, salt, iteraciones, HashAlgorithmName.SHA256))
                return pbkdf2.GetBytes(longitud);
        }

        private static bool ComparacionEnTiempoFijo(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];

            return diff == 0;
        }
    }
}

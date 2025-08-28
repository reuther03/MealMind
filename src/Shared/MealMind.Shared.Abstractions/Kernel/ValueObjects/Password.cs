using System.Security.Cryptography;
using System.Text.RegularExpressions;
using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Shared.Abstractions.Kernel.ValueObjects;

public partial record Password : ValueObject
{
    public string Value { get; }

    public Password(string passwordHash)
    {
        Value = passwordHash;
    }

    /// <summary>
    /// Create a new password
    /// </summary>
    /// <param name="rawPassword">Raw string password</param>
    /// <returns>New <see cref="Password"/> object</returns>
    public static Password Create(string rawPassword)
    {
        if (!IsValid(rawPassword))
            throw new DomainException("Password cannot be empty");

        return new Password(PasswordHasher.Hash(rawPassword));
    }

    public static bool IsValid(string rawPassword) => PasswordRegex().IsMatch(rawPassword);

    public bool Verify(string rawPassword) => PasswordHasher.Verify(rawPassword, Value);

    public static implicit operator string(Password password) => password.Value;
    public static implicit operator Password(string value) => new(value);

    public override string ToString() => Value;

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }


    [GeneratedRegex(@"^(?=.*\d)(?=.*\W)(?!.*\s).{6,}$")]
    public static partial Regex PasswordRegex();

    private static class PasswordHasher
    {
        private const char SegmentDelimiter = ':';
        private const int KeySize = 32;
        private const int SaltSize = 16;
        private const int Iterations = 50000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

        public static string Hash(string rawTextInput)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(rawTextInput, salt, Iterations, Algorithm, KeySize);
            return string.Join(SegmentDelimiter, Convert.ToBase64String(hash), Convert.ToBase64String(salt), Iterations,
                Algorithm);
        }

        public static bool Verify(string rawTextInput, string hashString)
        {
            var segments = hashString.Split(SegmentDelimiter);
            var hash = Convert.FromBase64String(segments[0]);
            var salt = Convert.FromBase64String(segments[1]);
            var iterations = int.Parse(segments[2]);
            var algorithm = new HashAlgorithmName(segments[3]);
            var inputHash = Rfc2898DeriveBytes.Pbkdf2(rawTextInput, salt, iterations, algorithm, hash.Length);
            return CryptographicOperations.FixedTimeEquals(inputHash, hash);
        }
    }
}
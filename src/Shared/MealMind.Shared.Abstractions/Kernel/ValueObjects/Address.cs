using System.Text.RegularExpressions;
using MealMind.Shared.Abstractions.Exception;
using MealMind.Shared.Abstractions.Kernel.Primitives;

namespace MealMind.Shared.Abstractions.Kernel.ValueObjects;

public sealed partial record Address : ValueObject
{
    public string Country { get; }
    public string City { get; }
    public string Street { get; }
    public string PostalCode { get; }
    public string Phone { get; }

    public Address(string country, string city, string street, string postalCode, string phone)
    {
        if (!IsValid(country, city, street, postalCode, phone))
            throw new DomainException("Address is not valid", nameof(country));

        Country = country.FirstCharToUpper();
        City = city.FirstCharToUpper();
        Street = street.FirstCharToUpper();
        PostalCode = postalCode.FirstCharToUpper();
        Phone = phone;
    }

    private static bool IsValid(string country, string city, string street, string zipCode, string phone)
    {
        return !string.IsNullOrWhiteSpace(country) &&
            !string.IsNullOrWhiteSpace(city) &&
            !string.IsNullOrWhiteSpace(street) &&
            PostalCodeRegex().IsMatch(zipCode) &&
            !string.IsNullOrWhiteSpace(phone);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Country;
        yield return City;
        yield return Street;
        yield return PostalCode;
        yield return Phone;
    }

    [GeneratedRegex("^[0-9]{2}-[0-9]{3}$")]
    private static partial Regex PostalCodeRegex();
}
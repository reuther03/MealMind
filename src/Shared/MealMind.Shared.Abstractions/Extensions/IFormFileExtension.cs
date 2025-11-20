using Microsoft.AspNetCore.Http;

namespace MealMind.Shared.Abstractions.Extensions;

public static class IFormFileExtension
{
    extension(IFormFile file)
    {
        public async Task<byte[]> ToByteArrayAsync(CancellationToken cancellationToken = default)
        {
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms, cancellationToken);
            return ms.ToArray();
        }
    }
}
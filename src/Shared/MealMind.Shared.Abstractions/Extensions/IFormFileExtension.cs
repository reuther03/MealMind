using Microsoft.AspNetCore.Http;

namespace MealMind.Shared.Abstractions.Extensions;

public static class IFormFileExtension
{
    extension(IFormFile file)
    {
        public async Task<ReadOnlyMemory<byte>> ToReadOnlyMemoryByteArrayAsync(CancellationToken cancellationToken = default)
        {
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms, cancellationToken);
            return new ReadOnlyMemory<byte>(ms.ToArray());
        }
    }
}
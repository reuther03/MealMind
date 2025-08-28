using Microsoft.AspNetCore.Http;

namespace MealMind.Shared.Abstractions.Services;

public interface IImgUploader
{
    Task<string> UploadImg(IFormFile file);

    void DeleteImg(string publicId);
    // Task<byte[]> DownloadImgAsync(string imgUrl);
}
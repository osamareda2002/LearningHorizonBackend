using Microsoft.AspNetCore.StaticFiles;

namespace LearningHorizon.Services
{
    public class MediaHelper
    {
        public static string GetBunnyVideoUrl(int libraryId, string guid)
        {
            return $"https://iframe.mediadelivery.net/embed/{libraryId}/{guid}";
        }

        public static string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out string contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

    }
}

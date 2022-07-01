namespace PeliculasAPI.PeliculasAPI.Services
{
    public class UploadLocal : IUploadFiles
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UploadLocal(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task DeleteFile(string route, string container)
        {
            if(route != null)
            {
                var fileName = Path.GetFileName(route);
                string directory = Path.Combine(env.WebRootPath, container, fileName);
                if (File.Exists(directory)) { File.Delete(directory); }
            }
            return Task.FromResult(0);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string container, string contentType)
        {
            var fileName = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(env.WebRootPath, container);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string route = Path.Combine(folder, fileName);
            await File.WriteAllBytesAsync(route, content);
            var urlNow = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var dbRoute = Path.Combine(urlNow, container, fileName).Replace("\\", "/");
            return dbRoute;
        }

        public async Task<string> UpdateFile(byte[] content, string extension, string container, string route, string contentType)
        {
            await DeleteFile(route, container);
            return await SaveFile(content, extension, container, contentType);
        }
    }
}

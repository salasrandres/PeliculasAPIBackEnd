namespace PeliculasAPI.PeliculasAPI.Services
{
    public interface IUploadFiles
    {
        Task<string> SaveFile(byte[] content, string extension, string container, string contentType);
        Task<string> UpdateFile(byte[] content, string extension, 
            string container, string route, string contentType);
        Task DeleteFile(string route, string container);
    }
}

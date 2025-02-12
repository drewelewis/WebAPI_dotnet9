namespace WebAPI_dotnet9
{
    public interface IStorage
    {
        List<UploadItem> GetAll();
        void Add(UploadItem upload);
        UploadItem Get(Guid guid);
        void Update(UploadItem upload);
        Task UpdateAsync(UploadItem upload);
        void Remove(Guid guid);

    }
}

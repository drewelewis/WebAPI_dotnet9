namespace WebAPI_dotnet9
{
    public class Storage :IStorage
    {
        private Dictionary<Guid, UploadItem> _items;
        public Storage()
        {
            _items = new Dictionary<Guid, UploadItem>();
        }
        public List<UploadItem>GetAll()
        {
            return _items.Values.ToList();
        }
        public void Add(UploadItem upload)
        {
            _items.Add(upload.Guid, upload);
        }
        public UploadItem Get(Guid guid)
        {
            return _items[guid];
        }
        public void Update(UploadItem upload)
        {
            _items[upload.Guid] = upload;
        }
        public async Task UpdateAsync(UploadItem upload)
        {
            await Task.Run(() => _items[upload.Guid] = upload);
        }
        public void Remove(Guid guid)
        {
            _items.Remove(guid);
        }
    }
}

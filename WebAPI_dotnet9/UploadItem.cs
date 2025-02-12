namespace WebAPI_dotnet9
{
    public class UploadItem
    {
        private Guid _guid { get; set; }
        public UploadItem()
        {
            _guid = Guid.NewGuid();
        }
        public Guid Guid { get { return _guid; } }
        public string Status { get; set; }
        public string FileName { get; set; }
        public string Location { get; set; }

    }
}

namespace IPMonitoring.Pages
{
    public class OnStartIP
    {
        public string SelectedFilePath { get; set; }

        public OnStartIP(string filepath)
        {
            SelectedFilePath = filepath;
        }
    }
}
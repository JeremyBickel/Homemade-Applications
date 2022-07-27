namespace DownloadBTPData
{
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            //
            //Get Data from BibleTruthPublishers.com
            //

            DLData dlData = new DLData();
            await Task.WhenAny(
                dlData.Go());
                //, Task.Delay(TimeSpan.FromSeconds(300))); //5 minutes
        }
    }
}
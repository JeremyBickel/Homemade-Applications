using System.Drawing.Imaging;

namespace AutomateDesktop
{
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();

            GDI32.GetImageFromScreen autoMain = new GDI32.GetImageFromScreen();
            Guid imageGuid = Guid.NewGuid();

            ImageFormat ifMain = new ImageFormat(imageGuid);
            autoMain.CaptureScreen(@"test.bmp", ifMain);
        }
    }
}
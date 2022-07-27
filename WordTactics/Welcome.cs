using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WordTactics
{
    public partial class Welcome : Form
    {
        public Welcome()
        {
            InitializeComponent();

            OWLDB owl = new OWLDB();
        }
    }
}
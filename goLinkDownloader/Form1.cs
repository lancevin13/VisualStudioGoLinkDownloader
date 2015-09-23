using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Collections.Specialized;

namespace goLinkDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //check for internet connection
        class checkConnection
        {
            public static bool connectionStatus()
            {
                try
                {
                    using (var conn = new WebClient()) 
                    {
                        using (var stream = conn.OpenRead("http://www.google.com"))
                        {
                            return true;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
        }
        //end check

        double totalBytes;

        private void Form1_Load(object sender, EventArgs e)
        {
            if(!checkConnection.connectionStatus())
            {
                MessageBox.Show("You do not seem to have an active internet connection. Please check and run this application again.", "No connection found!", MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                this.Close();
            }
            
            Uri link = new Uri("http://analytics.inservices.tatamotors.com:8080/analytics/saw.dll?GO&nquser=AROY_08846&nqpassword=CRM2016&path=/Shared/TMP/Athena%20DRP%20Test/SCV%20DRP/Sales%20Pipeline%20Tracker&Format=csv");
            var client = new HTMLParser.CookieAwareWebClient();
            
            var values = new NameValueCollection
            {
                {"nquser", "AROY_08846" },
                {"nqpassword", "CRM2016" },
            };
            client.UploadValues("http://analytics.inservices.tatamotors.com:8080/analytics/saw.dll?GO", values);
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:40.0) Gecko/20100101 Firefox/40.0");
            client.Headers.Add("Referer", "http://analytics.inservices.tatamotors.com:8080/analytics/saw.dll?GO&nquser=AROY_08846&nqpassword=CRM2016&path=/Shared/TMP/Athena%20DRP%20Test/SCV%20DRP/Sales%20Pipeline%20Tracker&Format=csv");
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            
            //client.OpenReadAsync(link);
            //double totalBytes = Convert.ToInt64(client.ResponseHeaders["Content-Length"]);
            //MessageBox.Show(totalBytes.ToString());
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(Client_DownloadFileCompleted);
            client.DownloadFileAsync(link, @"C:\Users\Vineeth\Desktop\SPT.csv");
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            MessageBox.Show("Download Complete!");
            this.Close();
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            //double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            if (totalBytes < 0) {
                totalBytes = 62914560;
            }
            double percentage = bytesIn / totalBytes * 100;
            lblStatus.Text = "Downloaded " + e.BytesReceived / 1048576 + " of estimated " + totalBytes / 1048576;
            //lblStatus.Text = "Downloaded {1} of {2} bytes | {3} % complete...";
            //progressBar1.Maximum = (int)e.TotalBytesToReceive/100;
            progressBar1.Value = int.Parse(Math.Truncate(percentage).ToString());
        }

    }

        namespace HTMLParser
    {
        internal class CookieAwareWebClient : WebClient
        {
            private CookieContainer cookie = new CookieContainer();

            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);
                if (request is HttpWebRequest)
                {
                    (request as HttpWebRequest).CookieContainer = cookie;
                    (request as HttpWebRequest).KeepAlive = false;
                }
                return request;
            }
        }
    }
    
}

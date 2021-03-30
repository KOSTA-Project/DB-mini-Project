using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;





namespace DB_mini_Project
{
    public partial class Form1 : Form { 

        List<Tuple<string, double, double>> tuples = new List<Tuple<string, double, double>>();
        public Form1()
        {
            InitializeComponent();
            Version ver = webBrowser1.Version;
            string name = webBrowser1.ProductName;
            string str = webBrowser1.ProductVersion;
            string html = "map.html";
            string dir = System.IO.Directory.GetCurrentDirectory();
            string path = System.IO.Path.Combine(dir, html);
            webBrowser1.Navigate(path);
        }


        // 현재 작업 : 검색창 입력 후 리스트박스 선택, 데이터 멥에 찍기.

         private void button1_Click(object sender, EventArgs e)
         {
            string address = textBox1.Text;

             string site = "https://dapi.kakao.com/v2/local/search/keyword.json";
             string query = string.Format("{0}?query={1}", site, address);
             WebRequest request = WebRequest.Create(query);

             string rkey = "4925a34ce72e895ab1290119ee11f9e1";
             string header = "KakaoAK " + rkey;
             request.Headers.Add("Authorization", header);

             WebResponse response = request.GetResponse();
             Stream stream = response.GetResponseStream();
             StreamReader reader = new StreamReader(stream, Encoding.UTF8);
             String json = reader.ReadToEnd();
             stream.Close();


             JavaScriptSerializer js = new JavaScriptSerializer();
             dynamic dob = js.Deserialize<dynamic>(json);
             dynamic docs = dob["documents"];
             object[] buf = docs;

             for(int i = 0; i < buf.Length; i++)
            {
                listBox1.Items.Add(docs[i]["place_name"]);
                tuples.Add(new Tuple<string, double, double>(docs[i]["place_name"], double.Parse(docs[i]["x"]), double.Parse(docs[i]["y"])));
            }

         }


        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int idx = listBox1.SelectedIndex;
            var sel = tuples[idx];
            object[] arr = new object[] { sel.Item3, sel.Item2 };
            object res = webBrowser1.Document.InvokeScript("panTo", arr);
        }
    }
}

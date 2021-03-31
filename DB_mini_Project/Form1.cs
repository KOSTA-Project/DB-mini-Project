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
        private void init()     //초기화
        {
            tuples.Clear();
            listBox1.Items.Clear();
            webBrowser1.Document.InvokeScript("clearMarkers");  //지도에 표시된 마커 지우기
            //필터 콤보박스 초기화 필요
        }

        private void plotMap(){

        }


        // 현재 작업 : 검색창 입력 후 리스트박스 선택, 데이터 멥에 찍기.

        private void button1_Click(object sender, EventArgs e)
         {
            init();
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

            double x = double.Parse(docs[0]["x"]);      //lng
            double y = double.Parse(docs[0]["y"]);      //lat
            object[] arr = new object[] { docs[0]["place_name"].ToString(), y, x };

            tuples.Add(new Tuple<string, double, double>(docs[0]["place_name"], x, y));
            webBrowser1.Document.InvokeScript("addMarker", arr);  //마커추가
            //리스트 첫 요소 위치를 지도 중심으로
            webBrowser1.Document.InvokeScript("panTo",new object[] {y, x});

        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int idx = listBox1.SelectedIndex;
            var sel = tuples[idx];
            object res = webBrowser1.Document.InvokeScript("panTo", new object[] { sel.Item3, sel.Item2 });
        }

    }
}

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
        
        List<Tuple<string, string>> search_result = new List<Tuple<string, string>>(); // 검색 결과가 담기는 List<Tuple<상호명, 주소>>


        public Form1()
        {
            InitializeComponent();
            Version ver = webBrowser1.Version;
            string name = webBrowser1.ProductName;
            string str = webBrowser1.ProductVersion;
            string html = "map.html";
            string dir = System.IO.Directory.GetCurrentDirectory();
            string path = System.IO.Path.Combine(dir, html);
            Console.WriteLine(path);
            webBrowser1.Navigate(path);
        }
        private void init()     //초기화
        {
            tuples.Clear();
            search_result.Clear();
            listBox1.Items.Clear();
            webBrowser1.Document.InvokeScript("clearMarkers");  //지도에 표시된 마커 지우기
            //필터 콤보박스 초기화 필요
        }

        private void plotMap(){
            // 예시 : 이런식으로 Search팀에서 값들을 넣었을 때. 
            search_result.Add(new Tuple<string, string>("(네오프런트)취영루	", "성남시 분당구 양현로 164(이매동,성남상공회의소 5층)"));
            search_result.Add(new Tuple<string, string>("(주)굿프랜드", "경기도 성남시 분당구 내정로113번길 4 2층 204호"));
            search_result.Add(new Tuple<string, string>("(주)두영상사 (성남지점)", "경기도 성남시 중원구 산성대로 266 (중앙동,1층)"));
            search_result.Add(new Tuple<string, string>("(주)레드미트코리아", "경기도 성남시 수정구 시민로 239 ."));

            string site = "https://dapi.kakao.com/v2/local/search/address.json";

            // 검색 결과 리스트를 조회.
            for (int i = 0; i < search_result.Count; i++)
            {
                // 주소를 활용하여 쿼리문 작성.
                string store_name = search_result[i].Item1;
                string store_address = search_result[i].Item2;
                string query = $"{site}?query={store_address}";

                // 검색 요청을 보냅니다.
                WebRequest request = WebRequest.Create(query);
                string rkey = "4925a34ce72e895ab1290119ee11f9e1";
                string header = "KakaoAK " + rkey;
                request.Headers.Add("Authorization", header);

                // 리스트 박스에는 상호명만 추가.
                listBox1.Items.Add(store_name);

                // 검색 결과가 없을 수 있는 예외 처리.
                try
                {
                    WebResponse response = request.GetResponse();   
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);  // 받아온 응답을 읽어오기.
                    String json = reader.ReadToEnd(); 
                    stream.Close();

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    dynamic dob = js.Deserialize<dynamic>(json);    //json 파일을 효과적으로 쓰기위해 역직렬화.
                    dynamic docs = dob["documents"];                //documents의 벨류들만 저장.

                    double x = double.Parse(docs[0]["x"]);      //lng
                    double y = double.Parse(docs[0]["y"]);      //lat
                    object[] arr = new object[] { store_name, y, x };
                    
                    // html 파일 내부의 javascript 함수 실행을 위한 전처리.  
                    tuples.Add(new Tuple<string, double, double>(store_name, x, y));
                    // 내부의 함수들 실행.
                    webBrowser1.Document.InvokeScript("addMarker", arr);  //마커추가 
                    webBrowser1.Document.InvokeScript("panTo", new object[] { y, x });  //리스트 첫 요소 위치를 지도 중심으로
                }
                catch(Exception err)
                {   //검색결과 없는 경우(바른 결과인경우-> 해당 영역에 사용처 없는 경우)
                    MessageBox.Show(err.Message);
                }
            }
        }


        // 현재 작업 : 검색창 입력 후 리스트박스 선택, 데이터 멥에 찍기.

        private void button1_Click(object sender, EventArgs e)
         {
            init();
            plotMap();
        }


        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int idx = listBox1.SelectedIndex;
            if (idx < 0 || idx > listBox1.Items.Count - 1) return;  //유효하지 않은 클릭 시 
            var sel = tuples[idx];
            object res = webBrowser1.Document.InvokeScript("panTo", new object[] { sel.Item3, sel.Item2 });
   
        }

    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Web.Script.Serialization;
using System.Net;

namespace DB_mini_Project
{
    public partial class Form1 : Form
    {
        List<String[]> data = new List<String[]>();     // csv 파일 데이터 저장 리스트

        List<String> location_list = new List<string>(); // "구" 데이터를 담는 리스트
        Dictionary<String, List<String>> location2_list = new Dictionary<string, List<string>>(); //"구"에 따른 "동" 데이터 리스트 (key:"구", value:"구"에 해당하는 "동" 리스트)
        List<String> item_list = new List<string>();     // "품목" 
        List<String> pay_list = new List<string>();      // "결제방식" 

        Control[] BTN_location; // "구" 버튼 컨트롤 배열
        Dictionary<String, Control[]> BTN_location2 = new Dictionary<string, Control[]>(); // "동" 
        Control[] BTN_item;     // "품목"
        Control[] BTN_pay;      // "결제방식"


        String location = "";  // 선택된 "구"값
        String location2 = ""; // "동"
        String item = "";      // "품목"
        String pay = "";       // "결제방식"


        List<Tuple<string, string>> search = new List<Tuple<string, string>>(); //필터 결과 <변수명, 상호명> 형태의 리스트로
        List<Tuple<string, double, double>> tuples = new List<Tuple<string, double, double>>(); //필터 결과 <상호명, x, y> 형태의 리스트 


        public Form1()
        {
            InitializeComponent();
            textBox1.KeyDown += Enter_KeyDown;  //엔터키 키다운 이벤트 활성화
            string html = "map.html";
            string dir = System.IO.Directory.GetCurrentDirectory();     //현 디렉토리
            string path = System.IO.Path.Combine(dir, html);            //디렉토리 + 파일명

            webBrowser1.Navigate(path); // 웹 브라우저 컨트롤에 로드
        }

        // 초기화
        private void init()
        {
            tuples.Clear();
            search.Clear();
            listBox1.Items.Clear();
            webBrowser1.Document.InvokeScript("clearMarkers");  //지도에 표시된 마커 지우기
        }

        // 데이터를 로드하고
        // 검색할 데이터를 설정하는 부분입니다 
        private void Form1_Load(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader("File_modi.csv", System.Text.Encoding.GetEncoding(949));
            string buf = sr.ReadLine();
            string[] sArr = buf.Split(',');

            // 모든 데이터를 컬럼 별로 리스트에 저장
            // data에는 모든 정보 저장
            while (true)
            {
                buf = sr.ReadLine();
                if (buf == null) break;

                sArr = buf.Split(',');
                location_list.Add(sArr[3]); //구
                item_list.Add(sArr[2]);     //품목 
                pay_list.Add(sArr[5]);      //결제 방식

                if (!location2_list.ContainsKey(sArr[3]))   //키 값으로 현재의 "구"데이터가 없으면
                {
                    location2_list[sArr[3]] = new List<string>(); //새로운 리스트 생성
                }
                location2_list[sArr[3]].Add(sArr[4]);   //키 값("구")에 맞는 리스트에 "동"추가
                data.Add(sArr);
            }

            //Distinct로 중복제거하여 컬럼별 필터값 추출
            location_list = location_list.Distinct().ToList();
            item_list = item_list.Distinct().ToList();
            pay_list = pay_list.Distinct().ToList();

            // "구"리스트의 값들을 보면서 각각에 대한 "동"리스트 중복 제거
            // "동" 검색을 위해 "동"리스트 수만큼의 버튼 생성
            foreach (String key in location_list)
            {
                location2_list[key] = location2_list[key].Distinct().ToList();
                BTN_location2[key] = new Control[location2_list[key].Count];
            }
            //각각 리스트 수만큼 버튼컨트롤 생성
            BTN_location = new Control[location_list.Count];
            BTN_item = new Control[item_list.Count];
            BTN_pay = new Control[pay_list.Count];

            //
            for (int i = 0; i < location_list.Count; i++)
            {
                BTN_location[i] = new Button();
                BTN_location[i].Name = i.ToString();
                BTN_location[i].Parent = this;
                BTN_location[i].Size = new Size(90, 30);
                BTN_location[i].Text = location_list[i];

                BTN_location[i].Click += Btn_location_Click;

                flowLayoutPanel1.Controls.Add(BTN_location[i]);

            }

            for (int i = 0; i < item_list.Count; i++)
            {
                BTN_item[i] = new Button();
                BTN_item[i].Name = i.ToString();
                BTN_item[i].Parent = this;
                BTN_item[i].Size = new Size(90, 30);
                BTN_item[i].Text = item_list[i];

                BTN_item[i].Click += Btn_item_Click;

                flowLayoutPanel3.Controls.Add(BTN_item[i]);

            }

            for (int i = 0; i < pay_list.Count; i++)
            {
                BTN_pay[i] = new Button();
                BTN_pay[i].Name = i.ToString();
                BTN_pay[i].Parent = this;
                BTN_pay[i].Size = new Size(90, 30);
                BTN_pay[i].Text = pay_list[i];

                BTN_pay[i].Click += Btn_pay_Click;

                flowLayoutPanel4.Controls.Add(BTN_pay[i]);

            }


            clear_location_btn();
            clear_item_btn();
            clear_pay_btn();
            //clear_location2_btn();

            sr.Close();

        }
        private void plotMap()
        {
            string site = "https://dapi.kakao.com/v2/local/search/address.json";

            // 검색 결과 리스트를 조회.
            for (int i = 0; i < search.Count; i++)
            {
                // 주소를 활용하여 쿼리문 작성.
                string store_name = search[i].Item1;
                string store_address = search[i].Item2;
                string query = $"{site}?query={store_address}";     //쿼리문

                // 검색 요청을 보냅니다.
                WebRequest request = WebRequest.Create(query);
                string rkey = "4925a34ce72e895ab1290119ee11f9e1";   //인증키
                string header = "KakaoAK " + rkey;
                request.Headers.Add("Authorization", header);

                // 검색 결과가 없을 수 있는 예외 처리.
                
                
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);  // 받아온 응답을 읽어오기.
                String json = reader.ReadToEnd();
                stream.Close();

                JavaScriptSerializer js = new JavaScriptSerializer();
                dynamic dob = js.Deserialize<dynamic>(json);    //json 파일을 효과적으로 쓰기위해 역직렬화.
                dynamic docs = dob["documents"];                //documents의 벨류들만 저장.

                // 검색 결과가 없을 때.
                if (docs.Length==0)
                {
                    continue;
                }
                else
                {
                    double x = double.Parse(docs[0]["x"]);      //lng
                    double y = double.Parse(docs[0]["y"]);      //lat
                    object[] arr = new object[] { store_name,store_address, y, x };

                    // html 파일 내부의 javascript 함수 실행을 위한 전처리.  
                    tuples.Add(new Tuple<string, double, double>(store_name, x, y));
                    listBox1.Items.Add(store_name);
                    // 내부의 함수들 실행.
                    webBrowser1.Document.InvokeScript("addMarker", arr);  //마커추가 
                    webBrowser1.Document.InvokeScript("panTo", new object[] { y, x });  //리스트 첫 요소 위치를 지도 중심으로
                }
 
            }
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("검색 결과가 없습니다.");
            }
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int idx = listBox1.SelectedIndex;
            if (idx < 0 || idx > listBox1.Items.Count - 1) return;  //유효하지 않은 클릭 시 
            var sel = tuples[idx];
            object res = webBrowser1.Document.InvokeScript("panTo", new object[] { sel.Item3, sel.Item2 });
        }

        // 검색 버튼입니다.
        private void button1_Click(object sender, EventArgs e)
        {
            init();
            data.ForEach(d =>
            {
             
                if ((d[3].Equals(location) || location.Equals(""))
                && (d[4].Equals(location2) || location2.Equals(""))
                && (d[2].Equals(item) || item.Equals(""))
                && (d[5].Equals(pay) || pay.Equals(""))
                    && ((d[1].Contains(textBox1.Text)) || (d[4].Contains(textBox1.Text)) || (d[5].Contains(textBox1.Text))))
                {
                    string store_name = d[1];
                    string store_address = d[7];
                    search.Add(new Tuple<string, string>(store_name, store_address));
                }
            });
            
            plotMap();

        }





        // enter key 처리 부분입니다.
        private void Enter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        // 
        // "구" 버튼이 나오게 하는 작업입니다.
        private void clear_location_btn()
        {
            for (int i = 0; i < location_list.Count; i++)
            {
                BTN_location[i].BackColor = Button.DefaultBackColor;
            }
        }

        // "동" 버튼이 나오게 하는 작업입니다.
        private void clear_location2_btn()
        {
            for (int i = 0; i < location2_list[location].Count; i++)
            {
                BTN_location2[location][i].BackColor = Button.DefaultBackColor;
            }
        }

        // "품목" 버튼이 나오게 하는 작업입니다.
        private void clear_item_btn()
        {
            for (int i = 0; i < item_list.Count; i++)
            {
                BTN_item[i].BackColor = Button.DefaultBackColor;
            }
        }

        // "결제 방법" 버튼이 나오게 하는 작업입니다.
        private void clear_pay_btn()
        {
            for (int i = 0; i < pay_list.Count; i++)
            {
                BTN_pay[i].BackColor = Button.DefaultBackColor;
            }
        }


        // "구"를 눌렀을때 그 안의 "동"이 나오도록 하는 작업 입니다.
        private void add_location2_btn(String key)
        {
            for (int i = 0; i < location2_list[key].Count; i++)
            {
                BTN_location2[key][i] = new Button();
                BTN_location2[key][i].Name = i.ToString();
                BTN_location2[key][i].Parent = this;
                BTN_location2[key][i].Size = new Size(90, 30);
                BTN_location2[key][i].Text = location2_list[key][i];

                BTN_location2[key][i].Click += Btn_location2_Click;

                BTN_location2[key][i].BackColor = Button.DefaultBackColor;

                flowLayoutPanel2.Controls.Add(BTN_location2[key][i]);
            }
        }

        // "구" 단위 버튼을 눌렀을때 호출되는 함수입니다.
        // "동" 을 초기화하고 해당 "구" 에 해당하는 "동" 을 새로 만듬
        public void Btn_location_Click(object sender, EventArgs e)
        {
            location = ((Button)sender).Text;
            location2 = "";
            flowLayoutPanel2.Controls.Clear();
            clear_location_btn();
            add_location2_btn(location);
            ((Button)sender).BackColor = Color.Gray;
        }

        // "동" 단위 버튼을 눌렀을때 호출되는 함수입니다.
        public void Btn_location2_Click(object sender, EventArgs e)
        {
            location2 = ((Button)sender).Text;
            clear_location2_btn();
            ((Button)sender).BackColor = Color.Gray;
        }

        // "동" 단위 버튼을 눌렀을때 호출되는 함수입니다.
        public void Btn_item_Click(object sender, EventArgs e)
        {
            item = ((Button)sender).Text;

            clear_item_btn();
            ((Button)sender).BackColor = Color.Gray;
        }

        // "동" 단위 버튼을 눌렀을때 호출되는 함수입니다.
        public void Btn_pay_Click(object sender, EventArgs e)
        {
            pay = ((Button)sender).Text;


            clear_pay_btn();
            ((Button)sender).BackColor = Color.Gray;
        }
    }
}

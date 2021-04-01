
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
using System.Data.OleDb;
using System.Globalization;

namespace DB_mini_Project
{
    public partial class Form1 : Form
    {
        List<String> location_list = new List<string>(); // "구" 데이터가 저장되는 변수
        Dictionary<String, List<String>> location2_list = new Dictionary<string, List<string>>(); // "동" 데이터가 저장되는 변수, 각 "구" 별로 "동" 이 저장된다.
        List<String> item_list = new List<string>(); // "품목" 데이터가 저장되는 변수
        List<String> pay_list = new List<string>(); // "결제" 데이터가 저장되는 변수

        List<Tuple<string, string>> search = new List<Tuple<string, string>>();

        String location = ""; // "구" 를 저장하는 변수
        String location2 = ""; // "동" 을 저장하는 변수
        String item = ""; // "품목" 를 저장하는 변수
        String pay = ""; // "결제" 를 저장하는 변수

        Control[] BTN_location; // "구" 버튼을 생성하는 변수
        Dictionary<String, Control[]> BTN_location2 = new Dictionary<string, Control[]>(); // "동" 변수를 생성하는 변수
        Control[] BTN_item; // "품목" 버튼을 생성하는 변수
        Control[] BTN_pay; // "결제" 버튼을 생성하는 변수

        List<String[]> data = new List<String[]>(); // 엑셀 파일에서 읽어온 데이터를 저장하는 변수

        public Form1()
        {
            InitializeComponent();
            textBox1.KeyDown += Enter_KeyDown;
        }

        // 검색 버튼입니다.
        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();


            data.ForEach(d =>
            {
                if ((d[3].Equals(location) || location.Equals(""))
                && (d[4].Equals(location2) || location2.Equals(""))
                && (d[2].Equals(item) || item.Equals(""))
                && (d[5].Equals(pay) || pay.Equals(""))
                    && ((d[1].Contains(textBox1.Text)) || (d[4].Contains(textBox1.Text)) || (d[5].Contains(textBox1.Text))))
                {
                    listBox1.Items.Add(d[1] + " " + d[7] + Environment.NewLine);

                    string store_name = d[1];
                    string store_address = d[7];
                    search.Add(new Tuple<string, string>(store_name, store_address));

                }
            });



        }


        // 데이터를 로드하고
        // 검색할 데이터를 설정하는 부분입니다 

        private void Form1_Load(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader("File_modi.csv", System.Text.Encoding.GetEncoding(949));
            string buf = sr.ReadLine();
            string[] sArr = buf.Split(',');


            for (int i = 0; i < sArr.Length; i++)
            {
                String a = sArr[i];
            }
            while (true)
            {
                buf = sr.ReadLine();
                if (buf == null) break;
                sArr = buf.Split(',');
                location_list.Add(sArr[3]);
                item_list.Add(sArr[2]);
                pay_list.Add(sArr[5]);
                if (!location2_list.ContainsKey(sArr[3]))
                {
                    location2_list[sArr[3]] = new List<string>();
                }
                location2_list[sArr[3]].Add(sArr[4]);
                data.Add(sArr);
            }

            location_list = location_list.Distinct().ToList();
            item_list = item_list.Distinct().ToList();
            pay_list = pay_list.Distinct().ToList();

            foreach (String key in location_list)
            {
                location2_list[key] = location2_list[key].Distinct().ToList();
                BTN_location2[key] = new Control[location2_list[key].Count];
            }

            BTN_location = new Control[location_list.Count];
            BTN_item = new Control[item_list.Count];
            BTN_pay = new Control[pay_list.Count];

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

            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel2.AutoScroll = true;
            flowLayoutPanel3.AutoScroll = true;
            flowLayoutPanel4.AutoScroll = true;


            sr.Close();

        }



        // enter key 처리 부분입니다.
        private void Enter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }


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

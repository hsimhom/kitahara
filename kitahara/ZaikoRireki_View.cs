﻿using CsvHelper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kitahara
{
    public partial class ZaikoRireki_View : Form
    {
        public string day1 = "";
        public string day2 = "";
        public string hinban1 = "";
        public string hinban2 = "";
        public string kubun = "";
        public string filename2 = "";

        public ZaikoRireki_View()
        {
            InitializeComponent();
        }

        private void ZaikoRireki_View_Load(object sender, EventArgs e)
        {
            List<Product> procucts = new List<Product>();

            string socd = "";
            string saki = "";
            string hinban = "";
            string shoinmei = "";
            string ymd = "";
            string nkbn = "";
            string dkbn = "";
            string tancd = "";
            string syukasu = "";
            string urikin = "";
            int ihattyusu = 0;
            int itzaiko = 0;
            int inyukasu = 0;
            int ijittusu = 0;
            int isyukasu = 0;
            int igzaiko = 0;

            long iHinban = 0;
            string sShohinmei = "";
            string sYmd = "";
            string sNkbn = "";
            string sTancd = "";
            string sTorimei = "";
            int iHatyusu = 0;
            int iNyukasu = 0;
            int iJutyusu = 0;
            int iSyukasiji = 0;
            int iHzan = 0;
            int iJzan = 0;
            int iTzaiko = 0;

            Boolean flg = false;
            int iJutyusu_bk = 0;

            string sql01 = "";
            string sql11 = "";
            string sql02 = "";
            string sql12 = "";
            string sql3 = "";
            string sql4 = "";

            
            //サーバー接続
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
            MySqlConnection conn = new MySqlConnection(connstr);
            conn.Open();
            string connstr1 = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
            MySqlConnection conn1 = new MySqlConnection(connstr1);
            conn1.Open();

            // データを格納するテーブル作成
            DataTable dt = new DataTable();

            DateTime dt0;
            
            

            if (day1 != "")
            {
                day1 = "'" + day1.Replace("/", "-") + "'";
                sql01 = " and nyukaymd >= " + day1;
                sql11 = " and sijibi >= " + day1;
            }
            if (day2 != "")
            {
                day2 = "'" + day2.Replace("/", "-") + "'";
                sql02 = " and nyukaymd <= " + day2;
                sql12 = " and sijibi <= " + day2;
            }
            if (hinban1 != "")
            {
                sql3 = " and hinban >= " + hinban1;
            }
            if (hinban2 != "")
            {
                sql4 = " and hinban <= " + hinban2;
            }

            //SQL文
            string sql = "select nyukasoko as socd, shiresaki as saki, hinban, shohinmei, nyukaymd as ymd, nkbn, 0 as dkbn, tancd, syukasu, 0 as urikin from nyukalog " +
                "where hinban > 0" + sql01 + sql02 + sql3 + sql4 + 
                " union all " +
                "select syukosocd as socd, tokuisaki as saki, hinban, shohinmei, sijibi as ymd, nkbn, syukasu as dkbn, tancd, 0 as syukasu, urikin from syukalog " +
                "where hinban > 0" +　sql11 + sql12 + sql3 + sql4 +
                " order by hinban, ymd";
            
            //SQL実行
            MySqlCommand cmd = new MySqlCommand(sql, conn1);
            //cmd.Parameters.Add(new MySqlParameter("shocd", dataGridView2[colhinban, row].Value.ToString()));
            MySqlDataReader reader = cmd.ExecuteReader();
            //cmd.Connection.Close();

            //テーブル出力
            while (reader.Read())
            {
                socd = reader["socd"].ToString();
                saki = reader["saki"].ToString();
                hinban = reader["hinban"].ToString();
                shoinmei = reader["shohinmei"].ToString();
                ymd = reader["ymd"].ToString().Substring(0, 10);
                nkbn = reader["nkbn"].ToString();
                dkbn = reader["dkbn"].ToString();
                tancd = reader["tancd"].ToString();
                syukasu = reader["syukasu"].ToString();
                urikin = reader["urikin"].ToString();

               
                //品番
                iHinban = long.Parse(hinban);
                //商品名
                sShohinmei = shoinmei;
                //日付
                sYmd = ymd;
                //担当
                MySqlCommand cmd1 = new MySqlCommand("select tanmei from tantousha where tancd = @tancd", conn);
                cmd1.Parameters.Add(new MySqlParameter("tancd", tancd));
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                while (reader1.Read())
                {
                    sTancd = reader1["tanmei"].ToString();
                }
                cmd1.Connection.Close();
                cmd1.Connection.Open();

                //入荷入力データの場合
                if (dkbn == "0")
                {
                    //区分
                    switch (nkbn)
                    {
                        case "1":
                            sNkbn = "受注";
                            break;
                        case "2":
                            sNkbn = "発注";
                            break;
                        case "3":
                            sNkbn = "入荷";
                            break;
                        case "4":
                            sNkbn = "取置";
                            break;
                    }
                    //取引先名
                    switch (nkbn)
                    {
                        case "1":
                            cmd1 = new MySqlCommand("select tokuisakimei from tokuisaki where tokuicd = @tokuicd", conn);
                            cmd1.Parameters.Add(new MySqlParameter("tokuicd", saki));
                            reader1 = cmd1.ExecuteReader();
                            while (reader1.Read())
                            {
                                sTorimei = reader1["tokuisakimei"].ToString();
                            }
                            cmd1.Connection.Close();
                            cmd1.Connection.Open();
                            
                            break;
                        case "2":
                        case "3":
                            cmd1 = new MySqlCommand("select shiresakimei from shiresaki where scd = @scd", conn);
                            cmd1.Parameters.Add(new MySqlParameter("scd", saki));
                            reader1 = cmd1.ExecuteReader();
                            while (reader1.Read())
                            {
                                sTorimei = reader1["shiresakimei"].ToString();
                            }
                            cmd1.Connection.Close();
                            cmd1.Connection.Open(); 
                            break;
                        case "4":
                            sTorimei = "取置";
                            break;
                    }
                    //発注数
                    if (nkbn == "2")
                        iHatyusu = int.Parse(syukasu);
                    else
                        ihattyusu = 0;
                    //入荷数
                    if (nkbn == "3")
                        iNyukasu = int.Parse(syukasu);
                    else
                        iNyukasu = 0;
                    //受注数
                    if (nkbn == "1")
                    {
                        iJutyusu = int.Parse(syukasu);
                        if (!flg)
                        {
                            iJutyusu_bk = iJutyusu;
                            flg = true;
                        }
                        else
                        {
                            iJutyusu = iJutyusu_bk - iNyukasu;
                        }
                            
                    }
                    else
                        iJutyusu = 0;
                    //出荷指示数
                    iSyukasiji = 0;
                    //発注残
                    if (flg)
                        iHzan = iJutyusu_bk - iNyukasu;
                    else
                        iHzan = 0;
                    //受注残
                    iJzan = 0;
                    //取置在庫
                    iTzaiko = 0;
                }
                
                //出荷指示入力データの場合
                else
                {
                    //区分
                    sNkbn = "出荷";
                    //取引先名
                    cmd1 = new MySqlCommand("select tokuisakimei from tokuisaki where tokuicd = @tokuicd", conn);
                    cmd1.Parameters.Add(new MySqlParameter("tokuicd", saki));
                    reader1 = cmd1.ExecuteReader();
                    while (reader1.Read())
                    {
                        sTorimei = reader1["tokuisakimei"].ToString();
                    }
                    cmd1.Connection.Close();
                    cmd1.Connection.Open();
                    //発注数
                    iHatyusu = 0;
                    //入荷数
                    iNyukasu = 0;
                    //受注数
                    iJutyusu = 0;
                    //出荷指示数
                    iSyukasiji = int.Parse(urikin);
                    //発注残
                    iHzan = 0;
                    //受注残
                    iJzan = 0;
                    //取置在庫
                    if (nkbn == "4")
                        itzaiko = int.Parse(urikin);
                    else
                        itzaiko = 0;
                    
                }

                procucts.Add(new Product(iHinban, sShohinmei, sYmd, sNkbn, sTancd, sTorimei, iHatyusu, iNyukasu, iJutyusu, iSyukasiji,
                    iHzan, iJzan, iTzaiko));
            }
            if (kubun == "1") {
                dataGridView1.DataSource = procucts;

                dataGridView1.Columns[0].HeaderText = "コード";
                dataGridView1.Columns[0].Width = 110;
                dataGridView1.Columns[0].ReadOnly = true;
                dataGridView1.Columns[1].HeaderText = "品名";
                dataGridView1.Columns[1].Width = 100;
                dataGridView1.Columns[2].HeaderText = "日付";
                dataGridView1.Columns[2].Width = 80;
                dataGridView1.Columns[2].ReadOnly = true;
                dataGridView1.Columns[3].HeaderText = "区分";
                dataGridView1.Columns[3].Width = 60;
                dataGridView1.Columns[3].ReadOnly = true;
                dataGridView1.Columns[4].HeaderText = "担当";
                dataGridView1.Columns[4].Width = 70;
                dataGridView1.Columns[4].ReadOnly = true;
                dataGridView1.Columns[5].HeaderText = "取引先名";
                dataGridView1.Columns[5].Width = 90;
                dataGridView1.Columns[5].ReadOnly = true;
                dataGridView1.Columns[6].HeaderText = "発注数";
                dataGridView1.Columns[6].Width = 70;
                dataGridView1.Columns[6].ReadOnly = true;
                dataGridView1.Columns[7].HeaderText = "入荷数";
                dataGridView1.Columns[7].Width = 70;
                dataGridView1.Columns[7].ReadOnly = true;
                dataGridView1.Columns[8].HeaderText = "受注数";
                dataGridView1.Columns[8].Width = 70;
                dataGridView1.Columns[8].ReadOnly = true;
                dataGridView1.Columns[9].HeaderText = "出荷指示数";
                dataGridView1.Columns[9].Width = 100;
                dataGridView1.Columns[9].ReadOnly = true;
                dataGridView1.Columns[10].HeaderText = "発注残";
                dataGridView1.Columns[10].Width = 70;
                dataGridView1.Columns[10].ReadOnly = true;
                dataGridView1.Columns[11].HeaderText = "受注残";
                dataGridView1.Columns[11].Width = 70;
                dataGridView1.Columns[11].ReadOnly = true;
                dataGridView1.Columns[12].HeaderText = "取置在庫数";
                dataGridView1.Columns[12].Width = 110;
                dataGridView1.Columns[12].ReadOnly = true;

                // カラムの右寄せ3桁区切り設定
                for (int i = 0; i <= 12; i++)
                {
                    //dataGridView1.Columns[i].DefaultCellStyle.Format = "#,0";
                    dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                for (int i = 6; i <= 12; i++)
                {
                    dataGridView1.Columns[i].DefaultCellStyle.Format = "#,0";
                    dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }

            }
            else if (kubun == "2"){
                string filename1 = "C:\\zaiko\\temp.csv";
                filename2 = "C:\\zaiko\\" + filename2 + ".csv";
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using (var streamWriter = new StreamWriter(filename1))
                using (var csvWriter = new CsvWriter(streamWriter))
                {
                    //csvWriter.Configuration.
                    csvWriter.Configuration.HasHeaderRecord = true;
                    csvWriter.Configuration.RegisterClassMap<ProductMapper>();
                    csvWriter.WriteRecords(procucts);

                }
                Utf8ToSjis(filename1, filename2);
                this.Close();
            }
        }
        static int Utf8ToSjis(string fname1, string fname2)
        {
            // ※既存ファイルへの上書きチェック等は別途行ってください

            // ファイルをbyte形で全て読み込み
            FileStream fs1 = new FileStream(fname1, FileMode.Open);
            byte[] data = new byte[fs1.Length];
            fs1.Read(data, 0, data.Length);
            fs1.Close();

            // UTF-8 -> Shift-JIS 変換（byte形）
            Encoding utf8Enc = Encoding.GetEncoding("UTF-8");
            string utf8str = utf8Enc.GetString(data);
            byte[] bytesData =
                System.Text.Encoding.GetEncoding("Shift_JIS").GetBytes(utf8str);

            // string型に変換したい場合はこんな感じに
            // Encoding sjisEnc = Encoding.GetEncoding("Shift-JIS");
            // string sjis = sjisEnc.GetString(bytesData);
            // Console.WriteLine(sjis);

            // 出力ファイルオープン（バイナリ形式）
            FileStream fs2 = new FileStream(fname2, FileMode.Create);
            // 書き込み設定（Shift-JIS）
            BinaryWriter bw = new
                BinaryWriter(fs2, System.Text.Encoding.GetEncoding("Shift_JIS"));

            // 出力ファイルへ全て書き込み
            bw.Write(bytesData);
            bw.Close();
            fs2.Close();

            return 0;
        }
    }

    
}

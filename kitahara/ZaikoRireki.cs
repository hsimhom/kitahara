using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kitahara
{
    public partial class ZaikoRireki : Form
    {
        int iKubun = 0;
        DateTime dt1;
        DateTime dt2;

        public ZaikoRireki()
        {
            InitializeComponent();
        }

        //DataGridView以外のコントロールのキー押下検知
        [System.Security.Permissions.UIPermission(
    System.Security.Permissions.SecurityAction.Demand,
    Window = System.Security.Permissions.UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogKey(Keys keyData)
        {
            //Enterキーが押されているか調べる
            if ((keyData & Keys.KeyCode) == Keys.Enter)
            {
                check_textbox();
                //this.SelectNextControl(this.ActiveControl, true, true, true, true);
                //本来の処理（左側のコントロールにフォーカスを移す）を
                //させたくないときは、trueを返す
                return true;
            }
            //Tabキーが押されているか調べる
            else if ((keyData & Keys.KeyCode) == Keys.Tab)
            {
                check_textbox();

                //MessageBox.Show(cControl.Name);
            }
            

            return base.ProcessDialogKey(keyData);
        }
        private void check_textbox()
        {
            Control cControl = this.ActiveControl;
            //string row = "";

            //サーバー接続
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67;Charset='utf8'";
            MySqlConnection conn = new MySqlConnection(connstr);
            conn.Open();

            switch (cControl.Name)
            {
                case "txtKubun":
                    if (Regex.IsMatch(txtKubun.Text, @"^[1-2]{1}$"))
                    {
                        iKubun = int.Parse(txtKubun.Text);
                        
                        txtDate1.Focus();
                    }
                    else
                    {
                        MessageBox.Show("1,2を入力してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtKubun.Focus();
                        return;
                    }
                    if (txtKubun.Text == "1")
                        txtCsv.Enabled = false;
                    else
                        txtCsv.Enabled = true;
                    break;
                case "txtDate1":
                    if (!DateTime.TryParse(txtDate1.Text, out dt1))
                    {
                        if (txtDate1.TextLength != 4)
                        {
                            //変換出来たら、dtにその値が入る
                            MessageBox.Show("日付が正しくありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtDate1.Focus();
                        }
                        else
                        {
                            DateTime dt0 = DateTime.Now;
                            string temp1 = txtDate1.Text;
                            string temp2 = temp1.Insert(2, "/");
                            string temp3 = temp2.Insert(0, "/");
                            txtDate1.Text = dt0.Year.ToString() + temp3;
                            DateTime.TryParse(txtDate1.Text, out dt1);
                            txtDate2.Focus();
                        }
                    }
                    else
                        txtDate2.Focus();
                    break;
                case "txtDate2":
                    if (!DateTime.TryParse(txtDate2.Text, out dt2))
                    {
                        if (txtDate2.TextLength != 4)
                        {
                            //変換出来たら、dtにその値が入る
                            MessageBox.Show("日付が正しくありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtDate2.Focus();
                        }
                        else
                        {
                            DateTime dt0 = DateTime.Now;
                            string temp1 = txtDate2.Text;
                            string temp2 = temp1.Insert(2, "/");
                            string temp3 = temp2.Insert(0, "/");
                            txtDate2.Text = dt0.Year.ToString() + temp3;
                            DateTime.TryParse(txtDate2.Text, out dt2);
                            txtHinban1.Focus();
                        }
                    }
                    else
                        txtHinban1.Focus();
                    break;
                case "txtHinban1":
                    txtHinban2.Focus();
                    break;
                case "txtHinban2":
                    if (iKubun == 1)
                        btn_Do.Focus();
                    else
                        txtCsv.Focus();
                    break;
                case "txtCsv":
                    btn_Do.Focus();
                    break;

            }
        }
        private void btn_Do_Click(object sender, EventArgs e)
        {
            //
            if (txtDate1.Text != "" && txtDate2.Text != "") {
                if (dt1 > dt2)
                {
                    MessageBox.Show("正しい日付を入力してください。");
                    return;
                }
            }
            if (txtHinban1.Text != "" && txtHinban2.Text != "")
            {
                if (long.Parse(txtHinban1.Text) > long.Parse(txtHinban2.Text))
                {
                    MessageBox.Show("正しい品番を入力してください。");
                    return;
                }
            }
            if (!Regex.IsMatch(txtKubun.Text, @"^[1-2]{1}$"))
            {
                MessageBox.Show("区分を入力してください。");
                return;
            }
            if (txtKubun.Text == "2")
            {
                if (txtCsv.Text == "")
                {
                    MessageBox.Show("ファイル名を入力してください。");
                    return;
                }
            }


            ZaikoRireki_View zr = new ZaikoRireki_View();
            zr.kubun = txtKubun.Text;
            zr.day1 = txtDate1.Text;
            zr.day2 = txtDate2.Text;
            zr.hinban1 = txtHinban1.Text;
            zr.hinban2 = txtHinban2.Text;
            zr.filename2 = txtCsv.Text;

            zr.ShowDialog();
            if (txtKubun.Text == "2")
                MessageBox.Show("CSVファイルを出力しました。");
        }

        private void btn_End_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        
    }
}

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kitahara
{
    public partial class Sizecolor : Form
    {
        public int[,] scdata = new int[11,11];
        public int total = 0;

        public Sizecolor()
        {
            InitializeComponent();

            InitGrid();
        }

        private void InitGrid()
        {
            //サーバー接続
            string connstr = "userid=root; password=baron6533; database = zaiko; Data Source=133.167.117.67";
            MySqlConnection conn = new MySqlConnection(connstr);
            conn.Open();

            // データを格納するテーブル作成
            DataTable dt = new DataTable();

            // SQL文と接続情報を指定し、データアダプタを作成
            MySqlDataAdapter da = new MySqlDataAdapter("select title,free,s,m,l,ll,3l,4l,5l,6l,ex1,ex2 from tmp_colorsize", conn);

            // データ取得
            da.Fill(dt);

            // データ表示
            dataGridView1.DataSource = dt;

            dataGridView1.Columns[0].HeaderText = "";
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].HeaderText = "フリー";
            dataGridView1.Columns[1].Width = 80;
            dataGridView1.Columns[2].HeaderText = "Ş";
            dataGridView1.Columns[2].Width = 80;
            dataGridView1.Columns[3].HeaderText = "M";
            dataGridView1.Columns[3].Width = 80;
            dataGridView1.Columns[4].HeaderText = "L";
            dataGridView1.Columns[4].Width = 80;
            dataGridView1.Columns[5].HeaderText = "LL";
            dataGridView1.Columns[5].Width = 80;
            dataGridView1.Columns[6].HeaderText = "３L";
            dataGridView1.Columns[6].Width = 80;
            dataGridView1.Columns[7].HeaderText = "４L";
            dataGridView1.Columns[7].Width = 80;
            dataGridView1.Columns[8].HeaderText = "５L";
            dataGridView1.Columns[8].Width = 80;
            dataGridView1.Columns[9].HeaderText = "６L";
            dataGridView1.Columns[9].Width = 80;
            dataGridView1.Columns[10].HeaderText = "　";
            dataGridView1.Columns[10].Width = 80;
            dataGridView1.Columns[11].HeaderText = "　";
            dataGridView1.Columns[11].Width = 80;
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
                //check_textbox();
                //this.SelectNextControl(this.ActiveControl, true, true, true, true);
                //本来の処理（左側のコントロールにフォーカスを移す）を
                //させたくないときは、trueを返す
                //return true;
            }
            //Tabキーが押されているか調べる
            else if ((keyData & Keys.KeyCode) == Keys.Tab)
            {
                //check_textbox();

                //MessageBox.Show(cControl.Name);
            }
            else if ((keyData & Keys.KeyCode) == Keys.F10)
            {
                btnTouroku.PerformClick();
            }
            else if ((keyData & Keys.KeyCode) == Keys.F5)
            {
                btnCancel.PerformClick();
            }

            return base.ProcessDialogKey(keyData);
        }

        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            //メッセージボックスを表示する
            DialogResult result = MessageBox.Show("取消しますか？",
                "質問",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.Yes)
            {
                //「はい」が選択された時
                InitGrid();
            }
            else if (result == DialogResult.No)
            {
                //「いいえ」が選択された時
                return;
            }
        }

        private void btnTouroku_Click(object sender, EventArgs e)
        {
            dataGridView1.EndEdit();
            //メッセージボックスを表示する
            DialogResult result = MessageBox.Show("登録しますか？",
                "質問",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.Yes)
            {
                //「はい」が選択された時
                for (int i = 0; i < 11; i++)
                {
                    for (int j = 0; j < 11; j++)
                    {
                        scdata[i, j] = int.Parse(dataGridView1[i+1, j].Value.ToString());
                    }
                    total += int.Parse(dataGridView1[i+1, 11].Value.ToString());
                }

                this.Close();
            }
            else if (result == DialogResult.No)
            {
                //「いいえ」が選択された時
                return;
            }
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            //メッセージボックスを表示する
            DialogResult result = MessageBox.Show("終了しますか？",
                "質問",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.Yes)
            {
                //「はい」が選択された時
                this.Close();
            }
            else if (result == DialogResult.No)
            {
                //「いいえ」が選択された時
                return;
            }
        }

        

        private void dataGridView1_CellEnter_1(object sender, DataGridViewCellEventArgs e)
        {
            switch (dataGridView1.CurrentCellAddress.X)
            {
                case 0:
                    Action a = () => dataGridView1.CurrentCell = dataGridView1[1, dataGridView1.CurrentCellAddress.Y];
                    BeginInvoke(a);
                    break;
            }
        }

        private void dataGridView1_EditingControlShowing_1(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //表示されているコントロールがDataGridViewTextBoxEditingControlか調べる
            if (e.Control is DataGridViewTextBoxEditingControl)
            {
                DataGridView dgv = (DataGridView)sender;

                //編集のために表示されているコントロールを取得
                DataGridViewTextBoxEditingControl tb =
                    (DataGridViewTextBoxEditingControl)e.Control;

                //イベントハンドラを削除
                tb.KeyDown -= dataGridView1_KeyDown_1;
                tb.PreviewKeyDown -= dataGridView1_PreviewKeyDown_1;
                tb.KeyDown += dataGridView1_KeyDown_1;
                tb.PreviewKeyDown += dataGridView1_PreviewKeyDown_1;
            }
        }

        private void dataGridView1_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                if (dataGridView1.CurrentCellAddress.X != 0)
                {
                    int sum = 0;
                    for (int i = 0; i < 11; i++)
                    {
                        sum += int.Parse(dataGridView1[dataGridView1.CurrentCellAddress.X, i].Value.ToString());
                    }
                    dataGridView1[dataGridView1.CurrentCellAddress.X, 11].Value = sum.ToString();
                    dataGridView1.EndEdit();
                }
            }
        }

        private void dataGridView1_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
                e.Handled = true;
            }
        }
    }
}

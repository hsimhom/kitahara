using System;
using System.Windows.Forms;

namespace System.Windows.Forms
{
    /// <summary>
    /// Enterキーが押された時に、Tabキーが押されたのと同じ動作をする
    /// （現在のセルを隣のセルに移動する）DataGridView
    /// </summary>
    internal class DataGridViewEX : DataGridView
    {
        [System.Security.Permissions.UIPermission(
            System.Security.Permissions.SecurityAction.Demand,
            Window = System.Security.Permissions.UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogKey(Keys keyData)
        {
            //Enterキーが押された時は、Tabキーが押されたようにする
            if ((keyData & Keys.KeyCode) == Keys.Enter)
            {
                return this.ProcessTabKey(keyData);
            }
            return base.ProcessDialogKey(keyData);
        }

        [System.Security.Permissions.SecurityPermission(
            System.Security.Permissions.SecurityAction.Demand,
            Flags = System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
        protected override bool ProcessDataGridViewKey(KeyEventArgs e)
        {
            //Enterキーが押された時は、Tabキーが押されたようにする
            if (e.KeyCode == Keys.Enter)
            {
                return this.ProcessTabKey(e.KeyCode);
            }
            return base.ProcessDataGridViewKey(e);
        }
    }

    class DataGridViewEx : DataGridViewEX
    {
    }
}
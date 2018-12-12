using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TecoRP.Models;

namespace TecoRP_ItemEditor
{
    public partial class frmFullList : Form
    {
        public frmFullList()
        {
            InitializeComponent();
            dgvItemsList.DataSource = Database.db_Items.currentItems.Items;

            foreach (var item in Enum.GetNames(typeof(ItemType)))
            {
                cmbCategories.Items.Add(item);
            }

            dgvItemsList.CellEndEdit += DgvItemsList_CellEndEdit;
        }

        private void DgvItemsList_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Database.db_Items.SaveChanges();
        }


        private void frmFullList_SizeChanged(object sender, EventArgs e)
        {
            dgvItemsList.Height = Convert.ToInt32(this.Height * 0.8f);
        }
        private void ResetList()
        {
            dgvItemsList.DataSource = Database.db_Items.currentItems;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            dgvItemsList.DataSource = Database.db_Items.currentItems.Items.Where(x =>x !=null ? ( x.Name.Contains(txtSearch.Text) || (String.IsNullOrEmpty(x.Description) ? false : x.Description.Contains(txtSearch.Text)) || x.ObjectId.ToString().StartsWith(txtSearch.Text)) : false).ToList();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            ResetList();
        }

        private void cmbCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCategories.SelectedIndex!=-1)
            {
                ItemType selectedType = (ItemType)Enum.Parse(typeof(ItemType), cmbCategories.SelectedItem.ToString());
                dgvItemsList.DataSource = Database.db_Items.currentItems.Items.Where(x => x.Type == selectedType).ToList();
            }else
            {
                ResetList();
            }
        }

        private void dosyaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Database.db_Items.SaveChanges();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            dgvItemsList.DataSource = null;
            dgvItemsList.DataSource = Database.db_Items.currentItems.Items;
        }
    }
}

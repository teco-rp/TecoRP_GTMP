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
    public partial class frmItemAdd : Form
    {
        public frmItemAdd()
        {
            InitializeComponent();
            foreach (var item in typeof(ItemType).GetEnumNames())
            {
                cmbType.Items.Add(item);
            }
        }

        private void btnCreate_Click_1(object sender, EventArgs e)
        {
            ItemType _Type;
            try{ _Type = (ItemType)Enum.Parse(typeof(ItemType), cmbType.SelectedItem.ToString());  }
            catch (Exception)
            {
                _Type = ItemType.None;
            }
            Database.db_Items.CreateItem(new Item
            {
                Name = txtName.Text,
                Description = txtDescription.Text,
                MaxCount = Convert.ToInt32(nmrMaxStack.Value),
                ObjectId = String.IsNullOrEmpty(txtObjectID.Text) ? 0 : Convert.ToInt32(txtObjectID.Text),
                Type = _Type,
                Value_0 = txtValue0.Text,
                Value_1 = Convert.ToInt32(nmrValue1.Value).ToString(),
                Value_2 = Convert.ToInt32(nmrValue2.Value).ToString(),
            });

            this.Hide();
        }

        private void frmItemAdd_Load(object sender, EventArgs e)
        {

        }
    }
}

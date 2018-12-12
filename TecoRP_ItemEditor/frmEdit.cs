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
using TecoRP_ItemEditor.Model;

namespace TecoRP_ItemEditor
{
    public partial class frmEdit : Form
    {
        public frmEdit(Item _willBeEditedItem)
        {
            InitializeComponent();
            txtName.Text = _willBeEditedItem.Name;
            txtDescription.Text = _willBeEditedItem.Description;
            txtObjectID.Text = _willBeEditedItem.ObjectId.ToString();
            txtValue0.Text = _willBeEditedItem.Value_0;
            nmrValue1.Value = Convert.ToDecimal(_willBeEditedItem.Value_1);
            nmrValue2.Value = Convert.ToDecimal(_willBeEditedItem.Value_2);
            foreach (var item in Enum.GetValues(typeof(ItemType)))
            {
                cmbType.Items.Add(item);
            }
            cmbType.SelectedIndex = Convert.ToInt32(_willBeEditedItem.Type);
        }

        private void frmEdit_Load(object sender, EventArgs e)
        {

        }
        
    }
}

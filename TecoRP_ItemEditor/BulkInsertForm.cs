using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TecoRP_ItemEditor.Database;

namespace TecoRP_ItemEditor
{
    public partial class BulkInsertForm : Form
    {
        public BulkInsertForm()
        {
            InitializeComponent();

            cmbType.Items.Clear();
            foreach (var item in typeof(TecoRP.Models.ItemType).GetEnumNames())
            {
                cmbType.Items.Add(item);
            }
        }

        private void nmrValue1_ValueChanged(object sender, EventArgs e)
        {
            if (nmrValue1End.Value > nmrValue1End.Value)
            {
                nmrValue1End.Value = nmrValue1.Value;
            }
        }

        private void nmrValue2_ValueChanged(object sender, EventArgs e)
        {
            if (nmrValue2.Value > nmrValue2End.Value)
            {
                nmrValue2End.Value = nmrValue2.Value;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int count = (int)(nmrValue1End.Value - nmrValue1.Value + 1) * (int)(nmrValue2End.Value - nmrValue2.Value + 1);
                var result = MessageBox.Show( $"{count} adet item oluşturulacak ve id {(int)nmrID.Value + count} 'de bitecek.\n\nEmin misiniz?","", MessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                    return;


                progressBar1.Value = 0;

                int id = (int)nmrID.Value;
                for (int i = (int)nmrValue1.Value; i <= (int)nmrValue1End.Value; i++)
                {

                    for (int j = (int)nmrValue2.Value; j <= (int)nmrValue2End.Value; j++)
                    {
                        db_Items.currentItems.Items.Add(new TecoRP.Models.Item
                        {
                            Description = ReplaceParameters(txtDescription.Text),
                            Name = ReplaceParameters(txtName.Text),
                            Droppable = chkDroppable.Checked,
                            ID = id,
                            MaxCount = (int)nmrMaxCount.Value,
                            ObjectId = Convert.ToInt32(txtObjectId.Text),
                            Type = (TecoRP.Models.ItemType)Enum.Parse(typeof(TecoRP.Models.ItemType), cmbType.SelectedItem?.ToString()),
                            Value_0 = txtValue0.Text,
                            Value_1 = i.ToString(),
                            Value_2 = j.ToString(),
                            Value_3 = nmrValue3.Value.ToString()
                        });
                        string ReplaceParameters(string text)
                        {
                            return text.Replace("{Value_1}", i.ToString())
                                        .Replace("{Value_2}", j.ToString())
                                        .Replace("{id}", id.ToString());
                        }
                        id++;
                        progressBar1.Value += (int)((1f / count) * 100);
                    }
                }

                db_Items.SaveChanges();
                MessageBox.Show($"{count} adet eşya başarıyla eklendi.");
                progressBar1.Value = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}

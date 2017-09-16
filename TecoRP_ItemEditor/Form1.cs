using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TecoRP_ItemEditor.Database;
using TecoRP_ItemEditor.Model;

namespace TecoRP_ItemEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dgvTaxes.CellEndEdit += DgvTaxes_CellEndEdit;
            dgvWhiteList.CellEndEdit += DgvWhiteList_CellEndEdit;

            dtpWhitelistLastValidateTime.Value = DateTime.Now.AddYears(2);
            foreach (var item in typeof(ItemType).GetEnumNames())
            {
                cmbType.Items.Add(item);
            }
        }

        private void DgvWhiteList_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Database.WhiteListDAL.Save();
        }

        private void DgvTaxes_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            db_Taxes.SaveChanges();
        }

        private void ekleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmItemAdd frm = new TecoRP_ItemEditor.frmItemAdd();
            frm.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FillData();
            FillCraftingData();
            ResetCraftingTab();
            //db_Taxes.GenerateAllVehicles();
        }
        private void FillCraftingData()
        {
            //foreach (var item in Enum.GetValues(typeof(ItemType)))
            //{
            //    cmbFilterItems.Items.Add(item);
            //}
            ////TecoRP.Database.db_Craftings.Init();
            //foreach (var item in TecoRP.Database.db_Craftings.GetAllCraftingTableModels())
            //{
            //    lstCraftingMachines.Items.Add(item);
            //}
        }
        public void FillData()
        {
            lstItems.Items.Clear();
            Task.Run(() =>
            {
                db_Taxes.GetAllTaxes();
                dgvTaxes.DataSource = db_Taxes.currentTaxes;
            });

            db_Taxes.GetAllTaxes();
            foreach (var item in db_Items.GetAll().Items)
            {
                lstItems.Items.Add(item.Name);
            }


            dgvWhiteList.DataSource = Database.WhiteListDAL.currentUsers.Users;
            chkWhiteListActive.Checked = Database.WhiteListDAL.currentUsers.IsEnabled;
            
        }

        private void ResetCraftingTab()
        {
            gbAddNew.Enabled = false;
            gbAddCrafting.Enabled = false;
            btnAddRequiredItems.Enabled = false;
        }

        public void FillData(List<Model.Item> _list)
        {
            lstItems.Items.Clear();
            foreach (var item in _list)
            {
                lstItems.Items.Add(item.Name);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            db_Items.currentItems.Items = db_Items.currentItems.Items.OrderBy(o => o.ID).ToList();
            db_Items.SaveChanges();
            FillData();
        }

        private void valueTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ValueTypes frmType = new TecoRP_ItemEditor.ValueTypes();
            frmType.Show();
        }

        private void lstItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstItems.SelectedIndex >= 0)
            {
                btnRemove.Enabled = true;
                btnSave.Enabled = true;
                var model = db_Items.currentItems.Items[lstItems.SelectedIndex];
                txtName.Text = model.Name;
                txtDescription.Text = model.Description;
                txtObjectId.Text = model.ObjectId.ToString();
                cmbType.SelectedItem = typeof(ItemType).GetEnumName(model.Type);
                chkDroppable.Checked = model.Droppable;
                txtValue0.Text = model.Value_0;
                nmrValue1.Text = model.Value_1;
                nmrValue2.Text = model.Value_2;
                nmrMaxCount.Value = model.MaxCount;
                nmrID.Value = model.ID;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            db_Items.RemoveByIndex(lstItems.SelectedIndex);
            lstItems.Items.RemoveAt(lstItems.SelectedIndex);
            btnRemove.Enabled = false;
            btnSave.Enabled = false;
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            FillData();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try { Convert.ToInt32(txtObjectId.Text); } catch { MessageBox.Show("Obje ID'si sayı olmalı."); return; }
            var oldId = db_Items.currentItems.Items.FirstOrDefault(x => x.ID == Convert.ToInt32(nmrID.Value));
            if (oldId != null)
            {
                oldId.ID = Convert.ToInt32(nmrID.Value);
            }
            var _Index = lstItems.SelectedIndex;
            db_Items.currentItems.Items[_Index].Name = txtName.Text;
            db_Items.currentItems.Items[_Index].Description = txtDescription.Text;
            db_Items.currentItems.Items[_Index].ObjectId = Convert.ToInt32(txtObjectId.Text);
            db_Items.currentItems.Items[_Index].MaxCount = Convert.ToInt32(nmrMaxCount.Value);
            db_Items.currentItems.Items[_Index].Value_0 = txtValue0.Text;
            db_Items.currentItems.Items[_Index].Value_1 = nmrValue1.Value.ToString();
            db_Items.currentItems.Items[_Index].Value_2 = nmrValue2.Value.ToString();
            db_Items.currentItems.Items[_Index].Droppable = chkDroppable.Checked;
            db_Items.currentItems.Items[_Index].MaxCount = Convert.ToInt32(nmrMaxCount.Value);
            db_Items.currentItems.Items[_Index].Type = (ItemType)Enum.Parse(typeof(ItemType), cmbType.SelectedItem.ToString());
            db_Items.currentItems.Items[_Index].ID = Convert.ToInt32(nmrID.Value);
            db_Items.SaveChanges();
            FillData();
        }

        private void kaydetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            db_Items.SaveChanges();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            TecoRP.Models.CraftingTable _table = new TecoRP.Models.CraftingTable();
            _table.Name = "Birincil Boya Tezgahı";
            _table.ObjectId = 2126419969;
            _table.Craftings = new List<TecoRP.Models.CraftingItem>();
            for (int i = 800; i < 875; i++)
            {
                _table.Craftings.Add(new TecoRP.Models.CraftingItem
                {
                    CraftedGameItemId = i,
                     RequiredJob = 11,
                     RequiredJobLevel = 1,
                     RequiredMetalPart = 65,
                });
            }

            TecoRP.Database.db_Craftings.CreateCraftingTableModel(_table);
            MessageBox.Show("Tamamlandı");


            //foreach (var item in db_Items.currentItems.Items)
            //{
            //    if (item.Type == ItemType.Skin)
            //    {
            //        item.Name = item.Name + " Kıyafeti";
            //    }
            //}
            //db_Items.SaveChanges();
            //MessageBox.Show("Completed");
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            db_Items.currentItems.Items = db_Items.currentItems.Items.OrderBy(x => x.ID).ToList();
            db_Items.SaveChanges();
            FillData();
        }

        private void tamListeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFullList newForm = new TecoRP_ItemEditor.frmFullList();
            newForm.Show();
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            lstItems.Height = tabControl1.Height;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            dgvTaxes.DataSource = db_Taxes.currentTaxes.Where(x => x != null ? (x.VehicleName.ToString().ToLower().Contains(txtSearchBoxTax.Text.ToLower())) : false).ToList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FillData();
        }

        private void btnAddWhiteList_Click(object sender, EventArgs e)
        {
            Database.WhiteListDAL.currentUsers.Users.Add(new TecoRP.Models.WhiteListUser
            {
                LastValidateTime = dtpWhitelistLastValidateTime.Value,
                SocialClubName = txtWhitelistSocialClubName.Text
            });
            Database.WhiteListDAL.Save();
        }

        private void dgvTaxes_CellEndEdit_1(object sender, DataGridViewCellEventArgs e)
        {
            db_Taxes.SaveChanges();
        }

        private void cmbFilterItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstItemListInCrafting.Items.Clear();
            var _type = (ItemType)Enum.Parse(typeof(ItemType), cmbFilterItems.SelectedItem.ToString());
            foreach (var item in db_Items.GetAll().Items)
            {
                if (item.Type == _type)
                {
                    lstItemListInCrafting.Items.Add(item);
                }
            }
        }

        private void btnAddToCraftedItem_Click(object sender, EventArgs e)
        {
            if (lstItemListInCrafting.SelectedIndex != -1)
            {
                lstCraftedItem.Items.Add(lstItemListInCrafting.SelectedItem);
                (sender as Button).Enabled = false;
                btnAddRequiredItems.Enabled = true;
                gbAddCrafting.Enabled = true;
            }
        }

        private void btnAddRequiredItems_Click(object sender, EventArgs e)
        {
            if (lstItemListInCrafting.SelectedIndex != -1)
            {
                lstRequiredItems.Items.Add(lstItemListInCrafting.SelectedItem);
            }
        }

        private void lstRequiredItems_DoubleClick(object sender, EventArgs e)
        {
            if (lstRequiredItems.SelectedIndex != -1)
            {
                lstRequiredItems.Items.RemoveAt(lstRequiredItems.SelectedIndex);
            }
        }

        private void lstCraftedItem_DoubleClick(object sender, EventArgs e)
        {
            if (lstCraftedItem.SelectedIndex != -1)
            {
                lstRequiredItems.Items.Clear();
                lstCraftedItem.Items.RemoveAt(lstCraftedItem.SelectedIndex);
                btnAddToCraftedItem.Enabled = true;
            }
        }

        private void btnAddToMachine_Click(object sender, EventArgs e)
        {
            if (lstCraftedItem.Items.Count > 0)
            {
                gbAddNew.Enabled = true;
                btnAddToCraftedItem.Enabled = true;
                btnAddRequiredItems.Enabled = false;
                TecoRP.Models.CraftingItem _model = new TecoRP.Models.CraftingItem();
                _model.CraftedGameItemId = (lstCraftedItem.Items[0] as Item).ID;
                _model.RequiredJob = (int)nmrRequiredJob.Value;
                _model.RequiredJobLevel = (int)nmrRequiredJobLevel.Value;
                _model.RequiredMetalPart = (int)nmrRequiredMetalPart.Value;
                foreach (var item in lstRequiredItems.Items)
                {
                    _model.RequredItemIds.Add((item as Item).ID);
                }
                lstCraftings.Items.Add(_model);
                lstRequiredItems.Items.Clear();
                lstCraftedItem.Items.Clear();
            }
            else
            {
                MessageBox.Show("Lütfen Önce Üretilecek eşyayı ekleyiniz.");
            }
        }

        private void btnCreateCraftingMachine_Click(object sender, EventArgs e)
        {
            if (lstCraftings.Items.Count > 0)
            {
                List<TecoRP.Models.CraftingItem> _craftings = new List<TecoRP.Models.CraftingItem>();
                foreach (var item in lstCraftings.Items)
                {
                    _craftings.Add(item as TecoRP.Models.CraftingItem);
                }

                lstCraftingMachines.Items.Add(
                    TecoRP.Database.db_Craftings.CreateCraftingTableModel(new TecoRP.Models.CraftingTable
                    {
                        Craftings = _craftings,
                        ObjectId = int.Parse(txtObjectIDCraftingMachine.Text),
                        Name = txtNameCraftingTable.Text
                    })
                 );

                lstCraftings.Items.Clear();
                txtNameCraftingTable.Clear();
                txtObjectIDCraftingMachine.Clear();
            }
        }

        private void lstCraftingMachines_DoubleClick(object sender, EventArgs e)
        {
            if ((sender as ListBox).SelectedIndex > -1)
            {

                if (TecoRP.Database.db_Craftings.RemoveCraftingTableModel(((sender as ListBox).SelectedItem as TecoRP.Models.CraftingTable).CraftingTableId))
                    (sender as ListBox).Items.RemoveAt((sender as ListBox).SelectedIndex);
            }
        }

        private void tpCraftings_Click(object sender, EventArgs e)
        {

        }

        private void chkWhiteListActive_CheckedChanged(object sender, EventArgs e)
        {
            WhiteListDAL.currentUsers.IsEnabled = chkWhiteListActive.Checked;
            TecoRP.Database.db_WhiteList.SaveChanges(WhiteListDAL.currentUsers);
        }

        private void lstCraftings_DoubleClick(object sender, EventArgs e)
        {
            if ((sender as ListBox).SelectedIndex != -1)
            {
                (sender as ListBox).Items.RemoveAt((sender as ListBox).SelectedIndex);
            }
        }
    }
}
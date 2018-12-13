namespace TecoRP_ItemEditor
{
    partial class BulkInsertForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkDroppable = new System.Windows.Forms.CheckBox();
            this.lblName = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblId = new System.Windows.Forms.Label();
            this.nmrID = new System.Windows.Forms.NumericUpDown();
            this.lblMaxCount = new System.Windows.Forms.Label();
            this.nmrMaxCount = new System.Windows.Forms.NumericUpDown();
            this.lblDescription = new System.Windows.Forms.Label();
            this.nmrValue3 = new System.Windows.Forms.NumericUpDown();
            this.nmrValue2 = new System.Windows.Forms.NumericUpDown();
            this.lblType = new System.Windows.Forms.Label();
            this.nmrValue1 = new System.Windows.Forms.NumericUpDown();
            this.lblObjectId = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.lblValue0 = new System.Windows.Forms.Label();
            this.txtObjectId = new System.Windows.Forms.TextBox();
            this.lblValue1 = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.lblValue2 = new System.Windows.Forms.Label();
            this.txtValue0 = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nmrValue2End = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.nmrValue1End = new System.Windows.Forms.NumericUpDown();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.nmrID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrMaxCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrValue3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrValue2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrValue1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrValue2End)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrValue1End)).BeginInit();
            this.SuspendLayout();
            // 
            // chkDroppable
            // 
            this.chkDroppable.AutoSize = true;
            this.chkDroppable.Location = new System.Drawing.Point(192, 149);
            this.chkDroppable.Name = "chkDroppable";
            this.chkDroppable.Size = new System.Drawing.Size(75, 17);
            this.chkDroppable.TabIndex = 32;
            this.chkDroppable.Text = "Droppable";
            this.chkDroppable.UseVisualStyleBackColor = true;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(26, 82);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(22, 13);
            this.lblName.TabIndex = 13;
            this.lblName.Text = "Adı";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(107, 351);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(226, 23);
            this.btnSave.TabIndex = 31;
            this.btnSave.Text = "KAYDET";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblId
            // 
            this.lblId.AutoSize = true;
            this.lblId.Location = new System.Drawing.Point(26, 37);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(67, 13);
            this.lblId.TabIndex = 15;
            this.lblId.Text = "Başlangıç ID";
            // 
            // nmrID
            // 
            this.nmrID.Location = new System.Drawing.Point(119, 30);
            this.nmrID.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.nmrID.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmrID.Name = "nmrID";
            this.nmrID.Size = new System.Drawing.Size(54, 20);
            this.nmrID.TabIndex = 29;
            this.nmrID.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblMaxCount
            // 
            this.lblMaxCount.AutoSize = true;
            this.lblMaxCount.Location = new System.Drawing.Point(207, 66);
            this.lblMaxCount.Name = "lblMaxCount";
            this.lblMaxCount.Size = new System.Drawing.Size(88, 13);
            this.lblMaxCount.TabIndex = 16;
            this.lblMaxCount.Text = "Max Deste Sayısı";
            // 
            // nmrMaxCount
            // 
            this.nmrMaxCount.Location = new System.Drawing.Point(221, 82);
            this.nmrMaxCount.Maximum = new decimal(new int[] {
            5600,
            0,
            0,
            0});
            this.nmrMaxCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmrMaxCount.Name = "nmrMaxCount";
            this.nmrMaxCount.Size = new System.Drawing.Size(64, 20);
            this.nmrMaxCount.TabIndex = 28;
            this.nmrMaxCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(21, 119);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(50, 13);
            this.lblDescription.TabIndex = 18;
            this.lblDescription.Text = "Açıklama";
            // 
            // nmrValue3
            // 
            this.nmrValue3.Location = new System.Drawing.Point(269, 243);
            this.nmrValue3.Maximum = new decimal(new int[] {
            5600,
            0,
            0,
            0});
            this.nmrValue3.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            -2147483648});
            this.nmrValue3.Name = "nmrValue3";
            this.nmrValue3.Size = new System.Drawing.Size(64, 20);
            this.nmrValue3.TabIndex = 27;
            // 
            // nmrValue2
            // 
            this.nmrValue2.Location = new System.Drawing.Point(192, 243);
            this.nmrValue2.Maximum = new decimal(new int[] {
            5600,
            0,
            0,
            0});
            this.nmrValue2.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            -2147483648});
            this.nmrValue2.Name = "nmrValue2";
            this.nmrValue2.Size = new System.Drawing.Size(64, 20);
            this.nmrValue2.TabIndex = 26;
            this.nmrValue2.ValueChanged += new System.EventHandler(this.nmrValue2_ValueChanged);
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(36, 147);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(23, 13);
            this.lblType.TabIndex = 11;
            this.lblType.Text = "Tür";
            // 
            // nmrValue1
            // 
            this.nmrValue1.Location = new System.Drawing.Point(107, 243);
            this.nmrValue1.Maximum = new decimal(new int[] {
            5600,
            0,
            0,
            0});
            this.nmrValue1.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            -2147483648});
            this.nmrValue1.Name = "nmrValue1";
            this.nmrValue1.Size = new System.Drawing.Size(64, 20);
            this.nmrValue1.TabIndex = 30;
            this.nmrValue1.ValueChanged += new System.EventHandler(this.nmrValue1_ValueChanged);
            // 
            // lblObjectId
            // 
            this.lblObjectId.AutoSize = true;
            this.lblObjectId.Location = new System.Drawing.Point(30, 179);
            this.lblObjectId.Name = "lblObjectId";
            this.lblObjectId.Size = new System.Drawing.Size(41, 13);
            this.lblObjectId.TabIndex = 20;
            this.lblObjectId.Text = "Obje Id";
            // 
            // cmbType
            // 
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(73, 147);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(100, 21);
            this.cmbType.TabIndex = 25;
            // 
            // lblValue0
            // 
            this.lblValue0.AutoSize = true;
            this.lblValue0.Location = new System.Drawing.Point(36, 226);
            this.lblValue0.Name = "lblValue0";
            this.lblValue0.Size = new System.Drawing.Size(46, 13);
            this.lblValue0.TabIndex = 19;
            this.lblValue0.Text = "Value_0";
            // 
            // txtObjectId
            // 
            this.txtObjectId.Location = new System.Drawing.Point(73, 176);
            this.txtObjectId.Multiline = true;
            this.txtObjectId.Name = "txtObjectId";
            this.txtObjectId.Size = new System.Drawing.Size(100, 20);
            this.txtObjectId.TabIndex = 22;
            // 
            // lblValue1
            // 
            this.lblValue1.AutoSize = true;
            this.lblValue1.Location = new System.Drawing.Point(117, 226);
            this.lblValue1.Name = "lblValue1";
            this.lblValue1.Size = new System.Drawing.Size(46, 13);
            this.lblValue1.TabIndex = 17;
            this.lblValue1.Text = "Value_1";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(73, 119);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(192, 20);
            this.txtDescription.TabIndex = 21;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(277, 226);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "Value_3";
            // 
            // lblValue2
            // 
            this.lblValue2.AutoSize = true;
            this.lblValue2.Location = new System.Drawing.Point(200, 226);
            this.lblValue2.Name = "lblValue2";
            this.lblValue2.Size = new System.Drawing.Size(46, 13);
            this.lblValue2.TabIndex = 12;
            this.lblValue2.Text = "Value_2";
            // 
            // txtValue0
            // 
            this.txtValue0.Location = new System.Drawing.Point(23, 242);
            this.txtValue0.Name = "txtValue0";
            this.txtValue0.Size = new System.Drawing.Size(64, 20);
            this.txtValue0.TabIndex = 24;
            this.txtValue0.Text = "0";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(73, 79);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(100, 20);
            this.txtName.TabIndex = 23;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(188, 276);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Value_2 End";
            // 
            // nmrValue2End
            // 
            this.nmrValue2End.Location = new System.Drawing.Point(192, 293);
            this.nmrValue2End.Maximum = new decimal(new int[] {
            5600,
            0,
            0,
            0});
            this.nmrValue2End.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            -2147483648});
            this.nmrValue2End.Name = "nmrValue2End";
            this.nmrValue2End.Size = new System.Drawing.Size(64, 20);
            this.nmrValue2End.TabIndex = 26;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(105, 276);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Value_1 End";
            // 
            // nmrValue1End
            // 
            this.nmrValue1End.Location = new System.Drawing.Point(107, 293);
            this.nmrValue1End.Maximum = new decimal(new int[] {
            5600,
            0,
            0,
            0});
            this.nmrValue1End.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            -2147483648});
            this.nmrValue1End.Name = "nmrValue1End";
            this.nmrValue1End.Size = new System.Drawing.Size(64, 20);
            this.nmrValue1End.TabIndex = 30;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(29, 398);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(414, 23);
            this.progressBar1.TabIndex = 33;
            // 
            // BulkInsertForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 450);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.chkDroppable);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblId);
            this.Controls.Add(this.nmrID);
            this.Controls.Add(this.lblMaxCount);
            this.Controls.Add(this.nmrMaxCount);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.nmrValue3);
            this.Controls.Add(this.nmrValue2End);
            this.Controls.Add(this.nmrValue2);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.nmrValue1End);
            this.Controls.Add(this.nmrValue1);
            this.Controls.Add(this.lblObjectId);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.lblValue0);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtObjectId);
            this.Controls.Add(this.lblValue1);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lblValue2);
            this.Controls.Add(this.txtValue0);
            this.Controls.Add(this.txtName);
            this.Name = "BulkInsertForm";
            this.Text = "BulkInsertForm";
            ((System.ComponentModel.ISupportInitialize)(this.nmrID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrMaxCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrValue3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrValue2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrValue1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrValue2End)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrValue1End)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkDroppable;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.NumericUpDown nmrID;
        private System.Windows.Forms.Label lblMaxCount;
        private System.Windows.Forms.NumericUpDown nmrMaxCount;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.NumericUpDown nmrValue3;
        private System.Windows.Forms.NumericUpDown nmrValue2;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.NumericUpDown nmrValue1;
        private System.Windows.Forms.Label lblObjectId;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label lblValue0;
        private System.Windows.Forms.TextBox txtObjectId;
        private System.Windows.Forms.Label lblValue1;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblValue2;
        private System.Windows.Forms.TextBox txtValue0;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nmrValue2End;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nmrValue1End;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}
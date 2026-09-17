namespace MiniSuperMarket.WinForm
{
    partial class FormCategoryManagement
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
            components = new System.ComponentModel.Container();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            dgvCategories = new DataGridView();
            bindingSource1 = new BindingSource(components);
            btnLoad = new Button();
            btnAdd = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            btnSearch = new Button();
            txtId = new TextBox();
            txtDescription = new TextBox();
            txtCategoryName = new TextBox();
            textBox1 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dgvCategories).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).BeginInit();
            SuspendLayout();
            // 
            // dgvCategories
            // 
            dgvCategories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvCategories.Location = new Point(158, 226);
            dgvCategories.Name = "dgvCategories";
            dgvCategories.Size = new Size(319, 192);
            dgvCategories.TabIndex = 1;
            dgvCategories.CellContentClick += dataGridView1_CellContentClick;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(42, 16);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(75, 23);
            btnLoad.TabIndex = 2;
            btnLoad.Text = "tải lại ";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += button1_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(12, 50);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 23);
            btnAdd.TabIndex = 3;
            btnAdd.Text = "thêm mới";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += button2_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(25, 101);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(75, 23);
            btnUpdate.TabIndex = 4;
            btnUpdate.Text = "cập nhật ";
            btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(140, 41);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(75, 23);
            btnDelete.TabIndex = 5;
            btnDelete.Text = "xóa";
            btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(314, 16);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(75, 23);
            btnSearch.TabIndex = 6;
            btnSearch.Text = "tìm kiếm ";
            btnSearch.UseVisualStyleBackColor = true;
            // 
            // txtId
            // 
            txtId.Location = new Point(230, 83);
            txtId.Name = "txtId";
            txtId.Size = new Size(100, 23);
            txtId.TabIndex = 7;
            // 
            // txtDescription
            // 
            txtDescription.Location = new Point(564, 51);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(100, 23);
            txtDescription.TabIndex = 8;
            txtDescription.TextChanged += textBox2_TextChanged;
            // 
            // txtCategoryName
            // 
            txtCategoryName.Location = new Point(424, 72);
            txtCategoryName.Name = "txtCategoryName";
            txtCategoryName.Size = new Size(100, 23);
            txtCategoryName.TabIndex = 9;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(483, 150);
            textBox1.Name = "txtKeyword ";
            textBox1.Size = new Size(100, 23);
            textBox1.TabIndex = 10;
            // 
            // FormCategoryManagement
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(textBox1);
            Controls.Add(txtCategoryName);
            Controls.Add(txtDescription);
            Controls.Add(txtId);
            Controls.Add(btnSearch);
            Controls.Add(btnDelete);
            Controls.Add(btnUpdate);
            Controls.Add(btnAdd);
            Controls.Add(btnLoad);
            Controls.Add(dgvCategories);
            Name = "FormCategoryManagement";
            Text = "txtId";
            Load += FormCategoryManagement_Load;
            ((System.ComponentModel.ISupportInitialize)dgvCategories).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private DataGridView dgvCategories;
        private BindingSource bindingSource1;
        private Button btnLoad;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnSearch;
        private TextBox txtId;
        private TextBox txtDescription;
        private TextBox txtCategoryName;
        private TextBox textBox1;
    }
}
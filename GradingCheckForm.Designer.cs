namespace CooperationExam
{
    partial class GradingCheckForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.cboSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.lblSemester = new DevComponents.DotNetBar.LabelX();
            this.cboSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.lblSchoolYear = new DevComponents.DotNetBar.LabelX();
            this.cboExam = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.lblExam = new DevComponents.DotNetBar.LabelX();
            this.dgStatus = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.btnExport = new DevComponents.DotNetBar.ButtonX();
            this.btnRefresh = new DevComponents.DotNetBar.ButtonX();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.chkUnsuccess = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.cp1 = new DevComponents.DotNetBar.Controls.CircularProgress();
            this.ep1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.chCourseName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chT1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chT2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chT3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ep1)).BeginInit();
            this.SuspendLayout();
            // 
            // cboSchoolYear
            // 
            this.cboSchoolYear.DisplayMember = "Text";
            this.cboSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSchoolYear.FormattingEnabled = true;
            this.cboSchoolYear.ItemHeight = 19;
            this.cboSchoolYear.Location = new System.Drawing.Point(65, 12);
            this.cboSchoolYear.Name = "cboSchoolYear";
            this.cboSchoolYear.Size = new System.Drawing.Size(61, 25);
            this.cboSchoolYear.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboSchoolYear.TabIndex = 0;
            // 
            // lblSemester
            // 
            this.lblSemester.AutoSize = true;
            this.lblSemester.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lblSemester.BackgroundStyle.Class = "";
            this.lblSemester.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lblSemester.Location = new System.Drawing.Point(145, 14);
            this.lblSemester.Name = "lblSemester";
            this.lblSemester.Size = new System.Drawing.Size(34, 21);
            this.lblSemester.TabIndex = 1;
            this.lblSemester.Text = "學期";
            // 
            // cboSemester
            // 
            this.cboSemester.DisplayMember = "Text";
            this.cboSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSemester.FormattingEnabled = true;
            this.cboSemester.ItemHeight = 19;
            this.cboSemester.Location = new System.Drawing.Point(185, 12);
            this.cboSemester.Name = "cboSemester";
            this.cboSemester.Size = new System.Drawing.Size(61, 25);
            this.cboSemester.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboSemester.TabIndex = 0;
            // 
            // lblSchoolYear
            // 
            this.lblSchoolYear.AutoSize = true;
            this.lblSchoolYear.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lblSchoolYear.BackgroundStyle.Class = "";
            this.lblSchoolYear.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lblSchoolYear.Location = new System.Drawing.Point(12, 14);
            this.lblSchoolYear.Name = "lblSchoolYear";
            this.lblSchoolYear.Size = new System.Drawing.Size(47, 21);
            this.lblSchoolYear.TabIndex = 1;
            this.lblSchoolYear.Text = "學年度";
            // 
            // cboExam
            // 
            this.cboExam.DisplayMember = "Text";
            this.cboExam.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboExam.FormattingEnabled = true;
            this.cboExam.ItemHeight = 19;
            this.cboExam.Location = new System.Drawing.Point(65, 44);
            this.cboExam.Name = "cboExam";
            this.cboExam.Size = new System.Drawing.Size(181, 25);
            this.cboExam.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboExam.TabIndex = 0;
            // 
            // lblExam
            // 
            this.lblExam.AutoSize = true;
            this.lblExam.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lblExam.BackgroundStyle.Class = "";
            this.lblExam.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lblExam.Location = new System.Drawing.Point(12, 46);
            this.lblExam.Name = "lblExam";
            this.lblExam.Size = new System.Drawing.Size(34, 21);
            this.lblExam.TabIndex = 1;
            this.lblExam.Text = "評量";
            // 
            // dgStatus
            // 
            this.dgStatus.AllowUserToAddRows = false;
            this.dgStatus.AllowUserToDeleteRows = false;
            this.dgStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgStatus.BackgroundColor = System.Drawing.Color.White;
            this.dgStatus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgStatus.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chCourseName,
            this.chT1,
            this.chT2,
            this.chT3});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgStatus.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgStatus.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgStatus.Location = new System.Drawing.Point(12, 75);
            this.dgStatus.Name = "dgStatus";
            this.dgStatus.ReadOnly = true;
            this.dgStatus.RowHeadersVisible = false;
            this.dgStatus.RowTemplate.Height = 24;
            this.dgStatus.Size = new System.Drawing.Size(603, 311);
            this.dgStatus.TabIndex = 2;
            // 
            // btnExport
            // 
            this.btnExport.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExport.BackColor = System.Drawing.Color.Transparent;
            this.btnExport.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExport.Location = new System.Drawing.Point(12, 394);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "匯出";
            // 
            // btnRefresh
            // 
            this.btnRefresh.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnRefresh.BackColor = System.Drawing.Color.Transparent;
            this.btnRefresh.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnRefresh.Location = new System.Drawing.Point(255, 45);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "查詢";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(540, 394);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "離開";
            // 
            // chkUnsuccess
            // 
            this.chkUnsuccess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkUnsuccess.AutoSize = true;
            this.chkUnsuccess.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.chkUnsuccess.BackgroundStyle.Class = "";
            this.chkUnsuccess.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.chkUnsuccess.Location = new System.Drawing.Point(471, 46);
            this.chkUnsuccess.Name = "chkUnsuccess";
            this.chkUnsuccess.Size = new System.Drawing.Size(147, 21);
            this.chkUnsuccess.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.chkUnsuccess.TabIndex = 4;
            this.chkUnsuccess.Text = "僅顯示未完成之課程";
            // 
            // cp1
            // 
            this.cp1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.cp1.BackgroundStyle.Class = "";
            this.cp1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cp1.Location = new System.Drawing.Point(331, 46);
            this.cp1.Name = "cp1";
            this.cp1.Size = new System.Drawing.Size(30, 22);
            this.cp1.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2010;
            this.cp1.TabIndex = 5;
            // 
            // ep1
            // 
            this.ep1.ContainerControl = this;
            // 
            // chCourseName
            // 
            this.chCourseName.DataPropertyName = "CourseName";
            this.chCourseName.HeaderText = "課程名稱";
            this.chCourseName.Name = "chCourseName";
            this.chCourseName.ReadOnly = true;
            this.chCourseName.Width = 270;
            // 
            // chT1
            // 
            this.chT1.DataPropertyName = "Teacher1Name";
            this.chT1.HeaderText = "教師一";
            this.chT1.Name = "chT1";
            this.chT1.ReadOnly = true;
            this.chT1.Width = 110;
            // 
            // chT2
            // 
            this.chT2.DataPropertyName = "Teacher2Name";
            this.chT2.HeaderText = "教師二";
            this.chT2.Name = "chT2";
            this.chT2.ReadOnly = true;
            this.chT2.Width = 110;
            // 
            // chT3
            // 
            this.chT3.DataPropertyName = "Teacher3Name";
            this.chT3.HeaderText = "教師三";
            this.chT3.Name = "chT3";
            this.chT3.ReadOnly = true;
            this.chT3.Width = 110;
            // 
            // GradingCheckForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 423);
            this.Controls.Add(this.cp1);
            this.Controls.Add(this.chkUnsuccess);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.dgStatus);
            this.Controls.Add(this.lblExam);
            this.Controls.Add(this.lblSchoolYear);
            this.Controls.Add(this.lblSemester);
            this.Controls.Add(this.cboExam);
            this.Controls.Add(this.cboSemester);
            this.Controls.Add(this.cboSchoolYear);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(418, 364);
            this.Name = "GradingCheckForm";
            this.Text = "成績未輸入檢查";
            this.Load += new System.EventHandler(this.GradingCheckForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ep1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSchoolYear;
        private DevComponents.DotNetBar.LabelX lblSemester;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSemester;
        private DevComponents.DotNetBar.LabelX lblSchoolYear;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboExam;
        private DevComponents.DotNetBar.LabelX lblExam;
        private DevComponents.DotNetBar.Controls.DataGridViewX dgStatus;
        private DevComponents.DotNetBar.ButtonX btnExport;
        private DevComponents.DotNetBar.ButtonX btnRefresh;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.Controls.CheckBoxX chkUnsuccess;
        private DevComponents.DotNetBar.Controls.CircularProgress cp1;
        private System.Windows.Forms.ErrorProvider ep1;
        private System.Windows.Forms.DataGridViewTextBoxColumn chCourseName;
        private System.Windows.Forms.DataGridViewTextBoxColumn chT1;
        private System.Windows.Forms.DataGridViewTextBoxColumn chT2;
        private System.Windows.Forms.DataGridViewTextBoxColumn chT3;
    }
}
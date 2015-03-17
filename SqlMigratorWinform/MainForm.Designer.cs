namespace SqlMigratorWinform
{
    partial class MainForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDBConnection = new System.Windows.Forms.TextBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.statusStripDB = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelDB = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dgViewInfo = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtLastVersion = new System.Windows.Forms.TextBox();
            this.btnMigrate = new System.Windows.Forms.Button();
            this.btnRollbackOnce = new System.Windows.Forms.Button();
            this.btnRollbackAll = new System.Windows.Forms.Button();
            this.statusStripDB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgViewInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("SimSun", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(183, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "SQL版本管理";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "连接字符串";
            // 
            // txtDBConnection
            // 
            this.txtDBConnection.Location = new System.Drawing.Point(83, 75);
            this.txtDBConnection.Multiline = true;
            this.txtDBConnection.Name = "txtDBConnection";
            this.txtDBConnection.Size = new System.Drawing.Size(310, 52);
            this.txtDBConnection.TabIndex = 2;
            this.txtDBConnection.Text = "Server=10.2.5.184;UID=b_du;password=123456;database=LiteSpeedCentral;";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(415, 75);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 52);
            this.btnLoad.TabIndex = 27;
            this.btnLoad.Text = "加载";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // txtLog
            // 
            this.txtLog.AcceptsReturn = true;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtLog.Location = new System.Drawing.Point(0, 420);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(502, 241);
            this.txtLog.TabIndex = 31;
            // 
            // statusStripDB
            // 
            this.statusStripDB.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelDB});
            this.statusStripDB.Location = new System.Drawing.Point(0, 661);
            this.statusStripDB.Name = "statusStripDB";
            this.statusStripDB.Size = new System.Drawing.Size(502, 22);
            this.statusStripDB.TabIndex = 32;
            this.statusStripDB.Text = "statusStripDB";
            // 
            // toolStripStatusLabelDB
            // 
            this.toolStripStatusLabelDB.Name = "toolStripStatusLabelDB";
            this.toolStripStatusLabelDB.Size = new System.Drawing.Size(87, 17);
            this.toolStripStatusLabelDB.Text = "Please reload";
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.Location = new System.Drawing.Point(81, 48);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(312, 22);
            this.lblMessage.TabIndex = 34;
            this.lblMessage.Text = "NoMessage";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 23);
            this.label3.TabIndex = 33;
            this.label3.Text = "提示信息";
            // 
            // dgViewInfo
            // 
            this.dgViewInfo.AllowUserToAddRows = false;
            this.dgViewInfo.AllowUserToDeleteRows = false;
            this.dgViewInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgViewInfo.Location = new System.Drawing.Point(12, 250);
            this.dgViewInfo.Name = "dgViewInfo";
            this.dgViewInfo.ReadOnly = true;
            this.dgViewInfo.RowTemplate.Height = 23;
            this.dgViewInfo.Size = new System.Drawing.Size(478, 164);
            this.dgViewInfo.TabIndex = 35;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(10, 224);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 23);
            this.label4.TabIndex = 36;
            this.label4.Text = "历史版本信息";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(10, 201);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 23);
            this.label5.TabIndex = 38;
            this.label5.Text = "当前版本";
            // 
            // txtLastVersion
            // 
            this.txtLastVersion.Location = new System.Drawing.Point(83, 198);
            this.txtLastVersion.Name = "txtLastVersion";
            this.txtLastVersion.Size = new System.Drawing.Size(100, 21);
            this.txtLastVersion.TabIndex = 39;
            // 
            // btnMigrate
            // 
            this.btnMigrate.Location = new System.Drawing.Point(83, 140);
            this.btnMigrate.Name = "btnMigrate";
            this.btnMigrate.Size = new System.Drawing.Size(75, 52);
            this.btnMigrate.TabIndex = 40;
            this.btnMigrate.Text = "版本跟进";
            this.btnMigrate.UseVisualStyleBackColor = true;
            this.btnMigrate.Click += new System.EventHandler(this.btnMigrate_Click);
            // 
            // btnRollbackOnce
            // 
            this.btnRollbackOnce.Location = new System.Drawing.Point(187, 140);
            this.btnRollbackOnce.Name = "btnRollbackOnce";
            this.btnRollbackOnce.Size = new System.Drawing.Size(104, 52);
            this.btnRollbackOnce.TabIndex = 41;
            this.btnRollbackOnce.Text = "版本回滚上一次";
            this.btnRollbackOnce.UseVisualStyleBackColor = true;
            this.btnRollbackOnce.Click += new System.EventHandler(this.btnRollbackOnce_Click);
            // 
            // btnRollbackAll
            // 
            this.btnRollbackAll.Location = new System.Drawing.Point(318, 140);
            this.btnRollbackAll.Name = "btnRollbackAll";
            this.btnRollbackAll.Size = new System.Drawing.Size(75, 52);
            this.btnRollbackAll.TabIndex = 42;
            this.btnRollbackAll.Text = "全部回滚";
            this.btnRollbackAll.UseVisualStyleBackColor = true;
            this.btnRollbackAll.Click += new System.EventHandler(this.btnRollbackAll_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(502, 683);
            this.Controls.Add(this.btnRollbackAll);
            this.Controls.Add(this.btnRollbackOnce);
            this.Controls.Add(this.btnMigrate);
            this.Controls.Add(this.txtLastVersion);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dgViewInfo);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.statusStripDB);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.txtDBConnection);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.Text = "SqlMigrator";
            this.statusStripDB.ResumeLayout(false);
            this.statusStripDB.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgViewInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDBConnection;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.StatusStrip statusStripDB;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelDB;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgViewInfo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtLastVersion;
        private System.Windows.Forms.Button btnMigrate;
        private System.Windows.Forms.Button btnRollbackOnce;
        private System.Windows.Forms.Button btnRollbackAll;
    }
}


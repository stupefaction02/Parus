namespace ControlPanel
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.runBackend = new System.Windows.Forms.Button();
            this.runWebui = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.runDummyUser = new System.Windows.Forms.Button();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // runBackend
            // 
            this.runBackend.Location = new System.Drawing.Point(45, 21);
            this.runBackend.Name = "runBackend";
            this.runBackend.Size = new System.Drawing.Size(159, 28);
            this.runBackend.TabIndex = 0;
            this.runBackend.Text = "Run Backend";
            this.runBackend.UseVisualStyleBackColor = true;
            this.runBackend.Click += new System.EventHandler(this.runBackend_Click);
            // 
            // runWebui
            // 
            this.runWebui.Location = new System.Drawing.Point(45, 55);
            this.runWebui.Name = "runWebui";
            this.runWebui.Size = new System.Drawing.Size(159, 25);
            this.runWebui.TabIndex = 1;
            this.runWebui.Text = "Run WebUI";
            this.runWebui.UseVisualStyleBackColor = true;
            this.runWebui.Click += new System.EventHandler(this.runWebui_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(312, 125);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(8, 8);
            this.button3.TabIndex = 2;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(210, 28);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(61, 17);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "running";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(210, 60);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(61, 17);
            this.checkBox2.TabIndex = 4;
            this.checkBox2.Text = "running";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // runDummyUser
            // 
            this.runDummyUser.Location = new System.Drawing.Point(45, 86);
            this.runDummyUser.Name = "runDummyUser";
            this.runDummyUser.Size = new System.Drawing.Size(159, 26);
            this.runDummyUser.TabIndex = 5;
            this.runDummyUser.Text = "Run Dummy User";
            this.runDummyUser.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(210, 92);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(80, 17);
            this.checkBox3.TabIndex = 6;
            this.checkBox3.Text = "checkBox3";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.runDummyUser);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.runWebui);
            this.Controls.Add(this.runBackend);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button runBackend;
        private System.Windows.Forms.Button runWebui;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Button runDummyUser;
        private System.Windows.Forms.CheckBox checkBox3;
    }
}


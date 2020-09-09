namespace CheatDetectionDemo
{
    partial class Demo
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
            this.AssemblyLoadButton = new System.Windows.Forms.Button();
            this.LoadDetectedAssemblyButton = new System.Windows.Forms.Button();
            this.LaunchDebuggerButton = new System.Windows.Forms.Button();
            this.LaunchDetectedCheatButton = new System.Windows.Forms.Button();
            this.RunGoodwitchAndGameButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // AssemblyLoadButton
            // 
            this.AssemblyLoadButton.Location = new System.Drawing.Point(22, 118);
            this.AssemblyLoadButton.Name = "AssemblyLoadButton";
            this.AssemblyLoadButton.Size = new System.Drawing.Size(160, 65);
            this.AssemblyLoadButton.TabIndex = 0;
            this.AssemblyLoadButton.Text = "Load Assembly";
            this.AssemblyLoadButton.UseVisualStyleBackColor = true;
            this.AssemblyLoadButton.Click += new System.EventHandler(this.AssemblyLoadButton_Click);
            // 
            // LoadDetectedAssemblyButton
            // 
            this.LoadDetectedAssemblyButton.Location = new System.Drawing.Point(211, 118);
            this.LoadDetectedAssemblyButton.Name = "LoadDetectedAssemblyButton";
            this.LoadDetectedAssemblyButton.Size = new System.Drawing.Size(160, 65);
            this.LoadDetectedAssemblyButton.TabIndex = 1;
            this.LoadDetectedAssemblyButton.Text = "Load Detected Assembly";
            this.LoadDetectedAssemblyButton.UseVisualStyleBackColor = true;
            this.LoadDetectedAssemblyButton.Click += new System.EventHandler(this.LoadDetectedAssemblyButton_Click);
            // 
            // LaunchDebuggerButton
            // 
            this.LaunchDebuggerButton.Location = new System.Drawing.Point(211, 217);
            this.LaunchDebuggerButton.Name = "LaunchDebuggerButton";
            this.LaunchDebuggerButton.Size = new System.Drawing.Size(160, 65);
            this.LaunchDebuggerButton.TabIndex = 2;
            this.LaunchDebuggerButton.Text = "Launch Debugger";
            this.LaunchDebuggerButton.UseVisualStyleBackColor = true;
            this.LaunchDebuggerButton.Click += new System.EventHandler(this.LaunchDebuggerButton_Click);
            // 
            // LaunchDetectedCheatButton
            // 
            this.LaunchDetectedCheatButton.Location = new System.Drawing.Point(22, 217);
            this.LaunchDetectedCheatButton.Name = "LaunchDetectedCheatButton";
            this.LaunchDetectedCheatButton.Size = new System.Drawing.Size(160, 65);
            this.LaunchDetectedCheatButton.TabIndex = 3;
            this.LaunchDetectedCheatButton.Text = "Launch Detected Cheat Process";
            this.LaunchDetectedCheatButton.UseVisualStyleBackColor = true;
            this.LaunchDetectedCheatButton.Click += new System.EventHandler(this.LaunchDetectedCheatButton_Click);
            // 
            // RunGoodwitchAndGameButton
            // 
            this.RunGoodwitchAndGameButton.Location = new System.Drawing.Point(111, 24);
            this.RunGoodwitchAndGameButton.Name = "RunGoodwitchAndGameButton";
            this.RunGoodwitchAndGameButton.Size = new System.Drawing.Size(160, 65);
            this.RunGoodwitchAndGameButton.TabIndex = 4;
            this.RunGoodwitchAndGameButton.Text = "Launch Goodwitch and game";
            this.RunGoodwitchAndGameButton.UseVisualStyleBackColor = true;
            this.RunGoodwitchAndGameButton.Click += new System.EventHandler(this.RunGoodwitchAndGameButton_Click);
            // 
            // Demo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 308);
            this.Controls.Add(this.RunGoodwitchAndGameButton);
            this.Controls.Add(this.LaunchDetectedCheatButton);
            this.Controls.Add(this.LaunchDebuggerButton);
            this.Controls.Add(this.LoadDetectedAssemblyButton);
            this.Controls.Add(this.AssemblyLoadButton);
            this.Name = "Demo";
            this.Text = "Demo";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button AssemblyLoadButton;
        private System.Windows.Forms.Button LoadDetectedAssemblyButton;
        private System.Windows.Forms.Button LaunchDebuggerButton;
        private System.Windows.Forms.Button LaunchDetectedCheatButton;
        private System.Windows.Forms.Button RunGoodwitchAndGameButton;
    }
}


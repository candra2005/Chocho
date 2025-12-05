namespace ChochoNest.View
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            tbUsername = new TextBox();
            tbPassword = new TextBox();
            btnLogin = new Button();
            registerLink = new LinkLabel();
            SuspendLayout();
            // 
            // tbUsername
            // 
            tbUsername.BackColor = Color.Sienna;
            tbUsername.ForeColor = SystemColors.Window;
            tbUsername.Location = new Point(816, 340);
            tbUsername.Name = "tbUsername";
            tbUsername.Size = new Size(286, 27);
            tbUsername.TabIndex = 0;
            tbUsername.TextChanged += tbUsername_TextChanged;
            // 
            // tbPassword
            // 
            tbPassword.BackColor = Color.Sienna;
            tbPassword.ForeColor = SystemColors.Window;
            tbPassword.Location = new Point(816, 457);
            tbPassword.Name = "tbPassword";
            tbPassword.Size = new Size(286, 27);
            tbPassword.TabIndex = 1;
            // 
            // btnLogin
            // 
            btnLogin.BackColor = Color.DarkSeaGreen;
            btnLogin.Location = new Point(907, 584);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(237, 49);
            btnLogin.TabIndex = 2;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // registerLink
            // 
            registerLink.AutoSize = true;
            registerLink.Location = new Point(1073, 528);
            registerLink.Name = "registerLink";
            registerLink.Size = new Size(59, 20);
            registerLink.TabIndex = 3;
            registerLink.TabStop = true;
            registerLink.Text = "register";
            registerLink.LinkClicked += linkLabel1_LinkClicked;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(1463, 960);
            Controls.Add(registerLink);
            Controls.Add(btnLogin);
            Controls.Add(tbPassword);
            Controls.Add(tbUsername);
            FormBorderStyle = FormBorderStyle.None;
            Name = "LoginForm";
            Text = "LoginForm";
            Load += LoginForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tbUsername;
        private TextBox tbPassword;
        private Button btnLogin;
        private LinkLabel registerLink;
    }
}
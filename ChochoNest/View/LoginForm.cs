using ChochoNest.Controller;
using ChochoNest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChochoNest.View
{
    public partial class LoginForm : Form
    {
        private AuthController _authController;
        public LoginForm()
        {
            InitializeComponent();
            _authController = new AuthController();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _authController.showRegisterForm(this);
        }

        private void tbUsername_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            User loginAttemptUser = new User();
            loginAttemptUser.Username = tbUsername.Text;
            loginAttemptUser.Password = tbPassword.Text;

            if (string.IsNullOrWhiteSpace(loginAttemptUser.Username) || string.IsNullOrWhiteSpace(loginAttemptUser.Password))
            {
                MessageBox.Show("Username dan Password harus di isi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            User loggedInUser = _authController.Login(loginAttemptUser);

            if (loggedInUser != null)
            {
                MessageBox.Show($"Login berhasil. Selamat datang {loggedInUser.Username} (Role: {loggedInUser.Role})", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (loggedInUser.Role == "admin")
                {
                    AdminDashboard adminDashboard = new AdminDashboard(loggedInUser);
                    adminDashboard.Show();
                    this.Hide();
                }
                else if (loggedInUser.Role == "user")
                {
                    KatalogPelanggan katalogPelanggan = new KatalogPelanggan(loggedInUser);
                    katalogPelanggan.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Role pengguna tidak dikenal.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Username atau Password salah. Silahkan periksa kredensial akun anda!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}

using ChochoNest.Controller;
using ChochoNest.Models;
using System;
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

            // Panggil Login
            User loggedInUser = _authController.Login(loginAttemptUser);

            if (loggedInUser != null)
            {
                MessageBox.Show($"Login berhasil. Selamat datang {loggedInUser.Username}", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (loggedInUser.Role == "admin")
                {
                    // Buka Dashboard Admin
                    AdminDashboard adminDashboard = new AdminDashboard(loggedInUser);
                    adminDashboard.Show();
                    this.Hide();
                }
                else if (loggedInUser.Role == "user")
                {
                    // Buka Katalog Pelanggan (Bawa ID untuk filter riwayat nanti)
                    KatalogPelanggan katalogPelanggan = new KatalogPelanggan(loggedInUser);
                    katalogPelanggan.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Role tidak dikenali!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Username atau Password salah!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _authController.showRegisterForm(this);
        }
        private void tbUsername_TextChanged(object sender, EventArgs e) { }
        private void LoginForm_Load(object sender, EventArgs e) { }
    }
}
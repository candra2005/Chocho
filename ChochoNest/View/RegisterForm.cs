using ChochoNest.Controller;
using ChochoNest.Models;

namespace ChochoNest.View
{
    public partial class RegisterForm : Form
    {
        private AuthController _authController;
        public RegisterForm()
        {
            InitializeComponent();
            _authController = new AuthController();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            tbPassword.UseSystemPasswordChar = true;
        }


        private void tbUsername_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnRegis_Click(object sender, EventArgs e)
        {
            User user = new User();
            user.Username = tbUsername.Text;
            user.Password = tbPassword.Text;

            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
            {
                MessageBox.Show("Username dan Password harus di isi", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var valid = _authController.Register(user);

            if (valid)
            {
                MessageBox.Show("Registrasi berhasil. Silahkan Login", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _authController.showLoginForm(this);
            }
            else
            {
                MessageBox.Show("Registrasi Gagal. Silahkan coba lagi!!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RegisterForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}

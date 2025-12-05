using ChochoNest.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetEnv;
using Npgsql;
using ChochoNest.View;
using ChochoNest.Models;


namespace ChochoNest.Controller
{
    public class AuthController
    {
        private DbContext _dbContext;

        public AuthController()
        {
            _dbContext = new DbContext();
        }

        public User Login(User user)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_dbContext.connStr))
                {
                    conn.Open();
                    string query = "SELECT id_user, username, password, role FROM \"user\" WHERE username = @username AND password = @password LIMIT 1";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", user.Username);
                        cmd.Parameters.AddWithValue("@password", user.Password);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id_user")),
                                    Username = reader.GetString(reader.GetOrdinal("username")),
                                    Password = reader.GetString(reader.GetOrdinal("password")),
                                    Role = reader.GetString(reader.GetOrdinal("role"))
                                };
                            }
                            return null; // Login failed
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login Error: " + ex.Message);
                return null; // Login failed due to an exception
            }
        }
        public bool Register(User user)
        {
            if (user.Username == "admin" && user.Password == "1234")
            {
                MessageBox.Show("Username dilarang! Tidak boleh menggunakan akun ini.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            try
            {
                using (var conn = new NpgsqlConnection(_dbContext.connStr))
                {
                    conn.Open();
                    string query = "INSERT INTO \"user\" (username, password) VALUES(@username, @password)";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", user.Username);
                        cmd.Parameters.AddWithValue("@password", user.Password);

                        // 3. EKSEKUSI
                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Register Error: " + ex.Message);
                return false;
            }
        }
        public void showLoginForm(Form form)
        {
            form.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        public void showRegisterForm(Form form)
        {
            form.Hide();
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show();
        }
    }
}
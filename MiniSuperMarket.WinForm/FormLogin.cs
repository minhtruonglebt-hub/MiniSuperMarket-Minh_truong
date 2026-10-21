using System.Net.Http.Json;
using System.Text.Json;

namespace MiniSuperMarket.WinForm
{
    public partial class FormLogin : Form
    {
        private static readonly HttpClient _client = new() { BaseAddress = new Uri(SessionManager.ApiBaseUrl) };

        public FormLogin()
        {
            InitializeComponent();
            lblHint.Text = "Thử: admin01 / 123456  |  cashier01 / 123456  |  ware01 / 123456";
            ClientSize = new Size(440, 240);
            lblHint.Location = new Point(30, 178);
            lblHint.Size = new Size(380, 30);
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            var username = txtUser.Text.Trim();
            var password = txtPass.Text;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tài khoản và mật khẩu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnLogin.Enabled = false;
                using var response = await _client.PostAsJsonAsync("auth/login", new { Username = username, Password = password });
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show(await ApiClientService.GetErrorMessageAsync(response), "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                var root = doc.RootElement;
                SessionManager.JwtToken = root.GetProperty("token").GetString() ?? string.Empty;
                SessionManager.CurrentRole = root.GetProperty("role").GetString() ?? string.Empty;
                SessionManager.CurrentUsername = root.GetProperty("username").GetString() ?? username;
                SessionManager.CurrentFullName = root.GetProperty("fullName").GetString() ?? SessionManager.CurrentUsername;

                Hide();
                using (var shell = new FormMainShell()) shell.ShowDialog();
                if (string.IsNullOrEmpty(SessionManager.JwtToken))
                {
                    txtPass.Clear();
                    Show();
                    Activate();
                }
                else Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không kết nối được API: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { btnLogin.Enabled = true; }
        }
    }
}

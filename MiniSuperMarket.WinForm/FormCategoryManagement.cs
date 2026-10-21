using System.Net.Http.Json;

namespace MiniSuperMarket.WinForm
{
    public partial class FormCategoryManagement : Form
    {
        public FormCategoryManagement()
        {
            InitializeComponent();
        }

        private async void FormCategoryManagement_Load(object? sender, EventArgs e)
        {
            lblRole.Text = $"Tài khoản: {SessionManager.CurrentUsername} | Vai trò hiện tại: {SessionManager.CurrentRole}";
            await LoadDataAsync();
        }

        // Tải danh sách (hoặc kết quả tìm kiếm) từ API lên DataGridView
        private async Task LoadDataAsync(string? keyword = null)
        {
            try
            {
                string url = string.IsNullOrWhiteSpace(keyword)
                    ? "categories"
                    : $"categories/search?keyword={Uri.EscapeDataString(keyword.Trim())}";

                var categories = await ApiClientService.GetListAsync<CategoryDto>(url);
                if (categories != null) dgvCategories.DataSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối Server: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnReload_Click(object? sender, EventArgs e)
        {
            txtKeyword.Clear();
            await LoadDataAsync();
        }

        private async void btnSearch_Click(object? sender, EventArgs e)
        {
            await LoadDataAsync(txtKeyword.Text);
        }

        // Click vào một dòng: đưa dữ liệu lên các ô nhập để Sửa/Xóa
        private void dgvCategories_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvCategories.Rows[e.RowIndex].DataBoundItem is CategoryDto cat)
            {
                txtId.Text = cat.CategoryId.ToString();
                txtName.Text = cat.CategoryName;
                txtDesc.Text = cat.Description ?? string.Empty;
            }
        }

        private async void btnAdd_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Nhập tên nhóm hàng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var dto = new { CategoryName = txtName.Text.Trim(), Description = txtDesc.Text.Trim() };
                var res = await ApiClientService.Client.PostAsJsonAsync("categories", dto);
                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Thêm mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadDataAsync();
                }
                else
                {
                    MessageBox.Show("Thêm thất bại: " + await ApiClientService.GetErrorMessageAsync(res),
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnUpdate_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn nhóm hàng cần sửa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Nhập tên nhóm hàng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var dto = new { CategoryId = id, CategoryName = txtName.Text.Trim(), Description = txtDesc.Text.Trim() };
                var res = await ApiClientService.Client.PutAsJsonAsync($"categories/{id}", dto);
                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadDataAsync();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại: " + await ApiClientService.GetErrorMessageAsync(res),
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn nhóm hàng cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa nhóm hàng ID = {id}?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                var res = await ApiClientService.Client.DeleteAsync($"categories/{id}");
                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputs();
                    await LoadDataAsync();
                }
                else
                {
                    MessageBox.Show(await ApiClientService.GetErrorMessageAsync(res), "Xóa thất bại",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Mở màn hình Khách hàng (Admin, Cashier)
        private void btnCustomers_Click(object? sender, EventArgs e)
        {
            using var form = new FormCustomerManagement();
            form.ShowDialog(this);
        }

        // Mở màn hình Vai trò (chỉ Admin - Cashier sẽ nhận 403)
        private void btnRoles_Click(object? sender, EventArgs e)
        {
            using var form = new FormRoleManagement();
            form.ShowDialog(this);
        }

        private void ClearInputs()
        {
            txtId.Clear();
            txtName.Clear();
            txtDesc.Clear();
        }
    }
}

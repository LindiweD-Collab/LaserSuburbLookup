using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LaserSuburbLookup.Services;
using LaserSuburbLookup.Models;

namespace LaserSuburbLookup.UI
{
    public partial class MainForm : Form
    {
        private readonly ApiClient _apiClient;
        private string _token = string.Empty;

        public MainForm()
        {
            InitializeComponent();
            _apiClient = new ApiClient("https://laserrest.laserlogistics.co.za/api/Laser/");
            pictureBoxLogo.ImageLocation = "Resources/laser-logistics.png"; 
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            dgvResults.Rows.Clear();

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string searchTerm = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show("Please enter username, password, and search term.");
                return;
            }

            try
            {
                _token = await _apiClient.GetTokenAsync(username, password);

                var suburbs = await _apiClient.GetSuburbsAsync(_token, searchTerm);

                var sorted = suburbs.OrderBy(s => s.SuburbName).ToList();

                foreach (var s in sorted)
                {
                    dgvResults.Rows.Add(s.SuburbName);
                }

                lblStatus.Text = $"{sorted.Count} suburbs found.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using LaserSuburbLookup.Models;
using LaserSuburbLookup.Services;

namespace LaserSuburbLookup.UI
{
    public partial class MainForm : Form
    {
        private ApiClient _apiClient;
        private IConfigurationRoot _config;

        public MainForm()
        {
            InitializeComponent();

            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            // Logo
            var logo = _config["App:LogoPath"];
            if (!string.IsNullOrEmpty(logo) && System.IO.File.Exists(logo))
            {
                try { pictureBoxLogo.Image = System.Drawing.Image.FromFile(logo); }
                catch {  }
            }

            // ApiClient with baseUrl from config
            var baseUrl = _config["ApiSettings:BaseUrl"];
            if (string.IsNullOrEmpty(baseUrl))
            {
                lblStatus.Text = "BaseUrl not configured. Click Config to set API credentials.";
                btnSearch.Enabled = false;
            }
            else
            {
                _apiClient = new ApiClient(baseUrl);
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            var term = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(term))
            {
                MessageBox.Show("Please enter part of a suburb name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnSearch.Enabled = false;
            lblStatus.Text = "Fetching token...";
            try
            {
                var username = _config["ApiSettings:Username"];
                var password = _config["ApiSettings:Password"];
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Username or password not configured. Click Config to update appsettings.json.", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string token = null;
                try
                {
                    token = await _apiClient.GetTokenAsync(username, password);
                }
                catch (Exception exToken)
                {
                    MessageBox.Show($"Failed to retrieve token: {exToken.Message}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                lblStatus.Text = "Fetching suburbs...";
                List<Suburb> suburbs = null;
                try
                {
                    suburbs = await _apiClient.GetSuburbsAsync(token, term);
                }
                catch (Exception exSub)
                {
                    MessageBox.Show($"Failed to retrieve suburbs: {exSub.Message}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (suburbs == null || suburbs.Count == 0)
                {
                    MessageBox.Show("No suburbs found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dgvResults.DataSource = null;
                    lblStatus.Text = "No results.";
                    return;
                }

                var sorted = suburbs.OrderBy(s => s.SuburbName).ToList();
                dgvResults.DataSource = sorted;
                lblStatus.Text = $"Loaded {sorted.Count} suburbs.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSearch.Enabled = true;
            }
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To change API settings, edit appsettings.json in the application folder.\n\nFields: ApiSettings:BaseUrl, ApiSettings:Username, ApiSettings:Password", "Configuration");
        }
    }
}

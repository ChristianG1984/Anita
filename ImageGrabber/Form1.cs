using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ImageGrabber
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            lblSelectedName.Text = string.Empty;
            lblProgressInfo.Text = string.Empty;
            btnFetchData.Enabled = false;
        }

        public void ShowCelebritySearchResult(IEnumerable<SearchCelebrityAnswerData> answer)
        {
            lstResults.SuspendLayout();
            lstResults.Items.Clear();
            lstResults.Items.AddRange(answer.ToArray());
            lstResults.ResumeLayout();
        }

        public void InputSetting(KeyValuePair<string, string> setting)
        {
            if (setting.Key == "ui.BasePath") {
                txtBaseFolder.Text = setting.Value;
            }
        }

        public void InputCancellationTokenSource(CancelSource<FetchCelebrityPicturesData> cancelSource)
        {
            if (_cancelTokenSource != null) {
                _cancelTokenSource.Cancel();
            }
            _cancelTokenSource = cancelSource.CancelTokenSource;
            btnCancel.Visible = true;
        }

        public void InputFetchProgressInfo(FetchProgressInfo progressInfo)
        {
            if (progressInfo.IsFinished == true) {
                lblProgressInfo.Text = "Finished the download!";
            } else {
                lblProgressInfo.Text = "#" + progressInfo.LinkNodeInfo.CurrentNumber + " of " + progressInfo.LinkNodeInfo.MaxImageCount + ":\n" +
                    progressInfo.PicUriPath;
            }
        }

        public void ReceiveErrorData(ErrorData error)
        {
            txtRawContent.Text = error.Description + ":" + Environment.NewLine + error.Error.Message;
        }

        public event Action<string> SearchCelebrityRequest;
        public event Action<FetchCelebrityPicturesData> FetchCelebrityPicturesRequest;
        public event Action<KeyValuePair<string, string>> StoreSettingRequest;

        private void btnFetchData_Click(object sender, EventArgs e)
        {
            txtRawContent.Text = string.Empty;
            var item = lstResults.SelectedItem as SearchCelebrityAnswerData;
            if (item == null) {
                return;
            }
            var data = new FetchCelebrityPicturesData
            {
                SearchCelebrityAnswerData = item,
                BaseFolder = txtBaseFolder.Text
            };
            btnFetchData.Enabled = false;
            FetchCelebrityPicturesRequest(data);
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            txtRawContent.Text = string.Empty;
            var fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Path.GetDirectoryName(Application.ExecutablePath);
            if (fbd.ShowDialog(this) == DialogResult.OK) {
                txtBaseFolder.Text = fbd.SelectedPath;
            }
            fbd.Dispose();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            txtRawContent.Text = string.Empty;
            lblSelectedName.Text = string.Empty;
            lblProgressInfo.Text = string.Empty;
            btnFetchData.Enabled = false;
            SearchCelebrityRequest(txtSearch.Text);
        }

        private void txtBasePath_TextChanged(object sender, EventArgs e)
        {
            txtRawContent.Text = string.Empty;
            StoreSettingRequest(txtBaseFolder.Text.AsStorable("ui.BasePath"));
        }

        private void lstResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtRawContent.Text = string.Empty;
            var item = lstResults.SelectedItem as SearchCelebrityAnswerData;
            if (item == null) {
                lblSelectedName.Text = string.Empty;
                btnFetchData.Enabled = false;
                return;
            }
            lblSelectedName.Text = item.FullName;
            btnFetchData.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtRawContent.Text = string.Empty;
            _cancelTokenSource.Cancel();
            _cancelTokenSource = null;
            btnCancel.Visible = false;
            btnFetchData.Enabled = true;
        }

        private CancellationTokenSource _cancelTokenSource;
    }
}
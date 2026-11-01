namespace NetworkPrintServiceTestTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonBrowseImage_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Image Files|*.bmp;*.gif;*.jpeg;*.jpg;*.png;*.tif;*.tiff|All Files|*.*";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                textBoxImagePath.Text = openFileDialog.FileName;
            }
        }

        private async void buttonSend_Click(object? sender, EventArgs e)
        {
            buttonSend.Enabled = false;
            textBoxResult.Clear();

            try
            {
                var endpoint = textBoxEndpoint.Text.Trim();
                var imagePath = textBoxImagePath.Text.Trim();
                var deviceName = textBoxDeviceName.Text.Trim();
                var documentName = textBoxDocumentName.Text.Trim();

                if (string.IsNullOrWhiteSpace(endpoint))
                {
                    textBoxResult.Text = "Endpoint URL を入力してください。";
                    return;
                }

                if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
                {
                    textBoxResult.Text = "有効な Image File を指定してください。";
                    return;
                }

                //if (string.IsNullOrWhiteSpace(deviceName))
                //{
                //    textBoxResult.Text = "DeviceName を入力してください。";
                //    return;
                //}

                using var fileStream = File.OpenRead(imagePath);
                using var fileContent = new StreamContent(fileStream);
                using var formData = new MultipartFormDataContent();
                using var httpClient = new HttpClient();

                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(GetMediaTypeFromExtension(imagePath));
                formData.Add(fileContent, "imageFile", Path.GetFileName(imagePath));
                formData.Add(new StringContent(deviceName), "DeviceConfigXMLname");

                if (!string.IsNullOrWhiteSpace(documentName))
                {
                    formData.Add(new StringContent(documentName), "documentName");
                }

                using var response = await httpClient.PostAsync(endpoint, formData);
                var responseText = await response.Content.ReadAsStringAsync();
                var responseBody = responseText;

                if (!string.IsNullOrWhiteSpace(responseText))
                {
                    try
                    {
                        using var jsonDocument = System.Text.Json.JsonDocument.Parse(responseText);
                        var root = jsonDocument.RootElement;
                        var message = root.TryGetProperty("message", out var messageElement)
                            ? messageElement.GetString()
                            : null;
                        var detail = root.TryGetProperty("detail", out var detailElement)
                            ? detailElement.GetString()
                            : null;

                        if (!string.IsNullOrWhiteSpace(message) || !string.IsNullOrWhiteSpace(detail))
                        {
                            responseBody = $"message: {message}{Environment.NewLine}detail: {detail}";
                        }
                    }
                    catch (System.Text.Json.JsonException)
                    {
                    }
                }

                textBoxResult.Text = $"HTTP {(int)response.StatusCode} {response.StatusCode}{Environment.NewLine}{Environment.NewLine}{responseBody}";
            }
            catch (Exception ex)
            {
                textBoxResult.Text = ex.ToString();
            }
            finally
            {
                buttonSend.Enabled = true;
            }
        }

        private static string GetMediaTypeFromExtension(string filePath)
        {
            return Path.GetExtension(filePath).ToLowerInvariant() switch
            {
                ".bmp" => "image/bmp",
                ".gif" => "image/gif",
                ".jpeg" or ".jpg" => "image/jpeg",
                ".png" => "image/png",
                ".tif" or ".tiff" => "image/tiff",
                _ => "application/octet-stream",
            };
        }
    }
}

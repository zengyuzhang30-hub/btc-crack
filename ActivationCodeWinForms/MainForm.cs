using System.Security.Cryptography;
using System.Text;

namespace ActivationCodeWinForms
{
    public partial class MainForm : Form
    {
        // �������򱣳�һ�µ���Կ��������ͬ����������Ч�����룩
        private const string SecretKey = "ActivationCodeWinForms"; // ��Ҫ�������������е���Կ��ȫһ��

        public MainForm()
        {
            InitializeComponent();
            // ��ʼ������Ĭ��ֵ
            dtpDate.Value = DateTime.Now;
            txtMachineCode.Text = GenerateSampleMachineCode();
            this.Text = "���������ɹ���";
        }

        /// <summary>
        /// ����ʾ�������루���ڲ��ԣ�
        /// </summary>
        private string GenerateSampleMachineCode()
        {
            // ģ�����������ɻ�������߼�
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes("SampleCPU111" + "SampleBoard222"));
                return BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 16);
            }
        }

        /// <summary>
        /// ���ɼ����루���������㷨��ȫһ�£�
        /// </summary>
        private string GenerateActivationCode(string machineCode, DateTime date)
        {
            string timestamp = date.ToString("yyyyMMdd");
            using (SHA256 sha256 = SHA256.Create())
            {
                // �������򱣳���ȫһ�µĹ�ϣ���㷽ʽ
                byte[] hashBytes = sha256.ComputeHash(
                    Encoding.UTF8.GetBytes(machineCode + timestamp + SecretKey));
                // ȡǰ24λ��Ϊ������
                return BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 24);
            }
        }

        /// <summary>
        /// ���ɼ����밴ť����¼�
        /// </summary>
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            string machineCode = txtMachineCode.Text.Trim();

            // ��֤����
            if (string.IsNullOrEmpty(machineCode))
            {
                MessageBox.Show("�����������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMachineCode.Focus();
                return;
            }

            if (machineCode.Length != 16)
            {
                MessageBox.Show("�������ʽ����ȷ��ӦΪ16λ�ַ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMachineCode.Focus();
                return;
            }

            try
            {
                // ���ɼ�����
                string activationCode = GenerateActivationCode(machineCode, dtpDate.Value);
                txtActivationCode.Text = activationCode;
                MessageBox.Show("���������ɳɹ���", "�ɹ�", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"����ʧ�ܣ�{ex.Message}", "����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ���Ƽ����밴ť����¼�
        /// </summary>
        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtActivationCode.Text))
            {
                Clipboard.SetText(txtActivationCode.Text);
                MessageBox.Show("�������Ѹ��Ƶ�������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("û�пɸ��Ƶļ����룬��������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
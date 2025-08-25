using UserWinForms.Services;

namespace UserWinForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ��鼤��״̬
            var activationService = new AppActivationService();
            if (activationService.IsActivated())
            {
                // �Ѽ��ֱ�ӽ���������
                Application.Run(new MainForm());
            }
            else
            {
                // δ�����ʾ�������
                var activationForm = new ActivationForm();
                if (activationForm.ShowDialog() == DialogResult.OK)
                {
                    // ����ɹ������ã�����������
                    Application.Run(new MainForm());
                }
                // �����˳�Ӧ��
            }

            //ApplicationConfiguration.Initialize();
            //Application.Run(new ActivationForm());
        }
    }
}
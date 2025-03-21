namespace CKCharaDataEditor
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }

        static Program()
        {
            string[] args = Environment.GetCommandLineArgs();
            IsDeveloper = args.Contains("--Usagi");
        }

        public static bool IsDeveloper { get; private set; }
    }
}
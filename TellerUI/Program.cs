using System;
using System.Windows.Forms;
using EffinghamLibrary.Vaults;
using SimpleInjector;

namespace TellerUI
{
    internal static class Program
    {
        private static Container injectionContainer = new Container();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            InitializeDependencyInjection();
            Application.Run(injectionContainer.GetInstance<MainForm>());
        }

        private static void InitializeDependencyInjection()
        {
            injectionContainer = new Container();
            injectionContainer.RegisterSingleton<IVault>(EntityFrameworkVault.Instance); // The type of vault needs to be specified here and only here.
            injectionContainer.Register<MainForm>();
        }
    }
}

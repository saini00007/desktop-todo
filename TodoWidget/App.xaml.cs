using Microsoft.UI.Xaml;
using System.IO;
using System;

namespace TodoWidget
{
    public partial class App : Application
    {
        private Window? m_window;

        public App()
        {
            try
            {
                File.WriteAllText("C:\\Users\\Public\\TodoWidget_log.txt", "App constructor called\n");
                this.InitializeComponent();
                File.AppendAllText("C:\\Users\\Public\\TodoWidget_log.txt", "App initialized\n");
            }
            catch (Exception ex)
            {
                File.WriteAllText("C:\\Users\\Public\\TodoWidget_log.txt", "Error in App constructor: " + ex.ToString());
            }
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            try
            {
                File.AppendAllText("C:\\Users\\Public\\TodoWidget_log.txt", "OnLaunched called\n");
                m_window = new MainWindow();
                File.AppendAllText("C:\\Users\\Public\\TodoWidget_log.txt", "MainWindow created\n");
                m_window.Activate();
                File.AppendAllText("C:\\Users\\Public\\TodoWidget_log.txt", "Window activated\n");
            }
            catch (Exception ex)
            {
                File.AppendAllText("C:\\Users\\Public\\TodoWidget_log.txt", "Error in OnLaunched: " + ex.ToString());
            }
        }
    }
} 
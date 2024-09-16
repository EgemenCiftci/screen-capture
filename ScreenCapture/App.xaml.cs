using Serilog;
using System.Windows;
using System.Windows.Threading;

namespace ScreenCapture;

public partial class App : System.Windows.Application
{
    private MainWindow? window;
    private NotifyIcon? notifyIcon;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.File("log.txt",
                                                            rollingInterval: RollingInterval.Day,
                                                            rollOnFileSizeLimit: true).CreateLogger();

        Log.Information("Application started.");

        try
        {
            notifyIcon = new()
            {
                Icon = new Icon("screenCapture.ico"),
                Visible = true,
                Text = "Screen Capture"
            };

            ContextMenuStrip contextMenu = new();
            _ = contextMenu.Items.Add("Exit", null, (s, ea) =>
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
                Current.Shutdown();
            });
            notifyIcon.ContextMenuStrip = contextMenu;

            KeyboardHook.SetHook(key =>
            {
                Log.Information(key);

                if (key == ScreenCapture.Properties.Settings.Default.EnterCaptureModeKey)
                {
                    window = new();
                    window.Show();
                }
                else if (key == ScreenCapture.Properties.Settings.Default.ExitCaptureModeKey)
                {
                    Current.MainWindow = null;
                    window?.Close();
                }
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception");
        }
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        try
        {
            KeyboardHook.UnhookWindowsHookEx();

            if(notifyIcon != null)
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
            }

            Log.Information("Application exited.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        try
        {
            Log.Error(e.Exception, "DispatcherUnhandledException");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception");
        }
    }
}

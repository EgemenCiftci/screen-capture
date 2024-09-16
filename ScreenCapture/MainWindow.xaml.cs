using Serilog;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ScreenCapture;

public partial class MainWindow : Window
{
    private bool isDragging = false;
    private System.Windows.Point startPoint;
    private System.Windows.Shapes.Rectangle? rectangle = null;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        try
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isDragging = true;
                startPoint = e.GetPosition(canvas);

                rectangle = new()
                {
                    Stroke = new BrushConverter().ConvertFromString(Properties.Settings.Default.Stroke) as System.Windows.Media.Brush,
                    StrokeThickness = Properties.Settings.Default.StrokeThickness,
                    Fill = System.Windows.Media.Brushes.Transparent
                };

                Canvas.SetLeft(rectangle, startPoint.X);
                Canvas.SetTop(rectangle, startPoint.Y);

                canvas.Children.Clear();
                _ = canvas.Children.Add(rectangle);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception");
            Hide();
        }
    }

    private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        try
        {
            if (isDragging && rectangle != null)
            {
                System.Windows.Point currentPoint = e.GetPosition(canvas);

                double width = currentPoint.X - startPoint.X;
                double height = currentPoint.Y - startPoint.Y;

                if (width < 0)
                {
                    Canvas.SetLeft(rectangle, currentPoint.X);
                    width = -width;
                }
                else
                {
                    Canvas.SetLeft(rectangle, startPoint.X);
                }

                if (height < 0)
                {
                    Canvas.SetTop(rectangle, currentPoint.Y);
                    height = -height;
                }
                else
                {
                    Canvas.SetTop(rectangle, startPoint.Y);
                }

                rectangle.Width = width;
                rectangle.Height = height;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception");
            Hide();
        }
    }

    private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
    {
        try
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                if (rectangle != null)
                {
                    Log.Information($"Capturing screen.");

                    PresentationSource source = PresentationSource.FromVisual(this);
                    Matrix transformToDevice = source.CompositionTarget.TransformToDevice;
                    double dpiX = transformToDevice.M11;
                    double dpiY = transformToDevice.M22;

                    Log.Information($"dpiX:{dpiX}");
                    Log.Information($"dpiY:{dpiY}");

                    int screenWidth = Convert.ToInt32(SystemParameters.PrimaryScreenWidth);
                    int screenHeight = Convert.ToInt32(SystemParameters.PrimaryScreenHeight);

                    int x = Convert.ToInt32(Canvas.GetLeft(rectangle) / dpiX);
                    int y = Convert.ToInt32(Canvas.GetTop(rectangle) / dpiY);
                    int w = Convert.ToInt32(rectangle.Width / dpiX);
                    int h = Convert.ToInt32(rectangle.Height / dpiY);

                    Log.Information($"screenWxH:{screenWidth}x{screenHeight}");
                    Log.Information($"rectangleWxH:{w}x{h}");
                    Log.Information($"rectanglePos:({x},{y})");

                    BitmapSource bs = ImageHelpers.Capture(screenWidth, screenHeight, x, y, w, h);

                    if (Properties.Settings.Default.CaptureToClipboard)
                    {
                        System.Windows.Clipboard.SetImage(bs);
                        Log.Information($"Captured to clipboard.");
                    }

                    if (Properties.Settings.Default.CaptureToFile)
                    {
                        string fileName = $"{DateTime.Now:yyyyMMddHHmmssfff}.png";
                        string filePath = System.IO.Path.Combine(Properties.Settings.Default.CaptureToFileDestination, fileName);
                        Log.Information($"FilePath: {filePath}");
                        ImageHelpers.SaveBitmapSourceToFile(bs, filePath);
                        Log.Information($"Captured to file.");
                    }
                }

                canvas.Children.Clear();
                isDragging = false;
                rectangle = null;
                Hide();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception");
            Hide();
        }
    }
}
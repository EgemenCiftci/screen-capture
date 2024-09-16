using System.IO;
using System.Windows.Media.Imaging;

namespace ScreenCapture;

public static class ImageHelpers
{
    public static BitmapSource Capture(int screenWidth, int screenHeight, int x, int y, int w, int h)
    {
        // Take a screenshot of the entire screen
        System.Drawing.Bitmap screenshot = new(screenWidth, screenHeight);
        using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(screenshot))
        {
            g.CopyFromScreen(0, 0, 0, 0, screenshot.Size);
        }

        // Crop the screenshot to the area of interest
        System.Drawing.Bitmap croppedImage = new(w, h);

        using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(croppedImage))
        {
            g.DrawImage(screenshot, 
                        new System.Drawing.Rectangle(0, 0, croppedImage.Width, croppedImage.Height),
                        new System.Drawing.Rectangle(x, y, w, h), 
                        System.Drawing.GraphicsUnit.Pixel);
        }

        return ConvertBitmapToImageSource(croppedImage);
    }

    private static BitmapSource ConvertBitmapToImageSource(System.Drawing.Bitmap bitmap)
    {
        using MemoryStream memoryStream = new();
        bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
        _ = memoryStream.Seek(0, SeekOrigin.Begin);
        BitmapImage bitmapImage = new();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = memoryStream;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();
        return bitmapImage;
    }

    public static void SaveBitmapSourceToFile(BitmapSource bitmapSource, string filePath)
    {
        PngBitmapEncoder encoder = new();
        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
        using FileStream fileStream = new(filePath, FileMode.Create);
        encoder.Save(fileStream);
    }
}
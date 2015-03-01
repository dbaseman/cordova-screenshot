using Microsoft.Xna.Framework.Media;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using WPCordovaClassLib;
using WPCordovaClassLib.Cordova;
using WPCordovaClassLib.Cordova.Commands;
using WPCordovaClassLib.Cordova.JSON;
using Microsoft.Phone.Controls;

namespace Cordova.Extension.Commands
{
    public class Screenshot : BaseCommand
    {
        public void save(string jsonArgs)
        {
			var options = JsonHelper.Deserialize<string[]>(jsonArgs);

            string format = options[0];
			int quality = int.Parse(options[1]);
			string filename = options[2];
			
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                CordovaView content;
                if (TryGetContentRoot(out content))
                {
                    var currentScreenImage = new WriteableBitmap((int)content.ActualWidth, (int)content.ActualHeight);
                    currentScreenImage.Render(content, new MatrixTransform());
                    currentScreenImage.Invalidate();
                    Picture picture = SaveToMediaLibrary(currentScreenImage, filename, 100);
                    
					string path = picture.GetPath();
                    MessageBox.Show("Captured image " + path + " Saved Sucessfully", "WP Capture Screen", MessageBoxButton.OK);

                    string res = string.Concat("{",
                        "filename: ",
                        path,
                        "}");
                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, res));
                }

                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR));
            });
        }

        public void URI(string jsonArgs)
        {
            DispatchCommandResult(new PluginResult(PluginResult.Status.OK));
        }

       #region Private methods

        private bool TryGetContentRoot(out CordovaView content)
        {
            content = null;
            var frame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (frame != null)
            {
                var page = frame.Content as PhoneApplicationPage;
                if (page != null)
                {
        			var view = page.FindName("CordovaView") as CordovaView;
                    if (view != null)
                    {
                        content = view;
                        return true;
                    }
                }
            }
            return false;
        }

        private Picture SaveToMediaLibrary(WriteableBitmap bitmap, string name, int quality)
        {
            using (var stream = new MemoryStream())
            {
                // Save the picture to the Windows Phone media library.
                bitmap.SaveJpeg(stream, bitmap.PixelWidth, bitmap.PixelHeight, 0, quality);
                stream.Seek(0, SeekOrigin.Begin);
                return new MediaLibrary().SavePicture(name, stream);
            }
        }

        #endregion
	}
}
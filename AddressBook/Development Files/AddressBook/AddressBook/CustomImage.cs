using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AddressBook
{
    class CustomImage : Image
    {
        public CustomImage(string imageName, int width)
        {
            // Create Image Element
            this.Width = width;

            // Create source
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(imageName, UriKind.RelativeOrAbsolute);
            bitmapImage.DecodePixelWidth = width;
            bitmapImage.EndInit();

            //set image source
            this.Source = bitmapImage;
        }
    }
}

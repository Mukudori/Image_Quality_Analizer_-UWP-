using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Image_Quality_Analizer
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class SelectionImageSizePage : Page
    {

        public static int sizeX, sizeY;
        public static bool accept;

        private void AcceptClick(object sender, RoutedEventArgs e)
        {
            switch (ImSizeBox.SelectedIndex)
            {
                case 0:
                    sizeX = 640;
                    sizeY = 480;
                    break;
                case 1:
                    sizeX = 800;
                    sizeY = 600;
                    break;
                case 2:
                    sizeX = 1024;
                    sizeY = 768;
                    break;
                case 3:
                    sizeX = 1280;
                    sizeY = 1024;
                    break;
            }

            accept = true;

            Frame.Navigate(typeof(QualityAnalizingPage));

        }

        private void CanselClick(object sender, RoutedEventArgs e)
        {
            accept = false;
            Frame.Navigate(typeof(QualityAnalizingPage));
        }

        public SelectionImageSizePage()
        {
            this.InitializeComponent();
        }
    }
}

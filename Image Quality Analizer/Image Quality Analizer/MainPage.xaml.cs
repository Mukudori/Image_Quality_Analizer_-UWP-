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

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace Image_Quality_Analizer
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CoeficientMatrixPage));
        }

        private void Galery_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ImageGaleryPage));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private async void EditBD_Click(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(typeof(EditDataBase));
            //Frame.Navigate(typeof(ImagesInfoFromBasePage));
            SelectionTableDialog dlg = new SelectionTableDialog();

            ContentDialogResult result = await dlg.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                Frame.Navigate(typeof(ImagesInfoFromBasePage));
            }
            else  Frame.Navigate(typeof(EditDataBase));
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}

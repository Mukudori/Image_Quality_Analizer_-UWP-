using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Image_Quality_Analizer
{
  
    public sealed partial class ViewAllInfoOfImage : Page
    {
        public ViewAllInfoOfImage()
        {
            this.InitializeComponent();
            Page_Loaded();
        }

        public static ImagesTable imageTable;
        public BitmapImage image;

        private void Page_Loaded()
        {
            Name.Text = imageTable.name;
            id.Text = imageTable.id.ToString();
            format.Text = imageTable.format;
            pathInport.Text = imageTable.pathInport;
            pathExport.Text = imageTable.pathExport;
            pathLocal.Text = imageTable.pathLocal;
            JQ.Text = imageTable.jq.ToString();
            Blur.Text = imageTable.blur.ToString();
            accepted.IsChecked = imageTable.accepted;
            exported.IsChecked = imageTable.exported;

            image = GlobalFuncs.getBirmapImageFromPath(imageTable.pathLocal);
            imageView.Source = image;

        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ImagesInfoFromBasePage));
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            imageView.Source = null;
            image = GlobalFuncs.getBirmapImageFromPath(imageTable.pathLocal);
            imageView.Source = image;
        }

        private async void Rename_Click(object sender, RoutedEventArgs e)
        {
            NameDialog dlg = new NameDialog()
            {
                Title = "Введите новое имя",
                Content = "",
                PrimaryButtonText = "Переименовать",
                SecondaryButtonText = "Отмена"
            };

            var TB = new TextBox();

            dlg.Content = TB;

            var result = await dlg.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                TB = (TextBox)dlg.Content;

                imageTable.name = TB.Text;

                using (ImageContext db = new ImageContext())
                {
                    
                    db.Images.Update(imageTable);
                    db.SaveChanges();
                    
                }
                var dialog = new MessageDialog("Файл переименован.");
                await dialog.ShowAsync();

                Page_Loaded();
            }

        }
    }
}

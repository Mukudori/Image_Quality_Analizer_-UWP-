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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Image_Quality_Analizer
{  
    public sealed partial class ViewImage : Page
    {
        static public ImagesTableView imageTable = new ImagesTableView();

        public ViewImage()
        {
            this.InitializeComponent();
            image.Source = imageTable.image;
            tbName.Text = imageTable.name;
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ImageGaleryPage));
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

                    var imageUpdate = db.Images.FirstOrDefault(c => c.id == imageTable.id);
                    var dialog1 = new Windows.UI.Popups.MessageDialog("Файл отсутствует в базе данных.");
                    try
                    {
                        imageUpdate.name = imageTable.name;

                        db.Images.Update(imageUpdate);
                        db.SaveChanges();
                    }
                    catch
                    {                        
                        await dialog1.ShowAsync();
                    }

                }
                var dialog = new Windows.UI.Popups.MessageDialog("Файл переименован.");
                tbName.Text = imageTable.name;
                await dialog.ShowAsync();

                
            }
        }

        private void Refresh_click(object sender, RoutedEventArgs e)
        {
            image.Source = null;
            image.Source = imageTable.image;
        }
    }
}

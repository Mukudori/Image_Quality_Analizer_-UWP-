using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;



namespace Image_Quality_Analizer
{
    public sealed partial class DataBaseImages : Page
    {
        ObservableCollection<ImagesTable> images;

        List<HistoryTable> histories;

        public DataBaseImages()
        {
            this.InitializeComponent();
            this.Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (ImageContext db = new ImageContext())
            {
                images = new ObservableCollection<ImagesTable>(db.Images.Include(x => x.HistoryTable).ToList());
                histories = db.History.ToList();
            }

            historiesList.ItemsSource = histories;
            imagesList.ItemsSource = images;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            HistoryTable history = historiesList.SelectedItem as HistoryTable;
            if (histories == null) return;

            // создаем объект Image
            ImagesTable image = new ImagesTable
            {
                name = nameBox.Text,
                jq = Double.Parse(priceBox.Text),
                HistoryTable = history,
                historyId = history.id
            };

            using (ImageContext db = new ImageContext())
            {
                db.History.Attach(history);
                db.Images.Add(image);
                if (db.SaveChanges() > 0)
                {
                    //images.Add(image);
                    // либо так
                    imagesList.ItemsSource = db.Images.Include(x => x.HistoryTable).ToList();
                }
            }
        }



        private void historiesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HistoryTable history = historiesList.SelectedItem as HistoryTable;
            if (histories == null) return;

            imagesList.ItemsSource = history.images;
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {            
                Frame.Navigate(typeof(EditDataBase));            
        }

        private async void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (imagesList.SelectedItem != null)
            {
                HistoryTable history = historiesList.SelectedItem as HistoryTable;
                if (histories == null) return;

                // создаем объект Image
                ImagesTable image = imagesList.SelectedItem as ImagesTable;

                Windows.Storage.StorageFolder Folder = await Package.Current.InstalledLocation.GetFolderAsync(GlobalFuncs.GetLocalFolder());
                if (Folder.TryGetItemAsync(image.id.ToString() + image.format) != null)
                {
                    StorageFile file = await Folder.GetFileAsync(image.id.ToString() + image.format);
                    await file.DeleteAsync();
                }

                using (ImageContext db = new ImageContext())
                {
                    db.History.Attach(history);
                    db.Images.Remove(image);
                    if (db.SaveChanges() > 0)
                    {
                        //images.Add(image);
                        // либо так
                        imagesList.ItemsSource = db.Images.Include(x => x.HistoryTable).ToList();
                    }
                }
            }
        }
    }
}

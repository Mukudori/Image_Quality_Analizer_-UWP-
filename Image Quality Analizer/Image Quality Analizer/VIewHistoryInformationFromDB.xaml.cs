using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class VIewHistoryInformationFromDB : Page
    {
        HistoryTable history;
        List<int> imagesId;
        ObservableCollection<ImagesTable> imgList;

        public VIewHistoryInformationFromDB()
        {
            this.InitializeComponent();
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                using (ImageContext db = new ImageContext())
                { history = (HistoryTable)e.Parameter;
                  imgList = new ObservableCollection<ImagesTable>(db.Images.Include(x => x.HistoryTable).ToList());
                    /*
                         history = db.History.FirstOrDefault(c => c.id == id);
                         db.History.Attach(history);
                         images = db.Images.ToList();
                     */
                }

            }

            if (history != null)
            {
                id.Text = history.id.ToString();
                dateTime.Text = history.dateTime.ToString();
                minJQ.Text = history.minJQ.ToString();
                maxJQ.Text = history.maxJQ.ToString();
                minBlur.Text = history.minBlur.ToString();
                maxBlur.Text = history.maxBlur.ToString();
                CopyToGalery.IsChecked = history.copyToGalery;

                using (ImageContext db = new ImageContext())//велосипед
                {
                    //List<ImagesTable> images = db.Images.ToList();
                    int  countAll = 0;
                    int  countAccept = 0;
                   // db.History.Attach(history);

                   List<ImagesTable> images = db.Images.ToList();

                    for (int i = 0; i < images.Count; i++)
                    {
                        if (images[i].historyId == history.id)
                        {
                            countAll++;
                           // imagesId.Add(images[i].id);
                            if (images[i].accepted) countAccept++;
                        }
                    }

                    allCount.Text = countAll.ToString();
                    acceptCount.Text = countAccept.ToString();
                }
            }
        }

        private void Page_Loaded()
        {
            id.Text = history.id.ToString();
            dateTime.Text = history.dateTime.ToString();
            minJQ.Text = history.minJQ.ToString();
            maxJQ.Text = history.maxJQ.ToString();
            minBlur.Text = history.minBlur.ToString();
            maxBlur.Text = history.maxBlur.ToString();
            CopyToGalery.IsChecked = history.copyToGalery;
            allCount.Text = history.images.Count.ToString();

            int count = 0;

            for (int i = 0; i < history.images.Count; i++)
            {
                if (history.images[i].accepted) count++;
            }

            acceptCount.Text = count.ToString();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EditDataBase));
        }

        private void ViewImages_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ImagesInfoFromBasePage), imagesId);
        }
    }
}

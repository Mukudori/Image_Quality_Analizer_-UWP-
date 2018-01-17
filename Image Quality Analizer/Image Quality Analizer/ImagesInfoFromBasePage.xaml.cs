using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace Image_Quality_Analizer
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class ImagesInfoFromBasePage : Page
    {
        public ImagesInfoFromBasePage()
        {
            this.InitializeComponent();
            this.Loaded += Page_Loaded;
        }

        ObservableCollection<ImagesTable> images;
        List<ImagesTable> imglist;
        private List<HistoryTable> histories;

      /*  protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            imglist.Clear();
            if (e.Parameter != null)
            {
                List<int> imgId=(List<int>)e.Parameter;
                //imglist = (List<ImagesTable>)e.Parameter;

                using (ImageContext db = new ImageContext())
                {
                    List<ImagesTable> Images = db.Images.ToList();
                    
                    for (int i = 0; i < Images.Count; i++)
                    {
                        if (Images[i].id == imgId[i]) imglist.Add(Images[i]);
                    }
                }

                    OutputImages();
            }
        }*/

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (ImageContext db = new ImageContext())
            {
                images = new ObservableCollection<ImagesTable>(db.Images.Include(x => x.HistoryTable).ToList());
                histories = db.History.ToList();
            }
            
            imglist = images.ToList();
            jqSlider.Minimum = imglist[0].jq;
            jqSlider.Maximum = imglist[0].jq;
            blurSlider.Minimum = imglist[0].blur;
            blurSlider.Maximum = imglist[0].blur;
            for (int i = 0; i < imglist.Count; i++)
            {
                if (jqSlider.Minimum > imglist[i].jq) jqSlider.Minimum = imglist[i].jq;
                if (jqSlider.Maximum < imglist[i].jq) jqSlider.Maximum = imglist[i].jq;

                if (blurSlider.Minimum > imglist[i].blur) blurSlider.Minimum = imglist[i].blur;
                if (blurSlider.Maximum < imglist[i].blur) blurSlider.Maximum = imglist[i].blur;
            }
            
            historyCB.ItemsSource = histories.ToList();
        }

        private async void OutputImages()
        {
            StorageFolder folderImages = await Package.Current.InstalledLocation.GetFolderAsync(GlobalFuncs.GetLocalFolder());            

            List<ImagesTableView> imgView = new List<ImagesTableView>();
            for (int i = 0; i < imglist.Count; i++)
            {
                if (await folderImages.TryGetItemAsync(imglist[i].id.ToString() + imglist[i].format) == null)
                    imglist[i].pathLocal = "ms-appx:///Assets//ImageDeleted.png";

               
                if (jqSlider.Value <= imglist[i].jq && blurSlider.Value<=imglist[i].blur) imgView.Add(new ImagesTableView(imglist[i]));
               
            }

            tbCountFiltred.Text = "Количество отфильтрованных изображений : " + imgView.Count.ToString();
            tbCountAll.Text = "Всего по данному анализу : " + imglist.Count.ToString();
            ImagesView.ItemsSource = null;
            ImagesView.ItemsSource = imgView.ToList();
            imgView.Clear();
        }

        private  void AcceptFiltrClick(object sender, RoutedEventArgs e)
        {
            if (historyCB.SelectedItem != null)
            {
                
                HistoryTable history = historyCB.SelectedItem as HistoryTable;
                if (histories == null) return;
                imglist = history.images;

                OutputImages();

                butDel.Visibility = Visibility.Visible;
                butExport.Visibility = Visibility.Visible;
                butView.Visibility = Visibility.Visible;

            }
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void ViewClic(object sender, RoutedEventArgs e)
        {
            if (ImagesView.SelectedItem != null)
            {
                ViewAllInfoOfImage.imageTable = imglist[ImagesView.SelectedIndex];
                Frame.Navigate(typeof(ViewAllInfoOfImage));
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {

            ContentDialog deleteDialog = new ContentDialog()
            {
                Title = "Удаление изображения",
                Content = "Вы действительно хотите удалить файл?",
                PrimaryButtonText = "Да",
                SecondaryButtonText = "Нет"

            };

            var result1 = await deleteDialog.ShowAsync();

            if (result1 == ContentDialogResult.Primary)
            {
                DeleteImagesDialog dlg = new DeleteImagesDialog();

                
                var result2 = await dlg.ShowAsync();
                

                if (result2 == ContentDialogResult.Primary)
                {
                    ImagesTableView image = ImagesView.SelectedItem as ImagesTableView;
                    GlobalFuncs.DeleteImage(image.id.ToString() + image.format);
                }
                else
                {
                   
                    for (int i = 0; i < ImagesView.Items.Count ; i++)
                    {
                        ImagesTableView image = ImagesView.Items[i] as ImagesTableView;
                        GlobalFuncs.DeleteImage(image.id.ToString() + image.format);
                    }
                }
                ImagesView.ItemsSource = null;
                OutputImages();
            }
        }

        

        private async void Export_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog yn= new ContentDialog()
            {
                Title = "Экспортирование изображений",
               // Content = "Вы действительно хотите удалить файл?",
                PrimaryButtonText = "Только выбранное",
                SecondaryButtonText = "Все по данному анализу" 

            };

            var result = await yn.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                FolderPicker folderPicker = new Windows.Storage.Pickers.FolderPicker();

                folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
                folderPicker.FileTypeFilter.Add("*");

                var ExportFolder = await folderPicker.PickSingleFolderAsync();

                if (ExportFolder != null)
                {
                    ImagesTableView image = ImagesView.SelectedItem as ImagesTableView;
                    GlobalFuncs.ExportImage(image.id.ToString() + image.format, ExportFolder, image.name);

                    var dialog = new MessageDialog("Файл "+image.name+" скопирован в "+ExportFolder.Path);
                    await dialog.ShowAsync();
                }
            }
            else
            {
                FolderPicker folderPicker = new Windows.Storage.Pickers.FolderPicker();

                folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
                folderPicker.FileTypeFilter.Add("*");

                var ExportFolder = await folderPicker.PickSingleFolderAsync();

                if (ExportFolder != null)
                {
                    int count = 0;
                    StorageFolder folderImages = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(GlobalFuncs.GetLocalFolder());

                    for (int i = 0; i < ImagesView.Items.Count; i++)
                    {
                        ImagesTableView image = ImagesView.Items[i] as ImagesTableView;
                        GlobalFuncs.ExportImage(image.id.ToString() + image.format, ExportFolder, image.name);
                        if (await folderImages.TryGetItemAsync(image.id.ToString()+image.format) != null)
                        {
                            count++;
                        }
                    }
                    string countString="файлов";
                    switch (count % 10)
                    {
                        case 1: countString = "файл";
                            break;
                        case 2: countString = "файла";
                            break;
                        case 3: countString = "файла";
                            break;
                        case 4: countString = "файла";
                            break;
                        default:
                            countString = "файлов";
                            break;

                    }

                    var dialog = new MessageDialog(count.ToString() + " "+ countString+" скопировано в " + ExportFolder.Path);
                    await dialog.ShowAsync();
                }

            }
        }
    }
}

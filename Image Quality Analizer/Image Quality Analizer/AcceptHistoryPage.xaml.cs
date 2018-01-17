using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace Image_Quality_Analizer
{ 
    public sealed partial class AcceptHistoryPage : Page
    {
        public static List<ImagesTable> imglist;
        public static HistoryTable history;

        StorageFolder ExportFolder;

        public AcceptHistoryPage()
        {
            this.InitializeComponent();            
        }

       
        private async void EndAndAccept(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = await Package.Current.InstalledLocation.GetFolderAsync(GlobalFuncs.GetLocalFolder());           

            for (int i = 0; i < imglist.Count; i++)
            {
                if (imglist[i].blur >= history.minBlur) imglist[i].accepted = true;

                if (CheckExport.IsChecked.Value)
                {
                    if (imglist[i].accepted)
                    {
                        imglist[i].exported = CheckExport.IsChecked.Value;
                        imglist[i].pathExport = ExportBox.Text;

                        StorageFile file = await folder.GetFileAsync(imglist[i].id.ToString() + imglist[i].format);
                        await file.CopyAsync(ExportFolder);
                    }
                }
                if (CheckGalery.IsChecked.Value)
                {
                    //imglist[i].pathLocal += FolderBox.Text.TrimStart().TrimEnd();
                    history.copyToGalery = true;
                    /*var f = await ApplicationData.Current.LocalFolder.GetFolderAsync("Migrations\\ImageGalery");
                    await f.CreateFolderAsync("papka");
                    StorageFile file = await folder.GetFileAsync(imglist[i].id.ToString() + imglist[i].format);
                    StorageFolder NewFolder = await Package.Current.InstalledLocation.GetFolderAsync(GlobalFuncs.GetLocalFolder(true) + FolderBox.Text.TrimStart().TrimEnd());
                    await file.MoveAsync(NewFolder);*/
                    if (!CheckSaveUnaccepted.IsChecked.Value)
                    {
                        StorageFile file = await folder.GetFileAsync(imglist[i].id.ToString() + imglist[i].format);
                        await file.DeleteAsync();
                    }
                }
                else
                {
                    StorageFile file = await folder.GetFileAsync(imglist[i].id.ToString() + imglist[i].format);
                    await file.DeleteAsync();
                }

                imglist[i].HistoryTable = history;
                imglist[i].historyId = history.id;
            }

            using (ImageContext db = new ImageContext())
            {
                db.History.Add(history);
                db.SaveChanges();
                db.History.Attach(history);
                for (int i = 0; i < imglist.Count; i++)
                {
                    db.Images.Add(imglist[i]);
                }
                db.SaveChanges();
            }
            Frame.Navigate(typeof(MainPage));
        }

        private async void FolderDialogClick(object sender, RoutedEventArgs e)
        {
           FolderPicker folderPicker = new Windows.Storage.Pickers.FolderPicker();

           

            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            ExportFolder = await folderPicker.PickSingleFolderAsync();

            if (ExportFolder != null)
            {
                ExportBox.Text = ExportFolder.Path;
            }
            else
            {
                ExportBox.Text = "Папка не выбрана";
            }

        }
    }
}

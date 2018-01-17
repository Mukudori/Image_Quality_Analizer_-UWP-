using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Image_Quality_Analizer
{

    public sealed partial class ImageGaleryPage : Page
    {
        private List<Evaluate> listEvaluate = new List<Evaluate>();        

        enum TypeButtonClick  { View, Delete, None }
   
        //TypeButtonClick click = TypeButtonClick.None;
        public ImageGaleryPage()
        {
            this.InitializeComponent();
            Page_Loaded();
        }

        private async void Page_Loaded()
        {

            gridView.ItemsSource = null;
            listEvaluate.Clear();
            StorageFolder folderImages;

            
            IReadOnlyList<StorageFile> fileList;


            folderImages = await Package.Current.InstalledLocation.GetFolderAsync(GlobalFuncs.GetLocalFolder());
            fileList = await folderImages.GetFilesAsync();

            List<ImagesTable> imgList;

            string JQ, Blur;

             using (ImageContext db = new ImageContext())
             {
                 imgList = db.Images.ToList();
             }
            
       
                for (int i = 0; i < fileList.Count; i++)
                {
                    JQ = "0";
                    Blur = "0";
                    int id=0;
                    string name=null;
                  for (int j = 0; j < imgList.Count; j++)
                      {
                        id = Convert.ToInt32(GlobalFuncs.GetFormatAndNameFromString(fileList[i].Name)[0]);
                        int id2 = imgList[j].id;
                           if (id == id2)
                            {
                                JQ = imgList[j].jq.ToString();
                                Blur = imgList[j].blur.ToString();
                                name = imgList[j].name;
                                break;
                            }                       

                      }
                if (name == null) name = fileList[i].Name;
               
               
                    listEvaluate.Add(new Evaluate(id, name, JQ, Blur, GlobalFuncs.GetLocalFolder(true, true) + fileList[i].Name));
                
            }

            gridView.ItemsSource = listEvaluate;
            tbCount.Text = " Количество изображений : " + fileList.Count.ToString();
            fileList = null;            
        }

        private void Back_Click(object sender, RoutedEventArgs e) { }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private async void DeleteImage(string name)
        {
            StorageFolder folder = await Package.Current.InstalledLocation.GetFolderAsync(GlobalFuncs.GetLocalFolder());

            {/* using (ImageContext db = new ImageContext())
             {
                 //imgList = db.Images.ToList();
                 for (int i = 0; i < imgList.Count; i++)
                 {
                     if (imgList[i].id == listEvaluate[gridView.SelectedIndex].id)
                     {
                         db.Images.Remove(imgList[i]);
                         break;
                     }
                 }
                 db.SaveChanges();
             }
             */
            }

            StorageFile file = await folder.GetFileAsync(name);
            folder = null;
            try
            {
                await file.DeleteAsync();
            }
            catch
            {
                var dialog = new MessageDialog("Не удается получить доступ к файлу из за отсутствия прав или файл занят другим процессом.");
                await dialog.ShowAsync();
            }
            Page_Loaded();
        }
     
        private void gridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            { /*
            switch (click)
            {
                case TypeButtonClick.View:
                    {
                        ViewImage.Image = listEvaluate[gridView.SelectedIndex].image;
                        Frame.Navigate(typeof(ViewImage));                        
                        break;
                    }
                case TypeButtonClick.Delete:
                    {
                        
                        if (gridView.SelectedItem != null)
                        {
                            string name = listEvaluate[gridView.SelectedIndex].name;
                            gridView.ItemsSource = null;
                            listEvaluate.Clear();                            
                            DeleteImage(name);
                           // Page_Loaded();
                        }

                        break;
                    }
            }

            click = TypeButtonClick.None;
            */
            }

           
        }        

        private async void DeleteClick(object sender, RoutedEventArgs e)
        {
            //click = TypeButtonClick.Delete;
            //tbAction.Text = "Выберите изображение для удаления";
            if (gridView.SelectedItem != null)
            {
                ContentDialog deleteDialog = new ContentDialog()
                {
                    Title = "Удаление изображения",
                    Content = "Вы действительно хотите удалить файл?",
                    PrimaryButtonText = "Да",
                    SecondaryButtonText = "Нет"

                };

                ContentDialogResult result = await deleteDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    // ButtonsVisible(Visibility.Collapsed);
                    Evaluate img = gridView.SelectedItem as Evaluate;
                    string name = img.id.ToString()+GlobalFuncs.GetFormatAndNameFromString(img.name)[1];

                    DeleteImage(name);
                }                
                
            }

        }

        private void ViewImageClick(object sender, RoutedEventArgs e)
        {
            
            if (gridView.SelectedItem != null)
            {
                Evaluate image = gridView.SelectedItem as Evaluate;

                ViewImage.imageTable.name = image.name;
                ViewImage.imageTable.id = image.id;
                ViewImage.imageTable.ChangeImage(image.path);
                Frame.Navigate(typeof(ViewImage));
            }

        }
        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            tbLocate.Text = "";
            Page_Loaded();
        }

        private void ButtonsVisible(Visibility vis)
        {
            RefreshButton.Visibility = vis;
            ViewButton.Visibility = vis;
            DeleteButton.Visibility = vis;
            BackButton.Visibility = vis;

            if (vis == Visibility.Collapsed) CancelButton.Visibility = Visibility.Visible;
            else CancelButton.Visibility = Visibility.Collapsed;
        }// Включает/выключает все кнопки кроме отмены, сбрасывает флаг типа кнопки

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            ButtonsVisible(Visibility.Visible);
            //click = TypeButtonClick.None;
        }

        private void Texbox_TextChange(TextBox sender, TextBoxTextChangingEventArgs args)
        {

            gridView.ItemsSource = GlobalFuncs.LocateFromTextBox(tbLocate, listEvaluate);
        }
    }
}

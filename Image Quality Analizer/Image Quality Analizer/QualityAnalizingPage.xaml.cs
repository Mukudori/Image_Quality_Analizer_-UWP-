using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using WRCOpenCvFuncs;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Foundation;
using System.Net.Http;

namespace Image_Quality_Analizer
{

    public sealed partial class QualityAnalizingPage : Page
    {
        private List<Evaluate> listEvaluate = new List<Evaluate>();
        
        List<ImagesTable> listImages = new List<ImagesTable>();

        static int imageId, historyId; // ведущие индексы таблиц

        public static double[] matCoeff = new double[] { 1,1,1,  1,1,1, 1,1,1 }; // Матрица коэфициентов
        public static bool Acceptmatrix;
        

        public QualityAnalizingPage()
        {
            this.InitializeComponent();
            if (listEvaluate.Count == 0)
            {
                imageId = GlobalFuncs.FreeId(GlobalFuncs.KindTable.Images);
                historyId = GlobalFuncs.FreeId(GlobalFuncs.KindTable.History);
            }

            listImageView.ItemsSource = listEvaluate.ToList();

            this.Loaded += ShowList;
            GridAccepte.Visibility = Visibility.Collapsed;
        }

        private void ShowList(object sender, RoutedEventArgs e)
        {
            if (listEvaluate.Count > 0)
            {
                GridAccepte.Visibility = Visibility.Visible;
            }
            listImageView.ItemsSource = listEvaluate.ToList();

        }


        private async void OpenManyFiles()
        {
            progressRing.IsActive = true;
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var files = await picker.PickMultipleFilesAsync();

            StorageFolder folderImages = await Package.Current.InstalledLocation.GetFolderAsync(GlobalFuncs.GetLocalFolder());
            string localFolder = GlobalFuncs.GetLocalFolder(true, true);
            

            if (files.Count > 0)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    double jq, Blur;
                    string[] NewName = GlobalFuncs.GetFormatAndNameFromString(files[i].Name);
                    string oldName = files[i].Name;

                    if (await folderImages.TryGetItemAsync(NewName[0]+NewName[1]) == null) // Если файл с таким именем не существует, то загружаем выбраный
                    {
                       
                        StorageFile file = files[i];
                        await file.CopyAsync(folderImages);

                        StorageFile newFile = await folderImages.GetFileAsync(file.Name);
                        await newFile.RenameAsync(imageId.ToString() + NewName[1]);

                       OpenCvFuncs of = new OpenCvFuncs(GlobalFuncs.GetLocalFolder(true, false) + imageId.ToString() + NewName[1]);
                        jq = of.GetJQ();
                        Blur = of.GetBlur(matCoeff);

                        listEvaluate.Add(new Evaluate(imageId, files[i].Name, String.Format("{0:0.000}", jq), String.Format("{0:0.000}", (Blur)), GlobalFuncs.GetLocalFolder(true,true)+newFile.Name));
                       
                    }
                    else
                    {
                        var file = await folderImages.GetFileAsync(files[i].Name);                       

                        OpenCvFuncs of = new OpenCvFuncs(GlobalFuncs.GetLocalFolder(true, false) + file.Name);
                        jq = of.GetJQ();
                        Blur = of.GetBlur(matCoeff);

                        listEvaluate.Add(new Evaluate(imageId, file.Name, String.Format("{0:0.000}", jq), String.Format("{0:0.000}", (Blur)), localFolder + file.Name));
                        await file.RenameAsync(oldName);
                    }
                   
                    listImageView.ItemsSource = listEvaluate.ToList();

                    SliderJQ.Minimum = GlobalFuncs.MaxMinValue(listEvaluate, GlobalFuncs.KindQuality.JQ, GlobalFuncs.KindQuality.Min);
                    SliderJQ.Minimum = GlobalFuncs.MaxMinValue(listEvaluate, GlobalFuncs.KindQuality.JQ, GlobalFuncs.KindQuality.Max);

                    SliderBlur.Minimum = GlobalFuncs.MaxMinValue(listEvaluate, GlobalFuncs.KindQuality.Blur, GlobalFuncs.KindQuality.Min);
                    SliderBlur.Maximum = GlobalFuncs.MaxMinValue(listEvaluate, GlobalFuncs.KindQuality.Blur, GlobalFuncs.KindQuality.Max);


                    // можно было обойтись без выделения памяти под переменную
                    // но это не удобно при отладке потому, что отладчик не позволит 
                    // залезть в инициализацию элемента List 
                    ImagesTable image = new ImagesTable();
                    image.id = imageId;
                    image.name = oldName;
                    image.format = NewName[1];
                    image.pathInport = files[i].Path;
                    image.pathLocal = localFolder + imageId.ToString() + NewName[1];
                    image.jq = jq;
                    image.blur = Blur;
                    image.pathExport = " ";
                    image.accepted = false;
                    image.exported = false;


                    listImages.Add(image);

                    imageId++;
                }
            }
            else
            {

            }
            listImageView.ItemsSource = listEvaluate.ToList();
            listImageView.ItemsSource = null;
            listImageView.ItemsSource = listEvaluate.ToList();
            GridAccepte.Visibility = Visibility.Visible;
        } // старая функция добавления группы изображений
        private async void AddFolder_Click(object sender, RoutedEventArgs e)
        { }//пустая заглушка

        private async void AddInternetLink_Click(object sender, RoutedEventArgs e)
        {
            InternetLinkDialog dlg = new InternetLinkDialog();
            ContentDialogResult result = await dlg.ShowAsync();
            
            if (result == ContentDialogResult.Primary)
            {

                string fileName = imageId.ToString() + ".jpg";               
            }         
            
            

           // GlobalFuncs.WriteableBitmapToStorageFile(Uri(url), imageId.ToString(), ".jpg"); 
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {            
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                openPicker.FileTypeFilter.Add(".jpg");
                openPicker.FileTypeFilter.Add(".jpeg");
                openPicker.FileTypeFilter.Add(".png");
                openPicker.FileTypeFilter.Add(".bmp");
                StorageFile file = await openPicker.PickSingleFileAsync();

                StorageFolder folderImages = await Package.Current.InstalledLocation.GetFolderAsync(GlobalFuncs.GetLocalFolder());
                string localFolder = GlobalFuncs.GetLocalFolder(true, true);

                if (file != null) // Если выбрали файл
                {

                    double jq, Blur;
                    string[] NewName = GlobalFuncs.GetFormatAndNameFromString(file.Name);
                    string oldName = file.Name;
                    await file.RenameAsync(imageId.ToString() + NewName[1]);

                    if (await folderImages.TryGetItemAsync(file.Name) == null) // Если файл с таким именем не существует, то загружаем выбраный
                    {
                        var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                        await file.CopyAsync(folderImages);
                        OpenCvFuncs of = new OpenCvFuncs(GlobalFuncs.GetLocalFolder(true, false) + file.Name);
                        jq = of.GetJQ();
                        Blur = of.GetBlur(matCoeff);

                        listEvaluate.Add(new Evaluate(imageId, file.Name, String.Format("{0:0.000}", jq), String.Format("{0:0.000}", (Blur)), localFolder + file.Name));
                    }
                    else
                    {
                        var dialog = new MessageDialog("Файл с таким именем существует!");
                        await dialog.ShowAsync();

                        file = await folderImages.GetFileAsync(file.Name);
                        var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

                        OpenCvFuncs of = new OpenCvFuncs(GlobalFuncs.GetLocalFolder(true, false) + file.Name);
                        jq = of.GetJQ();
                        Blur = of.GetBlur(matCoeff);

                        listEvaluate.Add(new Evaluate(imageId,file.Name, String.Format("{0:0.000}", jq), String.Format("{0:0.000}", (Blur)), localFolder + file.Name));
                    }
                    await file.RenameAsync(oldName);
                    listImageView.ItemsSource = listEvaluate.ToList();

                    SliderJQ.Minimum = GlobalFuncs.MaxMinValue(listEvaluate, GlobalFuncs.KindQuality.JQ, GlobalFuncs.KindQuality.Min);
                    SliderJQ.Maximum = GlobalFuncs.MaxMinValue(listEvaluate, GlobalFuncs.KindQuality.JQ, GlobalFuncs.KindQuality.Max);

                    SliderBlur.Minimum = GlobalFuncs.MaxMinValue(listEvaluate, GlobalFuncs.KindQuality.Blur, GlobalFuncs.KindQuality.Min);
                    SliderBlur.Maximum = GlobalFuncs.MaxMinValue(listEvaluate, GlobalFuncs.KindQuality.Blur, GlobalFuncs.KindQuality.Max);


                    // можно было обойтись без выделения памяти под переменную
                    // но это не удобно при отладке потому, что отладчик не позволит 
                    // залезть в инициализацию элемента List 
                    ImagesTable image = new ImagesTable();
                    image.id = imageId;
                    image.name = oldName;
                    image.format = NewName[1];
                    image.pathInport = file.Path;
                    image.pathLocal = localFolder + imageId.ToString() + NewName[1];
                    image.jq = jq;
                    image.blur = Blur;
                    image.pathExport = " ";
                    image.accepted = false;
                    image.exported = false;


                    listImages.Add(image);

                    imageId++;


                    GridAccepte.Visibility = Visibility.Visible;

                }
                else
                {
                    //  файл не выбран - ничего не делаем
                }
            }
        } // старая функция добавления 1 изображения

        private async Task AddFilesAnotherTread(object sender, RoutedEventArgs e) // актуальная функция добавления одного или группы изображений
        {
            progressRing.IsActive = true;
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var files = await picker.PickMultipleFilesAsync();

            StorageFolder folderImages = await Package.Current.InstalledLocation.GetFolderAsync(GlobalFuncs.GetLocalFolder());
            string localFolder = GlobalFuncs.GetLocalFolder(true, true);

            progressTb.Visibility = Visibility.Visible;
            if (files.Count > 0)
            {
                progressTb.Text = "Идет анализ... Оценено " + Convert.ToString(0) + " из " + Convert.ToString(files.Count);
                for (int i = 0; i < files.Count; i++)
                {


                    string[] NewName = GlobalFuncs.GetFormatAndNameFromString(files[i].Name);
                    NewName[0] = imageId.ToString();

                    if (await folderImages.TryGetItemAsync(NewName[0] + NewName[1]) == null) // Если файл с таким именем не существует, то загружаем выбраный
                    {
                        StorageFile file = files[i];
                        await file.CopyAsync(folderImages, NewName[0] + NewName[1]);
                    }
                    else
                    {
                        var file = await folderImages.GetFileAsync(files[i].Name);
                    }

                  
                       AddInfoToDB(imageId.ToString() + NewName[1], files[i].Name, NewName[1], files[i].Path, localFolder + NewName[0] + NewName[1]);

                       progressTb.Text = "Идет анализ... Оценено " + Convert.ToString(i + 1) + " из " + Convert.ToString(files.Count);
                   

                    imageId++;
                }
            }
            else
            {

            }

            listImageView.ItemsSource = listEvaluate.ToList();
            GridAccepte.Visibility = Visibility.Visible;

            progressRing.IsActive = false;
            progressTb.Visibility = Visibility.Collapsed;
        }

        private async void AddManyFiles_Click(object sender, RoutedEventArgs e) 
        {

            await AddFilesAnotherTread(sender,e);
            
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            CancelEvaluate();
            Frame.Navigate(typeof(MainPage));
        }//Отменить всю информацию и вернуться в главное меню

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            
            listImageView.ItemsSource = null;
            List<Evaluate> list = new List<Evaluate>();
            for (int i = 0; i < listEvaluate.Count; i++)
            {
                list.Add( new Evaluate(listEvaluate[i].id, listEvaluate[i].name, listEvaluate[i].JQ, listEvaluate[i].Blur, GlobalFuncs.GetLocalFolder(true, true) + listEvaluate[i].id.ToString() + GlobalFuncs.GetFormatAndNameFromString(listEvaluate[i].name)[1]));
            }
            listImageView.ItemsSource = listEvaluate.ToList();
            listEvaluate.Clear();
            listEvaluate = list;
        }// Обновить грид

        private async void CancelEvaluate()
        {
            StorageFolder folderImages = await Package.Current.InstalledLocation.GetFolderAsync(GlobalFuncs.GetLocalFolder());
            listEvaluate.Clear();
            listImageView.ItemsSource = listEvaluate;
            for (int i = 0; i < listImages.Count; i++)
            {
                StorageFile file = await folderImages.GetFileAsync(listImages[i].id.ToString() + listImages[i].format);
                try
                {
                    await file.DeleteAsync();
                }
                catch
                {
                    break;
                }
                
            }
            listImages.Clear();

            imageId = GlobalFuncs.FreeId(GlobalFuncs.KindTable.Images);
            historyId = GlobalFuncs.FreeId(GlobalFuncs.KindTable.History);
            GridAccepte.Visibility = Visibility.Collapsed;
        } // Отменить все изменения и удалить скопированные файлы

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            CancelEvaluate();
        } //Отменить изменения без перехода в главное меню

        private void ChangeMatrixCoeff(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CoeficientMatrixPage));
        }

        private async void AddFromCamera(object sender, RoutedEventArgs e)
        {
           

            if (SelectionImageSizePage.accept)
            {

                progressRing.IsActive = true;
                CameraCaptureUI captureUI = new CameraCaptureUI();
                captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
                captureUI.PhotoSettings.CroppedSizeInPixels = new Size(SelectionImageSizePage.sizeX, SelectionImageSizePage.sizeY);

                StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

                if (photo != null)
                {

                    StorageFolder destinationFolder = await Package.Current.InstalledLocation.GetFolderAsync(GlobalFuncs.GetLocalFolder());
                    string name = imageId.ToString() + ".jpg";
                    await photo.CopyAsync(destinationFolder, name);

                    AddInfoToDB(name, name, ".jpg", photo.Path, GlobalFuncs.GetLocalFolder(true) + name);

                    await photo.DeleteAsync();

                    imageId++;

                }

                listImageView.ItemsSource = listEvaluate.ToList();
                GridAccepte.Visibility = Visibility.Visible;

                progressRing.IsActive = false;
                progressTb.Visibility = Visibility.Collapsed;
            }
        }//Актуальная функция вызова камеры и считывания фото

        private void btAccept_Click(object sender, RoutedEventArgs e)
        {
            HistoryTable history = new HistoryTable();
            history.id = historyId;
            history.dateTime = DateTime.Now;
            history.maxBlur = SliderBlur.Maximum;
            history.minBlur = Convert.ToDouble(SliderBlur.Value.ToString());
            history.maxJQ = SliderJQ.Maximum;
            history.minJQ = SliderJQ.Minimum;
            AcceptHistoryPage.history = history;
            AcceptHistoryPage.imglist = listImages.ToList();

            Frame.Navigate(typeof(AcceptHistoryPage));

            listImages.Clear();
            listEvaluate.Clear();
            listImageView.ItemsSource = listEvaluate;
            imageId = GlobalFuncs.FreeId(GlobalFuncs.KindTable.Images);
            historyId = GlobalFuncs.FreeId(GlobalFuncs.KindTable.History);
            GridAccepte.Visibility = Visibility.Collapsed;
        }//Принять изменения и перейти в меню настройки сохранения

        private void AddInfoToDB(string nameNew, string nameOld, string format, string pathInport, string pathLocal)
        {
            OpenCvFuncs of = new OpenCvFuncs(GlobalFuncs.GetLocalFolder(true, false) + nameNew);
            double jq = of.GetJQ();
            double Blur = of.GetBlur(matCoeff);

            listEvaluate.Add(new Evaluate(imageId, nameOld, String.Format("{0:0.000}", jq), String.Format("{0:0.000}", (Blur)), GlobalFuncs.GetLocalFolder(true, true) + nameNew));
            listImageView.ItemsSource = listEvaluate.ToList();

            SliderJQ.Minimum = GlobalFuncs.MaxMinValue(listEvaluate, GlobalFuncs.KindQuality.JQ, GlobalFuncs.KindQuality.Min);
            SliderJQ.Maximum = GlobalFuncs.MaxMinValue(listEvaluate, GlobalFuncs.KindQuality.JQ, GlobalFuncs.KindQuality.Max);

            SliderBlur.Minimum = GlobalFuncs.MaxMinValue(listEvaluate, GlobalFuncs.KindQuality.Blur, GlobalFuncs.KindQuality.Min);
            SliderBlur.Maximum = GlobalFuncs.MaxMinValue(listEvaluate, GlobalFuncs.KindQuality.Blur, GlobalFuncs.KindQuality.Max);


            // можно было обойтись без выделения памяти под переменную
            // но это не удобно при отладке потому, что отладчик не позволит 
            // залезть в инициализацию элемента List 
            ImagesTable image = new ImagesTable();
            image.id = imageId;
            image.name = nameOld;
            image.format = format;
            image.pathInport = pathInport;
            image.pathLocal = pathLocal;
            image.jq = jq;
            image.blur = Blur;
            image.pathExport = " ";
            image.accepted = false;
            image.exported = false;


            listImages.Add(image);
        }//Анализ и добавление информации в базу данных
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Image_Quality_Analizer
{


    static public class GlobalFuncs // библиотека глобальных функций
    {
        // Типы для использования в функциях
        public enum KindQuality { Blur, JQ, Min, Max } // максимальное/минимальное значение
        public enum KindTable { Images, History } // первый свободный индекс
        public enum FileFormat { Jpg, Jpeg, Png, Bmp, Tiff, Gif, Error } // формат изображения
        private static BitmapImage image; // переменная для возвращения изображения из потока


        static public string GetLocalFolder(bool sleshEnd = false, bool appx = false)
        {
            if (appx)
                if (sleshEnd)
                    return "ms-appx:///Migrations//ImageGalery//";
                else
                    return "ms-appx:///Migrations//ImageGalery";
            else
                if (sleshEnd)
                return "Migrations//ImageGalery//";
            else
                return "Migrations\\ImageGalery";
        }//вернуть адрес локальной папки

        static public int MaxMinValue(List<Evaluate> listEvaluate, KindQuality q, KindQuality sravn)
        {
            double value = 0;

            switch (q)
            {
                case KindQuality.Blur:
                    {

                        value = Convert.ToDouble(listEvaluate[0].Blur);
                        switch (sravn)
                        {
                            case KindQuality.Max:
                                {
                                    for (int i = 0; i < listEvaluate.Count; i++)
                                        if (Convert.ToDouble(listEvaluate[i].Blur) > value) value = Convert.ToDouble(listEvaluate[i].Blur);
                                    break;
                                }
                            case KindQuality.Min:
                                {
                                    for (int i = 0; i < listEvaluate.Count; i++)
                                        if (Convert.ToDouble(listEvaluate[i].Blur) < value) value = Convert.ToDouble(listEvaluate[i].Blur);
                                    break;
                                }
                        }

                        break;
                    }
                case KindQuality.JQ:
                    {

                        value = Convert.ToDouble(listEvaluate[0].JQ);
                        switch (sravn)
                        {
                            case KindQuality.Max:
                                {
                                    for (int i = 0; i < listEvaluate.Count; i++)
                                        if (Convert.ToDouble(listEvaluate[i].JQ) > value) value = Convert.ToDouble(listEvaluate[i].JQ);
                                    break;
                                }
                            case KindQuality.Min:
                                {
                                    for (int i = 0; i < listEvaluate.Count; i++)
                                        if (Convert.ToDouble(listEvaluate[i].JQ) < value) value = Convert.ToDouble(listEvaluate[i].JQ);
                                    break;
                                }
                        }

                        break;
                    }
            }
            return Convert.ToInt32(value);
        }//посчитать максимальное/минимальное значение в списке

        static public int FreeId(KindTable table)
        {
            int id = 0, j;
            using (ImageContext db = new ImageContext())
            {
                switch (table)
                {
                    case KindTable.Images:
                        {
                            List<ImagesTable> imageList = db.Images.ToList();

                            if (imageList.Count != 0)
                            {
                                id = 0;
                                for (j = 0; j < imageList.Count; j++)
                                {
                                    if (imageList[j].id > id) id = imageList[j].id;
                                    //if (imageList[j].id == id) id++;
                                }
                            }
                            else return 1;
                            return id + 1;


                        }

                    case KindTable.History:
                        {
                            List<HistoryTable> history = db.History.ToList();
                            if (history.Count != 0)
                            {
                                id = 0;
                                for (j = 0; j < history.Count; j++)
                                {
                                    if (history[j].id > id) id = history[j].id;
                                    //if (history[j].id == id) id++;
                                }
                            }
                            else
                                return 1;
                            return id + 1;

                        }
                }

                return id;
            }
        }//Поиск свободного индекса

        static public string[] GetFormatAndNameFromString(string str)
        {

            string Name = str;
            string format = "";


            int i = Name.Length - 1;

            while (Name[i] != '.')
            {
                format = format.Substring(0, 0) + new string(Name[i], 1) + format.Substring(0); // index - индекс, с которого менять; repCount - количество звездочек
                Name = Name.Remove(i, 1);
                i--;
            }

            format = format.Substring(0, 0) + new string(Name[i], 1) + format.Substring(0);
            Name = Name.Remove(i, 1);
            string[] result = new string[] { Name, format };

            return result;
        }//Возвращает массив 2х строк 0 - имя, 1 - формат

        static public FileFormat GetFileFormatFromString(string formatStr)
        {
            string[] FormatLib = { ".jpg", ".png", ".jpeg", ".bmp", ".tiff", ".gif" };

            for (int i = 0; i < 6; i++)
            {
                if (string.Compare(formatStr, FormatLib[i], true) == 0)
                {

                    switch (i)
                    {
                        case 0:
                            return FileFormat.Jpg;
                        case 1:
                            return FileFormat.Png;
                        case 2:
                            return FileFormat.Jpeg;
                        case 3:
                            return FileFormat.Bmp;
                        case 4:
                            return FileFormat.Tiff;
                        case 5:
                            return FileFormat.Gif;

                    }
                }
            }

            return FileFormat.Error;
        } // вернуть числовой тип формата

        static public async Task<StorageFile> WriteableBitmapToStorageFile(WriteableBitmap WB, string FileName, GlobalFuncs.FileFormat fileFormat)
        {
            string fileName = FileName;
            Guid BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
            switch (fileFormat)
            {
                case GlobalFuncs.FileFormat.Jpeg:
                    FileName += ".jpeg";
                    BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
                    break;

                case GlobalFuncs.FileFormat.Png:
                    FileName += ".png";
                    BitmapEncoderGuid = BitmapEncoder.PngEncoderId;
                    break;

                case GlobalFuncs.FileFormat.Bmp:
                    FileName += ".bmp";
                    BitmapEncoderGuid = BitmapEncoder.BmpEncoderId;
                    break;

                case GlobalFuncs.FileFormat.Tiff:
                    FileName += ".tiff";
                    BitmapEncoderGuid = BitmapEncoder.TiffEncoderId;
                    break;

                case GlobalFuncs.FileFormat.Gif:
                    FileName += ".gif";
                    BitmapEncoderGuid = BitmapEncoder.GifEncoderId;
                    break;
            }

            var file = await Windows.Storage.ApplicationData.Current.TemporaryFolder.CreateFileAsync(FileName, CreationCollisionOption.GenerateUniqueName);
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoderGuid, stream);
                Stream pixelStream = WB.PixelBuffer.AsStream();
                byte[] pixels = new byte[pixelStream.Length];
                await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                                    (uint)WB.PixelWidth,
                                    (uint)WB.PixelHeight,
                                    96.0,
                                    96.0,
                                    pixels);
                await encoder.FlushAsync();
            }
            return file;
        }// конвертировать WriteableBitmap в StorageFile

        static async void CreateImageStream(string path)
        {
            RandomAccessStreamReference rs = RandomAccessStreamReference.CreateFromUri(new Uri(path));
            BitmapImage bi = new BitmapImage();
            var rstream = await rs.OpenReadAsync();
            bi.SetSource(rstream);
            image = bi;
        }//Загрузка в поток изображения без привязки файла к процессу

        static public BitmapImage getBirmapImageFromPath(string path)
        {
           CreateImageStream(path);
           return image;
        }//Вернуть изображение свободное от файла

        static public async void DeleteImage(string name)
        {
            StorageFolder folderImages = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(GlobalFuncs.GetLocalFolder());

            if (await folderImages.TryGetItemAsync(name) != null)
            {
                StorageFile file = await folderImages.GetFileAsync(name);
                try
                {
                    await file.DeleteAsync();
                }
                catch
                {
                    var dialog = new Windows.UI.Popups.MessageDialog("Не удается получить доступ к файлу из за отсутствия прав или файл занят другим процессом.");
                    await dialog.ShowAsync();
                }
            }
            return;
        } // Удаляет изображение из галереи

        static public async void ExportImage(string name, StorageFolder pathExport, string nameExport)
        {
            StorageFolder folderImages = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(GlobalFuncs.GetLocalFolder());
            if (await folderImages.TryGetItemAsync(name) != null)
            {
                StorageFile file = await folderImages.GetFileAsync(name);

                await file.CopyAsync(pathExport, nameExport);
            }
        }//Экпортирует изображение из галереи     

        static public List<Evaluate> LocateFromTextBox(TextBox tb, List<Evaluate> inputList)
        {
            List<Evaluate> FiltredList = new List<Evaluate>();

            if (tb.Text != "")
            {
                for (int i = 0; i < inputList.Count; i++)
                {
                    if (inputList[i].name.IndexOf(tb.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        FiltredList.Add(inputList[i]);
                    }
                }
            }
            else return inputList;

            return FiltredList;
        }


    }

   

    public class Evaluate // Класс строки в listview
    {
        public int id { get; set; }
        public string name { get; set; }
        public string JQ { get; set; }
        public string Blur { get; set; }
        public bool Check { get; set; }
        public BitmapImage image { get; set; }
        public string path { get; set; }

        private  async Task GetImage(string path)
        {
            
            try
            {
                RandomAccessStreamReference rs = RandomAccessStreamReference.CreateFromUri(new Uri(path));
                BitmapImage bi = new BitmapImage();

                var rstream = await rs.OpenReadAsync();
                bi.SetSource(rstream);
                image = bi;
            }
            catch { image = null; }
        }

        public Evaluate(int _id, string _name, string jq, string blur, string _path)
        {
            name = _name;
            id = _id;
            JQ = jq;
            Blur = blur;
            path = _path;
            GetImage(path);
            
        }

        ~Evaluate()
        {
            image = null;
        }

    }


    public class ImagesTableView : ImagesTable
    {
        
        public BitmapImage image { get; set; }
        

        private async void GetImage(string path)
        {           

            try
            {
                RandomAccessStreamReference rs = RandomAccessStreamReference.CreateFromUri(new Uri(path));
                BitmapImage bi = new BitmapImage();
                var rstream = await rs.OpenReadAsync();
                bi.SetSource(rstream);
                image = bi;
            }
            catch
            {
                image = null;
            }
          
        }

        public void ChangeImage(string _path)
        {
            GetImage(_path);
        }

        public ImagesTableView()
        {
            id = 0;
            name = null;
            format = null;
            pathInport = null;
            pathExport = null;
            pathLocal = null;
            jq = 0;
            blur = 0;
            accepted = false;
            exported = false;
            historyId = 0;
        }

        public ImagesTableView(ImagesTable it)
        {
            id = it.id;
            name = it.name;
            format = it.format;
            pathInport = it.pathInport;
            pathExport = it.pathExport;
            pathLocal = it.pathLocal;
            jq = it.jq;
            blur = it.blur;
            accepted = it.accepted;
            exported = it.exported;
            historyId = it.historyId;

            GetImage(pathLocal);
        }


    } // Класс наследник ImageTable, с добавлением поля BitmapImage для привязки к обьектам интерфейса
}

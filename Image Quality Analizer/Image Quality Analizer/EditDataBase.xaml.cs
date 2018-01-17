using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace Image_Quality_Analizer
{

    public sealed partial class EditDataBase : Page
    {
        public EditDataBase()
        {
            this.InitializeComponent();
            this.Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {


            using (ImageContext db = new ImageContext())
            {
                historyList.ItemsSource = db.History.ToList();
                tbCount.Text = "Общее количество оценок изображений : " + db.Images.ToList().Count.ToString();
            }
        }



        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EditHistory));
        }
        private void Image_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DataBaseImages));
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // получаем выделеный пункт меню
            if (historyList.SelectedItem != null)
            {
                HistoryTable history = historyList.SelectedItem as HistoryTable;
                if (history != null)
                    Frame.Navigate(typeof(EditHistory), history.id);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // получаем выделеный пункт меню
            if (historyList.SelectedItem != null)
            {
                HistoryTable history = historyList.SelectedItem as HistoryTable;
                if (history != null)
                {
                    using (ImageContext db = new ImageContext())
                    {

                        db.History.Remove(history);

                        db.SaveChanges();
                        historyList.ItemsSource = db.History.ToList();
                    }
                }
            }
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void View_Click(object sender, RoutedEventArgs e)
        {
            // получаем выделеный пункт меню
            if (historyList.SelectedItem != null)
            {
                HistoryTable history = historyList.SelectedItem as HistoryTable;
                if (history != null)
                {
                    //VIewHistoryInformationFromDB.history = history;
                    Frame.Navigate(typeof(VIewHistoryInformationFromDB), history);
                }
            }


        }
    }
}

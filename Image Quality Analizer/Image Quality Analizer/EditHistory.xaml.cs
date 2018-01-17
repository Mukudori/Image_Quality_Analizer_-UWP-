using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Image_Quality_Analizer
{

    public sealed partial class EditHistory : Page
    {
        HistoryTable history;
       
        public EditHistory()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                int id = (int)e.Parameter;
                using (ImageContext db = new ImageContext())
                {
                    history = db.History.FirstOrDefault(c => c.id == id);
                }
            }

            if (history != null)
            {
                headerBlock.Text = "Редактирование истории";
                Date.Date = history.dateTime;
                Time.Time = history.dateTime.TimeOfDay;
                //id.Text = history.id.ToString();
                //CopyToGalery.IsChecked = history.copyToGalery;


                maxBlur.Text = history.maxBlur.ToString();
                minBlur.Text = history.minBlur.ToString();
                maxJQ.Text = history.maxJQ.ToString();
                minJQ.Text = history.minJQ.ToString();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            using (ImageContext db = new ImageContext())
            {
                if (history != null)
                {
                    history.dateTime = Date.Date.DateTime;
                    //history.id = Convert.ToInt16(id.Text);
                    history.copyToGalery = copyToGalery.IsChecked.Value;
                    history.minJQ = Convert.ToInt16(minJQ.Text);
                    history.maxJQ = Convert.ToInt16(maxJQ.Text);
                    history.minBlur = Convert.ToInt16(minBlur.Text);
                    history.maxBlur = Convert.ToInt16(maxBlur.Text);

                    db.History.Update(history);
                }
                else
                {
                    db.History.Add(new HistoryTable
                    {

                        dateTime = Date.Date.DateTime,
                        //id = Convert.ToInt16(id.Text),
                        minJQ = Convert.ToDouble(minJQ.Text),
                        maxJQ = Convert.ToDouble(maxJQ.Text),
                        minBlur = Convert.ToDouble(minBlur.Text),
                        maxBlur = Convert.ToDouble(maxBlur.Text)
                    });
                }
                db.SaveChanges();
            }
            GoToMainPage();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            GoToMainPage();
        }

        private void GoToMainPage()
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
            else
                Frame.Navigate(typeof(MainPage));
        }
    }
}

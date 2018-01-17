using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Image_Quality_Analizer
{
    public sealed partial class CoeficientMatrixPage : Page
    {
        public CoeficientMatrixPage()
        {
            this.InitializeComponent();
            LoadMatrix();
        }

        private void LoadMatrix()
        {
            tb00.Text = QualityAnalizingPage.matCoeff[0].ToString();
            tb01.Text = QualityAnalizingPage.matCoeff[1].ToString();
            tb02.Text = QualityAnalizingPage.matCoeff[2].ToString();

            tb10.Text = QualityAnalizingPage.matCoeff[3].ToString();
            tb11.Text = QualityAnalizingPage.matCoeff[4].ToString();
            tb12.Text = QualityAnalizingPage.matCoeff[5].ToString();

            tb20.Text = QualityAnalizingPage.matCoeff[6].ToString();
            tb21.Text = QualityAnalizingPage.matCoeff[7].ToString();
            tb22.Text = QualityAnalizingPage.matCoeff[8].ToString();

        }

        private void AcceptMatrixClick(object sender, RoutedEventArgs e)
        {
            QualityAnalizingPage.Acceptmatrix = true;

            QualityAnalizingPage.matCoeff[0] = Convert.ToDouble(tb00.Text);
            QualityAnalizingPage.matCoeff[1] = Convert.ToDouble(tb01.Text);
            QualityAnalizingPage.matCoeff[2] = Convert.ToDouble(tb02.Text);

            QualityAnalizingPage.matCoeff[3] = Convert.ToDouble(tb10.Text);
            QualityAnalizingPage.matCoeff[4] = Convert.ToDouble(tb11.Text);
            QualityAnalizingPage.matCoeff[5] = Convert.ToDouble(tb12.Text);

            QualityAnalizingPage.matCoeff[6] = Convert.ToDouble(tb20.Text);
            QualityAnalizingPage.matCoeff[7] = Convert.ToDouble(tb21.Text);
            QualityAnalizingPage.matCoeff[8] = Convert.ToDouble(tb22.Text);

            Frame.Navigate(typeof(QualityAnalizingPage));
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            QualityAnalizingPage.Acceptmatrix = false;
            Frame.Navigate(typeof(MainPage));
        }
    }
}

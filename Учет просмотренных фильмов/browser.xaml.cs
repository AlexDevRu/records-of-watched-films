using System;
using System.Windows;

namespace Учет_просмотренных_фильмов
{
    /// <summary>
    /// Логика взаимодействия для browser.xaml
    /// </summary>
    public partial class browser : Window
    {
        public browser(string link)
        {
            InitializeComponent();
            web.Source = new Uri(link);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            string url = web.Source.ToString();
            int i = url.IndexOf("imgrc=");
            string img = url.Substring(i + 6);
            string link = "https://encrypted-tbn0.gstatic.com/images?q=tbn:" + img;
            (Owner as edit).new_poster(link);
            Close();
        }
    }
}

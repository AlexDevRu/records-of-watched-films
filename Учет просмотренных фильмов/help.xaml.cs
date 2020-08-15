using System;
using System.Windows;

namespace Учет_просмотренных_фильмов
{
    /// <summary>
    /// Логика взаимодействия для help.xaml
    /// </summary>
    public partial class help : Window
    {
        public help()
        {
            InitializeComponent();
            web.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + "help/index.html");
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Учет_просмотренных_фильмов
{
    /// <summary>
    /// Логика взаимодействия для panel_film.xaml
    /// </summary>
    public partial class panel_film : UserControl
    {
        public panel_film()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty NameFilmDependency = DependencyProperty.Register("NameFilm", typeof(string), typeof(panel_film));


        public string NameFilm
        {
            get
            {
                return (string)GetValue(NameFilmDependency);
            }
            set
            {
                SetValue(NameFilmDependency, value);
            }
        }

        public int Year
        {
            get { return (int)GetValue(YearProperty); }
            set { SetValue(YearProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Year.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YearProperty =
            DependencyProperty.Register("Year", typeof(int), typeof(panel_film), new PropertyMetadata(0));

        public int Ozenka
        {
            get { return (int)GetValue(OzenkaProperty); }
            set { SetValue(OzenkaProperty, value); set_ozenka(value); }
        }

        // Using a DependencyProperty as the backing store for Ozenka.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OzenkaProperty =
            DependencyProperty.Register("Ozenka", typeof(int), typeof(panel_film), new PropertyMetadata(0));


        private void set_ozenka(int o)
        {
            ozenka.Children.Clear();
            for (int k = 0; k < o; k++)
            {
                Polygon z = DataBase.draw_star(15,12.5,Brushes.Yellow);
                ozenka.Children.Add(z);
            }
        }

        public string Genres
        {
            get { return (string)GetValue(GenresProperty); }
            set { SetValue(GenresProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Genres.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GenresProperty =
            DependencyProperty.Register("Genres", typeof(string), typeof(panel_film), new PropertyMetadata("0"));

        public int Views
        {
            get { return (int)GetValue(ViewsProperty); }
            set { SetValue(ViewsProperty, value);text_views = "Просмотров: " + value; }
        }

        // Using a DependencyProperty as the backing store for Views.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewsProperty =
            DependencyProperty.Register("Views", typeof(int), typeof(panel_film), new PropertyMetadata(0));

        string text_views
        {
            set { views.Text = value; }
        }

        public ImageSource Poster
        {
            get { return (ImageSource)GetValue(PosterProperty); }
            set { SetValue(PosterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Poster.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PosterProperty =
            DependencyProperty.Register("Poster", typeof(ImageSource), typeof(panel_film), new PropertyMetadata(null));
    }
}

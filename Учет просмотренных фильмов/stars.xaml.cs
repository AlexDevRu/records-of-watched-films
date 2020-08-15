using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Учет_просмотренных_фильмов
{
    /// <summary>
    /// Логика взаимодействия для stars.xaml
    /// </summary>
    public partial class stars : UserControl
    {
        public stars()
        {
            InitializeComponent();
            Available = true;
        }

        public bool Available
        {
            get { return (bool)GetValue(AvailableProperty); }
            set { SetValue(AvailableProperty, value); set_condition(); }
        }

        public static readonly DependencyProperty AvailableProperty =
            DependencyProperty.Register("Available", typeof(bool), typeof(stars), new PropertyMetadata(true));

        public double S_Width
        {
            get { return (double)GetValue(S_WidthProperty); }
            set { SetValue(S_WidthProperty, value); }
        }

        public static readonly DependencyProperty S_WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(stars), new PropertyMetadata(30.0));

        public double S_Height
        {
            get { return (double)GetValue(S_HeightProperty); }
            set { SetValue(S_HeightProperty, value); }
        }

        public static readonly DependencyProperty S_HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(stars), new PropertyMetadata(25.0));

        private void set_condition()
        {
            st_ozenka.Children.Clear();
            for (int i = 0; i < 5; i++)
            {
                if (i < Ozenka) st_ozenka.Children.Add(DataBase.draw_star(S_Width, S_Height, Brushes.Yellow));
                else st_ozenka.Children.Add(DataBase.draw_star(S_Width, S_Height, Brushes.Gray));
            }
            if (Available)
            {
                for (int k = 0; k < 5; k++)
                {
                    Polygon star = st_ozenka.Children[k] as Polygon;
                    star.MouseEnter += S_MouseEnter;
                    star.MouseLeave += S_MouseLeave;
                    star.MouseDown += S_Clicked;
                }
                st_ozenka.MouseLeave += StackPanel_MouseLeave;
            }
            else
            {
                st_ozenka.MouseLeave -= StackPanel_MouseLeave;
                for (int i = 0; i < 5; i++)
                {
                    Polygon star = st_ozenka.Children[i] as Polygon;
                    star.MouseEnter -= S_MouseEnter;
                    star.MouseLeave -= S_MouseLeave;
                    star.MouseDown -= S_Clicked;
                }
            }
        }

        public void reset()
        {
            st_ozenka.Children.Clear();
            for (int i = 0; i < 5; i++)
            {
                st_ozenka.Children.Add(DataBase.draw_star(S_Width, S_Height, Brushes.Gray));
            }
            for (int k = 0; k < 5; k++)
            {
                Polygon star = st_ozenka.Children[k] as Polygon;
                star.MouseEnter += S_MouseEnter;
                star.MouseLeave += S_MouseLeave;
                star.MouseDown += S_Clicked;
            }
            ozenka = 0;
        }

        private void S_MouseEnter(object sender, MouseEventArgs e)
        {
            Polygon p = (sender as Polygon);
            bool color = true;
            for (int i = 0; i < 5; i++)
            {
                Polygon star = (st_ozenka.Children[i] as Polygon);
                if (color)
                    star.Fill = Brushes.Yellow;
                else
                    star.Fill = Brushes.Gray;
                if (star == p) { star.Fill = Brushes.Yellow; color = false; }
            }
        }

        public int ozenka = 0;

        public int Ozenka
        {
            get { if (!Available) return (int)GetValue(OzenkaProperty); else return ozenka; }
            set
            {
                SetValue(OzenkaProperty, value);
                /*if(!Available) */set_condition();
            }
        }

        // Using a DependencyProperty as the backing store for Ozenka.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OzenkaProperty =
            DependencyProperty.Register("Ozenka", typeof(int), typeof(stars), new PropertyMetadata(0));


        private void S_MouseLeave(object sender, MouseEventArgs e)
        {
            Polygon p = (sender as Polygon);
            p.Fill = Brushes.Gray;
        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            IEnumerable<Polygon> c = st_ozenka.Children.OfType<Polygon>();
            if (ozenka != 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    Polygon star = (st_ozenka.Children[i] as Polygon);
                    if (i < ozenka)
                        star.Fill = Brushes.Yellow;
                    else
                        star.Fill = Brushes.Gray;
                }
            }
            else foreach (var i in c) i.Fill = Brushes.Gray;
        }

        private void S_Clicked(object sender, MouseButtonEventArgs e)
        {
            Polygon p = (sender as Polygon);
            for (int i = 0; i < 5; i++)
                if (st_ozenka.Children[i] == p) { ozenka = i + 1; break; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Data;

namespace Учет_просмотренных_фильмов
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<film> films = new List<film>();

        private film get_film_by_id(int id)
        {
            foreach(var film in films)
            {
                if (film.get_id() == id) return film;
            }
            return null;
        }

        private film get_film_by_name(string name)
        {
            foreach (var film in films)
            {
                if (film.get_name() == name) return film;
            }
            return null;
        }

        public void insert_film_and_add(int id)
        {
            select_filter("select Год from Фильмы group by Год", ref years_filter);
            select_filter("select Жанр from Жанры", ref genres_filter);
            film f = new film(id);
            films.Add(f);
            checked_sort(null,null);
        }

        public void update_views(int id)
        {
            film film = get_film_by_id(id);
            film.update_views();
            panel_film pf = (active.Parent as Border).Parent as panel_film;
            pf.Views = film.get_views();
            if(film.get_sr_ozenka()!=pf.Ozenka)
            {
                pf.Ozenka = film.get_sr_ozenka();
            }   
        }

        public void update_film_info(film f,BitmapImage poster)
        {
            panel_film pf = (active.Parent as Border).Parent as panel_film;
            pf.NameFilm = f.get_name();
            pf.Year = f.get_year();
            List<string> genres = f.get_genres();
            string gnr = "";
            for (int i = 0; i < genres.Count; i++)
            {
                gnr += genres[i];
                if (i != genres.Count - 1) gnr += ", ";
            }
            pf.Genres = gnr;
            if (poster != null) pf.Poster = poster;
        }

        private void add_film(film f)
        {
            panel_film film = new panel_film();
            film.Poster = f.get_poster();
            film.NameFilm= f.get_name();
            film.Year = f.get_year();
            film.Ozenka = f.get_sr_ozenka();
            List<string> m_genres = f.get_genres();
            string genres="";
            for (int g = 0; g < m_genres.Count; g++)
            {
                genres += m_genres[g];
                if (g != m_genres.Count - 1) genres += ", ";
            }
            film.Genres = genres;
            film.Views = f.get_views();
            film.edit.Click += open_edit_film;
            film.stat.Click += stat_film;
            film.close.Click += delete_film;
            film.MouseDown += dbl_click_film;
            wrp.Children.Add(film);
        }

        private void delete_film(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result=MessageBox.Show("Удалить фильм из БД?","Сообщение",MessageBoxButton.YesNo,MessageBoxImage.Warning);
            if(result==MessageBoxResult.Yes)
            {
                string name = ((((sender as Button).Parent as Grid).Children[1] as StackPanel).Children[0] as TextBlock).Text;
                string sql = "delete from Фильмы where Название='" + name + "'";
                DataBase.exec_zapros(sql);
                films.Remove(get_film_by_name(name));
                wrp.Children.Remove(get_film(name));
            }
        }

        private void open_edit_film(object sender, RoutedEventArgs e)
        {
            Grid grid = (sender as Button).Parent as Grid;
            active = grid;
            string name = ((grid.Parent as Border).Parent as panel_film).name.Text;
            film film = get_film_by_name(name);
            edit edit = new edit(film);
            edit.Owner = this;
            edit.ShowDialog();
        }

        private void stat_film(object sender, RoutedEventArgs e)
        {
            Grid grid = (sender as Button).Parent as Grid;
            active = grid;
            string name = ((grid.Parent as Border).Parent as panel_film).name.Text;
            film film = get_film_by_name(name);
            stat stat = new stat(film,films);
            stat.Owner = this;
            stat.ShowDialog();
        }

        private panel_film get_film(string name)
        {
            foreach(panel_film b in wrp.Children)
            {
                if (b.name.Text == name) return b;
            }
            return null;
        }

        private List<film> select_films_all(string sql= "select [id фильма] from Фильмы order by Название asc")
        {
            DataTable table = DataBase.exec_zapros(sql);
            List<film> list_film = new List<film>();
            for (int i = 0; i < table.Rows.Count; i++) list_film.Add(new film(Convert.ToInt32(table.Rows[i][0])));
            return list_film;
        }

        private void add_films(List<film> l)
        {
            for (int i = 0; i < l.Count; i++)
            {
                add_film(l[i]);
            }
        }  
        
        Grid active = null;

        private void dbl_click_film(object sender, MouseEventArgs e)
        {
            panel_film pf = sender as panel_film;
            active = pf.grid;
            string name=pf.name.Text;
            int id = get_film_by_name(name).get_id();
            views v = new views(id);
            v.Owner = this;
            v.ShowDialog();
        }

        private void select_filter(string sql,ref ComboBox filter)
        {
            filter.Items.Clear();
            DataTable filter_table = DataBase.exec_zapros(sql);
            for (int i = 0; i < filter_table.Rows.Count; i++) filter.Items.Add(filter_table.Rows[i][0].ToString());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            select_filter("select Год from Фильмы group by Год", ref years_filter);
            select_filter("select Жанр from Жанры", ref genres_filter);
            films=select_films_all();
            asc_name.IsChecked = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            addfilm af = new addfilm();
            af.Owner = this;
            af.ShowDialog();
        }

        private void TextBox_LostMouseCapture(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (t.Text == "")
            {
                t.Text = "Поиск...";
                t.FontStyle = FontStyles.Italic;
            }
            wrp.Children.Clear();
            add_films(films);
        }

        private void TextBox_GotMouseCapture(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (t.Text == "Поиск...")
            {
                t.Text = "";
                t.FontStyle = FontStyles.Normal;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            wrp.Children.Clear();
            List<film> films_filter = new List<film>();
            List<film> films_all = select_films_all();
            foreach (var film in films_all)
            {
                bool condition = true;
                if (ozenka_filter.Text != "") condition = film.get_sr_ozenka() == Convert.ToInt32(ozenka_filter.Text);
                if (ot.Text != "") condition = condition && film.get_views() >= Convert.ToInt32(ot.Text);
                if (_do.Text != "") condition = condition && film.get_views() <= Convert.ToInt32(_do.Text);
                if (years_filter.Text != "") condition = condition && film.get_year() == Convert.ToInt32(years_filter.Text);
                if (genres_filter.Text != "") condition = condition && film.get_genres().Contains(genres_filter.Text);
                if (condition) films_filter.Add(film);
            }
            films = films_filter;
            add_films(films);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ozenka_filter.Text = "";
            ot.Text = "1";
            _do.Text = "1";
            years_filter.Text = "";
            genres_filter.Text = "";
            wrp.Children.Clear();
            films=select_films_all();
            checked_sort(null, null);
        }

        private void Search_KeyDown(object sender, KeyEventArgs e)
        {
            wrp.Children.Clear();
            List<film> films_filter = new List<film>();
            foreach (var film in films)
            {
                if(film.get_name().ToLower().Contains(search.Text.ToLower()))
                    films_filter.Add(film);
            }
            add_films(films_filter);
        }

        private void open_about(object sender, RoutedEventArgs e)
        {
            about about = new about();
            about.Owner = this;
            about.ShowDialog();
        }

        private void checked_sort(object sender, RoutedEventArgs e)
        {
            if(asc_name.IsChecked ?? false)
            {
                var sorted_films = from film in films orderby film.get_name() ascending select film;
                films = sorted_films.ToList();
            }
            else if(desc_name.IsChecked ?? false)
            {
                var sorted_films = from film in films orderby film.get_name() descending select film;
                films = sorted_films.ToList();
            }
            else if (asc_ozenka.IsChecked ?? false)
            {
                var sorted_films = from film in films orderby film.get_sr_ozenka() ascending select film;
                films = sorted_films.ToList();
            }
            else
            {
                var sorted_films = from film in films orderby film.get_sr_ozenka() descending select film;
                films = sorted_films.ToList();
            }
            wrp.Children.Clear();
            add_films(films);
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            help help = new help();
            help.Owner = this;
            help.ShowDialog();
        }
    }
}

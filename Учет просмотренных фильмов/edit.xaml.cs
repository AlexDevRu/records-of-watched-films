using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Data;

namespace Учет_просмотренных_фильмов
{
    /// <summary>
    /// Логика взаимодействия для edit.xaml
    /// </summary>
    /// 
    public partial class edit : Window
    {
        List<item> Items = new List<item>();
        film film;
        List<string> genres_in_db = new List<string>();
        BitmapImage picture = null;

        public edit(film f)
        {
            InitializeComponent();
            film=f;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            name.Text = film.get_name();
            year.Text = film.get_year().ToString();
            pic.Source = film.get_poster();
            DataTable t_janres = DataBase.exec_zapros("select Жанр from Жанры");
            for (int i = 0; i < t_janres.Rows.Count; i++)
            {
                genres_in_db.Add(t_janres.Rows[i][0].ToString());
                if(film.get_genres().Contains(t_janres.Rows[i][0].ToString()))
                    Items.Add(new item { Name = t_janres.Rows[i][0].ToString(), Isselected = true });
                else
                    Items.Add(new item { Name = t_janres.Rows[i][0].ToString(), Isselected = false });
            }
            janres.ItemsSource = Items;
        }

        private void TextBox_GotMouseCapture(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (t.Text == "Название" || t.Text == "Год" || t.Text == "Жанр")
            {
                t.Text = "";
                t.FontStyle = FontStyles.Normal;
            }
        }

        private void TextBox_LostMouseCapture(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (t.Text == "")
            {
                t.Text = t.Name == "name" ? "Название" : t.Name == "year" ? "Год" : "Жанр";
                t.FontStyle = FontStyles.Italic;
            }
        }

        private void add_genre(object sender, RoutedEventArgs e)
        {
            if (janr.Text != "Жанр")
            {
                Items.Add(new item { Name = janr.Text, Isselected = false });
                janres.Items.Refresh();
            }
            else
                MessageBox.Show("Не удалось добавить жанр", "Ошибка");
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            List<parameter> parameters = new List<parameter>();
            string upd = "update Фильмы set Название=@name,Год=@year where [id фильма]=@id";
            parameters.Add(new parameter { name="@name",value=name.Text });
            parameters.Add(new parameter { name = "@year", value = Convert.ToInt32(year.Text) });

            if (picture != null)
            {
                byte[] data = DataBase.get_bytes_from_image(picture);
                upd = "update Фильмы set Название=@name,Год=@year,Постер=@poster where [id фильма]=@id";
                parameters.Add(new parameter { name = "@poster", value = data });
            } 
            parameters.Add(new parameter { name = "@id", value = film.get_id() });
            DataBase.exec_zapros(upd,parameters);
            
            foreach (var item in Items)
            {
                if(item.Isselected&&!film.get_genres().Contains(item.Name))
                {
                    parameters.Clear();
                    parameters.Add(new parameter { name = "@janr", value = item.Name });
                    parameters.Add(new parameter { name = "@id", value = film.get_id() });
                    DataBase.exec_proc("insert_janr",parameters);
                }
                if (!item.Isselected && film.get_genres().Contains(item.Name))
                {
                    DataTable t = DataBase.exec_zapros("select [id жанра] from Жанры where Жанр='"+ item.Name + "'");
                    int id = Convert.ToInt32(t.Rows[0][0]);
                    parameters.Clear();
                    parameters.Add(new parameter { name = "@janr", value = id });
                    parameters.Add(new parameter { name = "@id", value = film.get_id() });
                    DataBase.exec_zapros("delete from Жанры_Фильмы where [id фильма]=@id and [id жанра]=@janr",parameters);
                }
            }
            film.update_all();
            var mw = this.Owner as MainWindow;
            mw.update_film_info(film,picture);
        }

        private void Poster_Click(object sender, RoutedEventArgs e)
        {
            browser br = new browser("http://google.ru/images?q=" + (film.get_name()+"+фильм").Replace(' ','+'));
            br.Owner = this;
            br.ShowDialog();
        }

        public void new_poster(string link)
        {
            BitmapImage bitmapImage = DataBase.get_image_by_link(link);
            pic.Source = bitmapImage;
            picture = bitmapImage;
        }
    }
}

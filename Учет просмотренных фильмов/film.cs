using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Data;

namespace Учет_просмотренных_фильмов
{
    public class film
    {
        int id;
        string name;
        int year;
        int views;
        int sr_ozenka;
        List<string> genres;
        public film(int id)
        {
            this.id = id;
            name = db_get_name();
            year = db_get_year();
            views = db_get_views();
            sr_ozenka = db_get_sr_ozenka();
            genres = db_get_genres();
        }
        public string get_name()
        {
            return name;
        }
        public int get_year()
        {
            return year;
        }
        public int get_views()
        {
            return views;
        }
        public List<string> get_genres()
        {
            return genres;
        }
        public int get_id() { return id; }

        public int get_sr_ozenka()
        {
            return sr_ozenka;
        }

        public ImageSource get_poster()
        {
            string sql = "select Постер from Фильмы where [id фильма]=" + id;
            DataTable poster = DataBase.exec_zapros(sql);
            return new ImageSourceConverter().ConvertFrom(poster.Rows[0][0]) as ImageSource;
        }

        public void update_all()
        {
            name = db_get_name();
            year = db_get_year();
            genres = db_get_genres();
        }

        public void update_views()
        {
            sr_ozenka = db_get_sr_ozenka();
            views = db_get_views();
        }

        private int db_get_views()
        {
            string sql = "select COUNT(Оценка) from Просмотры where [id фильма]=" + id;
            DataTable views = DataBase.exec_zapros(sql);
            return Convert.ToInt32(views.Rows[0][0]);
        }
        private List<string> db_get_genres()
        {
            string sql = "select Жанры.Жанр from Жанры_Фильмы, Жанры where Жанры_Фильмы.[id фильма]=" + id + " and Жанры_Фильмы.[id жанра]= Жанры.[id жанра]";
            DataTable janres = DataBase.exec_zapros(sql);
            List<string> genres = new List<string>();
            for (int i = 0; i < janres.Rows.Count; i++) genres.Add(janres.Rows[i][0].ToString());
            return genres;
        }
        private string db_get_name()
        {
            string sql = "select Название from Фильмы where [id фильма]=" + id;
            DataTable name = DataBase.exec_zapros(sql);
            return name.Rows[0][0].ToString();
        }
        private int db_get_year()
        {
            string sql = "select Год from Фильмы where [id фильма]=" + id;
            DataTable year = DataBase.exec_zapros(sql);
            return Convert.ToInt32(year.Rows[0][0]);
        }
        private int db_get_sr_ozenka()
        {
            string sql = "select AVG(Оценка) from Просмотры where [id фильма]=" + id;
            DataTable avg = DataBase.exec_zapros(sql);
            return Convert.ToInt32(avg.Rows[0][0]);
        }
    }
}

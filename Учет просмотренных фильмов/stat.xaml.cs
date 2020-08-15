using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms.DataVisualization.Charting;
using System.Data;

namespace Учет_просмотренных_фильмов
{
    /// <summary>
    /// Логика взаимодействия для stat.xaml
    /// </summary>
    public partial class stat : Window
    {
        film film;
        List<film> list;

        class value
        {
            public string X { get; set; }
            public int Y { get; set; }
            public bool this_film { get; set; }
        }

        public stat(film f,List<film> list)
        {
            InitializeComponent();
            film = f;
            this.list = list;
        }

        private void draw_diagram(ref Chart chart,List<value> d)
        {
            chart.ChartAreas.Add("area");
            chart.ChartAreas[0].AxisY.Interval = 1;
            chart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            chart.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
            chart.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chart.Series.Add("series1");
            chart.Series["series1"].Color = System.Drawing.Color.Blue;
            for (int i = 0; i < d.Count; i++)
            {
                chart.Series["series1"].ChartType = SeriesChartType.Column;
                chart.Series["series1"].Points.AddXY(d[i].X, d[i].Y);
                chart.Series["series1"].Points.Last().Label = d[i].Y.ToString();
                if (d[i].this_film) chart.Series["series1"].Points.Last().Color = System.Drawing.Color.Red;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<value> rating_ = new List<value>();
            List<value> genre = new List<value>();
            List<value> year_ = new List<value>();
            List<value> views_ = new List<value>();
            DataTable genres_count = DataBase.exec_zapros("select Жанры.Жанр,count(Жанры_Фильмы.[id жанра]) from Жанры_Фильмы inner join Жанры on Жанры.[id жанра]=Жанры_Фильмы.[id жанра] group by Жанр");
            DataTable years_count = DataBase.exec_zapros("select Год,COUNT(Год) from Фильмы group by Год");
            for (int i = 0; i < genres_count.Rows.Count; i++)
            {
                if (film.get_genres().Contains(genres_count.Rows[i][0].ToString()))
                    genre.Add(new value { X = genres_count.Rows[i][0].ToString(), Y = Convert.ToInt32(genres_count.Rows[i][1]), this_film = true });
                else
                    genre.Add(new value { X = genres_count.Rows[i][0].ToString(), Y = Convert.ToInt32(genres_count.Rows[i][1]), this_film = false });
            }
            for (int i = 0; i < years_count.Rows.Count; i++)
            {
                if (film.get_year()== Convert.ToInt32(years_count.Rows[i][0]))
                    year_.Add(new value { X = years_count.Rows[i][0].ToString(), Y = Convert.ToInt32(years_count.Rows[i][1]), this_film = true });
                else
                    year_.Add(new value { X = years_count.Rows[i][0].ToString(), Y = Convert.ToInt32(years_count.Rows[i][1]), this_film = false });
            }
            foreach (var item in list)
            {
                if(item==film)
                {
                    rating_.Add(new value { X = item.get_name(), Y = item.get_sr_ozenka(),this_film = true });
                    views_.Add(new value { X = item.get_name(), Y = item.get_views(), this_film = true });
                }
                else
                {
                    rating_.Add(new value { X = item.get_name(), Y = item.get_sr_ozenka(), this_film = false });
                    views_.Add(new value { X = item.get_name(), Y = item.get_views(), this_film = false });
                }
            }
            
            draw_diagram(ref rating,rating_);
            draw_diagram(ref janr, genre);
            draw_diagram(ref year, year_);
            draw_diagram(ref views, views_);
        }
    }
}

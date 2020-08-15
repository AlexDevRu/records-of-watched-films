using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Data;

namespace Учет_просмотренных_фильмов
{
    /// <summary>
    /// Логика взаимодействия для views.xaml
    /// </summary>
    /// 
    public partial class views : Window
    {
        int id;

        public views(int id)
        {
            InitializeComponent();
            this.id = id;
        }

        class view
        {
            public string date { get; set; }
            public string otzyv { get; set; }
            public int ozenka { get; set; }
        }

        List<view> l_views = new List<view>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable t_views = DataBase.exec_zapros("select Дата, Отзыв, Оценка from Просмотры where [id фильма]=" + id);
            for(int i=0;i<t_views.Rows.Count;i++)
            {
                l_views.Add(new view { date=Convert.ToDateTime(t_views.Rows[i][0]).ToShortDateString(),
                                       otzyv= t_views.Rows[i][1].ToString(),
                                       ozenka= Convert.ToInt32(t_views.Rows[i][2])
                });
            }
            dates.ItemsSource = l_views;
            dates.DisplayMemberPath = "date";
            dates.SelectedIndex = 0;
        }

        private void update_selection(view view)
        {
            if(view!=null)
            {
                otzyv.Text = view.otzyv;
                ozenka.Available = false;
                ozenka.S_Height = 46;
                ozenka.S_Width = 55.2;
                ozenka.Ozenka = view.ozenka;
            }
        }

        private void Dates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            view view = dates.SelectedItem as view;
            update_selection(view);
        }

        view selected_item=null;

        private void Add_view_Click(object sender, RoutedEventArgs e)
        {
            selected_item = dates.SelectedItem as view;
            date.Visibility = Visibility.Visible;
            save.Visibility = Visibility.Visible;
            cancel.Visibility = Visibility.Visible;
            otzyv.IsEnabled = true;
            otzyv.Text = "";
            ozenka.Ozenka = 0;
            ozenka.Available = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            date.Visibility = Visibility.Hidden;
            save.Visibility = Visibility.Hidden;
            cancel.Visibility = Visibility.Hidden;
            ozenka.Available = false;
            update_selection(selected_item);
            otzyv.IsEnabled = false;
        }
        
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            l_views.Add(new view
            {
                date = Convert.ToDateTime(date.Text).ToShortDateString(),
                otzyv = otzyv.Text,
                ozenka = ozenka.Ozenka
            });
            view new_view = l_views[l_views.Count - 1];
            string sql = "insert into Просмотры (Дата,Отзыв,Оценка,[id фильма]) values(@date,@otzyv,@ozenka,@id)";
            List<parameter> parameters = new List<parameter>();
            parameters.Add(new parameter { name = "@date", value = Convert.ToDateTime(new_view.date) });
            parameters.Add(new parameter { name = "@otzyv", value = new_view.otzyv });
            parameters.Add(new parameter { name = "@ozenka", value = new_view.ozenka });
            parameters.Add(new parameter { name = "@id", value = id });
            DataBase.exec_zapros(sql,parameters);
            dates.Items.Refresh();
            dates.SelectedIndex = l_views.Count - 1;
            date.Visibility = Visibility.Hidden;
            save.Visibility = Visibility.Hidden;
            cancel.Visibility = Visibility.Hidden;
            ozenka.Available = false;
            update_selection(selected_item);
            otzyv.IsEnabled = false;
            (this.Owner as MainWindow).update_views(id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Net;
using HtmlAgilityPack;
using System.IO;
using System.Data;

namespace Учет_просмотренных_фильмов
{
    /// <summary>
    /// Логика взаимодействия для addfilm.xaml
    /// </summary>
    /// 
    public class item
    {
        public string Name { get; set; }
        public bool Isselected { get; set; }
    }

    public partial class addfilm : Window
    {
        BitmapImage poster;
        List<item> Items = new List<item>();

        public addfilm()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            poster = new BitmapImage(new Uri(Path.GetFullPath("resources/no_poster.jpg")));
            DataTable jnr = DataBase.exec_zapros("select Жанр from Жанры order by Жанр asc");     
            for (int i = 0; i < jnr.Rows.Count; i++) Items.Add(new item { Name = jnr.Rows[i][0].ToString(), Isselected = false });
            janres.ItemsSource = Items;
        }

        private void TextBox_GotMouseCapture(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox t = sender as TextBox;
            if(t.Text=="Название" || t.Text == "Год" || t.Text == "Жанр")
            {
                t.Text = "";
                t.FontStyle = FontStyles.Normal;
            }
        }

        private void TextBox_LostMouseCapture(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox t = sender as TextBox;
            if(t.Text=="")
            {
                t.Text = t.Name == "name" ? "Название" : t.Name == "year" ? "Год" : "Жанр";
                t.FontStyle = FontStyles.Italic;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataTable t = DataBase.exec_zapros("select Жанр from Жанры where Жанр='"+ janr.Text + "'");
            if (janr.Text != "Жанр" && t.Rows.Count==0)
            {
                Items.Add(new item { Name = janr.Text, Isselected = false });
                janres.Items.Refresh();
            }
            else
                MessageBox.Show("Не удалось добавить жанр","Ошибка");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            List<string> j = new List<string>();
            foreach (var i in Items) if (i.Isselected) j.Add(i.Name);
            if (name.Text=="Название"||year.Text=="Год"||date.Text==""||otzyv.Text==""||ozenka.Ozenka==0||j.Count==0)
            {
                MessageBox.Show("Не все поля заполнены", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }
            DataTable t = DataBase.exec_zapros("select [id фильма] from Фильмы where Название='" + name.Text + "'");
            if (t.Rows.Count==0)
            {
                List<parameter> parameters = new List<parameter>();
                parameters.Add(new parameter { name = "@name", value = name.Text });
                parameters.Add(new parameter { name = "@year", value = Convert.ToInt32(year.Text) });
                BitmapImage img = poster;
                byte[] data = DataBase.get_bytes_from_image(img);
                parameters.Add(new parameter { name = "@poster", value = data });
                parameters.Add(new parameter { name = "@date", value = Convert.ToDateTime(date.Text) });
                parameters.Add(new parameter { name = "@otzyv", value = otzyv.Text });
                parameters.Add(new parameter { name = "@ozenka", value = ozenka.Ozenka });
                parameters.Add(new parameter { name = "@id", dir = ParameterDirection.Output });
                int id = DataBase.exec_proc("insert_film", parameters);

                for (int i = 0; i < j.Count; i++)
                {
                    parameters.Clear();
                    parameters.Add(new parameter { name = "@janr", value = j[i] });
                    parameters.Add(new parameter { name = "@id", value = id });
                    DataBase.exec_proc("insert_janr", parameters);
                }
                name.FontStyle = FontStyles.Italic;
                year.FontStyle = FontStyles.Italic;
                janr.FontStyle = FontStyles.Italic;
                name.Text = "Название"; year.Text = "Год"; janr.Text = "Жанр";
                ozenka.reset();
                foreach (var i in Items) if (i.Isselected) i.Isselected = false;
                janres.Items.Refresh();
                otzyv.Text = "";
                var mw = this.Owner as MainWindow;
                mw.insert_film_and_add(id);
            }
            else MessageBox.Show("Фильм уже существует в БД", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void Name_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    Uri uri = new Uri("https://megogo.net/ru/search-extended?q=" + (sender as TextBox).Text.Replace(" ","+") + "&tab=video");

                    string html = new WebClient().DownloadString(uri);
                    byte[] bytes_ = Encoding.GetEncoding(1251).GetBytes(html);
                    html = Encoding.UTF8.GetString(bytes_);
                    HtmlDocument hap = new HtmlDocument();
                    hap.LoadHtml(html);

                    HtmlNodeCollection par = hap.DocumentNode.SelectNodes("//div[@class]");
                    HtmlNode div = null;
                    if (par != null)
                        foreach (HtmlNode node in par)
                        {
                            if (node.Attributes["class"].Value == "thumb") { div = node; break; }
                        }
                    html = div.InnerHtml;
                    hap.LoadHtml(html);
                    string link = hap.DocumentNode.SelectNodes("//a")[0].Attributes["href"].Value;
                    string picture = hap.DocumentNode.SelectNodes("//img")[0].Attributes["data-original"].Value;
                    string namee = hap.DocumentNode.SelectNodes("//img")[0].Attributes["alt"].Value;

                    uri = new Uri("https://megogo.net" + link);
                    html = new WebClient().DownloadString(uri);
                    bytes_ = Encoding.GetEncoding(1251).GetBytes(html);
                    html = Encoding.UTF8.GetString(bytes_);
                    hap.LoadHtml(html);
                    par = hap.DocumentNode.SelectNodes("//div[@class]");
                    div = null;
                    if (par != null)
                        foreach (HtmlNode node in par)
                        {
                            if (node.Attributes["class"].Value == "video-info") { div = node; break; }
                        }
                    html = div.InnerHtml;
                    hap.LoadHtml(html);
                    string yearr = hap.DocumentNode.SelectNodes("//span")[0].Attributes["content"].Value;
                    HtmlNodeCollection genres = hap.DocumentNode.SelectNodes("//a");
                    foreach (var i in Items) if (i.Isselected) i.Isselected = false;
                    foreach (HtmlNode node in genres)
                        if (node.GetAttributeValue("itemprop", "") == "genre")
                        {
                            bool f = false;
                            foreach (var item in Items) if (item.Name == node.InnerText) { f = true; item.Isselected = true; break; }
                            if (!f)
                                Items.Add(new item { Name = node.InnerText, Isselected = true }); 
                        }
                    janres.Items.Refresh();
                    name.Text = namee;
                    year.Text = yearr;
                    BitmapImage bitmapImage = DataBase.get_image_by_link(picture);
                    pic.Source = bitmapImage;
                    poster = bitmapImage;
                    
                }
            }
            catch(Exception)
            {
                MessageBox.Show("Фильм не найден", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }
    }
}

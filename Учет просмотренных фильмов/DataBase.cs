using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Net;

namespace Учет_просмотренных_фильмов
{
    class parameter
    {
        public string name;
        public object value;
        public ParameterDirection dir = ParameterDirection.Input;
    }

    class DataBase
    {
        static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + AppDomain.CurrentDomain.BaseDirectory + @"Фильмы.mdf;Integrated Security=True";
        static SqlConnection connection = new SqlConnection(connectionString);
        static public DataTable exec_zapros(string sql, List<parameter> param = null)
        {
            DataTable table = new DataTable();
            SqlCommand command = new SqlCommand(sql, connection);
            if(param!=null)
            {
                foreach(var p in param)
                    command.Parameters.AddWithValue(p.name,p.value);
            }
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            connection.Open();
            adapter.Fill(table);
            connection.Close();
            return table;
        }

        static public int exec_proc(string name_proc, List<parameter> param)
        {
            SqlCommand command = new SqlCommand(name_proc, connection);
            command.CommandType = CommandType.StoredProcedure;
            SqlParameter output=new SqlParameter();
            output.Value = 0;
            for(int i=0;i<param.Count;i++)
            {
                SqlParameter p = new SqlParameter();
                p.ParameterName = param[i].name;
                if (param[i].dir == ParameterDirection.Output)
                {
                    p.Direction = param[i].dir;
                    p.Size = 4;
                    output = p;
                }
                else
                {
                    p.Value = param[i].value;
                }
                command.Parameters.Add(p);
            }
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            return Convert.ToInt32(output.Value);
        }

        public static Polygon draw_star(double width, double height, Brush color)
        {
            Polygon z = new Polygon();
            z.Fill = color;
            z.Stroke = Brushes.Black;
            PointCollection pc = new PointCollection();
            pc.Add(new Point(width / 6, height)); pc.Add(new Point(width / 3, height * 0.6)); pc.Add(new Point(0, height * 0.4));
            pc.Add(new Point(width * 0.4, height * 0.4)); pc.Add(new Point(width / 2, 0)); pc.Add(new Point(width * 0.6, height * 0.4));
            pc.Add(new Point(width, height * 0.4)); pc.Add(new Point(width / 3 * 2, height * 0.6)); pc.Add(new Point(height, height));
            pc.Add(new Point(width / 2, height * 0.7));
            z.Points = new PointCollection(pc);
            return z;
        }

        public static byte[] get_bytes_from_image(BitmapImage picture)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(picture));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        public static BitmapImage get_image_by_link(string link)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            int BytesToRead = 100;

            WebRequest request = WebRequest.Create(new Uri(link, UriKind.Absolute));
            request.Timeout = -1;
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            BinaryReader reader = new BinaryReader(responseStream);
            MemoryStream memoryStream = new MemoryStream();

            byte[] bytebuffer = new byte[BytesToRead];
            int bytesRead = reader.Read(bytebuffer, 0, BytesToRead);

            while (bytesRead > 0)
            {
                memoryStream.Write(bytebuffer, 0, bytesRead);
                bytesRead = reader.Read(bytebuffer, 0, BytesToRead);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.StreamSource.Dispose();
            return bitmapImage;
        }
    }
}

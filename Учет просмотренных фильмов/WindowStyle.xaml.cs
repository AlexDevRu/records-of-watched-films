using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Учет_просмотренных_фильмов
{
    public partial class WindowStyle : ResourceDictionary
    {
      public WindowStyle()
      {
          InitializeComponent();
      }
     
      private void CloseClick(object sender, RoutedEventArgs e)
      {
         var window = (Window)((FrameworkElement)sender).TemplatedParent;
         window.Close();
      }
    
     private void MinimizeClick(object sender, RoutedEventArgs e)
     {
         var window = (Window)((FrameworkElement)sender).TemplatedParent;
         window.WindowState = System.Windows.WindowState.Minimized;
     }
    }
}

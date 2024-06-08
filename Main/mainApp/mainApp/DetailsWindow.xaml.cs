using Mysqlx.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1;

namespace mainApp
{
    /// <summary>
    /// Interaction logic for DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window
    {
        public DetailsWindow(string[] results, string imagePath)
        {
            InitializeComponent();
            DataContext = this;
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath);
                bitmap.EndInit();

                Image image = new Image();
                image.Source = bitmap;
                image.HorizontalAlignment = HorizontalAlignment.Center;
                ImageContainer.Child = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}");
            }
            for (int i = 0; i < results.Length; i++) 
            {
                TextBlock textBlock = new TextBlock();

                // Set the TextBlock properties
                textBlock.Text = results[i];
                textBlock.FontSize = 16;
                textBlock.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#1B4ECD"));
                textBlock.FontWeight = FontWeights.Bold;
                textBlock.Margin = new Thickness(5, 3, 0, 0);
                ResultStack.Children.Add(textBlock);
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Window_MouseLeftButton(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}

using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using mainApp;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<CardItem> CardItems { get; set; }
        public string Method;
        public string imagePath;
        public string[] result;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public string time;
        public string Time
        {
            get { return time; }
            set
            {
                time = value;
                OnPropertyChanged(nameof(Time));
            }
        }

        public string match;
        public string Match
        {
            get { return match; }
            set
            {
                match = value;
                OnPropertyChanged(nameof(Match));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            time = "Time execution : ";
            match = "Match found : ";
            CardItems = new ObservableCollection<CardItem>();
            CardListView.ItemsSource = CardItems;
        }

        private void Window_MouseLeftButton(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnOpenImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(filePath);
                    bitmap.EndInit();

                    Image image = new Image();
                    image.Source = bitmap;
                    image.HorizontalAlignment = HorizontalAlignment.Center;

                    imageContainer.Child = image;
                    this.imagePath = filePath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}");
                }
            }
        }

        private void btnBM_Click(object sender, RoutedEventArgs e)
        {
            this.Method = "BM";
            System.Diagnostics.Debug.WriteLine("Test");
            Debug.WriteLine(this.Method);
        }

        private void btnKMP_Click(object sender, RoutedEventArgs e)
        {
            this.Method = "KMP";
            Debug.WriteLine(this.Method);
        }

        public void Details_Click(object sender, RoutedEventArgs e)
        {
            DetailsWindow details = new DetailsWindow(this.result, this.imagePath);
            details.Show();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Program program = new Program();
            program.mainProgram(this.Method, 80, this.imagePath);
            Debug.WriteLine(program.solutionsValid.Length.ToString());
            Debug.WriteLine(program.timeNeeded.ToString());
            Debug.WriteLine(program.matches.ToString());
            this.Time = "Time execution : " + program.timeNeeded.ToString() + " ms";
            int matches = 0;
            if (CardItems != null) CardItems.Clear();
            if (program.solutionsValid[0,0] != null)
            {
                CardItems.Add(new CardItem 
                { 
                    Nama = "Nama : " + program.solutionsValid[0,1], 
                    Kemiripan = "Kemiripan : " + program.solutionsValid[0, 11],
                });
                matches++;
            }
            this.Match = "Match found : " + matches.ToString();
            string[] res =
            {
                "Nama\t\t\t: " + program.solutionsValid[0, 1],
                "NIK\t\t\t: " + program.solutionsValid[0, 0],
                "Tempat Lahir\t\t: " + program.solutionsValid[0, 2],
                "Tanggal Lahir\t\t: " + program.solutionsValid[0, 3],
                "Jenis Kelamin\t\t: " + program.solutionsValid[0, 4],
                "Golongan Darah\t\t: " + program.solutionsValid[0, 5],
                "Alamat\t\t\t: " + program.solutionsValid[0, 6],
                "Agama\t\t\t: " + program.solutionsValid[0, 7],
                "Status Perkawinan\t: " + program.solutionsValid[0, 8],
                "Pekerjaan\t\t: " + program.solutionsValid[0, 9],
                "Kewarganegaraan\t: " + program.solutionsValid[0, 10],
            };

            this.result = res;
        }
    }

    public class CardItem
    {
        public string Kemiripan { get; set; }
        public string Nama { get; set; }
        public string NIK { get; set; }
        public string gender { get; set; }
        public string tgl_lahir { get; set; }
        public string tempat_lahir { get; set; }
        public string goldar { get; set; }
        public string alamat { get; set; }
        public string agama { get; set; }
        public string status { get; set; }
        public string pekerjaan { get; set; }
        public string warganegara { get; set; }
    }

}

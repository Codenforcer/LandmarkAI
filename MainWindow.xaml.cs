using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Media.Imaging;
using static LandmarkAI.Classes.Tags;

namespace LandmarkAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Functionality for selecting the image and displaying it
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Image files (*.png; *.jpg)|*.png;*.jpg;*jpeg|AllowDrop files (*.*)|*.*";

            // This sets the directory that loads up when button clicked.
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (dialog.ShowDialog() == true)
            {
                string fileName = dialog.FileName;
                selectedImage.Source = new BitmapImage(new Uri(fileName));

                MakePredictionAsync(fileName);
            }

        }

        // Function to make a request to the customview API
        private async void MakePredictionAsync(string fileName)
        {
            // below is taken from customevision.
            string url = "https://uksouth.api.cognitive.microsoft.com/customvision/v3.0/Prediction/50c609f1-ee72-4831-aacf-a6ca42bf34e2/classify/iterations/Iteration1/image";
            string prediction_key = "28fc8613fd3a407b9963a24566379280";
            string content_type = "application/octet-stream";
            var file = File.ReadAllBytes(fileName);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Prediction-Key", prediction_key);

                using (var content = new ByteArrayContent(file))
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(content_type);
                    var response = await client.PostAsync(url, content);

                    //so we can see the reply back from API
                    var responseString = await response.Content.ReadAsStringAsync();

                    List<prediction> prediction = (JsonConvert.DeserializeObject<List<CustomVision>>(responseString)).predictions;
                }

            }
        }
    }
}

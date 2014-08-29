using Aws;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Upload _upload;
        private Glacier _glacier = new Glacier();

        public MainWindow()
        {
            InitializeComponent();
            SetFileControlsVisibility(Visibility.Hidden);
            Reset();
        }

        private void BrowseFile(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();

            dlg.DefaultExt = ".zip";
            dlg.Filter = "ZIP Files (*.zip)|*.zip";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                File.Text = filename;
                SetFileControlsVisibility(Visibility.Visible);
            }
        }

        private void SetFileControlsVisibility(Visibility visibility)
        {
            HashGrid.Visibility = visibility;
        }

        private async void DoUpload(object sender, RoutedEventArgs e)
        {
            _upload.Failure = false;
            _upload.Success = false;
            _upload.Progress = 0;

            try
            {
                using (var stream = new FileStream(File.Text, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    Upload.IsEnabled = false;

                    _glacier.ProgressChanged += (o, pe) =>
                    {
                        ShowProgress(pe.Progress);
                    };
                    var checksum = CalculateHash(stream);

                    try
                    {
                        var result = await RunUpload(_upload.File);
                        _upload.Success = result.Checksum == checksum;
                        _upload.Failure = result.Checksum != checksum;
                    }
                    catch (GlacierException ex)
                    {
                        ShowError(ex.Message);
                        _upload.Failure = true;
                    }

                    Upload.IsEnabled = true;
                }
            }
            catch (IOException)
            {
                ShowError("The choosen file cannot be read. Check the permissions.");

                return;
            }
        }

        private async Task<Result> RunUpload(string file)
        {
            return await Task.Run(() =>
            {
                return _glacier.Upload(file, DateTime.Now.ToString());
            }).ConfigureAwait(continueOnCapturedContext: false);
        }

        private string CalculateHash(Stream stream)
        {
            var checksum = Checksum.Calculate(stream);
            Hash.Text = checksum;

            return checksum;
        }

        private void ShowProgress(int progress)
        {
            _upload.Progress = progress;
        }

        private void Reset()
        {
            _upload = new Upload(_glacier);
            FileUploadGrid.DataContext = _upload;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

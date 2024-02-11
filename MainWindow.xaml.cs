using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using System.Text;
using System.Windows.Input;
using ImageEditor.Funtionals;
using System.Drawing.Imaging;
using System.Drawing;
using Rectangle = System.Drawing.Rectangle;
using Pen = System.Windows.Media.Pen;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using System.Windows.Media.Effects;
using ImageEditor.Exposure;
using ImageEditor.Color;

namespace ImageEditor
{
    public partial class MainWindow : Window
    {
        private List<WriteableBitmap> previousVersions = new List<WriteableBitmap>();
        private BitmapImage originalImage;
        private WriteableBitmap editedBitmap;
        private Rectangle rectCropArea = new Rectangle();

        private double zoomLevel = 1.0;
        private double brightnessValue = 0;
        private int xDown = 0;
        private int yDown = 0;
        private int xUp = 0;
        private int yUp = 0;

        public MainWindow()
        {
            InitializeComponent();
            var pixelFormats = typeof(PixelFormats).GetProperties()
                .Where(p => p.PropertyType == typeof(System.Windows.Media.PixelFormat));

            var formatNames = pixelFormats.Select(p => p.Name);

            foreach (var formatName in formatNames)
            {
                filters.Items.Add(formatName);
            }
        }

        #region File

        #region Open
        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            originalImage = Open.OpenImage();
            editedBitmap = new WriteableBitmap(originalImage);
            UpdateImageDisplay();
        }
        #endregion

        #region Save
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (editedBitmap != null)
            {
                Save.SaveImage(editedBitmap);
            }
        }
        #endregion

        #endregion

        #region Exposure

        #region Brightness
        private void brightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (editedBitmap != null)
            {
                brightnessValue = Math.Min(Math.Max(brightnessSlider.Value, -100), 100);
                BitmapSource adjustedBitmap = Brightness.AdjustBrightness(originalImage, brightnessValue);
                editedBitmap = new WriteableBitmap(adjustedBitmap);
                UpdateImageDisplay();
            }
            else
            {
                brightnessSlider.Value = 0;
            }
        }
        #endregion

        #region Contrast
        private void contrastSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (editedBitmap != null)
            {
                double contrastValue = contrastSlider.Value;
                editedImage.Source = Contrast.ApplyContrastFilter(editedBitmap, contrastValue);
            }
            else 
            { 
                contrastSlider.Value = 0;
            }
        }
        #endregion

        #region Highlight
        private void highlightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (editedBitmap != null)
            {
                double highlightValue = highlightSlider.Value;
                editedImage.Source = Highlight.ApplyHighlightFilter(editedBitmap, highlightValue);
            }
            else
            {
                highlightSlider.Value = 0;
            }
        }
        #endregion

        #region Shadow
        private void shadowsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (editedBitmap != null)
            {
                double shadowsValue = shadowsSlider.Value;
                BitmapSource adjustedBitmap = Shadow.AdjustShadows(originalImage, shadowsValue);
                editedBitmap = new WriteableBitmap(adjustedBitmap);
                UpdateImageDisplay();
            }
            else
            {
                shadowsSlider.Value = 0;
            }
        }
        #endregion

        private void exposureToggle_Click(object sender, RoutedEventArgs e)
        {
            if (exposureToggle.IsChecked == true)
            {
                exposurePanel.Visibility = Visibility.Visible;
            }
            else
            {
                exposurePanel.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Color

        #region Hue
        private void hueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (editedBitmap != null)
            {
                double hueValue = hueSlider.Value;
                BitmapSource adjustedBitmap = Hue.AdjustHue(originalImage, hueValue);
                editedBitmap = new WriteableBitmap(adjustedBitmap);
                UpdateImageDisplay();
            }
            else
            {
                hueSlider.Value = 0;
            }
        }
        #endregion

        #region Saturation
        private void saturationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (editedBitmap != null)
            {
                double saturationValue = saturationSlider.Value;
                BitmapSource adjustedBitmap = Saturation.AdjustSaturation(originalImage, saturationValue);
                editedBitmap = new WriteableBitmap(adjustedBitmap);
                UpdateImageDisplay();
            }
            else
            {
                saturationSlider.Value = 0;
            }
        }
        #endregion

        #region Temperature
        private void temperatureSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (editedBitmap != null)
            {
                double temperatureValue = temperatureSlider.Value;
                BitmapSource adjustedBitmap = Temperature.AdjustTemperature(originalImage, temperatureValue);
                editedBitmap = new WriteableBitmap(adjustedBitmap);
                UpdateImageDisplay();
            }
            else
            {
                temperatureSlider.Value = 0;
            }
        }
        #endregion

        private void colorToggle_Click(object sender, RoutedEventArgs e)
        {
            if (colorToggle.IsChecked == true)
            {
                colorPanel.Visibility = Visibility.Visible;
            }
            else
            {
                colorPanel.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Sharpen
        private void sharpenSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (editedBitmap != null)
            {
                double sharpenValue = sharpenSlider.Value;
                BitmapSource adjustedBitmap = Sharpen.AdjustSharpen(originalImage, sharpenValue);
                editedBitmap = new WriteableBitmap(adjustedBitmap);
                UpdateImageDisplay();
            }
            else
            {
                sharpenSlider.Value = 0;
            }
        }
        private void sharpenToggle_Click(object sender, RoutedEventArgs e)
        {
            if (sharpenToggle.IsChecked == true)
            {
                sharpenPanel.Visibility = Visibility.Visible;
            }
            else
            {
                sharpenPanel.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Rotate
        private void rotateButton_Click(object sender, RoutedEventArgs e)
        {
            if (editedBitmap != null)
            {
                var rotatedImage = Rotate.RotateImage(editedBitmap);
                editedImage.Source = rotatedImage;
                editedBitmap = new WriteableBitmap(rotatedImage);
            }
        }
        #endregion

        #region Filter
        private void filterButton_Click(object sender, RoutedEventArgs e)
        {
            //    if (editedBitmap != null)
            //    {
            //        var filteredImage = Filter.SetFilter(PixelFormats.Gray8, editedBitmap);
            //        editedImage.Source = filteredImage;
            //        editedBitmap = new WriteableBitmap(filteredImage);
            //    }
        }
        private void filters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected format name
            if (editedBitmap != null)
            {
                var selectedFormatName = e.AddedItems[0].ToString();

                // Find the corresponding PixelFormat property using reflection
                var selectedPixelFormatProperty = typeof(PixelFormats).GetProperty(selectedFormatName);

                if (selectedPixelFormatProperty != null)
                {
                    var selectedPixelFormat = (System.Windows.Media.PixelFormat)selectedPixelFormatProperty.GetValue(null);
                    var filteredImage = Filter.SetFilter(selectedPixelFormat, editedBitmap);
                    editedImage.Source = filteredImage;
                    editedBitmap = new WriteableBitmap(filteredImage);
                }
            }
        }

        #endregion

        #region Crop
        private void cropButton_Click(object sender, RoutedEventArgs e)
        {
            xDown = 0;
            yDown = 0;

            editedImage.MouseDown += EditedImage_MouseDown;
            editedImage.MouseMove += EditedImage_MouseMove;
            editedImage.MouseUp += EditedImage_MouseUp;
            editedImage.Cursor = Cursors.Cross;
        }

        private void EditedImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            xDown = (int)e.GetPosition(editedImage).X;
            yDown = (int)e.GetPosition(editedImage).Y;
        }

        private void EditedImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                xUp = (int)e.GetPosition(editedImage).X;
                yUp = (int)e.GetPosition(editedImage).Y;

                int width = Math.Abs(xUp - xDown);
                int height = Math.Abs(yUp - yDown);

                rectCropArea = new Rectangle(Math.Min(xDown, xUp), Math.Min(yDown, yUp), width, height);

                editedImage.InvalidateVisual();
            }
        }

        private void EditedImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CroppedBitmap croppedImage = Crop.CropImage(originalImage, rectCropArea);
            editedBitmap = Crop.ConvertCroppedBitmapToWriteableBitmap(croppedImage);
            editedImage.MouseDown -= EditedImage_MouseDown;
            editedImage.MouseMove -= EditedImage_MouseMove;
            editedImage.MouseUp -= EditedImage_MouseUp;
            editedImage.Cursor = Cursors.Arrow;

            UpdateImageDisplay();
        }
        #endregion

        #region Zoom
        private void zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (originalImage != null)
            {
                double zoomValue = zoomSlider.Value;
                ApplyZoom(zoomValue);
                zoomLevel = zoomValue;
            }
        }

        private void ApplyZoom(double zoomValue)
        {
            double newHeight;
            double newWidth;

            if (zoomValue < 7 && zoomValue > -5)
            {
                if (zoomValue > 0)
                {
                    newHeight = originalImage.PixelHeight / (1 + zoomValue);
                    newWidth = originalImage.PixelWidth / (1 + zoomValue);
                }
                else
                {
                    newHeight = originalImage.PixelHeight * (1 - zoomValue);
                    newWidth = originalImage.PixelWidth * (1 - zoomValue);
                }

                editedImage.Height = newHeight;
                editedImage.Width = newWidth;
                UpdateImageDisplay();
            }
        }

        private void editedImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoomDelta = e.Delta > 0 ? 0.1 : -0.1;
            double newZoomLevel = zoomLevel + zoomDelta;
            ApplyZoom(newZoomLevel);
            zoomLevel = newZoomLevel;
            zoomSlider.Value = zoomLevel;
        }
        #endregion

        #region UpdateImage
        private void UpdateImageDisplay()
        {
            if (originalImage != null)
            {
                
                if (editedBitmap != null)
                {
                    previousVersions.Add(new WriteableBitmap(editedBitmap));
                    editedImage.Source = editedBitmap;
                }
                else
                {
                    editedImage.Source = originalImage;
                }
            }
        }
        #endregion

        #region Undo
        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            if (previousVersions.Count > 0)
            {
                WriteableBitmap previousVersion = previousVersions[previousVersions.Count - 1];
                previousVersions.Remove(previousVersion);
                editedImage.Source = previousVersion;
            }
        }
        #endregion
    }
}
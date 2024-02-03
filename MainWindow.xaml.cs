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

namespace ImageEditor
{
    public partial class MainWindow : Window
    {
        private List<WriteableBitmap> previousVersions = new List<WriteableBitmap>();
        private BitmapImage originalImage;
        private WriteableBitmap editedBitmap;

        private double zoomLevel = 1.0;
        private double brightnessValue = 0;
        int xDown = 0;
        int yDown = 0;
        int xUp = 0;
        int yUp = 0;
        Rectangle rectCropArea = new Rectangle();

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Open
        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            originalImage = Functions.OpenImage();
            editedBitmap = new WriteableBitmap(originalImage);
            UpdateImageDisplay();
        }
        #endregion

        #region Save
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Functions.Save(editedBitmap);
        }
        #endregion

        #region Rotate
        private void rotateButton_Click(object sender, RoutedEventArgs e)
        {
            var rotatedImage = Functions.Rotate(editedBitmap);
            editedImage.Source = rotatedImage;
            editedBitmap = new WriteableBitmap(rotatedImage);
        }
        #endregion

        #region Filter
        private void filterButton_Click(object sender, RoutedEventArgs e)
        {
            var filteredImage = Functions.SetFilter(PixelFormats.Gray8, editedBitmap);
            editedImage.Source = filteredImage;
            editedBitmap = new WriteableBitmap(filteredImage);
        }
        #endregion

        #region Brightness
        private void brightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            brightnessValue = Math.Min(Math.Max(brightnessSlider.Value, -100), 100); // Limit the brightness value within the range of -100 to 100
            BitmapSource adjustedBitmap = Functions.AdjustBrightness(originalImage, brightnessValue);
            editedBitmap = new WriteableBitmap(adjustedBitmap);
            UpdateImageDisplay();
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
                //TransformedBitmap transformedBitmap = new TransformedBitmap(originalImage, new ScaleTransform(zoomLevel, zoomLevel));
                //BitmapSource adjustedBitmap = Brightness.AdjustBrightness(transformedBitmap, brightnessValue);
                //editedBitmap = new WriteableBitmap(adjustedBitmap);
                //editedImage.Width = originalImage.PixelWidth * zoomLevel;
                //editedImage.Height = originalImage.PixelHeight * zoomLevel;
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
            CroppedBitmap croppedImage = Funtions.Crop(originalImage, rectCropArea);
            editedBitmap = Funtions.ConvertCroppedBitmapToWriteableBitmap(croppedImage);
            editedImage.MouseDown -= EditedImage_MouseDown;
            editedImage.MouseMove -= EditedImage_MouseMove;
            editedImage.MouseUp -= EditedImage_MouseUp;
            editedImage.Cursor = Cursors.Arrow;

            UpdateImageDisplay();
        }
        #endregion

        private void zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Get the current zoom value from the slider
            double zoomValue = zoomSlider.Value;

            // Update the zoom level and apply the zoom
            zoomLevel = zoomValue;
            //ApplyZoom();
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.OemPlus || e.Key == Key.Add)
                {
                    // Zoom in
                    zoomLevel += 0.1;
                    //ApplyZoom();
                }
                else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
                {
                    // Zoom out
                    if (zoomLevel > 0.1)
                    {
                        zoomLevel -= 0.1;
                        //ApplyZoom();
                    }
                }
            }
        }

        private void editedImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Handle zoom logic here
            double zoomDelta = e.Delta > 0 ? 0.1 : -0.1; // Adjust the zoom increment as desired
            zoomLevel += zoomDelta;
            //ApplyZoom();
        }
    }
}
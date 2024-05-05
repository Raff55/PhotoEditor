using ImageEditor.Collage;
using ImageEditor.Color;
using ImageEditor.Color_Adjustments;
using ImageEditor.Exposure;
using ImageEditor.Funtionals;
using ImageEditor.Transformation;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brushes = System.Windows.Media.Brushes;
using Rectangle = System.Drawing.Rectangle;


namespace ImageEditor
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<WriteableBitmap> previousVersions = new List<WriteableBitmap>(10);
        private BitmapImage originalImage;
        private WriteableBitmap editedBitmap;
        private Rectangle rectCropArea = new Rectangle();
        SolidColorBrush commonColor = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#353536"));

        private double zoomLevel = 0;
        private int xDown = 0;
        private int yDown = 0;
        private int xUp = 0;
        private int yUp = 0;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitializeComboBoxes();
        }

        #region Common
        private void InitializeComboBoxes()
        {
            var pixelFormats = typeof(PixelFormats).GetProperties()
                .Where(p => p.PropertyType == typeof(System.Windows.Media.PixelFormat));

            var formatNames = pixelFormats.Select(p => p.Name);

            foreach (var formatName in formatNames)
            {
                filters.Items.Add(formatName);
            }
            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                fontComboBox.Items.Add(fontFamily.Source);
            }
            colorComboBox.Items.Add("White");
            colorComboBox.Items.Add("Black");
            colorComboBox.Items.Add("Red");
            colorComboBox.Items.Add("Green");
            colorComboBox.Items.Add("Blue");
            colorComboBox.Items.Add("Yellow");
            colorComboBox.Items.Add("Orange");
            colorComboBox.Items.Add("Purple");
            colorComboBox.Items.Add("Gray");
        }

        private void CollageTabButton_Click(object sender, RoutedEventArgs e)
        {
            editingView.Visibility = Visibility.Collapsed;
            collageView.Visibility = Visibility.Visible;
            EditTabButton.Background = commonColor;
            EditTabButton.Foreground = Brushes.White;
            CollageTabButton.Background = Brushes.White;
            CollageTabButton.Foreground = commonColor;

            popup.IsOpen = false;
            popup.Child = null;
            popup.Placement = PlacementMode.Center;
            popup.PlacementTarget = this;
            popup.StaysOpen = true;

            collageWidthTextBox = new TextBox();
            collageHeightTextBox = new TextBox();
            collageWidthTextBox.Clear();
            collageHeightTextBox.Clear();
            CollageFunctions.ShowPopup(ref popup, ref collageWidthTextBox, ref collageHeightTextBox, enterSizesCollageButton_Click);
        }

        // In the EditTabButton_Click method, simply close the popup and clear its child
        private void EditTabButton_Click(object sender, RoutedEventArgs e)
        {
            editingView.Visibility = Visibility.Visible;
            collageView.Visibility = Visibility.Collapsed;
            EditTabButton.Background = Brushes.White;
            EditTabButton.Foreground = commonColor;
            CollageTabButton.Background = commonColor;
            CollageTabButton.Foreground = Brushes.White;
            popup.IsOpen = false;
            popup.Child = null;
            if(collageCanvas.Children.Count > 0)
            {
                editedBitmap = CollageFunctions.RenderCanvasToImage(collageCanvas);
                originalImage = CollageFunctions.originalImage;
                collageCanvas.Children.Clear();
                UpdateImageDisplay();
            }
        }

        #endregion

        #region Edit

        #region File

        #region Open
        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            originalImage = Open.OpenImage();
            if (originalImage != null)
            {
                editedBitmap = new WriteableBitmap(originalImage);
                UpdateImageDisplay();
            }
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

        #region Undo
        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            if (previousVersions.Count > 0)
            {
                WriteableBitmap previousVersion = previousVersions[previousVersions.Count - 1];
                previousVersions.Remove(previousVersion);
                editedBitmap = previousVersion;
                UpdateImageDisplay();
            }
        }
        #endregion

        #region Reset
        private void reserButton_Click(object sender, RoutedEventArgs e)
        {
            previousVersions.Clear();
            textCanvas.Children.Clear();
            editedBitmap = new WriteableBitmap(originalImage);
            editedImage.Source = originalImage;
        }
        #endregion

        #region Update Methods
        private async void UpdateImageDisplay()
        {
            if (originalImage != null)
            {
                if (editedBitmap != null)
                {
                    var bitmap = await UpdateImageDisplayWithFiltersWithReturning();
                    previousVersions.Add(bitmap);
                    editedImage.Source = bitmap;
                    Resources.MergedDictionaries.Clear();
                    if(previousVersions.Count == 9)
                    {
                        previousVersions.Remove(previousVersions[0]);
                    }
                    previousVersions.Add(bitmap);
                }
                else
                {
                    editedImage.Source = originalImage;
                }
            }
        }

        private async Task<WriteableBitmap> UpdateImageDisplayWithFiltersWithReturning()
        {
            var exampleBitmap = new WriteableBitmap(editedBitmap);
            if (editedBitmap != null)
            {
                if (!String.IsNullOrEmpty(brightnessTextBox.Text) && brightnessSlider.Value != 0 && brightnessSlider.Value != Image.Brightness)
                {
                    Image.Brightness = brightnessSlider.Value;
                    exampleBitmap = await Brightness.AdjustBrightness(exampleBitmap, Image.Brightness);
                }
                if (!String.IsNullOrEmpty(contrastTextBox.Text) && contrastSlider.Value != 0 && contrastSlider.Value != Image.Contrast)
                {
                    Image.Contrast = contrastSlider.Value;
                    exampleBitmap = await Contrast.ApplyContrastFilter(exampleBitmap, Image.Contrast);
                }
                if (!String.IsNullOrEmpty(highlightTextBox.Text) && highlightSlider.Value != 0 && highlightSlider.Value != Image.Highlight)
                {
                    exampleBitmap = await Highlight.ApplyHighlightFilter(exampleBitmap, Image.Highlight);
                }
                if (!String.IsNullOrEmpty(shadowsTextBox.Text) && shadowsSlider.Value != 0 && shadowsSlider.Value != Image.Shadows)
                {
                    exampleBitmap = await Shadow.AdjustShadows(exampleBitmap, Image.Shadows);
                }
                if (!String.IsNullOrEmpty(blurTextBox.Text) && blurSlider.Value != 0 && blurSlider.Value != Image.Blur)
                {
                    exampleBitmap = await Blur.ApplyBlur(exampleBitmap, Image.Blur);
                }
                if (!String.IsNullOrEmpty(hueTextBox.Text) && hueSlider.Value != 0 && hueSlider.Value != Image.Hue)
                {
                    exampleBitmap = await Hue.AdjustHue(exampleBitmap, Image.Hue);
                }
                if (!String.IsNullOrEmpty(saturationTextBox.Text) && saturationSlider.Value != 0 && saturationSlider.Value != Image.Saturation)
                {
                    exampleBitmap = await Saturation.AdjustSaturation(exampleBitmap, Image.Saturation);
                }
                if (!String.IsNullOrEmpty(temperatureTextBox.Text) && temperatureSlider.Value != 0 && temperatureSlider.Value != Image.Temperature)
                {
                    exampleBitmap = await Temperature.AdjustTemperature(exampleBitmap, Image.Temperature);
                }
                if (!String.IsNullOrEmpty(sharpenTextBox.Text) && sharpenSlider.Value != 0 && sharpenSlider.Value != Image.Sharpen)
                {
                    exampleBitmap = await Sharpen.AdjustSharpen(exampleBitmap, Image.Sharpen);
                }
            }
            return exampleBitmap;
        }

        private async void UpdateImageDisplayWithFilters()
        {
            if (originalImage != null)
            {
                var exampleBitmap = new WriteableBitmap(editedBitmap);
                if (double.TryParse(brightnessTextBox.Text, out double brightnessValue) && brightnessValue != Image.Brightness)
                {
                    Image.Brightness = brightnessSlider.Value;
                    exampleBitmap = await Brightness.AdjustBrightness(exampleBitmap, Image.Brightness);
                }
                if (double.TryParse(contrastTextBox.Text, out double contrastValue) && contrastValue != Image.Contrast)
                {
                    Image.Contrast = contrastSlider.Value;
                    exampleBitmap = await Contrast.ApplyContrastFilter(exampleBitmap, Image.Contrast);
                }
                if (double.TryParse(highlightTextBox.Text, out double highlightValue) && highlightValue != Image.Highlight)
                {
                    exampleBitmap = await Highlight.ApplyHighlightFilter(exampleBitmap, Image.Highlight);
                }
                if (double.TryParse(shadowsTextBox.Text, out double shadowsValue) && shadowsValue != Image.Shadows)
                {
                    exampleBitmap = await Shadow.AdjustShadows(exampleBitmap, Image.Shadows);
                }
                if (double.TryParse(blurTextBox.Text, out double blurValue) && blurValue != Image.Blur)
                {
                    exampleBitmap = await Blur.ApplyBlur(exampleBitmap, Image.Blur);
                }
                if (double.TryParse(hueTextBox.Text, out double hueValue) && hueValue != Image.Hue)
                {
                    exampleBitmap = await Hue.AdjustHue(exampleBitmap, Image.Hue);
                }
                if (double.TryParse(saturationTextBox.Text, out double saturationValue) && saturationValue != Image.Saturation)
                {
                    exampleBitmap = await Saturation.AdjustSaturation(exampleBitmap, Image.Saturation);
                }
                if (double.TryParse(temperatureTextBox.Text, out double temperatureValue) && temperatureValue != Image.Temperature)
                {
                    exampleBitmap = await Temperature.AdjustTemperature(exampleBitmap, Image.Temperature);
                }
                if (double.TryParse(sharpenTextBox.Text, out double sharpenValue) && sharpenValue != Image.Sharpen)
                {
                    exampleBitmap = await Sharpen.AdjustSharpen(exampleBitmap, Image.Sharpen);
                }
                editedBitmap = exampleBitmap;
                previousVersions.Add(exampleBitmap);
                //UpdateImageDisplay();
                editedImage.Source = exampleBitmap;
            }
        }

        #endregion

        #endregion

        #region Basic Editing

        #region Rotate
        private void rotateButton_Click(object sender, RoutedEventArgs e)
        {
            if (editedBitmap != null)
            {
                var rotatedImage = Rotate.RotateImage(editedBitmap);
                editedBitmap = new WriteableBitmap(rotatedImage);
                UpdateImageDisplay();
            }
        }
        #endregion

        #region Resize
        private void resizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (editedBitmap != null && !string.IsNullOrEmpty(widthTextBox.Text) && !string.IsNullOrEmpty(heightTextBox.Text))
            {
                double newWidth = double.Parse(widthTextBox.Text);
                double newHeight = double.Parse(heightTextBox.Text);

                BitmapSource resizedImage = Resize.ResizeImage(editedBitmap, newWidth, newHeight);
                editedBitmap = new WriteableBitmap(resizedImage);
                UpdateImageDisplay();
            }
        }
        private void resizeToggle_Click(object sender, RoutedEventArgs e)
        {
            if (resizeToggle.IsChecked == true)
            {
                resizePanel.Visibility = Visibility.Visible;
            }
            else
            {
                resizePanel.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Mirror
        private void mirrorButton_Click(object sender, RoutedEventArgs e)
        {
            if (editedBitmap != null)
            {
                WriteableBitmap mirroredBitmap = Mirror.MirrorImageHorizontal(editedBitmap);
                editedBitmap = new WriteableBitmap(mirroredBitmap);
                UpdateImageDisplay();
            }
        }
        #endregion

        #region Flip
        private void flipButton_Click(object sender, RoutedEventArgs e)
        {
            if (editedBitmap != null)
            {
                WriteableBitmap flippedBitmap = Flip.FlipImageVertical(editedBitmap);
                editedBitmap = new WriteableBitmap(flippedBitmap);
                UpdateImageDisplay();
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
            CroppedBitmap croppedImage = Crop.CropImage(editedBitmap, rectCropArea);
            editedBitmap = Crop.ConvertCroppedBitmapToWriteableBitmap(croppedImage);
            editedImage.MouseDown -= EditedImage_MouseDown;
            editedImage.MouseMove -= EditedImage_MouseMove;
            editedImage.MouseUp -= EditedImage_MouseUp;
            editedImage.Cursor = Cursors.Arrow;

            UpdateImageDisplay();
        }
        #endregion

        private void transformationToggle_Click(object sender, RoutedEventArgs e)
        {
            if (transformationToggle.IsChecked == true)
            {
                transformationPanel.Visibility = Visibility.Visible;
            }
            else
            {
                transformationPanel.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Color Adjustments

        #region Exposure

        #region Brightness
        private void brightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (editedBitmap != null)
            {
                brightnessTextBox.Text = brightnessSlider.Value.ToString();
                Image.Brightness = brightnessSlider.Value;
                UpdateImageDisplayWithFilters();
            }
            else
            {
                brightnessSlider.Value = 0;
            }
        }

        private void brightnessTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(brightnessTextBox.Text, out int value))
            {
                if (value >= -100 && value <= 100)
                {
                    brightnessSlider.Value = value;
                }
            }
        }
        #endregion

        #region Contrast
        private void contrastSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (editedBitmap != null)
            {
                contrastTextBox.Text = contrastSlider.Value.ToString();
                //Image.Contrast = contrastSlider.Value;
                UpdateImageDisplayWithFilters();
            }
            else
            {
                contrastSlider.Value = 0;
            }
        }

        private void contrastTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(contrastTextBox.Text, out int value))
            {
                if (value >= 0 && value <= 100)
                {
                    contrastSlider.Value = value;
                }
            }
        }
        #endregion

        #region Highlight
        private void highlightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (editedBitmap != null)
            {
                highlightTextBox.Text = highlightSlider.Value.ToString();
                Image.Highlight = highlightSlider.Value;
                UpdateImageDisplayWithFilters();
            }
            else
            {
                highlightSlider.Value = 0;
            }
        }

        private void highlightTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(highlightTextBox.Text, out int value))
            {
                if (value >= -50 && value <= 50)
                {
                    highlightSlider.Value = value;
                }
            }
        }
        #endregion

        #region Shadow
        private void shadowsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (editedBitmap != null)
            {
                shadowsTextBox.Text = shadowsSlider.Value.ToString();
                Image.Shadows = shadowsSlider.Value;
                UpdateImageDisplayWithFilters();
            }
            else
            {
                shadowsSlider.Value = 0;
            }
        }

        private void shadowsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(shadowsTextBox.Text, out int value))
            {
                if (value >= -50 && value <= 50)
                {
                    shadowsSlider.Value = value;
                }
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
        private async void hueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (editedBitmap != null)
            {
                hueTextBox.Text = hueSlider.Value.ToString();
                Image.Hue = hueSlider.Value;
                UpdateImageDisplayWithFilters();
            }
            else
            {
                hueSlider.Value = 0;
            }
        }
        private void hueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(hueTextBox.Text, out int value))
            {
                if (value >= 0 && value <= 10)
                {
                    hueSlider.Value = value;
                }
            }
        }
        #endregion

        #region Saturation
        private void saturationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (editedBitmap != null)
            {
                saturationTextBox.Text = saturationSlider.Value.ToString();
                Image.Saturation = saturationSlider.Value;
                UpdateImageDisplayWithFilters();
            }
            else
            {
                saturationSlider.Value = 0;
            }
        }

        private void saturationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(saturationTextBox.Text, out int value))
            {
                if (value >= -100 && value <= 100)
                {
                    saturationSlider.Value = value;
                }
            }
        }
        #endregion

        #region Temperature
        private void temperatureSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (editedBitmap != null)
            {
                temperatureTextBox.Text = temperatureSlider.Value.ToString();
                Image.Temperature = temperatureSlider.Value;
                UpdateImageDisplayWithFilters();
            }
            else
            {
                temperatureSlider.Value = 0;
            }
        }
        private void temperatureTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(temperatureTextBox.Text, out int value))
            {
                if (value >= -5 && value <= 5)
                {
                    temperatureSlider.Value = value;
                }
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
                sharpenTextBox.Text = sharpenSlider.Value.ToString();
                Image.Sharpen = sharpenSlider.Value;
                UpdateImageDisplayWithFilters();
            }
            else
            {
                sharpenSlider.Value = 0;
            }
        }

        private void sharpenTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(sharpenTextBox.Text, out int value))
            {
                if (value >= -5 && value <= 10)
                {
                    sharpenSlider.Value = value;
                }
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

        #region Filter
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
                    editedBitmap = new WriteableBitmap(filteredImage);
                    UpdateImageDisplay();
                }
            }
        }

        #endregion

        #region Blur
        private void blurSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (editedBitmap != null)
            {
                blurTextBox.Text = blurSlider.Value.ToString();
                Image.Blur = blurSlider.Value;
                UpdateImageDisplayWithFilters();
            }
        }

        private void blurTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(shadowsTextBox.Text, out int value))
            {
                if (value >= 0 && value <= 20)
                {
                    shadowsSlider.Value = value;
                }
            }
        }
        #endregion

        #region Vibrance
        private void vibranceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (editedBitmap != null)
            {
                vibranceTextBox.Text = vibranceSlider.Value.ToString();
                Image.Vibrance = vibranceSlider.Value;
                UpdateImageDisplayWithFilters();
            }
            else
            {
                vibranceSlider.Value = 0;
            }
        }

        private void vibranceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(vibranceTextBox.Text, out int value))
            {
                if (value >= 0 && value <= 5)
                {
                    vibranceSlider.Value = value;
                }
            }
        }
        #endregion

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
            if (newZoomLevel < 7 && newZoomLevel > -0.1)
            {
                ApplyZoom(newZoomLevel);
                zoomLevel = newZoomLevel;
                zoomSlider.Value = zoomLevel;
            }
        }
        #endregion

        #region Brush
        private SolidColorBrush selectedColor = Brushes.Black;
        private Brush currentBrush = Brushes.Black;
        private bool isBrushDrawingActive = false;
        private bool isDrawing = false;
        double brushSizeValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public SolidColorBrush SelectedColor
        {
            get { return selectedColor; }
            set
            {
                selectedColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedColor"));
            }
        }

        #region Draw
        private void brushEditedImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (editedBitmap != null)
            {
                isDrawing = true;
                var drawedBitmap = Tools.Brush.Draw(editedBitmap, e.GetPosition(editedImage), brushSizeValue, currentBrush, editedImage.ActualWidth, editedImage.ActualHeight);
                editedBitmap = drawedBitmap;
                UpdateImageDisplay();
            }
        }

        private void brushEditedImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && editedBitmap != null)
            {
                brushSizeValue = brushSize.Value;
                var drawedBitmap = Tools.Brush.Draw(editedBitmap, e.GetPosition(editedImage), brushSizeValue, currentBrush, editedImage.ActualWidth, editedImage.ActualHeight);
                editedBitmap = drawedBitmap;
                UpdateImageDisplay();
            }
        }

        private void brushEditedImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
        }
        #endregion

        #region Brush Button
        private void brushButton_Click(object sender, RoutedEventArgs e)
        {
            if (isBrushDrawingActive)
            {
                editedImage.MouseDown -= brushEditedImage_MouseDown;
                editedImage.MouseMove -= brushEditedImage_MouseMove;
                editedImage.MouseUp -= brushEditedImage_MouseUp;
                isBrushDrawingActive = false;
                brushButton.Background = Brushes.White;
                brushButton.Content = "Activate Brushe";
            }
            else
            {
                editedImage.MouseDown += brushEditedImage_MouseDown;
                editedImage.MouseMove += brushEditedImage_MouseMove;
                editedImage.MouseUp += brushEditedImage_MouseUp;
                isBrushDrawingActive = true;
                brushButton.Background = Brushes.Gray;
                brushButton.Content = "Activated";
            }
        }
        #endregion

        #region Brush Properties
        private void brushSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            brushSizeTextBox.Text = brushSize.Value.ToString();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            byte red = (byte)redSlider.Value;
            byte green = (byte)greenSlider.Value;
            byte blue = (byte)blueSlider.Value;
            redTextBox.Text = redSlider.Value.ToString();
            greenTextBox.Text = greenSlider.Value.ToString();
            blueTextBox.Text = blueSlider.Value.ToString();
            currentBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(red, green, blue));
            SelectedColor = (SolidColorBrush)currentBrush;
        }

        private void redTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(redTextBox.Text, out int value))
            {
                if (value >= 0 && value <= 255)
                {
                    redSlider.Value = value;
                }
            }
        }

        private void greenTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(greenTextBox.Text, out int value))
            {
                if (value >= 0 && value <= 255)
                {
                    greenSlider.Value = value;
                }
            }
        }

        private void blueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(blueTextBox.Text, out int value))
            {
                if (value >= 0 && value <= 255)
                {
                    blueSlider.Value = value;
                }
            }
        }

        private void brushSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(brushSizeTextBox.Text, out int value))
            {
                if (value >= 0 && value <= 100)
                {
                    brushSize.Value = value;
                }
            }
        }
        #endregion

        private void brushToggle_Click(object sender, RoutedEventArgs e)
        {
            if (brushToggle.IsChecked == true)
            {
                brushPanel.Visibility = Visibility.Visible;
            }
            else
            {
                brushPanel.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Text
        bool isBold = false;
        bool isItalic = false;
        bool isUnderline = false;
        private bool isTextMoving = false;
        private TextBlock selectedTextBlock;
        private System.Windows.Point mouseOffset;

        private void TextButton_Click(object sender, RoutedEventArgs e)
        {
            if (editedBitmap != null && !String.IsNullOrEmpty(textInput.Text) && fontSizeSlider != null && fontComboBox.SelectedIndex != -1 && colorComboBox.SelectedIndex != -1)
            {
                if (textButton.Content.ToString() == "Edit Text")
                {
                    Brush? selectedBrush = (Brush)new BrushConverter().ConvertFromString(colorComboBox.SelectedItem.ToString());
                    string newText = textInput.Text;
                    int fontSize = (int)fontSizeSlider.Value;
                    string fontFamilyName = fontComboBox.SelectedValue?.ToString();

                    if (!string.IsNullOrWhiteSpace(newText) && fontFamilyName != null)
                    {
                        selectedTextBlock.Text = newText;
                        selectedTextBlock.FontSize = fontSize;
                        selectedTextBlock.FontFamily = new FontFamily(fontFamilyName);
                        selectedTextBlock.Foreground = selectedBrush;
                        selectedTextBlock.FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal;
                        selectedTextBlock.FontStyle = isItalic ? FontStyles.Italic : FontStyles.Normal;
                        selectedTextBlock.TextDecorations = isUnderline ? TextDecorations.Underline : null;
                        deleteButton.Visibility = Visibility.Collapsed;
                    }
                    textButton.Content = "Add Text";
                }
                else if (textButton.Content.ToString() == "Cancel")
                {
                    editedImage.Cursor = Cursors.Arrow;
                    editedImage.MouseLeftButtonDown -= AddText_MouseDown;
                    textButton.Content = "Add Text";
                }
                else
                {
                    editedImage.MouseLeftButtonDown += AddText_MouseDown;
                    editedImage.Cursor = Cursors.IBeam;
                    textButton.Content = "Cancel";
                }
            }
        }

        #region Add
        private void AddText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point position = e.GetPosition(editedImage);

            string text = textInput.Text;//Microsoft.VisualBasic.Interaction.InputBox("Enter text:", "Add Text", "");

            if (!string.IsNullOrWhiteSpace(text))
            {
                AddTextToImage(text, position);
                boldButton.Background = Brushes.White;
            }

            editedImage.Cursor = Cursors.Arrow;
            editedImage.MouseLeftButtonDown -= AddText_MouseDown;
        }

        private void AddTextToImage(string text, System.Windows.Point position)
        {
            Brush selectedBrush = Brushes.White; // Default brush if nothing is selected
            if (colorComboBox.SelectedItem != null)
            {
                var converter = new System.Windows.Media.BrushConverter();
                try
                {
                    selectedBrush = (Brush)converter.ConvertFromString(colorComboBox.SelectedItem.ToString()) ?? Brushes.White;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }


            // Create a new TextBlock
            TextBlock textBlock = new TextBlock
            {
                Text = text,
                FontSize = fontSizeSlider.Value,
                FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal,
                FontStyle = isItalic ? FontStyles.Italic : FontStyles.Normal,
                TextDecorations = isUnderline ? TextDecorations.Underline : null,
                TextAlignment = TextAlignment.Center,
                FontFamily = fontComboBox.FontFamily,
                Foreground = selectedBrush,
            };

            // Set the position of the TextBlock within the Canvas
            Canvas.SetLeft(textBlock, position.X);
            Canvas.SetTop(textBlock, position.Y);

            textBlock.MouseLeftButtonDown += TextBlock_MouseLeftButtonDown;
            textBlock.MouseMove += TextBlock_MouseMove;
            textBlock.MouseLeftButtonUp += TextBlock_MouseLeftButtonUp;
            textBlock.MouseRightButtonDown += TextBlock_MouseRightButtonDown;

            // Add the TextBlock to the textCanvas
            textCanvas.Children.Add(textBlock);
            imageScrollViewer.Cursor = Cursors.Arrow;
            italicButton.Background = Brushes.White;
            underlineButton.Background = Brushes.White;
            boldButton.Background = Brushes.White;
            textInput.Text = "";
        }
        #endregion

        #region Move
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedTextBlock = sender as TextBlock;
            if (selectedTextBlock != null)
            {
                isTextMoving = true;
                mouseOffset = e.GetPosition(selectedTextBlock);
                selectedTextBlock.CaptureMouse();
            }
        }

        private void TextBlock_MouseMove(object sender, MouseEventArgs e)
        {
            if (isTextMoving && selectedTextBlock != null)
            {
                System.Windows.Point mousePos = e.GetPosition(textCanvas);
                double newX = mousePos.X - mouseOffset.X;
                double newY = mousePos.Y - mouseOffset.Y;
                Canvas.SetLeft(selectedTextBlock, newX);
                Canvas.SetTop(selectedTextBlock, newY);
            }
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedTextBlock != null)
            {
                isTextMoving = false;
                selectedTextBlock.ReleaseMouseCapture();
                textButton.Content = "Add Text";
            }
        }
        #endregion

        #region Edit
        private void TextBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            textButton.Content = "Edit Text";
            selectedTextBlock = sender as TextBlock;
            textInput.Text = selectedTextBlock.Text;
            fontSizeSlider.Value = selectedTextBlock.FontSize;
            fontComboBox.SelectedItem = selectedTextBlock.FontFamily;
            colorComboBox.SelectedItem = selectedTextBlock.Foreground;
            isBold = selectedTextBlock.FontWeight == FontWeights.Bold ? true : false;
            boldButton.Background = isBold ? Brushes.Gray : Brushes.White;
            isItalic = selectedTextBlock.FontStyle == FontStyles.Italic ? true : false;
            italicButton.Background = isItalic ? Brushes.Gray : Brushes.White;
            isUnderline = selectedTextBlock.TextDecorations == TextDecorations.Underline ? true : false;
            underlineButton.Background = isUnderline ? Brushes.Gray : Brushes.White;
            deleteButton.Visibility = Visibility.Visible;
        }
        #endregion

        #region Delete
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            textCanvas.Children.Remove(selectedTextBlock);
            deleteButton.Visibility = Visibility.Collapsed;
            textButton.Content = "Add Text";
            italicButton.Background = Brushes.White;
            underlineButton.Background = Brushes.White;
            boldButton.Background = Brushes.White;
            textInput.Text = "";
        }
        #endregion

        #region Style Buttons
        private void boldButton_Click(object sender, RoutedEventArgs e)
        {
            if (isBold == true)
            {
                isBold = false;
                boldButton.Background = Brushes.White;
            }
            else
            {
                isBold = true;
                boldButton.Background = Brushes.Gray;
            }
        }

        private void italicButton_Click(object sender, RoutedEventArgs e)
        {
            if (isItalic == true)
            {
                isItalic = false;
                italicButton.Background = Brushes.White;
            }
            else
            {
                isItalic = true;
                italicButton.Background = Brushes.Gray;
            }
        }

        private void underlineButton_Click(object sender, RoutedEventArgs e)
        {
            if (isUnderline == true)
            {
                isUnderline = false;
                underlineButton.Background = Brushes.White;
            }
            else
            {
                isUnderline = true;
                underlineButton.Background = Brushes.Gray;
            }
        }

        private void fontSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(fontSizeTextBox.Text, out int size))
            {
                if (size > 0)
                {
                    fontSizeSlider.Value = size;
                }
            }
        }
        
        private void fontSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            fontSizeTextBox.Text = fontSizeSlider.Value.ToString();
        }
        #endregion

        private void textToggle_Click(object sender, RoutedEventArgs e)
        {
            if (textToggle.IsChecked == true)
            {
                textPanel.Visibility = Visibility.Visible;
            }
            else
            {
                textPanel.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #endregion

        #region Collage
        Popup popup = new Popup();
        TextBox collageWidthTextBox = new TextBox();
        TextBox collageHeightTextBox = new TextBox();

        #region Create
        private void open4ImagesButton_Click(object sender, RoutedEventArgs e)
        {
            collageCanvas.Children.Clear();
            var images = new List<BitmapImage>();
            //for (int i = 0; i < 4; i++)
            //{
            while(images.Count != 4 ) 
            { 
                BitmapImage image = Open.OpenImage();
                if (image != null)
                {
                    images.Add(image);
                }
                else
                {
                    MessageBox.Show("Please select image");
                }
            }
            CollageFunctions.collageCanvas = collageCanvas;
            collageCanvas = CollageFunctions.AddImagesToCollage(images, 1);
        }

        private void open2VerticalImagesButton_Click(object sender, RoutedEventArgs e)
        {
            collageCanvas.Children.Clear();
            var images = new List<BitmapImage>();
            for(int i = 0; i < 2; i++)
            {
                BitmapImage image = Open.OpenImage();
                if (image != null)
                {
                    images.Add(image);
                }
            }
            CollageFunctions.collageCanvas = collageCanvas;
            collageCanvas = CollageFunctions.AddImagesToCollage(images, 2);
        }

        private void open2HorizontalImagesButton_Click(object sender, RoutedEventArgs e)
        {
            collageCanvas.Children.Clear();
            var images = new List<BitmapImage>();
            for (int i = 0; i < 2; i++)
            {
                BitmapImage image = Open.OpenImage();
                if (image != null)
                {
                    images.Add(image);
                }
            }
            CollageFunctions.collageCanvas = collageCanvas;
            collageCanvas = CollageFunctions.AddImagesToCollage(images, 3);
        }

        private void enterSizesCollageButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(collageWidthTextBox.Text, out double width) && double.TryParse(collageHeightTextBox.Text, out double height))
            {
                editedImage.Width = width;
                editedImage.Height = height;
                popup.Child = null;
                popup.IsOpen = false;
            }
        }
        #endregion

        #region Save
        private void saveCollageButton_Click(object sender, RoutedEventArgs e)
        {
            if (collageCanvas != null)
            {
                Save.SaveCollage(collageCanvas);
            }
        }
        #endregion

        #endregion
    }
}
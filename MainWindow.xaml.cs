using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;
using ImageEditor.Funtionals;
using Rectangle = System.Drawing.Rectangle;
using Brushes = System.Windows.Media.Brushes;
using ImageEditor.Exposure;
using ImageEditor.Color;
using ImageEditor.Transformation;
using SixLabors.ImageSharp;
using ImageEditor.Color_Adjustments;
using System.ComponentModel;
using System.Windows.Documents;


namespace ImageEditor
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<WriteableBitmap> previousVersions = new List<WriteableBitmap>();
        private BitmapImage originalImage;
        private WriteableBitmap editedBitmap;
        private Rectangle rectCropArea = new Rectangle();

        private double zoomLevel = 0;
        private double brightnessValue = 0;
        private int xDown = 0;
        private int yDown = 0;
        private int xUp = 0;
        private int yUp = 0;



        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
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
                editedImage.Source = previousVersion;
            }
        }
        #endregion

        #region Reset
        private void reserButton_Click(object sender, RoutedEventArgs e)
        {
            previousVersions.Clear();
            editedBitmap = new WriteableBitmap(originalImage);
            editedImage.Source = originalImage;
        }
        #endregion

        #region UpdateImage
        private void UpdateImageDisplay()
        {
            if (originalImage != null)
            {

                if (editedBitmap != null)
                {
                    previousVersions.Add(editedBitmap);
                    editedImage.Source = editedBitmap;
                }
                else
                {
                    editedImage.Source = originalImage;
                }
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
                brightnessValue = Math.Min(Math.Max(brightnessSlider.Value, -100), 100);
                BitmapSource adjustedBitmap = Brightness.AdjustBrightness(editedBitmap, brightnessValue);
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
                editedBitmap = Contrast.ApplyContrastFilter(editedBitmap, contrastValue);
                UpdateImageDisplay();
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
                editedBitmap = Highlight.ApplyHighlightFilter(editedBitmap, highlightValue);
                UpdateImageDisplay();
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
                BitmapSource adjustedBitmap = Shadow.AdjustShadows(editedBitmap, shadowsValue);
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
                BitmapSource adjustedBitmap = Hue.AdjustHue(editedBitmap, hueValue);
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
                BitmapSource adjustedBitmap = Saturation.AdjustSaturation(editedBitmap, saturationValue);
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
                BitmapSource adjustedBitmap = Temperature.AdjustTemperature(editedBitmap, temperatureValue);
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
                BitmapSource adjustedBitmap = Sharpen.AdjustSharpen(editedBitmap, sharpenValue);
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
                double blurRadius = blurSlider.Value;
                BitmapSource blurredImage;
                if (blurRadius > 0)
                {
                    blurredImage = Blur.ApplyBlur(editedBitmap, blurRadius);
                }
                else
                {
                    blurredImage = originalImage;
                }
                editedBitmap = new WriteableBitmap(blurredImage);
                UpdateImageDisplay();
            }
        }
        #endregion

        #region Vibrance
        private void vibranceSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (editedBitmap != null)
            {
                double vibranceValue = vibranceSlider.Value;
                BitmapSource adjustedBitmap = Vibrance.AdjustVibrance(editedBitmap, vibranceValue);
                editedBitmap = new WriteableBitmap(adjustedBitmap);
                UpdateImageDisplay();
            }
            else
            {
                vibranceSlider.Value = 0;
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
            if(newZoomLevel < 7 && newZoomLevel > -0.1)
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

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            byte red = (byte)redSlider.Value;
            byte green = (byte)greenSlider.Value;
            byte blue = (byte)blueSlider.Value;
            currentBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(red, green, blue));
            SelectedColor = (SolidColorBrush)currentBrush;
        }

        private void brushButton_Click(object sender, RoutedEventArgs e)
        {
            if (isBrushDrawingActive)
            {
                editedImage.MouseDown -= brushEditedImage_MouseDown;
                editedImage.MouseMove -= brushEditedImage_MouseMove;
                editedImage.MouseUp -= brushEditedImage_MouseUp;
                isBrushDrawingActive = false;
            }
            else
            {
                editedImage.MouseDown += brushEditedImage_MouseDown;
                editedImage.MouseMove += brushEditedImage_MouseMove;
                editedImage.MouseUp += brushEditedImage_MouseUp;
                isBrushDrawingActive = true;
            }
        }

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
                FontStyle = isItalic? FontStyles.Italic: FontStyles.Normal,
                TextDecorations = isUnderline? TextDecorations.Underline: null,
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
            textPanel.Visibility = Visibility.Visible;
            selectedTextBlock = sender as TextBlock;
            textInput.Text = selectedTextBlock.Text;
            fontSizeSlider.Value = selectedTextBlock.FontSize;
            fontComboBox.SelectedItem = selectedTextBlock.FontFamily;
            colorComboBox.SelectedItem = selectedTextBlock.Foreground;
            isBold = selectedTextBlock.FontWeight == FontWeights.Bold ? true : false;
            boldButton.Background = isBold ? Brushes.Gray: Brushes.White;
            isItalic = selectedTextBlock.FontStyle == FontStyles.Italic ? true : false;
            italicButton.Background = isItalic ? Brushes.Gray: Brushes.White;
            isUnderline = selectedTextBlock.TextDecorations == TextDecorations.Underline? true : false;
            underlineButton.Background = isUnderline? Brushes.Gray: Brushes.White;

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
    }
}
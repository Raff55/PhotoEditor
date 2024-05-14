using ImageEditor.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEditor.Collage
{
    public static class CollageFunctions
    {        
        public static BitmapImage originalImage;
        public static Canvas collageCanvas = new Canvas();
        public static List<BitmapImage> selectedImages = new List<BitmapImage>();


        public static void ShowPopup(ref Popup popup, ref TextBox collageWidthTextBox, ref TextBox collageHeightTextBox, RoutedEventHandler clickHandler)
        {
            SolidColorBrush commonColor = new SolidColorBrush((System.Windows.Media.Color)ColorConverter.ConvertFromString("#353536"));

            // Create a StackPanel to hold the controls
            StackPanel stackPanel = new StackPanel();
            //stackPanel.Children.Clear() ;
            stackPanel.Background = commonColor;
            stackPanel.Margin = new Thickness(20);

            TextBlock messageTextBlock = new TextBlock();
            messageTextBlock.Text = Resources.Editor.ENTER_IMAGE_WIDTH;
            messageTextBlock.TextAlignment = TextAlignment.Left;
            messageTextBlock.FontSize = 16;
            messageTextBlock.Foreground = Brushes.White;
            stackPanel.Children.Add(messageTextBlock);

            // Create the first TextBox
            collageWidthTextBox.Margin = new Thickness(0, 10, 0, 10);
            collageWidthTextBox.FontSize = 16;
            collageWidthTextBox.Foreground = Brushes.Black;
            collageWidthTextBox.Background = Brushes.White;
            stackPanel.Children.Add(collageWidthTextBox);

            TextBlock messageTextBlock1 = new TextBlock();
            messageTextBlock1.Text = Resources.Editor.ENTER_IMAGE_HEIGHT;
            messageTextBlock1.TextAlignment = TextAlignment.Left;
            messageTextBlock1.FontSize = 16;
            messageTextBlock1.Foreground = Brushes.White;
            stackPanel.Children.Add(messageTextBlock1);

            // Create the second TextBox
            collageHeightTextBox.Margin = new Thickness(0, 10, 0, 20);
            collageHeightTextBox.FontSize = 16;
            collageHeightTextBox.Foreground = Brushes.Black;
            collageHeightTextBox.Background = Brushes.White;
            stackPanel.Children.Add(collageHeightTextBox);

            Button button = new Button();
            button.Content = Resources.Editor.SUBMIT;
            button.Margin = new Thickness(0, 10, 0, 0);
            button.Padding = new Thickness(10);
            button.FontSize = 16;
            button.Background = Brushes.Gray;
            button.Foreground = Brushes.White;
            button.Click += clickHandler;
            stackPanel.Children.Add(button);

            // Create a Border to wrap the StackPanel
            Border border = new Border();
            border.Background = commonColor;
            border.CornerRadius = new CornerRadius(10);
            border.Child = stackPanel;

            // Clear the existing child of the Popup
            popup.Child = null;

            // Set the Border as the Child of the Popup
            popup.Child = border;

            // Set the dimensions of the Popup
            popup.Width = 350;
            popup.Height = 250;

            // Show the Popup
            popup.IsOpen = true;
        }

        public static WriteableBitmap RenderCanvasToImage(Canvas canvas)
        {
            // Create a RenderTargetBitmap
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);

            // Render the Canvas onto the RenderTargetBitmap
            rtb.Render(canvas);

            // Convert RenderTargetBitmap to a BitmapSource
            BitmapSource bitmap = rtb;

            // Convert BitmapSource to BitmapImage
            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new PngBitmapEncoder(); // or any other encoder you prefer
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(stream);

            originalImage = new BitmapImage();
            originalImage.BeginInit();
            originalImage.StreamSource = stream;
            originalImage.EndInit();

            return new WriteableBitmap(bitmap);
        }

        public static Canvas AddImagesToCollage(List<BitmapImage> images, int templateNumber)
        {
            collageCanvas.Children.Clear();
            foreach (var image in images)
            {
                switch (templateNumber)
                {
                    case 1:
                        if (selectedImages.Count < 4)
                        {
                            selectedImages.Add(image);
                            if (selectedImages.Count == 4)
                                UpdateCollageDisplay(2, 2);
                        }
                        else
                            MessageBox.Show(Editor.MAX_LIMIT_4);
                        break;
                    case 2:
                        if (selectedImages.Count < 2)
                        {
                            selectedImages.Add(image);
                            if(selectedImages.Count == 2)
                                UpdateCollageDisplay(2, 0);
                        }
                        else
                            MessageBox.Show(Editor.MAX_LIMIT_2);
                        break;
                    case 3:
                        if (selectedImages.Count < 2)
                        {
                            selectedImages.Add(image);
                            if (selectedImages.Count == 2)
                                UpdateCollageDisplay(0, 2);
                        }
                        else
                            MessageBox.Show(Editor.MAX_LIMIT_2);
                        break;
                }
            }
            selectedImages.Clear();
            return collageCanvas;
        }

        public static void UpdateCollageDisplay(int vertical, int horizontal)
        {
            collageCanvas.Children.Clear();
            int numRows = horizontal == 0 ? 1 : horizontal;
            int numCols = vertical == 0 ? 1 : vertical;
            double cellWidth = collageCanvas.ActualWidth / numCols;
            double cellHeight = collageCanvas.ActualHeight / numRows;

            for (int i = 0; i < selectedImages.Count; i++)
            {
                System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                img.Stretch = Stretch.UniformToFill;
                img.Source = selectedImages[i];
                img.Width = cellWidth;
                img.Height = cellHeight;

                int row = i / numCols;
                int col = i % numCols;
                Canvas.SetLeft(img, col * cellWidth);
                Canvas.SetTop(img, row * cellHeight);
                collageCanvas.Children.Add(img);
            }
        }
    }
}

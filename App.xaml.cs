using Haley.Utils;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Windows;

namespace ImageEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //public App()
        //{
        //    //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("hy-AM");
        //    LangUtils.Register();
        //}

        public static void Culture_HY()
        {
            // Set the current UI culture to Armenian (Armenia).
            CultureInfo culture = new CultureInfo("hy-AM");
            Thread.CurrentThread.CurrentUICulture = culture;

            // Adjust the base name to include the namespace and folder path.
            string baseName = "ImageEditor.Resources.Editor";

            ResourceManager resourceManager = new ResourceManager(baseName, typeof(MainWindow).Assembly);

            try
            {
                var resourceSet = resourceManager.GetResourceSet(culture, true, true);

                ResourceDictionary resourceDictionary = new ResourceDictionary();
                foreach (DictionaryEntry resource in resourceSet)
                {
                    string key = (string)resource.Key;
                    object value = resource.Value;

                    // Add the resource to the ResourceDictionary
                    if (resourceDictionary.Contains(key))
                    {
                        resourceDictionary[key] = value;
                    }
                    else
                    {
                        resourceDictionary.Add(key, value);
                    }
                }

                // Clear existing merged dictionaries
                Application.Current.Resources.MergedDictionaries.Clear();
                // Add the new dictionary
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);

                // Force UI refresh by reloading the window or updating bindings
                foreach (Window window in Application.Current.Windows)
                {
                    window.Resources = Application.Current.Resources;
                }
            }
            catch (MissingManifestResourceException ex)
            {
                MessageBox.Show($"Resource not found: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void Culture_RU()
        {
            // Set the current UI culture to Armenian (Armenia).
            CultureInfo culture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentUICulture = culture;

            // Adjust the base name to include the namespace and folder path.
            string baseName = "ImageEditor.Resources.Editor";

            ResourceManager resourceManager = new ResourceManager(baseName, typeof(MainWindow).Assembly);

            try
            {
                var resourceSet = resourceManager.GetResourceSet(culture, true, true);

                ResourceDictionary resourceDictionary = new ResourceDictionary();
                foreach (DictionaryEntry resource in resourceSet)
                {
                    string key = (string)resource.Key;
                    object value = resource.Value;

                    // Add the resource to the ResourceDictionary
                    if (resourceDictionary.Contains(key))
                    {
                        resourceDictionary[key] = value;
                    }
                    else
                    {
                        resourceDictionary.Add(key, value);
                    }
                }

                // Clear existing merged dictionaries
                Application.Current.Resources.MergedDictionaries.Clear();
                // Add the new dictionary
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);

                // Force UI refresh by reloading the window or updating bindings
                foreach (Window window in Application.Current.Windows)
                {
                    window.Resources = Application.Current.Resources;
                }
            }
            catch (MissingManifestResourceException ex)
            {
                MessageBox.Show($"Resource not found: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void Culture_EN()
        {
            // Set the current UI culture to Armenian (Armenia).
            CultureInfo culture = new CultureInfo("en");
            Thread.CurrentThread.CurrentUICulture = culture;

            // Adjust the base name to include the namespace and folder path.
            string baseName = "ImageEditor.Resources.Editor";

            ResourceManager resourceManager = new ResourceManager(baseName, typeof(MainWindow).Assembly);

            try
            {
                var resourceSet = resourceManager.GetResourceSet(culture, true, true);

                ResourceDictionary resourceDictionary = new ResourceDictionary();
                foreach (DictionaryEntry resource in resourceSet)
                {
                    string key = (string)resource.Key;
                    object value = resource.Value;

                    // Add the resource to the ResourceDictionary
                    if (resourceDictionary.Contains(key))
                    {
                        resourceDictionary[key] = value;
                    }
                    else
                    {
                        resourceDictionary.Add(key, value);
                    }
                }

                // Clear existing merged dictionaries
                Application.Current.Resources.MergedDictionaries.Clear();
                // Add the new dictionary
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);

                // Force UI refresh by reloading the window or updating bindings
                foreach (Window window in Application.Current.Windows)
                {
                    window.Resources = Application.Current.Resources;
                }
            }
            catch (MissingManifestResourceException ex)
            {
                MessageBox.Show($"Resource not found: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

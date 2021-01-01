using System;
using System.Collections.Generic;
using System.Windows;

namespace ModernWpf.Toolkit.UI
{
    public class ToolkitThemeDictionary
    {
        internal const string LightKey = "Light";
        internal const string DarkKey = "Dark";
        internal const string HighContrastKey = "HighContrast";

        private static Dictionary<string, ResourceDictionary> _defaultThemeDictionaries = new();

        public static void SetKey(ResourceDictionary themeDictionary, string key)
        {
            var baseThemeDictionary = GetToolkitThemeDictionary(key);
            themeDictionary.MergedDictionaries.Add(baseThemeDictionary);
        }

        private static ResourceDictionary GetToolkitThemeDictionary(string key)
        {
            if (!_defaultThemeDictionaries.TryGetValue(key, out ResourceDictionary dictionary))
            {
                dictionary = new ResourceDictionary { Source = GetDefaultSource(key) };
                _defaultThemeDictionaries[key] = dictionary;
            }
            return dictionary;
        }

        private static Uri GetDefaultSource(string theme)
        {
            return new Uri($"pack://application:,,,/ModernWpf.Toolkit.UI;component/ThemeResources/{theme}.xaml");
        }

        public static void SetMergeOnto(ResourceDictionary themeDictionary, string key)
        {
            var baseThemeDictionary = GetToolkitThemeDictionary(key);
            baseThemeDictionary.MergedDictionaries.Add(themeDictionary);
        }
    }
}

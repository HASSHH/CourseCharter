﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace WoTMapWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        string settingsFileName = "Settings.xaml";

        public ResourceDictionary ThemeDictionary
        {
            get { return Resources.MergedDictionaries[0]; }
        }

        public ResourceDictionary SettingsDictionary
        {
            get { return Resources.MergedDictionaries[1]; }
        }

        public void ChangeTheme(string themeFileName)
        {
            string themePath = "Themes/" + themeFileName;
            Uri themeUri = new Uri(themePath, UriKind.RelativeOrAbsolute);
            ThemeDictionary.MergedDictionaries.Clear();
            ThemeDictionary.MergedDictionaries.Add(new ResourceDictionary() { Source = themeUri });
        }

        public void RestoreDefaultSettings()
        {
            SettingsDictionary.MergedDictionaries.Clear();
            SettingsDictionary.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("DefaultSettings.xaml", UriKind.Relative) });
        }

        public void ChangeUserSetting(string settingName, object value)
        {
            SettingsDictionary.MergedDictionaries[0][settingName] = value;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string appTitle = "CourseCharter";
            string saveLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + appTitle;
            Resources["SaveLocation"] = saveLocation;
            string settingsFile = $"{saveLocation}\\{settingsFileName}";
            if (File.Exists(settingsFile))
            {
                using (FileStream stream = File.OpenRead(settingsFile))
                {
                    ResourceDictionary rd = (ResourceDictionary)XamlReader.Load(stream);
                    ResourceDictionary defaultDict = new ResourceDictionary() { Source = new Uri("DefaultSettings.xaml", UriKind.Relative) };
                    foreach (string key in defaultDict.Keys)
                    {
                        if (!rd.Contains(key))
                            rd.Add(key, defaultDict[key]);
                    }
                    //replace default settings dict with user specific settings
                    SettingsDictionary.MergedDictionaries.Clear();
                    SettingsDictionary.MergedDictionaries.Add(rd);
                }
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Indent = true;
            writerSettings.IndentChars = "\t";
            string saveLocation = (string)Resources["SaveLocation"];
            string settingsFile = $"{saveLocation}\\{settingsFileName}";
            Directory.CreateDirectory(saveLocation);
            using (FileStream stream = File.Create(settingsFile))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stream, writerSettings))
                {
                    ResourceDictionary resourceDictionary = SettingsDictionary.MergedDictionaries[0];
                    XamlWriter.Save(resourceDictionary, xmlWriter);
                }
            }
        }
    }
}

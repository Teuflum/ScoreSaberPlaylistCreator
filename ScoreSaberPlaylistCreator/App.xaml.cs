using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace ScoreSaberPlaylistCreator
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public static string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static int SongLimit = ScoreSaberPlaylistCreator.Properties.Settings.Default.SongLimit;
        public static string BeatSaberPath = ScoreSaberPlaylistCreator.Properties.Settings.Default.BeatSaberPath;
    }
}

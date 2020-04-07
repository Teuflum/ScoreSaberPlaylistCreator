using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using ScoreSaberPlaylistCreator.Classes;

namespace ScoreSaberPlaylistCreator
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Playlist _playlist;
        private List<PlaylistSong> _songlist;
        private string _userAgent = $"ScoreSaberPlaylistCreator/{App.Version}";
        private int _requestTimeout = 10000;

        public int SongLimit { get; set; }
        public string BeatSaberPath { get; set; }

        public MainWindow()
        {
            DataContext = this;
            SongLimit = App.SongLimit;
            BeatSaberPath = App.BeatSaberPath;

            InitializeComponent();
        }

        private void BtnCreatePlaylist_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                btnCreatePlaylist.IsEnabled = false;
                numLimit.IsEnabled = false;
            });

            BackgroundWorker bg = new BackgroundWorker();
            _songlist = new List<PlaylistSong>();
            int limit = numLimit.Value.Value;

            Dispatcher.Invoke(() =>
            {
                prgMain.Value = 0;
                txtMain.Text = "";
            });

            bg.DoWork += async (senderBG, eBG) =>
            {
                int page = 0;
                int pageLimit = 200;
                while (_songlist.Count < limit)
                {
                    var uri = new Uri($"http://scoresaber.com/api.php?function=get-leaderboards&cat=3&page={page + 1}&limit={pageLimit}&ranked=1");
                    WebClient client = new WebClient();
                    client.Headers.Add(HttpRequestHeader.UserAgent, _userAgent);
                    ScoresaberSongRootObject songList;
                    Dispatcher.Invoke(() => txtMain.AppendTextExt($"Trying to get page {page + 1} of ranked songs ({pageLimit} limit) from ScoreSaber..."));
                    try
                    {
                        string jsonResult = "";
                        jsonResult = await client.DownloadStringTaskAsync(uri);
                        try
                        {
                            songList = JsonConvert.DeserializeObject<ScoresaberSongRootObject>(jsonResult);
                        }
                        catch
                        {
                            Dispatcher.Invoke(() =>
                            {
                                txtMain.AppendTextExt($"Got something weird back from ScoreSaber:{Environment.NewLine}{jsonResult}");
                            });
                            break;
                        }
                        foreach (var song in songList.songs)
                        {
                            if (_songlist.Count == limit)
                                break;
                            if (_songlist.Where(x => x.Hash == song.id).Count() > 0)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    txtMain.AppendTextExt($"Skipping {song.name} " +
                                        $"{(String.IsNullOrWhiteSpace(song.songSubName) ? "" : song.songSubName + " ")}" +
                                        $"- {song.songAuthorName} mapped by {song.levelAuthorName}.");
                                });
                            }
                            else
                            {
                                BeatsaverSong beatsaverSong;

                                try
                                {
                                    var jsonResultBeatsaver = "";
                                    client.Headers.Add(HttpRequestHeader.UserAgent, _userAgent);
                                    jsonResultBeatsaver = await client.DownloadStringTaskAsync($"https://beatsaver.com/api/maps/by-hash/{song.id}");
                                    try
                                    {
                                        beatsaverSong = JsonConvert.DeserializeObject<BeatsaverSong>(jsonResultBeatsaver);
                                    }
                                    catch
                                    {
                                        Dispatcher.Invoke(() =>
                                        {
                                            txtMain.AppendTextExt($"Got something weird back from BeatSaver:{Environment.NewLine}{jsonResultBeatsaver}");
                                        });
                                        break;
                                    }
                                    _songlist.Add(new PlaylistSong(song.name, beatsaverSong.key, song.id));
                                    Dispatcher.Invoke(() =>
                                    {
                                        txtMain.AppendTextExt($"Adding {song.name} " +
                                        $"{(String.IsNullOrWhiteSpace(song.songSubName) ? "" : song.songSubName + " ")}" +
                                        $"- {song.songAuthorName} mapped by {song.levelAuthorName} ({song.stars}⭐).");
                                        prgMain.Value++;
                                    });
                                }
                                catch (WebException we)
                                {
                                    HttpWebResponse errorResponse = we.Response as HttpWebResponse;
                                    if (errorResponse != null && errorResponse.StatusCode == HttpStatusCode.NotFound)
                                    {
                                        Dispatcher.Invoke(() =>
                                        {
                                            txtMain.AppendTextExt($"Skipping {song.name} " +
                                        $"{(String.IsNullOrWhiteSpace(song.songSubName) ? "" : song.songSubName + " ")}" +
                                        $"- {song.songAuthorName} mapped by {song.levelAuthorName}. (404 NOT FOUND)");
                                        });
                                    }
                                    else if(errorResponse.StatusCode == HttpStatusCode.Forbidden)
                                    {
                                        Dispatcher.Invoke(() =>
                                        {
                                            txtMain.AppendTextExt($"Skipping {song.name} " +
                                        $"{(String.IsNullOrWhiteSpace(song.songSubName) ? "" : song.songSubName + " ")}" +
                                        $"- {song.songAuthorName} mapped by {song.levelAuthorName}. (403 FORBIDDEN)");
                                        });
                                    }
                                    else if (we.HResult == -2146233079)
                                    {
                                        Dispatcher.Invoke(() =>
                                        {
                                            txtMain.AppendTextExt($"Reached the timeout of {Math.Round(_requestTimeout / 1000.0, 2)} seconds, trying again...");
                                        });
                                    }
                                    else
                                    {
                                        Dispatcher.Invoke(() =>
                                        {
                                            txtMain.AppendTextExt($"The following error occured: {we.Message}{Environment.NewLine}{we.StackTrace}");
                                        });
                                    }
                                }
                            }
                        }
                        page++;
                    }
                    catch (Exception ex)
                    {
                        if (ex.HResult == -2146233079)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                txtMain.AppendTextExt($"Reached the timeout of {Math.Round(_requestTimeout / 1000.0, 2)} seconds, trying again...");
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                txtMain.AppendTextExt($"The following error occured: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                            });
                        }

                    }

                }


                _playlist = new Playlist(_songlist);

                if (!Directory.Exists(Path.Combine(BeatSaberPath, "Playlists")))
                    Directory.CreateDirectory(Path.Combine(BeatSaberPath, "Playlists"));
                File.WriteAllText(Path.Combine(BeatSaberPath, "Playlists", "RankedSongs.json"),
                    JsonConvert.SerializeObject(_playlist, Formatting.None), new UTF8Encoding(false));

                Dispatcher.Invoke(() =>
                {
                    btnCreatePlaylist.IsEnabled = true;
                    numLimit.IsEnabled = true;
                });
            };

            bg.RunWorkerAsync();
        }

        private void NumLimit_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                BtnCreatePlaylist_Click(this, null);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.SongLimit = SongLimit;
            Properties.Settings.Default.BeatSaberPath = BeatSaberPath;
            Properties.Settings.Default.Save();
        }
    }
}

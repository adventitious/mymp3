﻿using System;
using System.Windows;
using System.Windows.Media;

using System.Timers;
using System.Threading;

namespace mp3player
{
    public partial class MainWindow : Window
    {
        public MediaPlayer mediaPlayer;
        bool Paused = false;
        bool CodeEvent = false;

        System.Timers.Timer aTimer;
        Playlist Playlist;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mediaPlayer = new MediaPlayer();

            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 100;
        }
        private void Mp3_Play_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
            /*
            if (Playlist != null)
            {
                Playlist.Close();
            }
            */
        }

        private void Btn_Play_Click(object sender, RoutedEventArgs e)
        {
            if( Paused )
            {
                UnPause();
                return;
            }
            Open();
            Play();
        }



        private void Btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            aTimer.Enabled = false;

            mediaPlayer.Stop();
            UpdateInfo();
        }

        private void Btn_Pause_Click(object sender, RoutedEventArgs e)
        {
            // aTimer.Enabled = false;

            if( Paused )
            {
                UnPause();
            }
            else
            {
                mediaPlayer.Pause();
                aTimer.Enabled = false;
                Paused = true;
            }
            UpdateInfo();
        }

        private void Btn_Playlist_Click(object sender, RoutedEventArgs e)
        {
            if( Playlist != null )
            {
                if (Playlist.IsVisible)
                {
                    Playlist.Hide();
                }
                else
                {
                    Playlist.Show();
                }
            }
            else
            {
                Playlist = new Playlist( this ); 
                Playlist.Show();
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() => {
                    // Code causing the exception or requires UI thread access
                    // https://stackoverflow.com/questions/21306810/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
                    UpdateInfo();
                });
            }
            catch (Exception)
            {
                // MessageBox.Show(eee.Message);
            }
        }


        public void UpdateInfo()
        {
            double Position = mediaPlayer.Position.TotalSeconds;
            Duration t2 = mediaPlayer.NaturalDuration;

            /*
            if (!mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                Thread.Sleep(100);
            }
            */

            if (!t2.HasTimeSpan)
            {
                TxBk_Info.Text = ("0:00");
                return;
            }
            
                
            try
            {
                double total = t2.TimeSpan.TotalSeconds;
                double remaining = total - Position;
                // Display the time remaining in the current media.

                if (remaining == 0 )
                {
                    // MessageBox.Show("ending");
                    Playlist.NextTrack();
                }

                TxBk_Info.Text = (SecondsToText((int)Math.Floor(remaining)));

                Prg_Bar.Value = 100 / (total / Position);
                SetSlider( 100 / (total / Position) / 10 );
            }
            catch (Exception )
            {
                // MessageBox.Show(eee.Message);
            }
        }


        private string SecondsToText( int seconds )
        {
            int seconds2 = seconds % 60;
            if ( seconds2 < 10 )
            {
                return "" + seconds / 60 + ":0" + seconds2;
            }
            return "" + seconds / 60 + ":" + seconds2;
        }


        private void UnPause()
        {
            mediaPlayer.Play();
            aTimer.Enabled = true;
            Paused = false;
        }

        public void Open(string file)
        {
            mediaPlayer.Open(new System.Uri(file));
        }

        private void Open()
        {
            Open(Txb_File.Text);
        }

        public void Play()
        {
            mediaPlayer.Play();
            aTimer.Enabled = true;

            // Thread.Sleep(300);



            //UpdateInfo();
        }

        public void Seek( double sld )
        {
            // if stopped Sld_Position.Value = 0, return

            double total = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            double newPos = 0.1 * sld * total;
            mediaPlayer.Position = TimeSpan.FromSeconds( newPos );
        }

        private void SetSlider( double newVal )
        {
            CodeEvent = true;
            Sld_Position.Value = newVal;
            CodeEvent = false;
        }


        private void Sld_Position_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if( CodeEvent )
            {
                return;
            }
            //mediaPlayer.Position = TimeSpan.FromSeconds(100);
            // MessageBox.Show("" + Sld_Position.Value);
            Seek(Sld_Position.Value);
        }

        private void Slider_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SliderStereo.Value = 10; 
        }
    }
}


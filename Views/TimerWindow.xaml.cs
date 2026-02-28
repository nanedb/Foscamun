using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Foscamun.Views
{
    public partial class TimerWindow : Window
    {
        private readonly DispatcherTimer _timer = new();
        private TimeSpan _remainingTime = TimeSpan.Zero;
        private TimeSpan _initialTime = TimeSpan.Zero;
        private bool _running = false;
        private bool _isOvertime = false;
        private readonly MediaPlayer _tickPlayer = new();
        private readonly MediaPlayer _bellPlayer = new();
        private bool _tickSoundPlaying = false;

        public TimerWindow()
        {
            InitializeComponent();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;

            // Initialize audio players
            try
            {
                string tickPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Sounds", "tick.wav");
                string bellPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Sounds", "bell.wav");

                if (File.Exists(tickPath))
                {
                    _tickPlayer.Open(new Uri(tickPath, UriKind.Absolute));
                }

                if (File.Exists(bellPath))
                {
                    _bellPlayer.Open(new Uri(bellPath, UriKind.Absolute));
                }
            }
            catch
            {
                // Silently handle audio initialization errors
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Toggle Start/Pause with spacebar
            if (e.Key == Key.Space)
            {
                e.Handled = true; // Prevent the spacebar from triggering focused buttons
                StartPauseBtn_Click(this, new RoutedEventArgs());
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_running)
            {
                if (!_isOvertime)
                {
                    // Count down
                    _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));

                    if (_remainingTime.TotalSeconds <= 0)
                    {
                        // Timer reached zero - stop tick and play bell, enter overtime mode
                        StopTickSound();
                        PlayBellSound();
                        _isOvertime = true;
                        _remainingTime = TimeSpan.Zero;
                        UpdateTimerDisplay();
                    }
                    else
                    {
                        // Start tick sound once when reaching 10 seconds
                        if (_remainingTime.TotalSeconds == 10 && !_tickSoundPlaying)
                        {
                            PlayTickSound();
                        }
                        UpdateTimerDisplay();
                    }
                }
                else
                {
                    // Count up in overtime
                    _remainingTime = _remainingTime.Add(TimeSpan.FromSeconds(1));
                    UpdateTimerDisplay();
                }
            }
        }

        private void PlayTickSound()
        {
            try
            {
                _tickPlayer.Position = TimeSpan.Zero;
                _tickPlayer.Play();
                _tickSoundPlaying = true;
            }
            catch
            {
                // Silently handle audio playback errors
            }
        }

        private void StopTickSound()
        {
            try
            {
                if (_tickSoundPlaying)
                {
                    _tickPlayer.Stop();
                    _tickSoundPlaying = false;
                }
            }
            catch
            {
                // Silently handle audio stop errors
            }
        }

        private void PlayBellSound()
        {
            try
            {
                _bellPlayer.Position = TimeSpan.Zero;
                _bellPlayer.Play();
            }
            catch
            {
                // Silently handle audio playback errors
            }
        }

        private void UpdateTimerDisplay()
        {
            var minutes = (int)Math.Abs(_remainingTime.TotalMinutes);
            var seconds = Math.Abs(_remainingTime.Seconds);

            if (_isOvertime)
            {
                TimeText.Text = $"+{minutes:00}:{seconds:00}";
            }
            else
            {
                TimeText.Text = $"{minutes:00}:{seconds:00}";
            }

            UpdateTimerColors();
        }

        private void UpdateTimerColors()
        {
            if (_isOvertime)
            {
                // Black text on red background
                TimerBorder.Background = new SolidColorBrush(Colors.Red);
                TimeText.Foreground = new SolidColorBrush(Colors.Black);
            }
            else if (_remainingTime.TotalSeconds <= 10 && _remainingTime.TotalSeconds > 0)
            {
                // Red text in last 10 seconds
                TimerBorder.Background = new SolidColorBrush(Colors.Transparent);
                TimeText.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                // Normal white text
                TimerBorder.Background = new SolidColorBrush(Colors.Transparent);
                TimeText.Foreground = (SolidColorBrush)FindResource("TextPrimaryBrush");
            }
        }

        private void SetBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_running)
            {
                MessageBox.Show("Stop the timer before setting a new time.", "Timer Running", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (int.TryParse(MinutesInput.Text, out int minutes) && 
                int.TryParse(SecondsInput.Text, out int seconds))
            {
                if (minutes < 0 || seconds < 0 || seconds >= 60)
                {
                    MessageBox.Show("Please enter valid values (Minutes ≥ 0, Seconds 0-59).", 
                        "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _initialTime = new TimeSpan(0, minutes, seconds);
                _remainingTime = _initialTime;
                _isOvertime = false;
                UpdateTimerDisplay();
            }
            else
            {
                MessageBox.Show("Please enter valid numbers.", "Invalid Input", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void StartPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_running)
            {
                // Pause
                _running = false;
                _timer.Stop();
                StopTickSound();
            }
            else
            {
                // Start
                if (_initialTime.TotalSeconds == 0 && _remainingTime.TotalSeconds == 0 && !_isOvertime)
                {
                    MessageBox.Show(
                        (string)FindResource("NoTimeSetMsg"),
                        (string)FindResource("NoTimeSetTitle"), 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Information);
                    return;
                }

                _running = true;
                _timer.Start();

                // Restart tick sound if we're resuming within the last 10 seconds
                if (!_isOvertime && _remainingTime.TotalSeconds <= 10 && _remainingTime.TotalSeconds > 0)
                {
                    PlayTickSound();
                }
            }

            UpdateStartPauseButton();
        }

        private void UpdateStartPauseButton()
        {
            if (_running)
            {
                StartPauseIcon.Source = new Uri("pack://application:,,,/Resources/Icons/pause.svg");
                StartPauseText.Text = (string)FindResource("PauseBtn");
            }
            else
            {
                StartPauseIcon.Source = new Uri("pack://application:,,,/Resources/Icons/start.svg");
                StartPauseText.Text = (string)FindResource("StartBtn");
            }
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            _running = false;
            _timer.Stop();
            StopTickSound();
            _remainingTime = _initialTime;
            _isOvertime = false;
            UpdateTimerDisplay();
            UpdateStartPauseButton();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

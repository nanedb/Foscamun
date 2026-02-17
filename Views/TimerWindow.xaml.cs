using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Foscamun2026.Views
{
    public partial class TimerWindow : Window
    {
        private readonly DispatcherTimer _timer = new();
        private TimeSpan _remainingTime = TimeSpan.Zero;
        private TimeSpan _initialTime = TimeSpan.Zero;
        private bool _running = false;
        private bool _isOvertime = false;

        public TimerWindow()
        {
            InitializeComponent();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
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
                        // Enter overtime mode
                        _isOvertime = true;
                        _remainingTime = TimeSpan.Zero;
                        UpdateTimerDisplay();
                    }
                    else
                    {
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

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_initialTime.TotalSeconds == 0 && _remainingTime.TotalSeconds == 0 && !_isOvertime)
            {
                MessageBox.Show("Please set a time first using the Set button.", "No Time Set", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _running = true;
            _timer.Start();
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            _running = false;
            _timer.Stop();
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            _running = false;
            _timer.Stop();
            _remainingTime = _initialTime;
            _isOvertime = false;
            UpdateTimerDisplay();
        }
    }
}

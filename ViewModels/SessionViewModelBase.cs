using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Foscamun.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Media;
using System.Windows.Data;
using System.Windows.Threading;

namespace Foscamun.ViewModels
{
    /// <summary>
    /// A speaking-time preset shown as a chip under the timer.
    /// </summary>
    public partial class SpeakingTimePreset : ObservableObject
    {
        public int Seconds { get; }
        public string Label => $"{Seconds / 60}:{Seconds % 60:00}";

        [ObservableProperty]
        private bool isSelected;

        public SpeakingTimePreset(int seconds) => Seconds = seconds;
    }

    /// <summary>
    /// What the shared session panel (<see cref="Views.SessionSpeakersPanel"/>) needs,
    /// whatever kind of speaker the session has.
    /// </summary>
    public interface ISessionSpeakers
    {
        void AddSpeaker(object? speaker);
        void RemoveSpeaker(object? speaker);
        void Activate();
        void Deactivate();
        IRelayCommand StartPauseCommand { get; }
    }

    /// <summary>
    /// Shared logic of the committee and ICJ session pages: available and speakers lists
    /// (the head of the speakers list is the one speaking), the speaking-time clock with
    /// its sounds, and warnings. Subclasses say how warnings are saved and who votes.
    /// </summary>
    public abstract partial class SessionViewModelBase<TSpeaker> : ObservableObject, ISessionSpeakers
        where TSpeaker : class, ISpeaker
    {
        private const int TimeStepSeconds = 10;
        private const int MinSpeakingSeconds = 10;
        private const int MaxSpeakingSeconds = 10 * 60;
        private const int FinalSecondsThreshold = 10;

        private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
        // SoundPlayer loads a WAV silently (MediaPlayer gave an audible click on Open)
        private SoundPlayer? _tickPlayer;
        private SoundPlayer? _bellPlayer;
        private bool _tickPlaying;
        private bool _speechStarted; // the countdown has run at least once for the current speaker
        private TSpeaker? _lastSpeaker;

        public int Session { get; }

        public ObservableCollection<TSpeaker> AvailableSpeakers { get; } = new();
        public ObservableCollection<TSpeaker> SpeakersList { get; } = new();
        public ObservableCollection<TSpeaker> WarnedList { get; } = new();
        public ObservableCollection<SpeakingTimePreset> Presets { get; } =
            new(new[] { 60, 90, 120, 180 }.Select(s => new SpeakingTimePreset(s)));

        /// <summary>The one speaking now: the head of the speakers list.</summary>
        public TSpeaker? CurrentSpeaker => SpeakersList.Count > 0 ? SpeakersList[0] : null;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveWarningCommand))]
        private TSpeaker? selectedWarned;

        // ---------------- Clock state ----------------

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SpeakingTimeText))]
        private TimeSpan speakingTime = TimeSpan.FromSeconds(90);

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TimerText))]
        [NotifyPropertyChangedFor(nameof(IsFinalSeconds))]
        private TimeSpan remaining = TimeSpan.FromSeconds(90);

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TimerText))]
        [NotifyPropertyChangedFor(nameof(IsFinalSeconds))]
        private bool isOvertime;

        [ObservableProperty]
        private bool isRunning;

        public string SpeakingTimeText => Format(SpeakingTime);
        public string TimerText => (IsOvertime ? "+" : "") + Format(Remaining);

        /// <summary>Last seconds before time runs out: the countdown turns red.</summary>
        public bool IsFinalSeconds => !IsOvertime && Remaining.TotalSeconds <= FinalSecondsThreshold;

        protected SessionViewModelBase(int session, IEnumerable<TSpeaker> present)
        {
            Session = session;
            foreach (var speaker in present)
            {
                AvailableSpeakers.Add(speaker);
            }

            SpeakersList.CollectionChanged += SpeakersList_CollectionChanged;
            _timer.Tick += Timer_Tick;
            _tickPlayer = LoadSound("tick.wav");
            _bellPlayer = LoadSound("bell.wav");
            UpdatePresetSelection();
        }

        /// <summary>Saves a speaker's warning count.</summary>
        protected abstract Task SaveWarningsAsync(TSpeaker speaker);

        // ---------------- Page lifetime ----------------

        /// <summary>Called when the page is shown (also when coming back from voting).</summary>
        public void Activate()
        {
            App.LanguageChanged -= OnLanguageChanged;
            App.LanguageChanged += OnLanguageChanged;
            OnLanguageChanged();
        }

        /// <summary>Called when the page is left: the clock must not keep running unseen.</summary>
        public void Deactivate()
        {
            App.LanguageChanged -= OnLanguageChanged;
            PauseClock();
        }

        private void OnLanguageChanged()
        {
            CollectionViewSource.GetDefaultView(AvailableSpeakers).Refresh();
            CollectionViewSource.GetDefaultView(SpeakersList).Refresh();
            CollectionViewSource.GetDefaultView(WarnedList).Refresh();
            OnPropertyChanged(nameof(CurrentSpeaker));
        }

        // ---------------- Speakers ----------------

        /// <summary>Moves someone from the available list to the end of the speakers list.</summary>
        public void AddSpeaker(object? item)
        {
            if (item is not TSpeaker speaker || SpeakersList.Contains(speaker)) return;
            AvailableSpeakers.Remove(speaker);
            SpeakersList.Add(speaker);
        }

        /// <summary>Takes someone off the speakers list and back to the available list.</summary>
        public void RemoveSpeaker(object? item)
        {
            if (item is not TSpeaker speaker || !SpeakersList.Remove(speaker)) return;
            if (!AvailableSpeakers.Contains(speaker))
            {
                AvailableSpeakers.Add(speaker);
            }
        }

        private void SpeakersList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (ReferenceEquals(CurrentSpeaker, _lastSpeaker)) return;

            // A new speaker takes the floor: fresh clock, not yet started
            _lastSpeaker = CurrentSpeaker;
            ResetClock();
            OnPropertyChanged(nameof(CurrentSpeaker));
            StartPauseCommand.NotifyCanExecuteChanged();
            NextSpeakerCommand.NotifyCanExecuteChanged();
            WarnCurrentSpeakerCommand.NotifyCanExecuteChanged();
        }

        private bool HasSpeaker() => CurrentSpeaker != null;

        /// <summary>The current speaker is done: back to the available list, the next one is up.</summary>
        [RelayCommand(CanExecute = nameof(HasSpeaker))]
        private void NextSpeaker() => RemoveSpeaker(CurrentSpeaker);

        // ---------------- Warnings ----------------

        [RelayCommand(CanExecute = nameof(HasSpeaker))]
        private async Task WarnCurrentSpeaker()
        {
            var speaker = CurrentSpeaker;
            if (speaker == null) return;

            speaker.Warnings++;
            if (!WarnedList.Contains(speaker))
            {
                WarnedList.Add(speaker);
            }
            CollectionViewSource.GetDefaultView(WarnedList).Refresh();

            await SaveWarningsAsync(speaker);
        }

        private bool CanRemoveWarning() => SelectedWarned != null;

        [RelayCommand(CanExecute = nameof(CanRemoveWarning))]
        private async Task RemoveWarning()
        {
            var speaker = SelectedWarned;
            if (speaker == null) return;

            if (speaker.Warnings > 0)
            {
                speaker.Warnings--;
                await SaveWarningsAsync(speaker);
            }

            if (speaker.Warnings == 0)
            {
                WarnedList.Remove(speaker);
                SelectedWarned = null;
            }
            else
            {
                CollectionViewSource.GetDefaultView(WarnedList).Refresh();
                SelectedWarned = speaker;
            }
        }

        // ---------------- Speaking time ----------------

        [RelayCommand]
        private void SetPreset(SpeakingTimePreset? preset)
        {
            if (preset != null) ApplySpeakingTime(TimeSpan.FromSeconds(preset.Seconds));
        }

        [RelayCommand]
        private void IncreaseTime() =>
            ApplySpeakingTime(TimeSpan.FromSeconds(Math.Min(MaxSpeakingSeconds, SpeakingTime.TotalSeconds + TimeStepSeconds)));

        [RelayCommand]
        private void DecreaseTime() =>
            ApplySpeakingTime(TimeSpan.FromSeconds(Math.Max(MinSpeakingSeconds, SpeakingTime.TotalSeconds - TimeStepSeconds)));

        /// <summary>
        /// Changes the speaking time. If the current speaker has not started yet the clock
        /// shows the new time at once; otherwise it applies from the next speaker.
        /// </summary>
        private void ApplySpeakingTime(TimeSpan time)
        {
            SpeakingTime = time;
            UpdatePresetSelection();
            if (!_speechStarted)
            {
                Remaining = time;
            }
        }

        private void UpdatePresetSelection()
        {
            foreach (var p in Presets)
            {
                p.IsSelected = p.Seconds == (int)SpeakingTime.TotalSeconds;
            }
        }

        // ---------------- Clock ----------------

        [RelayCommand(CanExecute = nameof(HasSpeaker))]
        private void StartPause()
        {
            if (IsRunning)
            {
                PauseClock();
                return;
            }

            IsRunning = true;
            _speechStarted = true;
            _timer.Start();

            // Resuming inside the last seconds: the tick goes on
            if (!IsOvertime && Remaining.TotalSeconds <= FinalSecondsThreshold && Remaining.TotalSeconds > 0)
            {
                PlayTick();
            }
        }

        protected void PauseClock()
        {
            _timer.Stop();
            IsRunning = false;
            StopTick();
        }

        private void ResetClock()
        {
            PauseClock();
            _speechStarted = false;
            IsOvertime = false;
            Remaining = SpeakingTime;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (IsOvertime)
            {
                // Over time: count up
                Remaining += TimeSpan.FromSeconds(1);
                return;
            }

            Remaining -= TimeSpan.FromSeconds(1);

            if (Remaining <= TimeSpan.Zero)
            {
                StopTick();
                PlayBell();
                Remaining = TimeSpan.Zero;
                IsOvertime = true;
            }
            else if ((int)Remaining.TotalSeconds == FinalSecondsThreshold && !_tickPlaying)
            {
                PlayTick();
            }
        }

        // ---------------- Sounds ----------------

        private static SoundPlayer? LoadSound(string file)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Sounds", file);
                if (!File.Exists(path)) return null;
                var player = new SoundPlayer(path);
                player.Load();
                return player;
            }
            catch
            {
                return null; // Sounds are optional
            }
        }

        private void PlayTick()
        {
            try
            {
                _tickPlayer?.Play();
                _tickPlaying = _tickPlayer != null;
            }
            catch { }
        }

        private void StopTick()
        {
            try
            {
                if (_tickPlaying)
                {
                    _tickPlayer?.Stop();
                    _tickPlaying = false;
                }
            }
            catch { }
        }

        private void PlayBell()
        {
            try { _bellPlayer?.Play(); }
            catch { }
        }

        private static string Format(TimeSpan t) => $"{(int)t.TotalMinutes:00}:{t.Seconds:00}";
    }
}

using Foscamun.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Foscamun.Views
{
    /// <summary>
    /// Body of the committee and ICJ session pages. Its DataContext is an
    /// <see cref="ISessionSpeakers"/> (a SessionViewModelBase).
    /// </summary>
    public partial class SessionSpeakersPanel : UserControl
    {
        private Window? _window;

        public SessionSpeakersPanel()
        {
            InitializeComponent();
            Loaded += Panel_Loaded;
            Unloaded += Panel_Unloaded;
        }

        private ISessionSpeakers? Session => DataContext as ISessionSpeakers;

        // Shown (also when coming back from voting): listen to language changes and the spacebar
        private void Panel_Loaded(object sender, RoutedEventArgs e)
        {
            Session?.Activate();
            _window = Window.GetWindow(this);
            if (_window != null) _window.PreviewKeyDown += Window_PreviewKeyDown;
        }

        // Left: the clock pauses
        private void Panel_Unloaded(object sender, RoutedEventArgs e)
        {
            Session?.Deactivate();
            if (_window != null) _window.PreviewKeyDown -= Window_PreviewKeyDown;
            _window = null;
        }

        // Spacebar starts / pauses the clock wherever the focus is in the window
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Space || Keyboard.FocusedElement is TextBox || Session is not { } session) return;
            e.Handled = true; // the spacebar must not also press the focused button
            if (session.StartPauseCommand.CanExecute(null))
            {
                session.StartPauseCommand.Execute(null);
            }
        }

        // ---------------- Speaking time box ----------------

        // Enter applies the typed time, Esc gives up; either way the focus leaves the box,
        // so that the spacebar starts the clock again
        private void SpeakingTimeBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not TextBox box) return;
            var binding = box.GetBindingExpression(TextBox.TextProperty);

            if (e.Key == Key.Enter)
                binding?.UpdateSource();
            else if (e.Key == Key.Escape)
                binding?.UpdateTarget();
            else
                return;

            e.Handled = true;
            LeaveSpeakingTimeBox(box);
        }

        // Entering the box selects the whole time, ready to be typed over
        private void SpeakingTimeBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox box) box.SelectAll();
        }

        // The first click focuses and selects everything, instead of placing the caret
        private void SpeakingTimeBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox { IsKeyboardFocusWithin: false } box)
            {
                e.Handled = true;
                box.Focus();
            }
        }

        private void LeaveSpeakingTimeBox(TextBox box)
        {
            // The panel takes the focus: the box loses it without a button getting it
            // (the spacebar would press that button)
            Focusable = true;
            FocusVisualStyle = null;
            Focus();
            box.ScrollToHome();
        }

        // A click on a row moves the speaker between the two lists
        private void AvailableList_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView list && list.SelectedItem is { } speaker)
            {
                list.SelectedItem = null;
                Session?.AddSpeaker(speaker);
            }
        }

        private void SpeakersList_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView list && list.SelectedItem is { } speaker)
            {
                list.SelectedItem = null;
                Session?.RemoveSpeaker(speaker);
            }
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Foscamun.Views
{
    /// <summary>
    /// Message dialog in the app's style. <see cref="Show(string, string, MessageBoxButton, MessageBoxImage)"/>
    /// takes the same arguments as <see cref="MessageBox.Show(string, string, MessageBoxButton, MessageBoxImage)"/>.
    /// </summary>
    public partial class AppDialog : Window
    {
        private MessageBoxResult _result;

        private AppDialog()
        {
            InitializeComponent();
        }

        public static MessageBoxResult Show(string message, string title = "",
            MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None)
            => Show(message, title, buttons, image, null, out _);

        /// <summary>
        /// Shows the dialog with a check box under the message (e.g. "Don't ask again").
        /// </summary>
        public static MessageBoxResult Show(string message, string title, MessageBoxButton buttons,
            MessageBoxImage image, string? checkBoxText, out bool isChecked)
        {
            var dialog = new AppDialog();
            dialog.Setup(message, title, buttons, image, checkBoxText);

            var owner = Application.Current.Windows.OfType<Window>()
                            .FirstOrDefault(w => w.IsActive && w is not AppDialog)
                        ?? (Application.Current.MainWindow is { IsVisible: true } main ? main : null);
            if (owner != null)
                dialog.Owner = owner;
            else
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            dialog.ShowDialog();
            isChecked = dialog.OptionBox.IsChecked == true;
            return dialog._result;
        }

        private void Setup(string message, string title, MessageBoxButton buttons,
            MessageBoxImage image, string? checkBoxText)
        {
            Title = string.IsNullOrWhiteSpace(title) ? "Foscamun" : title;
            TitleText.Text = title;
            TitleText.Visibility = string.IsNullOrWhiteSpace(title) ? Visibility.Collapsed : Visibility.Visible;
            MessageText.Text = message;

            // Only warnings and errors carry an icon: the others read fine without
            if (image is MessageBoxImage.Warning or MessageBoxImage.Error)
            {
                IconView.Source = new Uri("pack://application:,,,/Resources/Icons/warning.svg");
                IconView.Visibility = Visibility.Visible;
            }

            if (!string.IsNullOrWhiteSpace(checkBoxText))
            {
                OptionBox.Content = checkBoxText;
                OptionBox.Visibility = Visibility.Visible;
            }

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    _result = MessageBoxResult.OK;
                    AddButton("OkBtn", "check", MessageBoxResult.OK, isDefault: true, isCancel: true);
                    break;
                case MessageBoxButton.OKCancel:
                    _result = MessageBoxResult.Cancel;
                    AddButton("CancelBtn", "close", MessageBoxResult.Cancel, isCancel: true);
                    AddButton("OkBtn", "check", MessageBoxResult.OK, isDefault: true);
                    break;
                case MessageBoxButton.YesNo:
                    _result = MessageBoxResult.No;
                    AddButton("NoBtn", "close", MessageBoxResult.No, isCancel: true);
                    AddButton("YesBtn", "check", MessageBoxResult.Yes, isDefault: true);
                    break;
                case MessageBoxButton.YesNoCancel:
                    _result = MessageBoxResult.Cancel;
                    AddButton("CancelBtn", "close", MessageBoxResult.Cancel, isCancel: true);
                    AddButton("NoBtn", "close", MessageBoxResult.No);
                    AddButton("YesBtn", "check", MessageBoxResult.Yes, isDefault: true);
                    break;
            }
        }

        private void AddButton(string labelKey, string icon, MessageBoxResult result,
            bool isDefault = false, bool isCancel = false)
        {
            var button = new Button
            {
                Style = (Style)FindResource("IconButton"),
                Tag = $"pack://application:,,,/Resources/Icons/{icon}.svg",
                Background = (System.Windows.Media.Brush)FindResource("SurfaceBrush"),
                Height = 38,
                MinWidth = 120,
                Margin = new Thickness(12, 0, 0, 0),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                IsDefault = isDefault,
                IsCancel = isCancel,
                FocusVisualStyle = null
            };
            button.SetResourceReference(ContentProperty, labelKey);
            button.Click += (_, _) =>
            {
                _result = result;
                DialogResult = true;
            };

            if (isDefault)
                Loaded += (_, _) => button.Focus();

            ButtonPanel.Children.Add(button);
        }

        /// <summary>A string of the active language, formatted with the given values.</summary>
        public static string Text(string key, params object[] args)
        {
            var text = Application.Current.TryFindResource(key) as string ?? key;
            return args.Length == 0 ? text : string.Format(text, args);
        }

        // No title bar: the panel itself moves the dialog
        private void Panel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }
    }
}

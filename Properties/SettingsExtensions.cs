using System.ComponentModel;

namespace Foscamun2026.Properties
{
    internal sealed partial class Settings
    {
        protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(sender, e);
            this.Save(); // Salvataggio automatico
        }
    }
}
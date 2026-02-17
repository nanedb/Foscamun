using CommunityToolkit.Mvvm.ComponentModel;

namespace Foscamun2026.ViewModels
{
    public partial class ICJRollCallViewModel : ObservableObject
    {
        public string CommitteeLogoPath => "pack://application:,,,/Resources/Committee Logo/ICJ.svg";

        public ICJRollCallViewModel()
        {
        }
    }
}
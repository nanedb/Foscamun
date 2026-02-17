using Foscamun2026.ViewModels;
using System.Windows.Controls;

namespace Foscamun2026.Views
{
    public partial class ICJRollCallPage : Page
    {
        public ICJRollCallPage()
        {
            InitializeComponent();
            DataContext = new ICJRollCallViewModel();
        }
    }
}
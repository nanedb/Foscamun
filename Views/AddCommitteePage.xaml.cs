using System.ComponentModel;
using System.Windows.Controls;
using Foscamun2026.ViewModels;

namespace Foscamun2026.Views
{
    public partial class AddCommitteePage : Page
    {
        public AddCommitteePage()
        {
            InitializeComponent();

            // Evita crash del designer
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = new AddCommitteeViewModel();
            }
        }
    }
}
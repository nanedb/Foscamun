using Foscamun2026.Data;
using Foscamun2026.ViewModels;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Foscamun2026.Views
{
    public partial class AddCommitteePage : Page
    {
        public AddCommitteePage()
        {
            InitializeComponent();

            var db = ((App)Application.Current).Services!.GetRequiredService<SqliteDataAccess>();
            DataContext = new AddCommitteeViewModel(db);
        }
    }
}
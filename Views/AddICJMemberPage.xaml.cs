using Foscamun2026.Data;
using Foscamun2026.ViewModels;
using System.Windows.Controls;

namespace Foscamun2026.Views
{
    public partial class AddICJMemberPage : Page
    {
        public AddICJMemberPage()
        {
            InitializeComponent();

            // Usa la stessa istanza di SqliteDataAccess usata in tutto il progetto
            DataContext = new AddICJMemberViewModel(MainWindow.Instance.Db);
        }

        public AddICJMemberPage(SqliteDataAccess db)
        {
            InitializeComponent();
            DataContext = new AddICJMemberViewModel(db);
        }
    }
}
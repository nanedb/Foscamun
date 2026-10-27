using CommunityToolkit.Mvvm.Input;
using Foscamun.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Foscamun.ViewModels
{
    /// <summary>
    /// Editable list of the advocates of one side of an ICJ case (plaintiffs or defense).
    /// Each advocate is added with a name and the country they represent.
    /// </summary>
    public class ICJAdvocateListViewModel : BaseViewModel
    {
        private ICJAdvocate? _selectedAdvocate;
        private string _newName = "";
        private Country? _newCountry;

        public ICJAdvocateListViewModel(IEnumerable<ICJAdvocate> advocates, ObservableCollection<Country> countries)
        {
            Advocates = new ObservableCollection<ICJAdvocate>(advocates);
            Countries = countries;

            AddCommand = new RelayCommand(Add, CanAdd);
            RemoveCommand = new RelayCommand(Remove, CanRemove);
        }

        public ObservableCollection<ICJAdvocate> Advocates { get; }

        public ObservableCollection<Country> Countries { get; }

        public ICJAdvocate? SelectedAdvocate
        {
            get => _selectedAdvocate;
            set
            {
                if (_selectedAdvocate != value)
                {
                    _selectedAdvocate = value;
                    OnPropertyChanged(nameof(SelectedAdvocate));
                    (RemoveCommand as RelayCommand)?.NotifyCanExecuteChanged();
                }
            }
        }

        public string NewName
        {
            get => _newName;
            set
            {
                if (_newName != value)
                {
                    _newName = value;
                    OnPropertyChanged(nameof(NewName));
                    (AddCommand as RelayCommand)?.NotifyCanExecuteChanged();
                }
            }
        }

        public Country? NewCountry
        {
            get => _newCountry;
            set
            {
                if (_newCountry != value)
                {
                    _newCountry = value;
                    OnPropertyChanged(nameof(NewCountry));
                    (AddCommand as RelayCommand)?.NotifyCanExecuteChanged();
                }
            }
        }

        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }

        private bool CanAdd() => !string.IsNullOrWhiteSpace(NewName) && NewCountry != null;

        private void Add()
        {
            if (NewCountry == null || string.IsNullOrWhiteSpace(NewName))
                return;

            Advocates.Add(new ICJAdvocate { Name = NewName.Trim(), IsoCode = NewCountry.IsoCode });

            // Clear only the name: the country stays selected, since several
            // advocates often represent the same country
            NewName = "";
        }

        private bool CanRemove() => SelectedAdvocate != null;

        private void Remove()
        {
            if (SelectedAdvocate != null)
            {
                Advocates.Remove(SelectedAdvocate);
            }
        }
    }
}

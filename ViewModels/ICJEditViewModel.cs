using CommunityToolkit.Mvvm.Input;
using Foscamun.Models;
using Foscamun.Repositories;
using Foscamun.ViewModels;
using Foscamun.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Foscamun.ViewModels
{
    /// <summary>
    /// ViewModel for editing ICJ (International Court of Justice) configuration.
    /// Manages judges, advocates, jurors, and case details.
    /// </summary>
    public class ICJEditViewModel : BaseViewModel
    {
        private readonly ICJRepository _repository;
        private readonly MainWindow _mainWindow;
        private string? _selectedJuror;
        private string _newJurorName = "";

        private string _judge = "";
        private string _viceJudge1 = "";
        private string _viceJudge2 = "";
        private string _topic = "";

    /// <summary>
    /// Initializes the ViewModel with ICJ data and available countries.
    /// </summary>
    public ICJEditViewModel(ICJ model,
                            IEnumerable<Country> countries,
                            ICJRepository repository,
                            MainWindow mainWindow)
    {
        _repository = repository;
        _mainWindow = mainWindow;

        Countries = new ObservableCollection<Country>(countries);

        // Use backing fields directly to avoid property change notifications during construction
        _judge = model.Judge;
        _viceJudge1 = model.ViceJudge1;
        _viceJudge2 = model.ViceJudge2;
        _topic = model.Topic;

        Plaintiffs = new ICJAdvocateListViewModel(model.Plaintiffs, Countries);
        Defense = new ICJAdvocateListViewModel(model.Defense, Countries);

        // Saving requires at least one advocate per side
        Plaintiffs.Advocates.CollectionChanged += (_, _) => (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
        Defense.Advocates.CollectionChanged += (_, _) => (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();

        Jurors = new ObservableCollection<string>(model.Jurors);

        AddJurorCommand = new RelayCommand(AddJuror, CanAddJuror);
        RemoveJurorCommand = new RelayCommand(RemoveJuror, CanRemoveJuror);
        SaveCommand = new RelayCommand(Save, CanSave);
        CancelCommand = new RelayCommand(Cancel);
    }

    public string Judge
    {
        get => _judge;
        set
        {
            if (_judge != value)
            {
                _judge = value;
                OnPropertyChanged(nameof(Judge));
               (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public string ViceJudge1
    {
        get => _viceJudge1;
        set
        {
            if (_viceJudge1 != value)
            {
                _viceJudge1 = value;
                OnPropertyChanged(nameof(ViceJudge1));
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public string ViceJudge2
    {
        get => _viceJudge2;
        set
        {
            if (_viceJudge2 != value)
            {
                _viceJudge2 = value;
                OnPropertyChanged(nameof(ViceJudge2));
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public string Topic
    {
        get => _topic;
        set
        {
            if (_topic != value)
            {
                _topic = value;
                OnPropertyChanged(nameof(Topic));
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public ICJAdvocateListViewModel Plaintiffs { get; }

    public ICJAdvocateListViewModel Defense { get; }

    public ObservableCollection<string> Jurors { get; set; }

    public string? SelectedJuror
    {
        get => _selectedJuror;
        set
        {
            if (_selectedJuror != value)
            {
                _selectedJuror = value;
                OnPropertyChanged(nameof(SelectedJuror));
                (RemoveJurorCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public string NewJurorName
    {
        get => _newJurorName;
        set
        {
            if (_newJurorName != value)
            {
                _newJurorName = value;
                OnPropertyChanged(nameof(NewJurorName));
                (AddJurorCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public ObservableCollection<Country> Countries { get; }

    public ICommand AddJurorCommand { get; }
    public ICommand RemoveJurorCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private bool CanAddJuror() => !string.IsNullOrWhiteSpace(NewJurorName);

    private void AddJuror()
    {
        if (!string.IsNullOrWhiteSpace(NewJurorName))
        {
            Jurors.Add(NewJurorName);
            NewJurorName = ""; // Clear TextBox after adding
            (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }
    }

    private bool CanRemoveJuror() => SelectedJuror != null;

    private void RemoveJuror()
    {
        if (SelectedJuror != null)
        {
            Jurors.Remove(SelectedJuror);
            (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }
    }

    /// <summary>
    /// Validates that all required fields are filled and at least one plaintiff,
    /// one defense advocate and one juror are added.
    /// </summary>
    private bool CanSave() =>
        !string.IsNullOrWhiteSpace(Judge) &&
        !string.IsNullOrWhiteSpace(ViceJudge1) &&
        !string.IsNullOrWhiteSpace(ViceJudge2) &&
        !string.IsNullOrWhiteSpace(Topic) &&
        Plaintiffs.Advocates.Count > 0 &&
        Defense.Advocates.Count > 0 &&
        Jurors.Count > 0;

    /// <summary>
    /// Saves ICJ configuration to database and navigates back to setup page.
    /// </summary>
    private void Save()
    {
        var model = new ICJ
        {
            Judge = Judge,
            ViceJudge1 = ViceJudge1,
            ViceJudge2 = ViceJudge2,
            Topic = Topic,

            Plaintiffs = Plaintiffs.Advocates.ToList(),
            Defense = Defense.Advocates.ToList(),

            Jurors = Jurors.ToList()
        };

        _repository.Save(model);

        _mainWindow.NavigateRightFrame(new SetupPage());
    }

    private void Cancel()
    {
        _mainWindow.NavigateRightFrame(new SetupPage());
    }
    }
}

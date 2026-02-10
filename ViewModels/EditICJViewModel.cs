using CommunityToolkit.Mvvm.Input;
using Foscamun2026.Models;
using Foscamun2026.Repositories;
using Foscamun2026.ViewModels;
using Foscamun2026.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Foscamun2026.ViewModels
{
    public class EditICJViewModel : BaseViewModel
    {
        private readonly ICJRepository _repository;
        private readonly MainWindow _mainWindow;
        private string? _selectedJuror;
        private string _newJurorName = "";
        
        private string _judge = "";
        private string _viceJudge1 = "";
        private string _viceJudge2 = "";
        private string _topic = "";
        private string _plaintiff1 = "";
        private string _plaintiff2 = "";
        private string _plaintiff3 = "";
        private Country? _pCountry;
        private string _defense1 = "";
        private string _defense2 = "";
        private string _defense3 = "";
        private Country? _dCountry;

    public EditICJViewModel(ICJ model,
                            IEnumerable<Country> countries,
                            ICJRepository repository,
                            MainWindow mainWindow)
    {
        _repository = repository;
        _mainWindow = mainWindow;

        Countries = new ObservableCollection<Country>(countries);

        // Usa i backing field direttamente per evitare notifiche durante la costruzione
        _judge = model.Judge;
        _viceJudge1 = model.ViceJudge1;
        _viceJudge2 = model.ViceJudge2;
        _topic = model.Topic;

        _plaintiff1 = model.Plaintiff1;
        _plaintiff2 = model.Plaintiff2;
        _plaintiff3 = model.Plaintiff3;
        _pCountry = model.PCountry;

        _defense1 = model.Defense1;
        _defense2 = model.Defense2;
        _defense3 = model.Defense3;
        _dCountry = model.DCountry;

        Jurors = new ObservableCollection<string>(model.Jurors);

        AddJurorCommand = new RelayCommand(AddJuror, CanAddJuror);
        RemoveJurorCommand = new RelayCommand(RemoveJuror, CanRemoveJuror);
        SaveCommand = new RelayCommand(Save, CanSave);
        CancelCommand = new RelayCommand(Cancel);
    }
    // -------------------------
    // PROPRIETÀ BINDATE
    // -------------------------

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

    public string Plaintiff1
    {
        get => _plaintiff1;
        set
        {
            if (_plaintiff1 != value)
            {
                _plaintiff1 = value;
                OnPropertyChanged(nameof(Plaintiff1));
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public string Plaintiff2
    {
        get => _plaintiff2;
        set
        {
            if (_plaintiff2 != value)
            {
                _plaintiff2 = value;
                OnPropertyChanged(nameof(Plaintiff2));
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public string Plaintiff3
    {
        get => _plaintiff3;
        set
        {
            if (_plaintiff3 != value)
            {
                _plaintiff3 = value;
                OnPropertyChanged(nameof(Plaintiff3));
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public Country? PCountry
    {
        get => _pCountry;
        set
        {
            if (_pCountry != value)
            {
                _pCountry = value;
                OnPropertyChanged(nameof(PCountry));
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public string Defense1
    {
        get => _defense1;
        set
        {
            if (_defense1 != value)
            {
                _defense1 = value;
                OnPropertyChanged(nameof(Defense1));
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public string Defense2
    {
        get => _defense2;
        set
        {
            if (_defense2 != value)
            {
                _defense2 = value;
                OnPropertyChanged(nameof(Defense2));
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public string Defense3
    {
        get => _defense3;
        set
        {
            if (_defense3 != value)
            {
                _defense3 = value;
                OnPropertyChanged(nameof(Defense3));
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

    public Country? DCountry
    {
        get => _dCountry;
        set
        {
            if (_dCountry != value)
            {
                _dCountry = value;
                OnPropertyChanged(nameof(DCountry));
                (SaveCommand as RelayCommand)?.NotifyCanExecuteChanged();
            }
        }
    }

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

    // -------------------------
    // COMANDI
    // -------------------------

    public ICommand AddJurorCommand { get; }
    public ICommand RemoveJurorCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    // -------------------------
    // METODI DEI COMANDI
    // -------------------------

    private bool CanAddJuror() => !string.IsNullOrWhiteSpace(NewJurorName);

    private void AddJuror()
    {
        if (!string.IsNullOrWhiteSpace(NewJurorName))
        {
            Jurors.Add(NewJurorName);
            NewJurorName = ""; // Pulisce la TextBox dopo l'aggiunta
        }
    }

    private bool CanRemoveJuror() => SelectedJuror != null;

    private void RemoveJuror()
    {
        if (SelectedJuror != null)
            Jurors.Remove(SelectedJuror);
    }

    private bool CanSave() =>
        !string.IsNullOrWhiteSpace(Judge) &&
        !string.IsNullOrWhiteSpace(ViceJudge1) &&
        !string.IsNullOrWhiteSpace(ViceJudge2) &&
        !string.IsNullOrWhiteSpace(Topic) &&
        !string.IsNullOrWhiteSpace(Plaintiff1) &&
        !string.IsNullOrWhiteSpace(Plaintiff2) &&
        !string.IsNullOrWhiteSpace(Plaintiff3) &&
        !string.IsNullOrWhiteSpace(PCountry?.IsoCode) &&
        !string.IsNullOrWhiteSpace(Defense1) &&
        !string.IsNullOrWhiteSpace(Defense2) &&
        !string.IsNullOrWhiteSpace(Defense3) &&
        !string.IsNullOrWhiteSpace(DCountry?.IsoCode);

    private void Save()
    {
        var model = new ICJ
        {
            Judge = Judge,
            ViceJudge1 = ViceJudge1,
            ViceJudge2 = ViceJudge2,
            Topic = Topic,

            Plaintiff1 = Plaintiff1,
            Plaintiff2 = Plaintiff2,
            Plaintiff3 = Plaintiff3,
            PCountry = PCountry!,

            Defense1 = Defense1,
            Defense2 = Defense2,
            Defense3 = Defense3,
            DCountry = DCountry!,

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
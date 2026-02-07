using CommunityToolkit.Mvvm.Input;
using Foscamun2026.Repositories;
using Foscamun2026.ViewModels;
using Foscamun2026.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Navigation;


public class EditICJViewModel : BaseViewModel
{
    private readonly ICJRepository _repository;
    private readonly MainWindow _mainWindow;

    public EditICJViewModel(ICJ model,
                            IEnumerable<Country> countries,
                            ICJRepository repository,
                            MainWindow mainWindow)
    {
        _repository = repository;
        _mainWindow = mainWindow;

        Countries = new ObservableCollection<Country>(countries);

        Judge = model.Judge;
        ViceJudge1 = model.ViceJudge1;
        ViceJudge2 = model.ViceJudge2;
        Topic = model.Topic;

        Plaintiff1 = model.Plaintiff1;
        Plaintiff2 = model.Plaintiff2;
        Plaintiff3 = model.Plaintiff3;
        PCountry = model.PCountry ?? new Country();

        Defense1 = model.Defense1;
        Defense2 = model.Defense2;
        Defense3 = model.Defense3;
        DCountry = model.DCountry ?? new Country();

        Jurors = new ObservableCollection<string>(model.Jurors);

        AddJurorCommand = new RelayCommand(AddJuror);
        RemoveJurorCommand = new RelayCommand(RemoveJuror, () => SelectedJuror != null);
        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
    }
    // -------------------------
    // PROPRIETÀ BINDATE
    // -------------------------

    public string Judge { get; set; } = "";
    public string ViceJudge1 { get; set; } = "";
    public string ViceJudge2 { get; set; } = "";
    public string Topic { get; set; } = "";

    public string Plaintiff1 { get; set; } = "";
    public string Plaintiff2 { get; set; } = "";
    public string Plaintiff3 { get; set; } = "";
    public Country PCountry { get; set; } = new();

    public string Defense1 { get; set; } = "";
    public string Defense2 { get; set; } = "";
    public string Defense3 { get; set; } = "";
    public Country DCountry { get; set; } = new();

    public ObservableCollection<string> Jurors { get; set; }
    public string? SelectedJuror { get; set; }

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

    private void AddJuror()
    {
        Jurors.Add("New Juror");
    }

    private void RemoveJuror()
    {
        if (SelectedJuror != null)
            Jurors.Remove(SelectedJuror);
    }

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
            PCountry = PCountry,

            Defense1 = Defense1,
            Defense2 = Defense2,
            Defense3 = Defense3,
            DCountry = DCountry,

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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym_kursovaya.DB;
using Gym_kursovaya.Models;
using Gym_kursovaya.ShowModels;
using Microsoft.Extensions.DependencyInjection;

namespace Gym_kursovaya.ViewModels;

public partial class SybscriptionEditVM: ViewModelBase
{
    [ObservableProperty] private FullExercise? _selectedDay;
    [ObservableProperty] private Programm? _selectedProgram;
    [ObservableProperty] private ObservableCollection<Programm> _allPrograms = new();
    [ObservableProperty] private ExerciseStatus? _selectedStatus;
    [ObservableProperty] private ObservableCollection<Client> _clients;
    [ObservableProperty] private ObservableCollection<Trainer> _trainers;
    [ObservableProperty] private ObservableCollection<Schedule> _schedules;
    

    //все статусы. Они разблокированы ,если не достигнут лимит
    [ObservableProperty] private ObservableCollection<ExerciseStatus> _allStatuses = new();
    [ObservableProperty] private ObservableCollection<ExerciseStatus> _availableStatuses = new();
    
    [ObservableProperty] private Programm _selectedProgramm;
    [ObservableProperty] private Trainer  _selectedTrainer;
    [ObservableProperty] private Schedule _selectedSchedule;
    [ObservableProperty] private Client _selectedClient;
    [ObservableProperty] private Subscription _selectedSubscription;
    [ObservableProperty] private ObservableCollection<FullExercise> _abonementDays = new();
    [ObservableProperty] private int _trainigsAmountLeft;
    [ObservableProperty] private DateTime _startDate;
    [ObservableProperty] private DateTime _endDate;
    private DateTime _today = DateTime.Today;
    private DateTime _currentMonth;//какой месяц будет показываться
    private DateTime _minMonth;//мин месяц, который будет показываться
    private DateTime _maxMonth;//макс месяц, который будет показываться
    private List<Exercise> _exerciseDays;//здесь все тренировки абонемента. Абонемент дейс просто показывает
       
    public bool CanAddTrainingStatus => TrainigsAmountLeft > 0;

    public string CurrentMonthTitle => _currentMonth.ToString("MMMM yyyy");
    
    IServiceProvider _serviceProvider;
    private SubscriptionRepository _subscriptionRep;
    private ScheduleRepository _scheduleRep;
    private ExerciseRepository _exerciseRep;
    private ProgrammRepository  _programmRep;
    ExerciseStatusRepository _exerciseStatusRep;
    ExerciseProgramRepository  _exerciseProgramRep;
    //Чтобы в конце проверял, прогрузился ли весь конструктор и мне нулл не выкидывало
    private bool isLoaded;
    private int _usedTrainings;
    private bool _isUpdatingStatus;
    public SybscriptionEditVM(IServiceProvider serviceProvider, Subscription selectedSubscription,ExerciseStatusRepository exerciseStatusRep, ExerciseRepository exerciseRep, ScheduleRepository scheduleRep, ProgrammRepository programmRep, ClientRepository clientRep, TrainerRepository trainerRep)
    {
        _serviceProvider = serviceProvider;
        _selectedSubscription = selectedSubscription;
        _subscriptionRep = serviceProvider.GetService<SubscriptionRepository>();
        _exerciseRep = exerciseRep;
        _scheduleRep = scheduleRep;
        _exerciseStatusRep = exerciseStatusRep;
        _exerciseProgramRep = serviceProvider.GetService<ExerciseProgramRepository>();
        _programmRep = programmRep;
        _exerciseDays = new List<Exercise>();
        AllPrograms = new ObservableCollection<Programm>(_programmRep.GetAll());
        AllStatuses = new ObservableCollection<ExerciseStatus>(_exerciseStatusRep.GetAll());

        
        Clients =new ObservableCollection<Client>(clientRep.GetNonValidClients());
        Schedules = new ObservableCollection<Schedule>(_scheduleRep.GetAll());
        Trainers = new ObservableCollection<Trainer>(trainerRep.GetAll());
        
        //здесь проверяем,получили ли мы тренировки с бд. Если тренировок нет , значит создаем базовую
        _exerciseDays = exerciseRep.GetBySubscriptionId(SelectedSubscription.Id);
        SelectedTrainer = trainerRep.GetById(selectedSubscription.TrainerId);
        SelectedClient = clientRep.GetById(selectedSubscription.TrainerId);
        SelectedSchedule =
            SelectedSubscription.ScheduleId > 1
                ? _scheduleRep.GetById(SelectedSubscription.ScheduleId)
                : _scheduleRep.GetById(1);

        StartDate = SelectedSubscription.StartDate;
        EndDate = SelectedSubscription.EndDate;
        if(StartDate == default)
        {
            StartDate = DateTime.Today;
            EndDate = StartDate.AddDays(SelectedSchedule.Duration);
        }
        _minMonth = new DateTime(StartDate.Year, StartDate.Month, 1);
        _maxMonth = new DateTime(EndDate.Year, EndDate.Month, 1);
        //выбранный месяц ставим первый
        _currentMonth = _minMonth;
        
        LoadOrGenerate();
        RecalculateTrainings();
        TrainigsAmountLeft = _selectedSchedule.DaysAmount - _usedTrainings;
        AbonementDays = new ObservableCollection<FullExercise>();
        FillMonth();
        AvailableStatuses = new ObservableCollection<ExerciseStatus>();
        UpdateAvailableStatuses();
        isLoaded = true;
    }

    //по названию Schedule создать месяцы и вычислить количество дней
    //по Schedule создать расписание тренировок которое будет выставлено изначально
    //проверку тренера не нужно. так как передают только с тренером

    
    
    
    
    private void UpdateMonth()
    {
        FillMonth();
        OnPropertyChanged(nameof(CurrentMonthTitle));
    }
    
    //итак база работы:
    //Чекаем есть ли расписание в бд
    //1)Есть -> значит берем из бд тренировки,вставляем в календарь и лист
    //2) Нету -> создаем базовое расписание

    public void LoadOrGenerate()
    {
        if (_exerciseDays.Any())
        {
            _usedTrainings =
                _exerciseDays.Count(x =>
                    x.Status.Id == 4 || x.Status.Id==1);
            Load();
            return;
        }

        WorkOutBase();
    }

    public void Load()
    {
        DateTime currentDate = StartDate;
        List<Exercise> exercises = new List<Exercise>();
        while (currentDate <= EndDate)
        {
            Exercise? exercise = _exerciseDays.FirstOrDefault(x => x.Date.Date ==  currentDate.Date);
            if (exercise == null)
            {
                exercise = new Exercise();
                exercise.Date = currentDate;
                exercise.SubscriptionId = SelectedSubscription.Id;
                exercise.Programms = new List<Programm>();
                bool isHoliday = currentDate.DayOfWeek == DayOfWeek.Sunday;
                if (isHoliday)
                {
                    exercise.Status = _exerciseStatusRep.GetById(2);
                }
                else
                {
                    exercise.Status = _exerciseStatusRep.GetById(3);
                }
               
            }
            exercises.Add(exercise);
            currentDate = currentDate.AddDays(1);
        }
        _exerciseDays.Clear();
        _exerciseDays = exercises;
    }
    
    //базовое расписание. Стоит изначально. по нажатию кнопки ставит его
    [RelayCommand]
    public void WorkOutBase()//здесь заполняем лист 
    {
        //сначала все очищаем . Тренировки(использованные) и  лист
        _exerciseDays.Clear();

        _usedTrainings = 0;

        DateTime currentDate = StartDate;

        while(currentDate <= EndDate)
        {
            Exercise exercise = new Exercise();

            exercise.Date = currentDate;
            exercise.SubscriptionId = SelectedSubscription.Id;

            bool isTrainingDay =
                currentDate.DayOfWeek == DayOfWeek.Monday ||
            currentDate.DayOfWeek == DayOfWeek.Wednesday ||
            currentDate.DayOfWeek == DayOfWeek.Friday;

            bool isHoliday = currentDate.DayOfWeek == DayOfWeek.Sunday;
            if(isTrainingDay &&
               _usedTrainings < SelectedSchedule.DaysAmount)
            {
                exercise.Status =
                    _exerciseStatusRep.GetById(4);

                exercise.Programms =
                    new List<Programm>()
                    {
                        _programmRep.GetById(2)
                    };

                _usedTrainings++;
            }
            else if (isHoliday)
            {
                exercise.Status = _exerciseStatusRep.GetById(2);
                exercise.Programms = new List<Programm>();
            }
            else
            {
                exercise.Status = _exerciseStatusRep.GetById(3);
                exercise.Programms = new List<Programm>();
            }

            _exerciseDays.Add(exercise);

            currentDate = currentDate.AddDays(1);
        }
        RecalculateTrainings();
        FillMonth();
    }

    
    
    public void FillMonth()
    {
        //создание календаря
        //создаем пустую коллекцию
        AbonementDays.Clear();
        //находим первый день месяца, чтобы найти дни прошлого месяца(для красивого вида)
        DateTime firstDay = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
        int offset = ((int)firstDay.DayOfWeek + 6) % 7;//здесь так, так как если бы было просто (int)firstDay.DayOfWeek - 1, то если бы у нас было бы Воскресенье(0), то вышло бы -1
        DateTime gridStart = firstDay.AddDays(-offset);
        for (int i = 0; i < 42; i++)
        {
            DateTime currentDate = gridStart.AddDays(i);
            
            //чекаем,есть ли в списке
            Exercise? model = _exerciseDays
                .FirstOrDefault(x=>x.Date.Date == currentDate.Date);
            bool isInsideSubscription =
                currentDate.Date >= StartDate.Date &&
                currentDate.Date <= EndDate.Date;
            if (model == null)
            {
                model = new Exercise()
                {
                    Date = currentDate,
                    Status = _exerciseStatusRep.GetById( isInsideSubscription ? 3 : 5),
                    Programms = new List<Programm>()
                };
            }

            FullExercise abonementDay = new FullExercise(model);
            
            abonementDay.IsCurrentMonth = currentDate.Month == _currentMonth.Month;

            abonementDay.IsEditable = currentDate >= StartDate
                                      && currentDate <= EndDate && _today <= currentDate;
            AbonementDays.Add(abonementDay);
        }
    }
    
    
    [RelayCommand]
    public void NextMonth()
    {
        DateTime next = _currentMonth.AddMonths(1);
        if (next > _maxMonth)
            return;
        
        _currentMonth = next;
        UpdateMonth();
        
    }
    
    [RelayCommand]
    public void PastMonth()
    {
        DateTime next = _currentMonth.AddMonths(-1);
        if (next < _minMonth)
            return;
        
        _currentMonth = next;
        UpdateMonth();
    }

    [RelayCommand]
    public void SaveSchedule()
    {
        if (SelectedClient.Id == 0 || SelectedSchedule.Id == 0 || SelectedTrainer.Id == 0)
            return;
        
        
        SelectedSubscription.ClientId = SelectedClient.Id;
        SelectedSubscription.TrainerId = SelectedTrainer.Id;
        SelectedSubscription.ScheduleId = SelectedSchedule.Id;
        SelectedSubscription.StartDate = StartDate;
        SelectedSubscription.EndDate = EndDate;
        SelectedSubscription.Validity = true;
        if (SelectedSubscription.Id == 0)
        {
            SelectedSubscription.Id = _subscriptionRep.InsertIdBack(SelectedSubscription);
            foreach(var exercise in _exerciseDays)
            {
                exercise.SubscriptionId =
                    SelectedSubscription.Id;
            }
        }
        else
        {
            _subscriptionRep.Update(
                SelectedSubscription);
        }
        foreach (Exercise exercise in _exerciseDays)
        {
            if (exercise.Status.Id == 5 )
                continue;

            if (exercise.Id == 0 && (exercise.Status.Id == 1 ||  exercise.Status.Id == 4 || exercise.Status.Id == 2))
            {
                _exerciseRep.Insert(exercise);
            }
            else
            {
                _exerciseRep.Update(exercise);
            }

            if (exercise.Status.Id != 2)
            {
                _exerciseProgramRep.DeleteByExerciseId(exercise.Id);
                _exerciseProgramRep.InsertProgramms(exercise);
            }
            
        }
        
    }
    
    partial void OnSelectedScheduleChanged(
        Schedule? oldValue,
        Schedule? newValue)
    {
        if(newValue == null || _exerciseDays == null || !isLoaded)
            return;

        TrainigsAmountLeft = newValue.DaysAmount;

        if(StartDate != default)
        {
            EndDate = StartDate.AddDays(newValue.Duration);
        }

        _maxMonth = new DateTime(EndDate.Year, EndDate.Month, 1);

        if(_exerciseDays.Count == 0)
        {
            WorkOutBase();
            FillMonth();
        }
    }
    
    partial void OnSelectedClientChanged(
        Client? oldValue,
        Client? newValue)
    {
        if(newValue == null)
            return;
        SelectedSubscription.ClientId = newValue.Id;
    }
    
    partial void OnSelectedTrainerChanged(
        Trainer? oldValue,
        Trainer? newValue)
    {
        if(newValue == null)
            return;
        SelectedSubscription.TrainerId = newValue.Id;
    }
    
    
    private void RecalculateTrainings()
    {
        TrainigsAmountLeft =
            SelectedSchedule.DaysAmount -
            _exerciseDays.Count(x =>
                x.Status.Id == 1 ||
                x.Status.Id == 4);
        
        UpdateAvailableStatuses();
        /*Console.WriteLine(
            $"Limit={SelectedSchedule.DaysAmount}, " +
            $"Used={_exerciseDays.Count(x => x.Status.Id == 1 || x.Status.Id == 4)}, " +
            $"Left={TrainigsAmountLeft}");    */
    }
    
    [RelayCommand]
    private void AddProgram()
    {
        if (SelectedDay == null)
            return;

        if (SelectedProgram == null)
            return;

        if (SelectedDay.Programs.Any(x =>
                x.Id == SelectedProgram.Id))
            return;

        SelectedDay.Programs.Add(SelectedProgram);
    }
    
    [RelayCommand]
    private void RemoveProgram( )
    {
        if (SelectedDay == null)
            return;

        SelectedDay.Programs.Remove(SelectedProgramm);
    }  
    
    

    
    private void UpdateAvailableStatuses()
    {
        bool canAddTraining = TrainigsAmountLeft > 0;

        AvailableStatuses.Clear();

        foreach (var status in AllStatuses)
        {
            if (!canAddTraining &&
                (status.Id == 1 || status.Id == 4))
            {
                continue;
            }

            AvailableStatuses.Add(status);
        }
    }
    
    

    partial void OnSelectedDayChanged(FullExercise? oldValue, FullExercise? newValue)
    {
        Console.WriteLine($"Day changed: {newValue?.Date}");
        if (newValue == null)
            return;
        _isUpdatingStatus = true;
        SelectedStatus = newValue.Status;
        _isUpdatingStatus = false;
    }
    
    partial void OnSelectedStatusChanged(
        ExerciseStatus? oldValue,
        ExerciseStatus? newValue)
    {
        Console.WriteLine($"Status changed: {oldValue?.Title} -> {newValue?.Title}");
        if (_isUpdatingStatus)
            return;

        if (SelectedDay == null ||  newValue == null)
        return;

        bool oldTraining =
            SelectedDay.Status.Id == 1 ||
        SelectedDay.Status.Id == 4;

        bool newTraining =
            newValue.Id == 1 ||
        newValue.Id == 4;

        if (!oldTraining && newTraining)
        {
            int currentTrainings =
                _exerciseDays.Count(x =>
                    x.Status.Id == 1 ||
            x.Status.Id == 4);

            if (currentTrainings >= SelectedSchedule.DaysAmount)
            {
                _isUpdatingStatus = true;
                SelectedStatus = SelectedDay.Status;
                _isUpdatingStatus = false;
                return;
            }
        }

        var exercise = _exerciseDays.FirstOrDefault(x =>
            x.Date.Date == SelectedDay.Date.Date);

        if (exercise == null)
            return;

        exercise.Status = newValue;
        SelectedDay.Status = newValue;

        RecalculateTrainings();
    }

}
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.AspNetCore.SignalR.Client;
using Quizard.Core.Entities;
using Quizard.Core.Enums;

namespace Quizard.WebApp.Services;

public class GameService
{
    public event Action? OnChange;
    public ObservableCollection<string> PlayersList { get; } = [];
    
    private HubConnection _hubConnection = null!;
    public HubConnection HubConnection
    {
        get => _hubConnection;
        set
        {
            _hubConnection = value;
            OnChange?.Invoke();
        }
    }
    
    private string _nickname = "";
    public string Nickname
    {
        get => _nickname;
        set
        {
            if (_nickname == value) return;
            _nickname = value;
            OnChange?.Invoke();
        }
    }
    
    private bool _playerInitial;
    public bool PlayerInitial
    {
        get => _playerInitial;
        set
        {
            if (_playerInitial == value) return;
            _playerInitial = value;
            OnChange?.Invoke();
        }
    }
    
    private GameTrigger _gameTrigger;
    public GameTrigger GameTrigger
    {
        get => _gameTrigger;
        set
        {
            if (_gameTrigger == value) return;
            _gameTrigger = value;
            OnChange?.Invoke();
        }
    }
    
    private GameState _gameState;
    public GameState GameState
    {
        get => _gameState;
        set
        {
            if (_gameState == value) return;
            _gameState = value;
            OnChange?.Invoke();
        }
    }

    private int _score;
    public int Score
    {
        get => _score;
        set
        {
            if (_score == value) return;
            _score = value;
            OnChange?.Invoke();
        }
    }

    private bool _mirrorInterface;
    public bool MirrorInterface
    {
        get => _mirrorInterface;
        set
        {
            _mirrorInterface = value;
            OnChange?.Invoke();
        }
    }

    private bool _smallText;
    public bool SmallText
    {
        get => _smallText;
        set
        {
            _smallText = value;
            OnChange?.Invoke();
        }
    }

    private bool _brightScreen;
    public bool BrightScreen
    {
        get => _brightScreen;
        set
        {
            _brightScreen = value;
            OnChange?.Invoke();
        }
    }
    
    private bool _mined;
    public bool Mined
    {
        get => _mined;
        set
        {
            _mined = value;
            OnChange?.Invoke();
        }
    }

    private bool _shuffleButtons;
    public bool ShuffleButtons
    {
        get => _shuffleButtons;
        set
        {
            _shuffleButtons = value;
            OnChange?.Invoke();
        }
    }
    
    private ModifierType? _chosenModifier;
    public ModifierType? ChosenModifier
    {
        get => _chosenModifier;
        set
        {
            _chosenModifier = value;
            OnChange?.Invoke();
        }
    }

    private string? _chosenForAttackPlayer;
    public string? ChosenForAttackPlayer
    {
        get => _chosenForAttackPlayer;
        set
        {
            _chosenForAttackPlayer = value;
            OnChange?.Invoke();
        }
    }

    private int _currentRoundScore;
    public int CurrentRoundScore
    {
        get => _currentRoundScore;
        set
        {
            if (_currentRoundScore == value) return;
            _currentRoundScore = value;
            OnChange?.Invoke();
        }
    }

    private int _roundNumber;
    public int RoundNumber
    {
        get => _roundNumber;
        set
        {
            if (_roundNumber == value) return;
            _roundNumber = value;
            OnChange?.Invoke();
        }
    }

    private Scenario? _scenario;
    public Scenario? Scenario
    {
        get => _scenario;
        set
        {
            _scenario = value;
            OnChange?.Invoke();
        }
    }

    private Round[]? _roundsForVoting;
    public Round[]? RoundsForVoting
    {
        get => _roundsForVoting;
        set
        {
            _roundsForVoting = value;
            OnChange?.Invoke();
        }
    }
    
    private Round? _chosenRound;
    public Round? ChosenRound
    {
        get => _chosenRound;
        set
        {
            _chosenRound = value;
            OnChange?.Invoke();
        }
    }

    private Round? _round;
    public Round? Round
    {
        get => _round;
        set
        {
            _round = value;
            OnChange?.Invoke();
        }
    }
    
    private Player? _targetPlayer;
    public Player? TargetPlayer
    {
        get => _targetPlayer;
        set
        {
            _targetPlayer = value;
            OnChange?.Invoke();
        }
    }
    
    private Question? _question;
    public Question? Question
    {
        get => _question;
        set
        {
            _question = value;
            OnChange?.Invoke();
        }
    }

    private int? _playerBet;
    public int? PlayerBet
    {
        get => _playerBet;
        set
        {
            _playerBet = value;
            OnChange?.Invoke();
        }
    }

    private Answer? _givenAnswer;
    public Answer? GivenAnswer
    {
        get => _givenAnswer;
        set
        {
            _givenAnswer = value;
            OnChange?.Invoke();
        }
    }
    public ObservableCollection<Answer> OrderedAnswers { get; } = [];
    public ObservableCollection<Hint> UsedHints { get; } = [];

    private bool _answerAlreadyGiven;
    public bool AnswerAlreadyGiven
    {
        get => _answerAlreadyGiven;
        set
        {
            if (_answerAlreadyGiven == value) return;
            _answerAlreadyGiven = value;
            OnChange?.Invoke();
        }
    }

    private string? _speedWinnerNickname;
    public string? SpeedWinnerNickname
    {
        get => _speedWinnerNickname;
        set
        {
            _speedWinnerNickname = value;
            OnChange?.Invoke();
        }
    }

    private RoundStatistic[]? _roundStatistics;
    public RoundStatistic[]? RoundStatistics
    {
        get => _roundStatistics;
        set
        {
            _roundStatistics = value;
            OnChange?.Invoke();
        }
    }

    private ScenarioStatistic[]? _scenarioStatistic;
    public ScenarioStatistic[]? ScenarioStatistic
    {
        get => _scenarioStatistic;
        set
        {
            _scenarioStatistic = value;
            OnChange?.Invoke();
        }
    }

    private int? _shopDuration;
    public int? ShopDuration
    {
        get => _shopDuration;
        set
        {
            _shopDuration = value;
            OnChange?.Invoke();
        }
    }

    public Stock[]? ShopBackUp = null;
    
    private Stock[]? _shopStock;
    public Stock[]? ShopStock
    {
        get => _shopStock;
        set
        {
            _shopStock = value;
            OnChange?.Invoke();
        }
    }

    private string? _shopStatus;
    public string? ShopStatus
    {
        get => _shopStatus;
        set
        {
            _shopStatus = value;
            OnChange?.Invoke();
        }
    }

    private Stock? _currentShopItem;
    public Stock? CurrentShopItem
    {
        get => _currentShopItem;
        set
        {
            _currentShopItem = value;
            OnChange?.Invoke();
        }
    }

    private Stock? _chosenShopItem;
    public Stock? ChosenShopItem
    {
        get => _chosenShopItem;
        set
        {
            _chosenShopItem = value;
            OnChange?.Invoke();
        }
    }

    private bool _shopStateFinish;
    public bool ShopStateFinish
    {
        get => _shopStateFinish;
        set
        {
            if (_shopStateFinish == value) return;
            _shopStateFinish = value;
            OnChange?.Invoke();
        }
    }

    private readonly HashSet<ModifierType> _purchasedItems = [];
    public IReadOnlyCollection<ModifierType> PurchasedItems => _purchasedItems;
    
    private readonly HashSet<ModifierType> _bag = [];
    public IReadOnlyCollection<ModifierType> Bag => _bag;
    
    public void AddItemInBag(ModifierType item)
    {
        _purchasedItems.Add(item);
        if (_bag.Add(item))
            NotifyStateChanged();
    }

    public void RemoveItemInBag(ModifierType item)
    {
        if (_bag.Remove(item))
            NotifyStateChanged();
    }

    public GameService()
    {
        PlayersList.CollectionChanged += CollectionChangedHandler;
        OrderedAnswers.CollectionChanged += CollectionChangedHandler;
        UsedHints.CollectionChanged += CollectionChangedHandler;
    }
    
    private void CollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e) => OnChange?.Invoke();
    private void NotifyStateChanged() => OnChange?.Invoke();
}
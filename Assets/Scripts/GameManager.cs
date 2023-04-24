using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using map;
using player;
using Turn.Phases;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private TerritoryRepository _tr;
    private ContinentRepository _cr;

    [SerializeField]
    private int _nPlayers = 2;
    public int NPlayers => _nPlayers;
    
    public List<Player> Players;

    private Queue<Player> _playerQueue = new();
    private Player _currentPlayer;

    private IPhase _currentPhase;

    private IPhase[] _phases;
    
    public int Turn => _turn;
    private int _turn;
    public ReinforcePhase _reinforcePhase { get; private set; }
    public AttackPhase _attackPhase { get; private set; }
    public FortifyPhase _fortifyPhase { get; private set; }
    public EmptyPhase _emptyPhase { get; private set; }
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        SetupPhases();
        _tr = TerritoryRepository.Instance;
        _cr = ContinentRepository.Instance;
    }

    private void SetupPhases()
    {
        var cr = ContinentRepository.Instance;
        var tr = TerritoryRepository.Instance;
        _reinforcePhase = new ReinforcePhase(this, cr, tr);
        _attackPhase = new AttackPhase(this, cr, tr);
        _fortifyPhase = new FortifyPhase(this, cr, tr);
        _emptyPhase = new EmptyPhase();
        _currentPhase = _emptyPhase;
        
        _phases = new IPhase[]
        {
            _reinforcePhase,
            _attackPhase,
            _fortifyPhase
        };
    }


    private void Start()
    {
        SetupGame();
        
        NextTurn();
    }

    private void SetupGame()
    {
        for (var i = Players.Count; i < NPlayers; i++)
        {
            Players.Add(PlayerCreator.Instance.NewPlayer());
        }
        

        _tr.RandomlyAssignTerritories(Players);
        DistributeTroops();
        EnqueuePlayers();
    }

    private void EnqueuePlayers()
    {
        var playerOrder = Enumerable.Range(0, NPlayers).ToList();
        playerOrder.Shuffle();
        foreach (var i in playerOrder) 
            _playerQueue.Enqueue(Players[i]);
        
        _playerQueue = new Queue<Player>();
    }

    private void DistributeTroops()
    {
        int[] troopsPerNumberOfPlayer = { -1, -1, 50, 35, 30, 25, 20 };
        int troopsPerPlayer = troopsPerNumberOfPlayer[NPlayers];
        
        foreach (var player in Players)
            player.RandomlyDistributeTroops(troopsPerPlayer);
    }
    
    public void NextTurnPhase()
    {
        _currentPhase.End(_currentPlayer);
        _currentPhase = _currentPhase switch
        {
            ReinforcePhase => _attackPhase,
            AttackPhase => _fortifyPhase,
            FortifyPhase => _emptyPhase,
            EmptyPhase => _emptyPhase,
            _ => throw new ArgumentOutOfRangeException()
        };

        StartTurnPhase();
        if (_currentPhase == _emptyPhase)
        { 
            NextTurn();
        }
    }

    private void NextTurn()
    {
        do
        {
            _currentPlayer = _playerQueue.Dequeue();
        } while (_currentPlayer.IsDead());
        
        _playerQueue.Enqueue(_currentPlayer);
        
        _turn++;
        _currentPhase = _reinforcePhase;
        StartTurnPhase();
    }

    private void StartTurnPhase() => _currentPhase.Start(_currentPlayer);

    private void GameOver()
    {
        throw new NotImplementedException();
    }

}


internal enum GameState
{
    Setup,
    Play,
    GameOver
}
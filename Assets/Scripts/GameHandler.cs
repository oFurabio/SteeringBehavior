using System;
using UnityEngine;

public class GameHandler : MonoBehaviour {
    public static GameHandler Instance { get; private set; }

    public int PlayersAmount { get { return _numberOfPlayers; } }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameResume;

    private enum GameStates { WaitingToStart, GamePlaying, GamePaused, GameOver }

    [SerializeField] private GameStates _gameState;
    [Space]
    [SerializeField] private int _numberOfPlayers = 12;
    [SerializeField] private float _roundDuration = 60f;

    private float _waitingToStartTimer = 2f;
    private float _gamePlayingTimer;
    private bool _isGamePaused = false;


    private void Awake() {
        Instance = this;

        _gameState = GameStates.WaitingToStart;
    }

    private void Update() {
        HandleGameState();
    }

    private void HandleGameState() {
        switch (_gameState) {
            case GameStates.WaitingToStart:
                _waitingToStartTimer -= Time.deltaTime;

                if (_waitingToStartTimer < 0f) {
                    _gameState = GameStates.GamePlaying;
                    _gamePlayingTimer = _roundDuration;
                    CreateATagger();
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

            break;

            case GameStates.GamePlaying:
                _gamePlayingTimer -= Time.deltaTime;

                if (_gamePlayingTimer < 0f) {
                    _gameState = GameStates.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

            break;

            case GameStates.GameOver:
                GameObject perdedor = GameObject.FindGameObjectWithTag("Pegador");
                Destroy(perdedor);

                _gameState = GameStates.WaitingToStart;
                OnStateChanged?.Invoke(this, EventArgs.Empty);
            break;
        }
    }

    public bool IsGamePlaying() {
        return _gameState == GameStates.GamePlaying;
    }

    public bool IsGamePaused() {
        return _gameState == GameStates.GamePaused;
    }

    public bool IsGameOver() {
        return _gameState == GameStates.GameOver;
    }

    public float GetGamePlayingTimerNormalized() {
        return 1 - (_gamePlayingTimer / _roundDuration);
    }

    public float GetGamePlayingTimer() {
        return _gamePlayingTimer;
    }

    public void TogglePauseGame() {
        if (IsGamePlaying() || IsGamePaused()) {
            _isGamePaused = !_isGamePaused;
            if (_isGamePaused) {
                Time.timeScale = 0f;
                _gameState = GameStates.GamePaused;
                OnGamePaused?.Invoke(this, EventArgs.Empty);
            } else {
                Time.timeScale = 1f;
                _gameState = GameStates.GamePlaying;
                OnGameResume?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void CreateATagger() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        players[UnityEngine.Random.Range(0, players.Length)].tag = "Pegador";
    }
}

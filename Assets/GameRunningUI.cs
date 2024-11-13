using UnityEngine;
using UnityEngine.UI;

public class GameRunningUI : MonoBehaviour {
    [SerializeField] private Slider _gameTimer;

    private void Start() {
        GameHandler.Instance.OnStateChanged += Instance_OnStateChanged;

        Hide();
    }

    private void OnDestroy() {
        GameHandler.Instance.OnStateChanged -= Instance_OnStateChanged;
    }

    private void Instance_OnStateChanged(object sender, System.EventArgs e) {
        if(GameHandler.Instance.IsGamePlaying())
            Show();
    }

    private void Update() {
        _gameTimer.value = GameHandler.Instance.GetGamePlayingTimerNormalized();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }
}

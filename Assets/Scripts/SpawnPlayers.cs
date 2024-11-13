using UnityEngine;

public class SpawnPlayers : MonoBehaviour {
    [SerializeField] private GameObject _playersPrefab;

    private void Start() {
        for (int i = 0; i < GameHandler.Instance.PlayersAmount; i++) {
            Instantiate(_playersPrefab);
        }
    }
}

using UnityEngine;

/// <summary>
/// Classe singleton que armazena os esconderijos do jogo.
/// </summary>
public sealed class GameEnvironment {
    private static readonly GameEnvironment Instance = new GameEnvironment();
    private static GameObject[] esconderijos;

    static GameEnvironment() {
        esconderijos = GameObject.FindGameObjectsWithTag("Esconderijo");
    }

    private GameEnvironment() { }
    public static GameEnvironment GetInstance { get { return Instance; } }
    public GameObject[] GetEsconderijos() { return esconderijos; }
}

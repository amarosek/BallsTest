using UnityEngine;
using UnityEngine.UI;

public class SpawnedBallsText : MonoBehaviour
{
    [SerializeField] GameObject UIPanel;
    [SerializeField] Text UITextBallsSpawned;
    [SerializeField] BallSpawner UpdateUIForPlayer;

    public float spawnedBalls = -1;

    void Update()
    {
        if (UpdateUIForPlayer == null)
        {
            return;
        }
        if (UpdateUIForPlayer.spawnedBalls != spawnedBalls)
        {
            spawnedBalls = UpdateUIForPlayer.spawnedBalls;
            UITextBallsSpawned.text = $"BALLS SPAWNED: {spawnedBalls}";
        }
    }
}

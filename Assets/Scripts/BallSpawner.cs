using System.Collections;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab;
    [SerializeField] public Transform newBallTransform;
    [SerializeField] Camera mainCamera;

    [SerializeField] float secondsBetweendspawn = 0.5f;

    Vector3 randomSpawnPosition;

    public int spawnedBalls { get; private set; } = 0;


    void Start()
    {
        StartCoroutine(SpawnBalls());
    }

    void Update()
    {
        if (spawnedBalls >= 160)
        { 
            StopAllCoroutines(); 
        }
    }

    IEnumerator SpawnBalls()
    {
        while (true)
        {
            // set spawn inside camera view
            float height = mainCamera.orthographicSize;
            float width = mainCamera.orthographicSize * mainCamera.aspect + 1;
            randomSpawnPosition = new Vector3(Random.Range(-width, width), Random.Range(-height, height), Random.Range(mainCamera.nearClipPlane - 5, mainCamera.farClipPlane - 20));

            // create new ball
            var newBall = Instantiate(ballPrefab,randomSpawnPosition, Quaternion.identity);
            newBall.transform.parent = newBallTransform;

            // count created balls
            spawnedBalls++;

            yield return new WaitForSeconds(secondsBetweendspawn);
        }
    }
}

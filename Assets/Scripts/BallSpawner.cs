using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab;
    [SerializeField] public Transform newBallTransform;
    [SerializeField] Camera mainCamera;

    [SerializeField] float secondsBetweendspawn = 0.5f;

    Vector3 randomSpawnPosition;
    Vector3 spawnPoint;
    Vector3 screenBounds;

    public int spawnedBalls { get; private set; } = 0;




    void Start()
    {
        screenBounds = mainCamera.ScreenToWorldPoint(spawnPoint);

        StartCoroutine(SpawnBalls());
    }

    void Update()
    {
        if (spawnedBalls >= 100)
        { 
            StopAllCoroutines(); 
        }
    }

    IEnumerator SpawnBalls()
    {
        while (true)
        {
            float height = mainCamera.orthographicSize;
            float width = mainCamera.orthographicSize * mainCamera.aspect + 1;
            randomSpawnPosition = new Vector3(Random.Range(-width, width), Random.Range(-height, height), Random.Range(mainCamera.nearClipPlane - 5, mainCamera.farClipPlane - 20));

            var newBall = Instantiate(ballPrefab,randomSpawnPosition, Quaternion.identity);
            newBall.transform.parent = newBallTransform;

            spawnedBalls++;

            yield return new WaitForSeconds(secondsBetweendspawn);
        }
    }
}

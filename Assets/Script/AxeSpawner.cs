using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 一定間隔で斧を生成するクラス
/// </summary>
public class AxeSpawner : MonoBehaviour
{
    [SerializeField] 
    private GameObject axePrefab;
    [SerializeField]
    private float spawnInterval = 1f;
    [SerializeField] 
    private float range = 5f;
    [SerializeField] 
    private float axeSpeed = 3f;

    private float spawnTimer;

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        while (spawnTimer >= spawnInterval)
        {
            SpawnAxe();
            spawnTimer -= spawnInterval;
        }
    }

    /// <summary>
    /// ランダムな位置へ向かう斧を生成する
    /// </summary>
    private void SpawnAxe()
    {
        Vector3 spawnPosition = transform.position;

        Vector3 targetPosition =
            spawnPosition +
            new Vector3(
                Random.Range(-range, range),
                Random.Range(0f, range),
                0f);

        GameObject axe =
            Instantiate(
                axePrefab,
                spawnPosition,
                Quaternion.identity);

        if (axe.TryGetComponent(out AxeController axeController))
        {
            axeController.Initialize(targetPosition);
            axeController.SetSpeed(axeSpeed);
        }
    }
}
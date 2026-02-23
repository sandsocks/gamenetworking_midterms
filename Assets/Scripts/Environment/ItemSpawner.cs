using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef[] _itemPrefabs;
    [SerializeField] private float _spawnInterval = 10f;
    [SerializeField] private Vector3 _spawnArea = new Vector3(10, 0, 10);

    private TickTimer _spawnTimer;

    public override void FixedUpdateNetwork()
    {
        if (Runner.IsServer && _spawnTimer.ExpiredOrNotRunning(Runner))
        {
            SpawnRandomItem();
            _spawnTimer = TickTimer.CreateFromSeconds(Runner, _spawnInterval);
        }
    }

    void SpawnRandomItem()
    {
        int index = Random.Range(0, _itemPrefabs.Length);
        Vector3 spawnPos = transform.position + new Vector3(
            Random.Range(-_spawnArea.x, _spawnArea.x),
            1,
            Random.Range(-_spawnArea.z, _spawnArea.z)
        );
        
        Runner.Spawn(_itemPrefabs[index], spawnPos, Quaternion.identity);
    }
}

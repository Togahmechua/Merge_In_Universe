using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class Spawner : MonoBehaviour
{
    private Planet curPlanet;

    private void OnEnable()
    {
        Planet.OnPlanetMerged += HandlePlanetMerged;
    }

    private void OnDisable()
    {
        Planet.OnPlanetMerged -= HandlePlanetMerged;
    }

    private void HandlePlanetMerged(Planet mergedPlanet)
    {
        // So sánh planet mới được tạo với current
        if (curPlanet == null)
        {
            Debug.Log("curPlanet đã bị despawn rồi → sẵn sàng spawn mới");
            return;
        }

        if (!curPlanet.gameObject.activeInHierarchy)
        {
            Debug.Log("curPlanet bị despawn → sẵn sàng spawn mới");
            curPlanet = null;
        }
        else if (mergedPlanet == curPlanet)
        {
            Debug.Log("Merged chính là current → OK");
            curPlanet = null;
        }
        else
        {
            Debug.Log("Merged but not current → Ignore");
        }
    }


    private void Start()
    {
        StartCoroutine(SpawnPlanetRoutine());
    }

    private IEnumerator SpawnPlanetRoutine()
    {
        while (true)
        {
            if (curPlanet == null || curPlanet.HasLanded)
            {
                SpawnPlanet();
            }

            yield return new WaitForSeconds(2f);
        }
    }

    private PoolType GetRandomPoolType()
    {
        // 90% là 4 loại đầu tiên, 10% là loại thứ 5
        float chance = Random.value;

        if (chance < 0.9f)
        {
            // Random từ 0 đến 3 (Mercury → Mars)
            int index = Random.Range(1, 5);
            return (PoolType)index;
        }
        else
        {
            return PoolType.Jupiter; // loại hiếm
        }
    }

    private void SpawnPlanet()
    {
        PoolType typeToSpawn = GetRandomPoolType();
        curPlanet = SimplePool.Spawn<Planet>(typeToSpawn, transform.position, Quaternion.identity);
    }
}

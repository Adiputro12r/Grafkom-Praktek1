using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    [SerializeField] private Transform treePrefab;
    
    private TaskSystem taskSystem;

    private void Start()
    {
        // PERBAIKAN: Gunakan FindFirstObjectByType (Versi Baru)
        taskSystem = FindFirstObjectByType<TaskSystem>();
    }

    public HashSet<int> Init(float z, float spawnChance)
    {
        // PERBAIKAN: Cek juga disini menggunakan perintah baru
        if (taskSystem == null) taskSystem = FindFirstObjectByType<TaskSystem>();

        transform.position = new Vector3(0, 0, z);
        HashSet<int> location = new() { -6, 6 };

        // 1. Spawn Pohon
        int numTrees = Random.Range(1, 5);
        for (int i = 0; i < numTrees; i++)
        {
            Transform tree = Instantiate(treePrefab, transform);
            int xPos = Random.Range(-5, 6);
            tree.position = new Vector3(xPos, 0.1f, z);
            location.Add(xPos);
        }

        // 2. Spawn Collectible (Hanya jika task belum selesai)
        if (Random.value < spawnChance)
        {
            int xPos = Random.Range(-5, 6);
            int attempts = 0;

            // Cari posisi kosong
            while (location.Contains(xPos) && attempts < 10)
            {
                xPos = Random.Range(-5, 6);
                attempts++;
            }

            // Pastikan taskSystem ketemu sebelum dipakai
            if (!location.Contains(xPos) && taskSystem != null)
            {
                // Minta prefab sampah yang dibutuhkan dari TaskSystem
                GameObject prefabToSpawn = taskSystem.GetNeededPrefab();

                if (prefabToSpawn != null)
                {
                    GameObject collectible = Instantiate(prefabToSpawn, transform);
                    collectible.transform.position = new Vector3(xPos, 0.5f, z);
                }
            }
        }
        return location;
    }
}
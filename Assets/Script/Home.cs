using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
    [SerializeField] private Transform homePrefab;
    
    // HAPUS List manual karena sekarang kita minta ke TaskSystem
    // [SerializeField] private List<GameObject> collectiblePrefabs; 

    private TaskSystem taskSystem;

    private void Start()
    {
        // Cari TaskSystem saat script mulai (Gunakan FindFirstObjectByType agar tidak warning)
        taskSystem = FindFirstObjectByType<TaskSystem>();
    }

    public HashSet<int> Init(float z, float spawnChance)
    {
        // Safety check: Jika Init dipanggil sebelum Start, cari dulu
        if (taskSystem == null) taskSystem = FindFirstObjectByType<TaskSystem>();

        transform.position = new Vector3(0, 0, z);
        HashSet<int> location = new() { -6, 6 };

        // --- 1. LOGIKA SPAWN RUMAH (OBSTACLE) ---
        int numHomes = Random.Range(1, 5);
        for (int i = 0; i < numHomes; i++)
        {
            Transform home = Instantiate(homePrefab, transform);
            int xPos = Random.Range(-5, 6);
            home.position = new Vector3(xPos, 0.1f, z);
            location.Add(xPos);
        }

        // --- 2. LOGIKA SPAWN COLLECTIBLE (SESUAI GOAL) ---
        if (Random.value < spawnChance)
        {
            int xPos = Random.Range(-5, 6);
            int attempts = 0; 

            // Cari posisi kosong yang tidak ada rumahnya
            while (location.Contains(xPos) && attempts < 10)
            {
                xPos = Random.Range(-5, 6);
                attempts++;
            }

            // Jika ketemu posisi kosong & TaskSystem ada
            if (!location.Contains(xPos) && taskSystem != null)
            {
                // MINTA PREFAB YANG DIBUTUHKAN DARI TASKSYSTEM
                GameObject prefabToSpawn = taskSystem.GetNeededPrefab();

                // Jika masih ada sampah yang dibutuhkan (tidak null), spawn!
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
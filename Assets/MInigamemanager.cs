using UnityEngine;
using System.Collections.Generic;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance; // Singleton sederhana

    [Header("Settings")]
    public List<GameObject> trashPrefabs; // Masukkan prefab sampah di sini
    public List<Transform> spawnPoints;   // Masukkan posisi spawn (Empty GameObject di UI)
    public GameObject nextStageButton;    // Tombol Next Stage

    private int totalTrash;
    private int collectedTrash = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        nextStageButton.SetActive(false); // Sembunyikan tombol di awal
        SpawnTrash();
    }

    void SpawnTrash()
    {
        // Hitung ada berapa titik spawn yang tersedia
        int numberOfSlots = spawnPoints.Count;
        
        // Update total sampah yang harus diselesaikan player sesuai jumlah slot
        totalTrash = numberOfSlots;

        // Loop sebanyak jumlah TEMPAT SPAWN, bukan jumlah prefab
        for (int i = 0; i < numberOfSlots; i++)
        {
            // --- BAGIAN KUNCI ---
            // Kita ambil satu prefab secara acak dari list 'trashPrefabs'
            // Walaupun list isinya cuma 1 (misal Apel), dia akan terambil terus menerus.
            int randomIndex = Random.Range(0, trashPrefabs.Count);
            GameObject prefabToSpawn = trashPrefabs[randomIndex];
            
            // ---------------------

            // Spawn sampah di posisi spawn point ke-i
            GameObject newTrash = Instantiate(prefabToSpawn, spawnPoints[i].position, Quaternion.identity);
            
            // Atur parent ke spawn point agar rapi
            newTrash.transform.SetParent(spawnPoints[i]);
            
            // Reset skala (penting untuk UI)
            newTrash.transform.localScale = Vector3.one; 
        }
     
    // Update total sampah yang harus dimasukkan player
    totalTrash = spawnPoints.Count;
    }

    public void CheckProgress()
    {
        collectedTrash++;
        
        // Cek apakah semua sampah sudah masuk
        if (collectedTrash >= totalTrash)
        {
            GameFinished();
        }
    }

    void GameFinished()
    {
        Debug.Log("Level Selesai!");
        nextStageButton.SetActive(true); // Munculkan tombol
    }
}
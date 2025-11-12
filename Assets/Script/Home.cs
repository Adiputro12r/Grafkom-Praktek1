using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
    [SerializeField] private Transform HomePrefab;

    [Header("Collectibles")]
    [SerializeField] private List<GameObject> collectiblePrefabs; // Tempat menaruh prefab Plastic, Iron, Stick
    [SerializeField] [Range(0, 1)] private float collectibleSpawnChance = 0.1f; // Peluang muncul (10%)

    public HashSet<int> Init(float z)
    {
        //place the obstacle at the location provided
        transform.position = new Vector3(0, 0, z);


        //we always have obstacles outside the area
        HashSet<int> location = new() { -6, 6 };

        //populate with some obstacles 
        int numHomes = Random.Range(1, 5);
        for (int i = 0; i < numHomes; i++)
        {
            //create new home object
            Transform home = Instantiate(HomePrefab, transform);
            //put home at random location
            int xPos = Random.Range(-5, 6);
            home.position = new Vector3(xPos, 0.1f, z);

            //record the location in our HashSet
            location.Add(xPos);

            // Decide whether to spawn a collectible

        }
        if (Random.value < collectibleSpawnChance && collectiblePrefabs.Count > 0)
        {
            int xPos = Random.Range(-5, 6);
            int attempts = 0; // Penjaga agar tidak looping selamanya

            // Cari posisi X yang kosong (tidak ada pohon)
            // Kita beri batas 10 kali percobaan
            while (location.Contains(xPos) && attempts < 10)
            {
                xPos = Random.Range(-5, 6);
                attempts++;
            }

            // Jika kita berhasil menemukan tempat kosong
            if (!location.Contains(xPos))
            {
                // Pilih sampah secara acak dari daftar (Plastic, Iron, atau Stick)
                int prefabIndex = Random.Range(0, collectiblePrefabs.Count);
                GameObject prefabToSpawn = collectiblePrefabs[prefabIndex];
                
                // Munculkan sampah di lokasi tersebut
                GameObject collectible = Instantiate(prefabToSpawn, transform);
                collectible.transform.position = new Vector3(xPos, 0.5f, z);
                
                // PENTING: Kita *tidak* menambahkan 'xPos' ke 'location'
                // Ini karena sampah bukan rintangan, pemain harus bisa melewatinya.
            }
        }
        return location;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

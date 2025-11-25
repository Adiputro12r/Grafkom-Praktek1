using System.Collections.Generic;
using UnityEngine;

public class River : MonoBehaviour
{
    [SerializeField] private List<Rigidbody> logs; // Masukkan prefab Log di sini
    [SerializeField] private float speed = 2f;
    
    private int direction = 1;
    private List<Rigidbody> spawnedLogs = new();

    public HashSet<int> Init(float z)
    {
        transform.position = new Vector3(0, 0.0f, z); // Posisi air sedikit di bawah

        // Tentukan arah aliran sungai (-1 kiri, 1 kanan)
        direction = 2 * Random.Range(0, 2) - 1;

        // Kecepatan sungai
        float minSpeed = Mathf.Lerp(2.0f, 4.0f, z / 500f);
        float maxSpeed = Mathf.Lerp(4.0f, 7.0f, z / 500f);
        speed = Random.Range(minSpeed, maxSpeed);

        // Spawn Logs
        int idx = Random.Range(0, logs.Count);
        // Kita butuh lebih banyak log agar player bisa lompat
        int logCount = Random.Range(3, 6); 
        float spacing = Random.Range(4.0f, 7.0f);

        for (int i = 0; i < logCount; i++)
        {
            // Spawn Log
            Rigidbody log = Instantiate(logs[idx],
                new Vector3(i * spacing * -direction, 0.1f, z), // Y=0.1f agar sejajar player
                Quaternion.identity,
                transform);
            
            // Atur arah hadap log (jika perlu)
            if (direction == 1) log.transform.rotation = Quaternion.Euler(0, 0, 0);
            else log.transform.rotation = Quaternion.Euler(0, 180, 0);

            spawnedLogs.Add(log);
        }

        // Return hanya dinding batas (-6 dan 6). 
        // Bagian tengah KOSONG (tidak diblokir) agar player bisa melompat ke sungai (log/air).
        return new() { -6, 6 };
    }

    private void FixedUpdate()
    {
        foreach (Rigidbody log in spawnedLogs)
        {
            Vector3 moveAmount = new(speed * direction * Time.fixedDeltaTime, 0, 0);
            log.MovePosition(log.position + moveAmount);

            // Loop log agar kembali muncul dari sisi lain
            Vector3 pos = log.position;
            if ((direction > 0) && (pos.x > 12))
            {
                pos.x = -12;
                log.position = pos;
            }
            else if ((direction < 0) && (pos.x < -12))
            {
                pos.x = 12;
                log.position = pos;
            }
        }
    }
}
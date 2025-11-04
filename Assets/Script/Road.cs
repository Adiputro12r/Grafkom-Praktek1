using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    [SerializeField] private List<Rigidbody> vehicles;

    private int direction = 1;
    private float speed = 1f;

    private List<Rigidbody> spawnedVehicles = new();

    public HashSet<int> Init(float z)

    {
        //place obstacle at the location
        transform.position = new Vector3(0, 0, z);

        //choose which direction the vehicles will go -1 or 1
        direction = 2 * Random.Range(0, 2) - 1;

        //choose speed we make it faster the further we go
        float minSpeed = Mathf.Lerp(1.0f, 5.0f, z / 500f);
        float maxSpeed = Mathf.Lerp(5.0f, 10.0f, z / 500f);
        speed = Random.Range(minSpeed, maxSpeed);

        //chose which vehicle  how many and how far apart they are
        int idx = Random.Range(0, vehicles.Count);
        int vehicleCount = Random.Range(1, 5);
        float spacing = Random.Range(3.0f, 6.0f);

        //Instantiate the vehicles
        for (int i = 0; i < vehicleCount; i++)
        {
            Rigidbody vehicle = Instantiate(vehicles[idx],
            new Vector3(i * spacing * -direction, 0.1f, z),
            Quaternion.Euler(0, 90 * direction, 0f),
             transform);
            spawnedVehicles.Add(vehicle);
        }
        //The only obstacle are those outside the game area
        return new() { -6, 6 };
    }
    private void FixedUpdate()
    {
        //move vehicles
        foreach (Rigidbody vehicle in spawnedVehicles)
        {
            //move along the road us the rb movement so collison are handled corectly
            Vector3 moveAmount = new(speed * direction * Time.fixedDeltaTime, 0, 0);
            vehicle.MovePosition(vehicle.position + moveAmount);

            //wrap around camera when they are out of camera view
            Vector3 pos = vehicle.position;
            if ((direction > 0) && (pos.x > 12))
            {
                pos.x = -12;
                vehicle.position = pos;
            }
            else if ((direction < 0) && (pos.x < -12))
            {
                pos.x = 12;
                vehicle.position = pos;
            }

        }
    }
}


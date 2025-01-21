using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(ObjectPooler))]
public class BubbleSpawner : MonoBehaviour
{
    private ObjectPooler pooler;

    // Define how the bubbles spawn
    // - create pattern and move them out
    // - how many at each time
    // - where around the spawn point at each time
    // 
    // Bubble will need some information about how it should move

    private bool spawned = false;

    void Start()
    {

        pooler = GetComponent<ObjectPooler>();


        InvokeRepeating(nameof(SpawnObjects), 1, 1);
    }
    void Update()
    {
        if (!spawned)
        {
            spawned = true;

            Quaternion rotation = transform.rotation;

            Vector3 position1 = transform.position + new Vector3(0.5f, 0.0f, 0.0f);
            Quaternion rotation1 = Quaternion.Euler(Vector3.up * 90.0f);
            pooler.SpawnFromPool("bubbles", position1, rotation1);

            Vector3 position2 = transform.position + new Vector3(-0.5f, 0.0f, 0.0f);
            Quaternion rotation2 = Quaternion.Euler(Vector3.up * 270.0f);
            pooler.SpawnFromPool("bubbles", position2, rotation2);

            Vector3 position3 = transform.position + new Vector3(0f, 0.0f, 0.5f);
            Quaternion rotation3 = Quaternion.Euler(Vector3.up * 0);
            pooler.SpawnFromPool("bubbles", position3, rotation3);

            Vector3 position4 = transform.position + new Vector3(0f, 0.0f, -0.5f);
            Quaternion rotation4 = Quaternion.Euler(Vector3.up * 180.0f);
            pooler.SpawnFromPool("bubbles", position4, rotation4);
        }

    }

    void SpawnObjects()
    {
        Quaternion rotation = transform.rotation;

        Vector3 position1 = transform.position + new Vector3(0.5f, 0.0f, 0.0f);
        Quaternion rotation1 = Quaternion.Euler(Vector3.up * 90.0f);
        pooler.SpawnFromPool("bubbles", position1, rotation1);

        Vector3 position2 = transform.position + new Vector3(-0.5f, 0.0f, 0.0f);
        Quaternion rotation2 = Quaternion.Euler(Vector3.up * 270.0f);
        pooler.SpawnFromPool("bubbles", position2, rotation2);

        Vector3 position3 = transform.position + new Vector3(0f, 0.0f, 0.5f);
        Quaternion rotation3 = Quaternion.Euler(Vector3.up * 0);
        pooler.SpawnFromPool("bubbles", position3, rotation3);

        Vector3 position4 = transform.position + new Vector3(0f, 0.0f, -0.5f);
        Quaternion rotation4 = Quaternion.Euler(Vector3.up * 180.0f);
        pooler.SpawnFromPool("bubbles", position4, rotation4);
    }
}

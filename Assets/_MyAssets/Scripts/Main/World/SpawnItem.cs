using SO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnItem : MonoBehaviour
{
    SO_Spawner spawnerSO;
    [SerializeField, Header("ÉXÉ|Å[ÉìÇµÇƒó«Ç¢îÕàÕ")] List<GameObject> SpawnRange;
    List<Vector2> spawnRangeVector;
    SpawnObj kinokoInstance;
    void Awake()
    {
        spawnerSO = SO_Spawner.Entity;
        spawnRangeVector = new List<Vector2>();
        foreach (GameObject obj in SpawnRange)
        {
            Vector2 position = new Vector2(obj.transform.position.x, obj.transform.position.z);
            spawnRangeVector.Add(position);
        }
    }
    void Start()
    {
        kinokoInstance = spawnerSO.GetInstanceByName("Kinoko");
        StartCoroutine(CreateKinoko());
    }
    IEnumerator CreateKinoko()
    {
        Vector2 spawnPos;
        do
        {
            spawnPos = new Vector2(
            Random.Range(spawnerSO.X0, spawnerSO.X1),
            Random.Range(spawnerSO.Y0, spawnerSO.Y1));
        } while (CheckSpawnPoint(spawnPos) == false);
        Instantiate(kinokoInstance.spawnObj, new Vector3(spawnPos.x, 0, spawnPos.y), Quaternion.identity);
        yield return new WaitForSeconds(kinokoInstance.createOffsetTime);
        StartCoroutine(CreateKinoko());
    }
    bool CheckSpawnPoint(Vector2 spawnPoint)
    {
        int boundaryPointsCount = spawnRangeVector.Count;
        float windingNum = 0.0f;
        for (int bp = 0; bp < boundaryPointsCount; bp++)
        {
            Vector2 firstPoint = spawnRangeVector[bp];
            Vector2 secondPoint = spawnRangeVector[(bp + 1) % boundaryPointsCount];

            Vector2 firstVector = spawnPoint - firstPoint;
            Vector2 secondVector = spawnPoint - secondPoint;

            if (firstVector.y * secondVector.y < 0.0f)
            {
                float dist = firstVector.x + (firstVector.y * (secondVector.x - firstVector.x))/ (firstVector.y - secondVector.y);
                if (dist < 0)
                {
                    if (firstVector.y < 0.0f) windingNum += 1.0f;
                    else windingNum -= 1.0f;
                }
            }
            else if (firstVector.y == 0.0f && firstVector.x > 0.0f)
            {
                if (secondVector.y > 0.0f) windingNum += 0.5f;
                else windingNum -= 0.5f;
            }
            else if (secondVector.y == 0.0f && secondVector.x > 0.0f)
            {
                if (firstVector.y < 0.0f) windingNum += 0.5f;
                else windingNum -= 0.5f;
            }
        }
        return ((int)windingNum % 2) != 0;
    }
 
}

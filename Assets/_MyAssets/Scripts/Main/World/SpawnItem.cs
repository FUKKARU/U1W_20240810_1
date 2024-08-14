using SO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    SO_Spawner spawnerSO;
    void Awake()
    {
        spawnerSO = SO_Spawner.Entity;
    }
    bool CheckSpawnPoint(Vector2 spawnPoint)
    {
        int boundaryPointsCount = spawnerSO.SpawnRange.Count;
        float windingNum = 0.0f;
        for (int bp = 0; bp < boundaryPointsCount; bp++)
        {
            Vector2 firstPoint = spawnerSO.SpawnRange[bp];
            Vector2 secondPoint = spawnerSO.SpawnRange[(bp + 1) % boundaryPointsCount];
            Vector2 firstVector = spawnPoint - firstPoint;
            Vector2 secondVector = spawnPoint - secondPoint;

            if (firstVector.y * secondVector.y < 0.0f)
            {
                float dist = firstPoint.x +
                    (firstPoint.y * (secondPoint.x - firstPoint.x))
                    / (firstPoint.y - secondPoint.y);
                if (dist < 0)
                {
                    if (firstPoint.y < 0.0f) windingNum += 1.0f;
                    else windingNum -= 1.0f;
                }
            }
            else if (firstPoint.y == 0 && firstPoint.x > 0.0f)
            {
                if (secondPoint.y > 0.0f) windingNum += 0.5f;
                else windingNum -= 0.5f;
            }
            else if (secondPoint.y == 0 && secondPoint.x > 0.0f)
            {
                if (firstPoint.y < 0.0f) windingNum += 0.5f;
                else windingNum -= 0.5f;
            }
        }

        return ((int)windingNum % 2) != 0;
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Main.Spawn
{
    [RequireComponent(typeof(LineRenderer))]
    public class SpawnRangeUI : MonoBehaviour
    {
        [SerializeField] List<GameObject> spawnRangeObjs;
        LineRenderer lineRenderer;
        void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
            lineRenderer.positionCount = spawnRangeObjs.Count + 1;
        }

        void Update()
        {
            for (int i = 0; i < spawnRangeObjs.Count; i++)
            {
                lineRenderer.SetPosition(i, spawnRangeObjs[i].transform.position);
            }
            lineRenderer.SetPosition(spawnRangeObjs.Count, spawnRangeObjs[0].transform.position);
        }
    }

}


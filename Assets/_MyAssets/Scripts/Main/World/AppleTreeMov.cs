using SO;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Apple
{
    public class AppleTreeMov : MonoBehaviour
    {
        [NonSerialized] public List<GameObject> apples = new List<GameObject>();
        [NonSerialized] public int appleNum = 0;
        [NonSerialized] public bool apple1Created = false;
        [NonSerialized] public bool apple2Created = false;
        [NonSerialized] public bool apple3Created = false;
        SO_Spawner spawnerSO;
        private void Awake()
        {
            spawnerSO = SO_Spawner.Entity;
        }
        public void FreeApple()
        {
            if (apple1Created)
            {
                try
                {
                    GameObject apple = transform.Find(spawnerSO.ApplePos1).GetChild(0).gameObject;
                    apple.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    apple.transform.parent = null;
                    apple.GetComponent<SphereCollider>().enabled = true;
                    apple1Created = false;
                    appleNum--;
                    Spawn.SpawnItem.Instance.AppleNumDec(1);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"例外がスローされました：{e}");
                }
            }


            if (apple2Created)
            {
                try
                {
                    GameObject apple = transform.Find(spawnerSO.ApplePos2).GetChild(0).gameObject;
                    apple.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    apple.transform.parent = null;
                    apple.GetComponent<SphereCollider>().enabled = true;
                    apple2Created = false;
                    appleNum--;
                    Spawn.SpawnItem.Instance.AppleNumDec(1);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"例外がスローされました：{e}");
                }
            }


            if (apple3Created)
            {
                try
                {
                    GameObject apple = transform.Find(spawnerSO.ApplePos3).GetChild(0).gameObject;
                    apple.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    apple.transform.parent = null;
                    apple.GetComponent<SphereCollider>().enabled = true;
                    apple3Created = false;
                    appleNum--;
                    Spawn.SpawnItem.Instance.AppleNumDec(1);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"例外がスローされました：{e}");
                }
            }
        }
    }
}
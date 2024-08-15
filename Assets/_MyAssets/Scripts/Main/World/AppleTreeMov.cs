using System;
using System.Collections;
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


        public void FreeApple()
        {
            if (apple1Created)
            {
                GameObject apple = transform.Find("applePos1").GetChild(0).gameObject;
                apple.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                apple.transform.parent = null;
                apple.GetComponent<SphereCollider>().enabled = true;
                apple1Created = false;
                appleNum--;
                Spawn.SpawnItem.Instance.AppleNumDec(1);
            }


            if (apple2Created)
            {
                GameObject apple = transform.Find("applePos2").GetChild(0).gameObject;
                apple.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                apple.transform.parent = null;
                apple.GetComponent<SphereCollider>().enabled = true;
                apple2Created = false;
                appleNum--;
                Spawn.SpawnItem.Instance.AppleNumDec(1);
            }


            if (apple3Created)
            {
                GameObject apple = transform.Find("applePos3").GetChild(0).gameObject;
                apple.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                apple.transform.parent = null;
                apple.GetComponent<SphereCollider>().enabled = true;
                apple3Created = false;
                appleNum--;
                Spawn.SpawnItem.Instance.AppleNumDec(1);
            }

        }
    }
}
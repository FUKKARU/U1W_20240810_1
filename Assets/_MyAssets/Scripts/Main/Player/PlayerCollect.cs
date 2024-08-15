using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Main.Player
{
    public class PlayerCollect : MonoBehaviour
    {
        int kinokoNum = 0;
        [SerializeField] int appleNum = 0;
        int aburaageNum = 0;
        bool onInteract = false;
        [SerializeField] Transform stackStartPoint;
        List<GameObject> collectedObjects = new List<GameObject>();
        public float moveSpeed = 5f;

        void Update()
        {
            if (onInteract)
            {
                //setactive = true;
            }
            // ���͂��擾
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");

            // �ړ��������v�Z
            Vector3 moveDirection = new Vector3(moveX, 0, moveY).normalized;

            // �L�����N�^�[���ړ�
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            StackHead();
        }

        void StackHead()
        {
            for (int i = 0; i < collectedObjects.Count; i++)
            {
                GameObject obj = collectedObjects[i];
                obj.transform.position = stackStartPoint.position + new Vector3(0, i * 0.25f, 0);
                obj.transform.localRotation = Quaternion.identity;
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Item/Kinoko")
            {
                kinokoNum++;
                Spawn.SpawnItem.Instance.KinokoNumDec();
                collectedObjects.Add(other.gameObject);
            }

            else if (other.gameObject.tag == "Tree")
            {
                GameObject appleTree = other.gameObject;
                Apple.AppleTreeMov appleTreeComponent = appleTree.GetComponent<Apple.AppleTreeMov>();
                appleTreeComponent.FreeApple();

            }

            else if (other.gameObject.tag == "Human")
            {
                onInteract = true;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Item/Apple")
            {
                appleNum++;
                collision.gameObject.GetComponent<SphereCollider>().enabled = false;
  
                collectedObjects.Add(collision.gameObject);
            }
        }


    }
}
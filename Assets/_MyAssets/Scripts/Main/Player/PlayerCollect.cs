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
        public float moveSpeed = 5f; // キャラクターの移動速度

        void Update()
        {
            if (onInteract)
            {
                //setactive = true;
            }
            // 入力を取得
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");

            // 移動方向を計算
            Vector3 moveDirection = new Vector3(moveX, 0, moveY).normalized;

            // キャラクターを移動
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Item/Kinoko")
            {
                kinokoNum++;
                Spawn.SpawnItem.Instance.KinokoNumDec();
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
                Destroy(collision.gameObject);
            }
        }


    }
}
using UnityEngine;

namespace Morikawa_Test_1
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private Transform playerBodyTf;
        [SerializeField] private Rigidbody playerBodyRb;
        [SerializeField] private Transform freeLookCamera;

        [SerializeField, Header("�ړ��X�s�[�h")] private float _moveSpeed;
        public float MoveSpeed
        {
            get { return _moveSpeed; }
            set { _moveSpeed = Mathf.Max(value, +0.0f); }
        }

        private void Start()
        {
            if (playerBodyTf == null) throw new System.Exception("PlayerBody��Transform���ݒ肳��Ă��܂���");
            if (playerBodyRb == null) throw new System.Exception("PlayerBody��Rigidbody���ݒ肳��Ă��܂���");
            if (freeLookCamera == null) throw new System.Exception("FreeLookCamera���ݒ肳��Ă��܂���");
            if (_moveSpeed == 0) throw new System.Exception("moveSpeed��0�ł�");
        }

        private void Update()
        {
            playerBodyRb.velocity = Vector3.zero;
            playerBodyTf.Turn(freeLookCamera);
            playerBodyTf.Move(IA.InputGetter.Instance, MoveSpeed);
        }
    }

    public static class Vector
    {
        /// <summary>
        /// �v���C���[���J�����̕����ɉ�]������
        /// </summary>
        public static void Turn(this Transform playerBodyTf, Transform freeLookCamera)
        {
            Vector3 lookAtLocal = playerBodyTf.position - freeLookCamera.position;
            Vector3 lookAtLocalXZ = new(lookAtLocal.x, 0, lookAtLocal.z);
            Vector3 lookAtXZ = playerBodyTf.position + lookAtLocalXZ;
            playerBodyTf.LookAt(lookAtXZ);
        }

        /// <summary>
        /// �v���C���[��O�㍶�E�ɓ�����
        /// </summary>
        public static void Move(this Transform playerBodyTf, IA.InputGetter inputGetter, float moveSpeed)
        {
            Vector2 moveValueInputted = inputGetter.Main_MoveValue2.Get<Vector2>();
            Vector3 moveValueLocalNormed = new Vector3(moveValueInputted.x, 0, moveValueInputted.y).normalized;
            Vector3 moveValueLocal = moveValueLocalNormed * (moveSpeed * Time.deltaTime);
            Vector3 moveValue = playerBodyTf.right * moveValueLocal.x + playerBodyTf.forward * moveValueLocal.z;
            playerBodyTf.localPosition += moveValue;
        }
    }
}
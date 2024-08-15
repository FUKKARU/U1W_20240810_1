using System;
using System.Collections.Generic;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_Spawner", fileName = "SO_Spawner")]
    public class SO_Spawner : ScriptableObject
    {
        #region
        public const string PATH = "SO_Spawner";

        private static SO_Spawner _entity = null;
        public static SO_Spawner Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<SO_Spawner>(PATH);

                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [SerializeField, Header("�X�|�[�����Ƀ����_���ɏo�͂���X���W�̎n�_")] float x0;
        public float X0 => x0;
        [SerializeField, Header("�X�|�[�����Ƀ����_���ɏo�͂���Y���W�̎n�_")] float y0;
        public float Y0 => y0;
        [SerializeField, Header("�X�|�[�����Ƀ����_���ɏo�͂���X���W�̏I�_")] float x1;
        public float X1 => x1;
        [SerializeField, Header("�X�|�[�����Ƀ����_���ɏo�͂���Y���W�̏I�_")] float y1;
        public float Y1 => y1;

        [SerializeField, Header("�X�|�[���p�I�u�W�F�N�g���i�[")] List<SpawnObj> spawnObjs;
        public List<SpawnObj> SpawnObjs => spawnObjs;

        [SerializeField, Header("�L�m�R�̍ő吔")] int maxKinokoNum;
        public int �laxKinokoNum => maxKinokoNum;

        /// <summary>
        /// �����o�ϐ��̖��O����C���X�^���X���擾
        /// </summary>
        /// <param name="name">SpawnObj�N���X�̃����o�ϐ��Ƃ��ēo�^����Ă��閼�O���w��</param>
        /// <returns></returns>
        public SpawnObj GetInstanceByName(string name)
        {
            foreach (SpawnObj spawnObj in SpawnObjs)
            {
                if (spawnObj.name == name)
                {
                    return spawnObj;
                }
            }
            return null;
        }
    }
}


[System.Serializable]
public class SpawnObj
{
    public string name;
    public GameObject spawnObj;
    public  float createOffsetTime;
}



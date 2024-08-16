using System.Collections.Generic;
using UnityEngine;
using Color = UnityEngine.Color; 

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

        [SerializeField, Header("�X�|�[���p�I�u�W�F�N�g���i�[")] List<SpawnObj> spawnObjs;
        public List<SpawnObj> SpawnObjs => spawnObjs;

        [SerializeField, Header("�L�m�R�̍ő吔")] int maxKinokoNum;
        public int �laxKinokoNum => maxKinokoNum;

        [SerializeField, Header("�L�m�R�̐����͈͂�`�悷�邩")] bool showKinokoRange;
        public bool ShowKinokoRange => showKinokoRange;

        [SerializeField, Header("�L�m�R�̐����͈͂�`�悷����̒���")] float rangeWidth;
        public float RangeWidth => rangeWidth;

        [SerializeField, Header("�L�m�R�̐����͈͂�`�悷����̐F")] Color rangeColor;
        public Color RangeColor => rangeColor;

        [SerializeField, Header("�����S������ꏊ�P�̖��O")] string applePos1;
        public string ApplePos1 => applePos1;

        [SerializeField, Header("�����S������ꏊ2�̖��O")] string applePos2;
        public string ApplePos2 => applePos2;

        [SerializeField, Header("�����S������ꏊ3�̖��O")] string applePos3;
        public string ApplePos3 => applePos3;

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
    public GameObject objdata;
    public  float createOffsetTime;
}



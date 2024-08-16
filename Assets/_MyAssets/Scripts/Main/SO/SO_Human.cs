using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_Human", fileName = "SO_Human")]
    public class SO_Human : ScriptableObject
    {
        #region
        public const string PATH = "SO_Human";

        private static SO_Human _entity = null;
        public static SO_Human Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<SO_Human>(PATH);

                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [SerializeField, Header("���ՏI�����̃��b�Z�[�W")] private string endMessage;
        public string EndMessage => endMessage;

        [SerializeField, Header("�L�m�R���Ճ��[�g�ő吔")] private int  kinokoRateMax;
        public int  KinokoRateMax => kinokoRateMax;

        [SerializeField, Header("�L�m�R���Ճ��[�g�ŏ���")] private int kinokoRateMin;
        public int KinokoRateMin => kinokoRateMin;

        [SerializeField, Header("�����S���Ճ��[�g�ő吔")] private int appleRateMax;
        public int AppleRateMax => appleRateMax;

        [SerializeField, Header("�����S���Ճ��[�g�ŏ���")] private int appleRateMin;
        public int AppleRateMin => appleRateMin;

    }
}
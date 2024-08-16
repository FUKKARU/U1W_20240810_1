using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_HierarchyPath", fileName = "SO_HierarchyPath")]
    public class SO_HierarchyPath : ScriptableObject
    {
        #region
        public const string PATH = "SO_HierarchyPath";

        private static SO_HierarchyPath _entity = null;
        public static SO_HierarchyPath Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<SO_HierarchyPath>(PATH);

                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [SerializeField, Header("�L�����o�X����g���[�hUI��")] string tradeUI;
        public string TradeUI => tradeUI;

        [SerializeField, Header("�g���[�h����g���[�h���[�g�e�L�X�g��")] string tradeRateText;
        public string TradeRateText => tradeRateText;

        [SerializeField, Header("�g���[�h����g���[�h�{�^����")] string tradeButton;
        public string TradeButton => tradeButton;

        [SerializeField, Header("�L�����o�X����g���[�h���邩�ۂ�������UI��")] string tradeOrNot;
        public string TradeOrNot => tradeOrNot;

        [SerializeField, Header("�L�����o�X������ՊJ�n�{�^����")] string initiateTradeButon;
        public string InitiateTradeButon => initiateTradeButon;
    }
}
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

        [SerializeField, Header("スポーン用オブジェクトを格納")] List<SpawnObj> spawnObjs;
        public List<SpawnObj> SpawnObjs => spawnObjs;

        [SerializeField, Header("キノコの最大数")] int maxKinokoNum;
        public int ＭaxKinokoNum => maxKinokoNum;

        [SerializeField, Header("キノコの生成範囲を描画するか")] bool showKinokoRange;
        public bool ShowKinokoRange => showKinokoRange;

        [SerializeField, Header("キノコの生成範囲を描画する線の長さ")] float rangeWidth;
        public float RangeWidth => rangeWidth;

        [SerializeField, Header("キノコの生成範囲を描画する線の色")] Color rangeColor;
        public Color RangeColor => rangeColor;

        [SerializeField, Header("リンゴが実る場所１の名前")] string applePos1;
        public string ApplePos1 => applePos1;

        [SerializeField, Header("リンゴが実る場所2の名前")] string applePos2;
        public string ApplePos2 => applePos2;

        [SerializeField, Header("リンゴが実る場所3の名前")] string applePos3;
        public string ApplePos3 => applePos3;

        /// <summary>
        /// メンバ変数の名前からインスタンスを取得
        /// </summary>
        /// <param name="name">SpawnObjクラスのメンバ変数として登録されている名前を指定</param>
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



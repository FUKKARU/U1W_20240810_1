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

        [SerializeField, Header("スポーン時にランダムに出力するX座標の始点")] float x0;
        public float X0 => x0;
        [SerializeField, Header("スポーン時にランダムに出力するY座標の始点")] float y0;
        public float Y0 => y0;
        [SerializeField, Header("スポーン時にランダムに出力するX座標の終点")] float x1;
        public float X1 => x1;
        [SerializeField, Header("スポーン時にランダムに出力するY座標の終点")] float y1;
        public float Y1 => y1;

        [SerializeField, Header("スポーン用オブジェクトを格納")] List<SpawnObj> spawnObjs;
        public List<SpawnObj> SpawnObjs => spawnObjs;

        [SerializeField, Header("キノコの最大数")] int maxKinokoNum;
        public int ＭaxKinokoNum => maxKinokoNum;

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
    public GameObject spawnObj;
    public  float createOffsetTime;
}



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

        [SerializeField,Header("スポーン範囲")] List<Vector2> spawnRange;
        public List<Vector2> SpawnRange => spawnRange;

        [SerializeField, Header("キノコオブジェクト")] private GameObject kinokoObj;
        public GameObject KinokoObj=> kinokoObj;

        [SerializeField, Header("リンゴオブジェクト")] private GameObject appleObj;
        public GameObject AppleObj => appleObj;

        [SerializeField, Header("油あげオブジェクト")] private GameObject aburaageObj;
        public GameObject AburaageObj => aburaageObj;
    }
}



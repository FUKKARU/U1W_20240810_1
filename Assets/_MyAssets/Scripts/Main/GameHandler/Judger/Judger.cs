using UnityEngine;

namespace Main.GameHandler
{
    public sealed class Judger : MonoBehaviour
    {
        private JudgerBhv impl;

        private void OnEnable()
        {
            impl = new();
        }

        private void OnDisable()
        {
            impl.Dispose();
            impl = null;
        }

        private void Update()
        {
            impl.Update();
        }

        internal void GameOver()
        {
            impl.GameOver();
        }

        internal void GameClear()
        {
            impl.GameClear();
        }
    }

    internal sealed class JudgerBhv : System.IDisposable
    {
        private enum Result
        {
            Undone,
            Clear,
            Over,
        }

        private Result result;

        internal JudgerBhv()
        {
            result = Result.Undone;
        }

        public void Dispose()
        {

        }

        internal void Update()
        {

        }

        internal void GameOver()
        {
            if (result != Result.Undone) return;
            result = Result.Over;
        }

        internal void GameClear()
        {
            if (result != Result.Undone) return;
            result = Result.Clear;
        }
    }
}
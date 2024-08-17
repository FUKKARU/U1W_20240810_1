using UnityEngine;

namespace Main.GameHandler
{
    internal enum Result
    {
        Undone,
        Clear,
        Over,
    }

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

        internal Result GetResult()
        {
            return impl.GetResult();
        }
    }

    internal sealed class JudgerBhv : System.IDisposable
    {
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

        internal Result GetResult()
        {
            return result;
        }

        internal void GameOver()
        {
            if (result != Result.Undone) return;
            result = Result.Over;

            GameOverBhv();
        }

        internal void GameClear()
        {
            if (result != Result.Undone) return;
            result = Result.Clear;

            GameClearBhv();
        }

        private void GameOverBhv()
        {
            Debug.Log("Game Over");
        }

        private void GameClearBhv()
        {
            Debug.Log("Game Clear");
        }
    }
}
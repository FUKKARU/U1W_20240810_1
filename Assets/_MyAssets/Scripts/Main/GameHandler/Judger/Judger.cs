using SO;
using TMPro;
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
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private Counter counter;

        private JudgerBhv impl;

        private void OnEnable()
        {
            impl = new(resultText, counter);
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
        private TextMeshProUGUI resultText;
        private Counter counter;

        private Result result;

        internal JudgerBhv(TextMeshProUGUI resultText, Counter counter)
        {
            this.resultText = resultText;
            this.counter = counter;

            result = Result.Undone;
        }

        public void Dispose()
        {
            resultText = null;
            counter = null;
        }

        internal void Update()
        {
            if (!resultText) return;
            if (!counter) return;
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
            SetResultText(SO_Judge.Entity.GameOverMessage);
            WaitAndGoToTitle();
        }

        private void GameClearBhv()
        {
            SetResultText(SO_Judge.Entity.GameClearMessage);
            WaitAndGoToTitle();
        }

        private void SetResultText(string message)
        {
            resultText.text = message;
        }

        private void WaitAndGoToTitle()
        {
            Wait(() => General.Flow.SceneChange(SO_SceneName.Entity.Title, false), SO_Judge.Entity.BeforeGoToTitleDur);
        }

        private void Wait(System.Action action, float waitSeconds)
        {
            counter.Wait(action, waitSeconds);
        }
    }
}
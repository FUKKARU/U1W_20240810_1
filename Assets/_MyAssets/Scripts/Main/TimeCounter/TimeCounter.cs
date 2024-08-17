using UnityEngine;
using TMPro;
using SO;
using UnityEditor.PackageManager;

namespace Main.GameHandler
{
    public sealed class TimeCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeUIText;

        private TimeCounterBhv impl;

        private void OnEnable()
        {
            impl = new(timeUIText);
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
    }

    internal sealed class TimeCounterBhv : System.IDisposable
    {
        private TextMeshProUGUI timeText;

        private float t = 0;  // �Q�[���̎��ԃJ�E���^

        internal TimeCounterBhv(TextMeshProUGUI timeText)
        {
            this.timeText = timeText;

            t = SO_Judge.Entity.TimeLimit;
        }

        public void Dispose()
        {
            timeText = null;
        }

        internal void Update()
        {
            if (!timeText) return;

            if (t <= 0) return;

            t -= Time.deltaTime;
            if (t <= 0) GameOver();
            else timeText.text = Mathf.RoundToInt(t).Normed();
        }

        private void GameOver()
        {
            Debug.Log("Game Over!");
        }
    }

    internal static class Ex
    {
        internal static void Error(string message)
        {
            throw new System.Exception(message);
        }

        // �^����ꂽ�b�����A�^�C�}�[�̕�����ɐ��K������
        internal static string Normed(this int sec)
        {
            if (sec < 0) Error($"{nameof(sec)}�����ł�");
            if (sec >= 6000) Error($"{nameof(sec)}��100���ȏ�ł�");

            int m = 0;
            int s = 0;

            while (sec >= 60)
            {
                m++;
                sec -= 60;
            }
            s = sec;

            return $"{m:D2}:{s:D2}";
        }
    }
}
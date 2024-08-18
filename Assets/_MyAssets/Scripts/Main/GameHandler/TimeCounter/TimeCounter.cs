using UnityEngine;
using TMPro;
using SO;

namespace Main.GameHandler
{
    public sealed class TimeCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeUIText;
        [SerializeField] private Transform sun;
        [SerializeField] private Judger judger;

        private TimeCounterBhv impl;

        private void OnEnable()
        {
            impl = new(timeUIText, sun, judger);
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
        private Transform sun;
        private Judger judger;

        private float t = 0;  // �Q�[���̎��ԃJ�E���^

        internal TimeCounterBhv(TextMeshProUGUI timeText, Transform sun, Judger judger)
        {
            this.timeText = timeText;
            this.sun = sun;
            this.judger = judger;

            t = SO_Judge.Entity.TimeLimit;
        }

        public void Dispose()
        {
            timeText = null;
            sun = null;
            judger = null;
        }

        internal void Update()
        {
            if (!timeText) return;
            if (!sun) return;
            if (!judger) return;

            if (judger.GetResult() != Result.Undone) return;
            if (t <= 0) return;

            t -= Time.deltaTime;
            if (t <= 0)
            {
                judger.GameOver();
            }
            else
            {
                timeText.text = Mathf.RoundToInt(t).Normed();
                UpdateSunRotation(t);
            }
        }

        private void UpdateSunRotation(float t)
        {
            float st = SO_Judge.Entity.TimeLimit;
            float et = 0;
            float sx = SO_Judge.Entity.SunStartAngle;
            float ex = SO_Judge.Entity.SunEndAngle;

            Quaternion sunRot = sun.rotation;
            sunRot = Quaternion.Euler(Ex.Remap(st, et, sx, ex, t), 0, 0);
            sun.rotation = sunRot;
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

        // ���X[a, b]���A���Y[c, d]�ɐ��`�}�b�s���O�����Ƃ���B���X���̒lx�ɂ��āA�ϊ���̋��Y�ɂ�����ly��Ԃ��B
        internal static float Remap(float a, float b, float c, float d, float x)
        {
            float y = (x - a) * (d - c) / (b - a) + c;
            return y;
        }

        internal static System.Collections.IEnumerator Wait(System.Action action, float waitSeconds)
        {
            yield return new WaitForSeconds(waitSeconds);
            action();
        }
    }
}
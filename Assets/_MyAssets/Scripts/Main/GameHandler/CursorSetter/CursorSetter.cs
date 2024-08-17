using UnityEngine;

namespace Main.GameHandler
{
    public sealed class CursorSetter : MonoBehaviour
    {
        private bool isCursorOn = true;

        private void OnEnable()
        {
            UpdateCursor(false);
        }

        private void Update()
        {
            if (IA.InputGetter.Instance.Main_ToggleCursorClick.Get<bool>())
                ToggleCursor();
        }

        private void OnDisable()
        {
            UpdateCursor(true);
        }

        private void ToggleCursor()
        {
            UpdateCursor(!isCursorOn);
        }

        private void UpdateCursor(bool isActive)
        {
            if (isActive)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                isCursorOn = true;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                isCursorOn = false;
            }
        }
    }
}
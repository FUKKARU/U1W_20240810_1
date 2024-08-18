using UnityEngine;

namespace Main.GameHandler
{
    public sealed class CursorSetter : MonoBehaviour
    {
        [SerializeField] Player.PlayerMove playerMove;

        private bool isCursorOn = true;

        private void OnEnable()
        {
            if (!playerMove) throw new System.Exception($"{nameof(playerMove)}Ç™ê›íËÇ≥ÇÍÇƒÇ¢Ç‹ÇπÇÒ");
        }

        private void Start()
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
            playerMove = null;
        }

        private void ToggleCursor()
        {
            UpdateCursor(!isCursorOn);
        }

        private void UpdateCursor(bool isActive)
        {
            playerMove.SetLookAroundable(!isActive);

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
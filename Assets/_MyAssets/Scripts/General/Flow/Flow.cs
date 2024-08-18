namespace General
{
    public static class Flow
    {
        // ƒV[ƒ“‘JˆÚ
        public static void SceneChange(string toSceneName, bool isAsync)
        {
            // ”ñ“¯Šú‘JˆÚ
            if (isAsync)
            {
                // ‚Ü‚¾–¢À‘•
            }
            // ‘¦‘JˆÚ
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(toSceneName);
            }
        }

        // ƒQ[ƒ€I—¹
        public static void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}

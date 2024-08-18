namespace General
{
    public static class Flow
    {
        // �V�[���J��
        public static void SceneChange(string toSceneName, bool isAsync)
        {
            // �񓯊��J��
            if (isAsync)
            {
                // �܂�������
            }
            // �����J��
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(toSceneName);
            }
        }

        // �Q�[���I��
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

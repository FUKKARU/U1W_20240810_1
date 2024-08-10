using UnityEditor;

public static class Ex
{
    internal static class CustomMenuItem
    {
        private const int MENU_ITEM_PRIORITY = -10000;
        private const string MENU_ITEM_ROOT_PATH = "Assets/Create/Scripts";
        private const string MENU_ITEM_Empty_PATH = "Empty";
        private const string MENU_ITEM_MonoBehaviour_PATH = "MonoBehaviour";
        private const string MENU_ITEM_ScriptableObject_PATH = "ScriptableObject";
        private const string MENU_ITEM_GameManager_PATH = "GameManager";
        private const string MENU_ITEM_InputGetter_PATH = "InputGetter";
        private const string MENU_ITEM_GameStateSetter_PATH = "GameStateSetter";

        private const string TEMPLATE_FOLDER_PATH = "Assets/_MyAssets/Scripts/ScriptTemplates";
        private const string TEMPLATE_Empty_PATH = "EmptyScriptTemplate.txt";
        private const string TEMPLATE_MonoBehaviour_PATH = "MonoBehaviourScriptTemplate.txt";
        private const string TEMPLATE_ScriptableObject_PATH = "ScriptableObjectScriptTemplate.txt";
        private const string TEMPLATE_GameManager_PATH = "GameManagerScriptTemplate.txt";
        private const string TEMPLATE_InputGetter_PATH = "InputGetterScriptTemplate.txt";
        private const string TEMPLATE_GameStateSetter_PATH = "GameStateSetterScriptTemplate.txt";

        private const string NEW_Empty_NAME = "X.cs";
        private const string NEW_MonoBehaviour_NAME = "X.cs";
        private const string NEW_ScriptableObject_NAME = "SO_X.cs";
        private const string NEW_GameManager_NAME = "GameManager.cs";
        private const string NEW_InputGetter_NAME = "InputGetter.cs";
        private const string NEW_GameStateSetter_NAME = "GameStateSetter.cs";

        [MenuItem(MENU_ITEM_ROOT_PATH + "/" + MENU_ITEM_Empty_PATH, priority = MENU_ITEM_PRIORITY - 5)]
        private static void CreateEmpty() => CreateScript
            (
            $"{TEMPLATE_FOLDER_PATH}/{TEMPLATE_Empty_PATH}",
            NEW_Empty_NAME
            );

        [MenuItem(MENU_ITEM_ROOT_PATH + "/" + MENU_ITEM_MonoBehaviour_PATH, priority = MENU_ITEM_PRIORITY - 4)]
        private static void CreateMonoBehaviourScript() => CreateScript
            (
            $"{TEMPLATE_FOLDER_PATH}/{TEMPLATE_MonoBehaviour_PATH}",
            NEW_MonoBehaviour_NAME
            );

        [MenuItem(MENU_ITEM_ROOT_PATH + "/" + MENU_ITEM_ScriptableObject_PATH, priority = MENU_ITEM_PRIORITY - 3)]
        private static void CreateScriptableObjectScript() => CreateScript
            (
            $"{TEMPLATE_FOLDER_PATH}/{TEMPLATE_ScriptableObject_PATH}",
            NEW_ScriptableObject_NAME
            );

        [MenuItem(MENU_ITEM_ROOT_PATH + "/" + MENU_ITEM_GameManager_PATH, priority = MENU_ITEM_PRIORITY - 2)]
        private static void CreateGameManager() => CreateScript
            (
            $"{TEMPLATE_FOLDER_PATH}/{TEMPLATE_GameManager_PATH}",
            NEW_GameManager_NAME
            );

        [MenuItem(MENU_ITEM_ROOT_PATH + "/" + MENU_ITEM_InputGetter_PATH, priority = MENU_ITEM_PRIORITY - 1)]
        private static void CreateInputGetter() => CreateScript
            (
            $"{TEMPLATE_FOLDER_PATH}/{TEMPLATE_InputGetter_PATH}",
            NEW_InputGetter_NAME
            );

        [MenuItem(MENU_ITEM_ROOT_PATH + "/" + MENU_ITEM_GameStateSetter_PATH, priority = MENU_ITEM_PRIORITY - 0)]
        private static void CreateGameStateSetter() => CreateScript
            (
            $"{TEMPLATE_FOLDER_PATH}/{TEMPLATE_GameStateSetter_PATH}",
            NEW_GameStateSetter_NAME
            );

        private static void CreateScript(string templateFilePath, string newScriptName)
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templateFilePath, newScriptName);
        }
    }
}


using UnityEditor;

// https://discussions.unity.com/t/menu-items-as-checkboxes-radio-buttons/57882/5
public static class AddressableURPShaderKeywordBugfixEditorMenu
{
    const string MENU_NAME = "Bug Test/Use Bugfix";

    private static bool _enabled;

    static AddressableURPShaderKeywordBugfixEditorMenu()
    {
        _enabled = EditorPrefs.GetBool(MENU_NAME, false);

        /// Delaying until first editor tick so that the menu
        /// will be populated before setting check state, and
        /// re-apply correct action
        EditorApplication.delayCall += () =>
        {
            PerformAction(_enabled);
        };
    }

    [MenuItem(MENU_NAME)]
    public static void ToggleAction()
    {
        PerformAction(!_enabled);
    }

    public static void PerformAction(bool enabled)
    {
        /// Set checkmark on menu item
        Menu.SetChecked(MENU_NAME, enabled);
        /// Saving editor state
        EditorPrefs.SetBool(MENU_NAME, enabled);

        _enabled = enabled;

        /// Perform your logic here...
        AddressableURPShaderKeywordBugfix.UseBugfix = enabled;
    }
}
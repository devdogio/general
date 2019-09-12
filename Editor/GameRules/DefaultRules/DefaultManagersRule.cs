using UnityEngine;
using UnityEditor;
using Devdog.General.Localization;

namespace Devdog.General.Editors.GameRules
{
    public class DefaultManagersRule : ManagersRuleBase
    {
        public override void UpdateIssue()
        {
            FindManagerOfTypeOrCreateIssue<AudioManager>();
            FindManagerOfTypeOrCreateIssue<GeneralSettingsManager>();
            FindManagerOfTypeOrCreateIssue<PlayerManager>();
            FindManagerOfTypeOrCreateIssue<InputManager>();
            FindManagerOfTypeOrCreateIssue<TriggerManager>();
            FindManagerOfTypeOrCreateIssue<LocalizationManager>();
        }
    }
}
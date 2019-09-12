using UnityEngine;
using UnityEditor;

namespace Devdog.General.Editors.GameRules
{
    public abstract class ManagersRuleBase : GameRuleBase
    {
        protected void FindManagerOfTypeOrCreateIssue<T>() where T: MonoBehaviour
        {
            var inst = UnityEngine.Object.FindObjectOfType<T>();
            if (inst == null)
            {
                issues.Add(new GameRuleIssue(typeof(T).Name + " not found in scene", MessageType.Warning, new GameRuleAction("Fix (add)",
                    () =>
                    {
                        var managers = GameObject.Find("_Managers");
                        if (managers == null)
                        {
                            managers = new GameObject("_Managers");
                            managers.transform.SetAsFirstSibling();
                        }

                        managers.AddComponent<T>();
                    })));
            }
        }
    }
}
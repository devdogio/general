using System;
using System.Collections.Generic;
using UnityEditor;

namespace Devdog.General.Editors.GameRules
{
    public class GameRuleIssue
    {
        public string message;
        public MessageType messageType;
        public GameRuleAction[] actions;

        public GameRuleIssue(string message, MessageType messageType, params GameRuleAction[] actions)
        {
            this.message = message;
            this.messageType = messageType;
            this.actions = actions;
        }
    }
}

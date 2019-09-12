using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.General.Editors.GameRules
{
    public interface IGameRule
    {
        string saveName { get; }
        bool ignore { get; set; }
        List<GameRuleIssue> issues { get; }

        void UpdateIssue();
    }
}

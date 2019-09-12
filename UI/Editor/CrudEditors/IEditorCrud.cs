using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.General.Editors
{
    public interface IEditorCrud
    {
        bool requiresDatabase { get; set; }

        void Focus();
        void Draw();
    }
}

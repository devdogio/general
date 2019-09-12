using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.General.Editors.ReflectionDrawers
{
    public interface IChildrenDrawer
    {
        List<DrawerBase> children { get; set; }
    }
}

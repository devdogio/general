using System;

namespace Devdog.General
{
    public interface ILocalIdentifier : IEquatable<ILocalIdentifier>
    {
        string ID { get; }

        string ToString();
    }
}

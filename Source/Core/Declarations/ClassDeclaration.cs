using System;

namespace Core.Declarations
{
    public readonly struct ClassDeclaration
    {
    }

    [Flags]
    public enum AccessModifier
    {
        None = 0,
        Private = 1,
        Public = 2,
        Protected = 4,
        Internal = 8,
        Static = 16,
    }
}

using System;

namespace Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PostCallAttribute : AOPAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PreCallAttribute : AOPAttribute
    {
    }

    public class AOPAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Enum)]
    public class EnumExtAttribute : Attribute;
}

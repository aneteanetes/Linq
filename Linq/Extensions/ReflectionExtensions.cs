namespace Bars.NuGet.Querying
{
    using System.Reflection;

    internal static class ReflectionExtensions
    {
        internal static void SetValue(this MemberInfo memberInfo, object target, object value)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)memberInfo).SetValue(target, value);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo)memberInfo).SetValue(target, value);
                    break;
                default: break;
            }
        }
    }
}
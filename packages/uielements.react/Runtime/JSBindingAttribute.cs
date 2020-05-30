using System;

namespace UnityReactUIElements
{
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class JSBindingAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class JSTypeBindingAttribute : Attribute
    {
        public JSTypeBindingAttribute(Type type)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class JSBindingSystemAttribute : Attribute
    {
    }
}

using System;

namespace SqlMigratorWinform.Utility
{
    public static class TypeHelper
    {
        public static Type GetArrayItemType(Type type)
        {
            if (!type.IsArray)
            {
                return null;
            }
            return type.GetElementType();
        }

        public static bool IsInheritBase(Type type, Type baseType)
        {
            if (baseType == null)
            {
                return false;
            }
            if (type == null)
            {
                return false;
            }
            return ((type.BaseType == baseType) || IsInheritBase(type.BaseType, baseType));
        }

        public static Type TryGetType(string typeName, Type defaultType)
        {
            return TryGetType(typeName, defaultType, false);
        }

        public static Type TryGetType(string typeName, Type defaultType, bool ignoreCase)
        {
            Type ret = null;
            if (!string.IsNullOrEmpty(typeName))
            {
                ret = Type.GetType(typeName, false, ignoreCase);
            }
            if ((ret == null) || IsInheritBase(defaultType, ret))
            {
                return defaultType;
            }
            return ret;
        }
    }
}


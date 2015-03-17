using System.Collections.Generic;

namespace SqlMigratorWinform.Utility
{
    public static class Generic<T>
    {
        private static readonly T _Default;
        private static T[] _EmptyArray;
        private static readonly System.Guid _Guid;
        private static System.Type _Type;

        static Generic()
        {
            _Default = default(T);
            _Guid = System.Guid.NewGuid();
        }

        public static bool EqualsDefault(T value)
        {
            return EqualityComparer<T>.Default.Equals(value, Default);
        }

        public static T Default
        {
            get
            {
                return _Default;
            }
        }

        public static T[] EmptyArray
        {
            get
            {
                return _EmptyArray ?? (_EmptyArray = new T[0]);
            }
        }

        public static System.Guid Guid
        {
            get
            {
                return _Guid;
            }
        }

        public static System.Type Type
        {
            get { return _Type ?? (_Type = typeof (T)); }
        }
    }
}


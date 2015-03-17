using System;
using System.Collections;
using System.Collections.Generic;

namespace SqlMigratorWinform.Utility
{
    public class DictionaryEnumerator<K, V> : IEnumerator<KeyValuePair<K, V>>, IDictionaryEnumerator
    {
        private readonly Converter<object, K> ConvertKey;
        private readonly Converter<object, V> ConvertValue;
        private readonly IEnumerator Items;
        private readonly IEnumerator<K> Keys;
        private readonly IEnumerator<V> Values;

        public DictionaryEnumerator(IEnumerable<KeyValuePair<K, V>> items) : this((items == null) ? null : items.GetEnumerator())
        {
        }

        public DictionaryEnumerator(IEnumerator<KeyValuePair<K, V>> items) : this(items, pair => ((KeyValuePair<K, V>) pair).Key, pair => ((KeyValuePair<K, V>) pair).Value)
        {
        }

        public DictionaryEnumerator(IEnumerable<K> keys, IEnumerable<V> values) : this((keys == null) ? null : keys.GetEnumerator(), (values == null) ? null : values.GetEnumerator())
        {
        }

        public DictionaryEnumerator(IEnumerator<K> keys, IEnumerator<V> values)
        {
            ParameterChecker.CheckNull("DictionaryEnumerator", "keys", keys);
            ParameterChecker.CheckNull("DictionaryEnumerator", "values", values);
            Keys = keys;
            Values = values;
        }

        public DictionaryEnumerator(IEnumerable items, Converter<object, K> convertKey, Converter<object, V> convertValue) : this((items == null) ? null : items.GetEnumerator(), convertKey, convertValue)
        {
        }

        public DictionaryEnumerator(IEnumerator items, Converter<object, K> convertKey, Converter<object, V> convertValue)
        {
            ParameterChecker.CheckNull("DictionaryEnumerator", "items", items);
            ParameterChecker.CheckNull("DictionaryEnumerator", "convertKey", convertKey);
            ParameterChecker.CheckNull("DictionaryEnumerator", "convertValue", convertValue);
            Items = items;
            ConvertKey = convertKey;
            ConvertValue = convertValue;
        }

        public void Dispose()
        {
            if (Items == null)
            {
                Keys.Dispose();
                Values.Dispose();
            }
        }

        public bool MoveNext()
        {
            return ((Items == null) ? (Keys.MoveNext() && Values.MoveNext()) : Items.MoveNext());
        }

        public void Reset()
        {
            if (Items != null)
            {
                Items.Reset();
            }
            else
            {
                Keys.Reset();
                Values.Reset();
            }
        }

        public KeyValuePair<K, V> Current
        {
            get
            {
                return new KeyValuePair<K, V>(Key, Value);
            }
        }

        public K Key
        {
            get
            {
                return ((Items == null) ? Keys.Current : ConvertKey(Items.Current));
            }
        }

        DictionaryEntry IDictionaryEnumerator.Entry
        {
            get
            {
                return new DictionaryEntry(Key, Value);
            }
        }

        object IDictionaryEnumerator.Key
        {
            get
            {
                return Key;
            }
        }

        object IDictionaryEnumerator.Value
        {
            get
            {
                return Value;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public V Value
        {
            get
            {
                return ((Items == null) ? Values.Current : ConvertValue(Items.Current));
            }
        }
    }
}


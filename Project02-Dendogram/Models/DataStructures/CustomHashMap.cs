using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project02_Dendogram.Models.DataStructures.Interfaces;

namespace Project02_Dendogram.Models.DataStructures
{
    public class CustomHashMap<TKey, TValue> : IIterable<KeyValuePair<TKey, TValue>>
    {
        private class Entry
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
            public Entry Next { get; set; }

            public Entry(TKey key, TValue value)
            {
                Key = key;
                Value = value;
                Next = null;
            }
        }

        private Entry[] _buckets;
        private int _size;
        private int _capacity;
        private const double LOAD_FACTOR = 0.75;

        public CustomHashMap(int initialCapacity = 16)
        {
            _capacity = initialCapacity;
            _buckets = new Entry[_capacity];
            _size = 0;
        }

        public int Count => _size;

        // O(1) amortizado - insertar o actualizar
        public void Put(TKey key, TValue value)
        {
            if ((double)_size / _capacity >= LOAD_FACTOR)
                Resize();

            int index = GetBucketIndex(key);
            Entry entry = _buckets[index];

            // Buscar si ya existe
            while (entry != null)
            {
                if (entry.Key.Equals(key))
                {
                    entry.Value = value;
                    return;
                }
                entry = entry.Next;
            }

            // Agregar nuevo
            Entry newEntry = new Entry(key, value);
            newEntry.Next = _buckets[index];
            _buckets[index] = newEntry;
            _size++;
        }

        // O(1) promedio - obtener valor
        public TValue Get(TKey key)
        {
            int index = GetBucketIndex(key);
            Entry entry = _buckets[index];

            while (entry != null)
            {
                if (entry.Key.Equals(key))
                    return entry.Value;
                entry = entry.Next;
            }

            throw new KeyNotFoundException($"Key not found: {key}");
        }

        // O(1) promedio - verificar si existe
        public bool ContainsKey(TKey key)
        {
            int index = GetBucketIndex(key);
            Entry entry = _buckets[index];

            while (entry != null)
            {
                if (entry.Key.Equals(key))
                    return true;
                entry = entry.Next;
            }

            return false;
        }

        // O(1) - calcular índice del bucket
        private int GetBucketIndex(TKey key)
        {
            int hash = key.GetHashCode();
            return (hash & 0x7FFFFFFF) % _capacity;
        }

        // O(n) - redimensionar tabla
        private void Resize()
        {
            int oldCapacity = _capacity;
            Entry[] oldBuckets = _buckets;

            _capacity *= 2;
            _buckets = new Entry[_capacity];
            _size = 0;

            for (int i = 0; i < oldCapacity; i++)
            {
                Entry entry = oldBuckets[i];
                while (entry != null)
                {
                    Put(entry.Key, entry.Value);
                    entry = entry.Next;
                }
            }
        }

        // O(1) - crear iterador
        public IIterator<KeyValuePair<TKey, TValue>> CreateIterator()
        {
            return new HashMapIterator(this);
        }

        private class HashMapIterator : IIterator<KeyValuePair<TKey, TValue>>
        {
            private readonly CustomHashMap<TKey, TValue> _map;
            private int _bucketIndex;
            private Entry _current;

            public HashMapIterator(CustomHashMap<TKey, TValue> map)
            {
                _map = map;
                _bucketIndex = 0;
                _current = null;
                FindNextBucket();
            }

            public bool HasNext()
            {
                return _current != null;
            }

            public KeyValuePair<TKey, TValue> Next()
            {
                if (!HasNext())
                    throw new InvalidOperationException("No more elements");

                var result = new KeyValuePair<TKey, TValue>(_current.Key, _current.Value);
                _current = _current.Next;

                if (_current == null)
                    FindNextBucket();

                return result;
            }

            public void Reset()
            {
                _bucketIndex = 0;
                _current = null;
                FindNextBucket();
            }

            private void FindNextBucket()
            {
                while (_bucketIndex < _map._capacity)
                {
                    if (_map._buckets[_bucketIndex] != null)
                    {
                        _current = _map._buckets[_bucketIndex];
                        _bucketIndex++;
                        return;
                    }
                    _bucketIndex++;
                }
                _current = null;
            }
        }
    }

    // ============================================================================
    // KeyValuePair.cs - Par clave-valor
    // ============================================================================
    public struct KeyValuePair<TKey, TValue>
    {
        public TKey Key { get; }
        public TValue Value { get; }

        public KeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}

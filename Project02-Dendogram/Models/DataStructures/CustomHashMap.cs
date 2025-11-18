using System;
using Project02_Dendogram.Models.DataStructures.Interfaces;

namespace Project02_Dendogram.Models.DataStructures
{
    public class CustomHashMap<TKey, TValue>
    {
        // Nodo interno para la lista enlazada usada en colisiones
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

        private Entry[] buckets;
        private int size;
        private int capacity;

        private const double LOAD_FACTOR = 0.75;

        public CustomHashMap(int initialCapacity = 16)
        {
            capacity = initialCapacity;
            buckets = new Entry[capacity];
            size = 0;
        }

        public int Count => size;

        // Inserta o actualiza un valor (O(1) amortizado)
        public void Put(TKey key, TValue value)
        {
            if ((double)size / capacity >= LOAD_FACTOR)
                Resize();

            int index = GetBucketIndex(key);
            Entry current = buckets[index];

            // Si ya existe la clave, actualizamos
            while (current != null)
            {
                if (current.Key.Equals(key))
                {
                    current.Value = value;
                    return;
                }
                current = current.Next;
            }

            // Si no existe, agregamos al inicio de la lista del bucket
            Entry newEntry = new Entry(key, value)
            {
                Next = buckets[index]
            };

            buckets[index] = newEntry;
            size++;
        }

        // Obtiene el valor asociado a una clave (O(1) promedio)
        public TValue Get(TKey key)
        {
            int index = GetBucketIndex(key);
            Entry current = buckets[index];

            while (current != null)
            {
                if (current.Key.Equals(key))
                    return current.Value;

                current = current.Next;
            }

            throw new KeyNotFoundException($"La clave no existe: {key}");
        }

        // Verifica si la clave existe (O(1) promedio)
        public bool ContainsKey(TKey key)
        {
            int index = GetBucketIndex(key);
            Entry current = buckets[index];

            while (current != null)
            {
                if (current.Key.Equals(key))
                    return true;

                current = current.Next;
            }

            return false;
        }

        // Calcula el bucket usando hash positivo
        private int GetBucketIndex(TKey key)
        {
            int hash = key.GetHashCode() & 0x7FFFFFFF;
            return hash % capacity;
        }

        // Duplica el tamaño y reubica todo (O(n))
        private void Resize()
        {
            int oldCapacity = capacity;
            Entry[] oldBuckets = buckets;

            capacity *= 2;
            buckets = new Entry[capacity];
            size = 0;

            // Reinsertamos todas las entradas
            for (int i = 0; i < oldCapacity; i++)
            {
                Entry current = oldBuckets[i];
                while (current != null)
                {
                    Put(current.Key, current.Value);
                    current = current.Next;
                }
            }
        }


        // Estructura simple clave-valor
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
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project02_Dendogram.Models.DataStructures.Interfaces;
using Project02_Dendogram.Models.DataStructures.Iterators;

namespace Project02_Dendogram.Models.DataStructures
{
    public class CustomVector<T> : IIterable<T>
    {
        private T[] _vector;
        private int _size;
        private int _capacity;

        public CustomVector(int capacity = 2)
        {
            _capacity = capacity;
            _vector = new T[capacity];
            _size = 0;
        }

        public CustomVector(CustomVector<T> other) : this(other._capacity)   
        {
            for (int i = 0; i < other._size; i++)
                Add(other._vector[i]);
        }

        public int Count => _size;

        // O(1) - acceso directo
        public T GetAt(int index)
        {
            return _vector[index];
        }

        public void SetAt(int index, T value)
        {
            _vector[index] = value;
        }

        // O(1) amortizado - agregar elemento
        public void Add(T item)
        {
            if (_size == _capacity)
                Expand();

            _vector[_size++] = item;
        }

        // O(n) - redimensionar array
        private void Expand()
        {
            _capacity *= 2;
            T[] newVector = new T[_capacity];

            for (int i = 0; i < _size; i++)
                newVector[i] = _vector[i];

            _vector = newVector;
        }

        // O(1) - crear iterador
        public IIterator<T> CreateIterator()
        {
            return new VectorIterator<T>(this);
        }

        public void Append(CustomVector<T> from)
        {
            IIterator<T> it = new VectorIterator<T>(from);
            while (it.HasNext())
            {
                T current = it.Next();
                Add(current);
            }
        }


    } 
}

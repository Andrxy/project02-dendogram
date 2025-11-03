using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project02_Dendogram.Models.DataStructures.Interfaces;

namespace Project02_Dendogram.Models.DataStructures.Iterators
{
    internal class VectorIterator<T> : IIterator<T>
    {
        private readonly CustomVector<T> _vector;
        private int _currentIndex;

        public VectorIterator(CustomVector<T> vector)
        {
            _vector = vector;
            _currentIndex = 0;
        }

        public bool HasNext()
        {
            return _currentIndex < _vector.Count;
        }

        public T Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No hay más elementos en la estructura.");
            return _vector.GetAt(_currentIndex++);
        }

        public void Reset() => _currentIndex = 0;

        public T[] ToArray()
        {
            T[] result = new T[_vector.Count];
            for (int i = 0; i < _vector.Count; i++)
                result[i] = _vector.GetAt(i);
            return result;
        }
    }
}
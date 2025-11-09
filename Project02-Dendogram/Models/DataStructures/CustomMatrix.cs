using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Project02_Dendogram.Models.DataStructures.Interfaces;
using Project02_Dendogram.Models.DataStructures.Iterators;

namespace Project02_Dendogram.Models.DataStructures
{
    internal class CustomMatrix<T>
    {
        private T[,] _matrix;
        private int _size;

        public CustomMatrix(int size)
        {
            _size = size;
            _matrix = new T[_size, _size];
        }

        public int Size => _size;

        // O(1) - acceso directo
        public T GetAt(int row, int column)
        {
            return _matrix[row, column];
        }
        public void SetAt(int row, int column, T value)
        {
            _matrix[row, column] = value;
        }
    }
}

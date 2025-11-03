using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Project02_Dendogram.Models.DataStructures.Interfaces;

namespace Project02_Dendogram.Models.DataStructures.Iterators
{
    internal class ListIterator<T> : IIterator<T>
    {
        private readonly CustomList<T> _list;
        private CustomList<T>.Node<T> _current;

        public ListIterator(CustomList<T> list)
        {
            _list = list;
            _current = list.Head;
        }

        public bool HasNext() => _current != null;

        public T Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No hay más elementos en la estructura");

            T data = _current.Data;
            _current = _current.Next;

            return data;    
        }

        public void Reset() => _current = _list.Head;
    }
}

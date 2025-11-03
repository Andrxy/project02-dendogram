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
    internal class CustomList<T> : IIterable<T>
    {
        private Node<T> _head;
        private Node<T> _tail;
        private int _size;

        public CustomList()
        {
            _head = null;
            _tail = null;
            _size = 0;
        }

        public int Count => _size;
        public Node<T> Head => _head;

        // O(1) - agregar al final
        public void Add(T item)
        {
            Node<T> newNode = new Node<T>(item);
            if (_head == null)
            {
                _head = newNode;
                _tail = newNode;
            }
            else
            {
                _tail.Next = newNode;
                _tail = newNode;
            }
            _size++;
        }

        // O(n) - acceso por índice
        public T GetAt(int index)
        {
            if (index < 0 || index >= _size)
                throw new IndexOutOfRangeException($"Índice {index} fuera de rango (_size={_size})");

            Node<T> current = _head;
            for (int i = 0; i < index; i++)
                current = current.Next;

            return current.Data;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _size)
                throw new IndexOutOfRangeException($"Índice {index} fuera de rango (_size={_size})");

            if (index == 0)
            {
                Node<T> temp = _head;
                _head = _head.Next;
                temp.Next = null;
                if (_head == null) _tail = null; // actualizar tail si quedó vacío
            }
            else
            {
                Node<T> prev = _head;
                for (int i = 0; i < index - 1; i++)
                    prev = prev.Next;

                Node<T> toRemove = prev.Next;
                prev.Next = toRemove.Next;
                toRemove.Next = null;

                if (prev.Next == null) _tail = prev; // actualizar tail si eliminamos el último
            }

            _size--;
        }



        public IIterator<T> CreateIterator()
        {
            return new ListIterator<T>(this);
        }

        public class Node<T>
        {
            public T Data { get; set; }
            public Node<T> Next { get; set; }
            public Node(T data)
            {
                Data = data;
                Next = null;
            }
        }
    }
}
using System;

namespace FundamentalStructures
{
    public class LRBTree<T> where T:IComparable
    {
        protected class ListNode
        {
            public ListNode prev, next;
            public T value { get; }

            public ListNode(T value)
            {
                this.value = value;
                prev = next = null;
            }
        }

        protected class BiList
        {
            private ListNode _head;

            private ListNode InsertBetween(ListNode insert, ListNode left, ListNode right)
            {
                left.next = right.prev = insert;
                insert.prev = left;
                insert.next = right;
                return insert;
            }

            public BiList() => _head = null;

            public void AddLast(T value)
            {
                var insertListNode = new ListNode(value);

                if (IsEmpty)
                    _head = InsertBetween(insertListNode, insertListNode, insertListNode);
                else 
                    InsertBetween(insertListNode, _head.prev, _head);
            }

            public void RemoveFirst()
            {
                if (IsEmpty) return;

                if (_head == _head.next) _head = null;
                else
                {
                    _head.next.prev = _head.prev;
                    _head.prev.next = _head.next;
                    _head = _head.next;
                }
            }

            /// <returns>Значение первого узла списка</returns>
            public T headValue => _head.value;

            public bool IsEmpty => _head == null;

            public void Clear() => _head = null;
        }
        
        private const bool BLACK = false;
        private const bool RED = true;

        protected class RBNode //Узел дерева
        {
            public RBNode(T val)
            {
                list = new BiList();
                list.AddLast(val);
                left = right = null;
                Color = RED; //По умолчанию цвет нового узла - красный
            }
            public T PeekObj() { return list.headValue; }

            public void Add(T val) => list.AddLast(val);

            public void Remove() => list.RemoveFirst();

            public bool Color;
            public RBNode left;
            public RBNode right;
            public BiList list;
        }

        private RBNode _root; //Корень дерева

        private bool _isRed(RBNode node) //Красный ли узел
        {
            if (node == null) return BLACK;
            return node.Color;
        }

        private RBNode _rotateLeft(RBNode node)
        {
            RBNode temp = node.right;
            node.right = temp.left;
            temp.left = node;
            temp.Color = node.Color;
            node.Color = RED;
            return temp;
        }

        private RBNode _rotateRight(RBNode node)
        {
            RBNode temp = node.left;
            node.left = temp.right;
            temp.right = node;
            temp.Color = node.Color;
            node.Color = RED;
            return temp;
        }

        private void _flipColors(RBNode node)
        {
            node.Color = !node.Color;
            node.left.Color = !node.left.Color;
            node.right.Color = !node.right.Color;
        }

        private RBNode _moveRedLeft(RBNode node)
        {
            _flipColors(node);
            if (_isRed(node.right.left))
            {
                node.right = _rotateRight(node.right);
                node = _rotateLeft(node);
                _flipColors(node);
            }

            return node;
        }
        
        private RBNode _moveRedRight(RBNode node)
        {
            _flipColors(node);
            if (_isRed(node.left.left))
            {
                node = _rotateRight(node);
                _flipColors(node);
            }

            return node;
        }
        
        private RBNode _balance(RBNode node)
        {
            if (_isRed(node.right) && !_isRed(node.left)) node = _rotateLeft(node);
            if (_isRed(node.left) && _isRed(node.left.left)) node = _rotateRight(node);
            if (_isRed(node.left) && _isRed(node.right)) _flipColors(node);
            
            return node;
        }

        private RBNode _add(RBNode node, T val)
        {
            if (node == null) return new RBNode(val);

            int res = node.PeekObj().CompareTo(val);
            if (res == 0) { node.Add(val); return node; }
            
            if (res > 0)
                node.left = _add(node.left, val);
            else
                node.right = _add(node.right, val);
            
            return _balance(node); //Балансировка дерева при вставке
        }

        //Вспомогательная, удаление минимального в дереве
        private RBNode _deleteMin(RBNode node)
        {
            if (node.left == null) return null;

            if (!_isRed(node.left) && !_isRed(node.left.left)) node = _moveRedLeft(node);
            node.left = _deleteMin(node.left);
            return _balance(node);
        }

        private RBNode _delete(RBNode node, T val)
        {
            int res = node.PeekObj().CompareTo(val);

            if (res > 0)
            {
                if (!_isRed(node.left) && !_isRed(node.left.left)) node = _moveRedLeft(node);
                node.left = _delete(node.left, val);
            }
            else
            {
                if (res == 0)
                {
                    node.Remove();
                    if (!node.list.IsEmpty) return node;
                    node.Add(val); //Возвращаем значение обратно.
                }

                if (_isRed(node.left)) node = _rotateRight(node);

                //Этот элемент единственный узел дерева
                res = node.PeekObj().CompareTo(val);
                if (res == 0 && node.right == null)
                    return null;

                if (!_isRed(node.right) && !_isRed(node.right.left))
                    node = _moveRedRight(node);
                
                res = node.PeekObj().CompareTo(val);
                if (res == 0)
                {
                    node.list = _findMin(node.right).list;
                    node.right = _deleteMin(node.right);
                }
                else node.right = _delete(node.right, val);
            }

            return _balance(node);
        }
        
        private RBNode _findMin(RBNode node) //Вспомогательная. Поиск узла с минимальным значением в дереве.
        {
            while (node.left != null)
                node = node.left;
            return node;
        }

        public LRBTree() => _root = null;

        private void _printSymLeftRight(RBNode node)
        {
            if (node == null) return;
            _printSymLeftRight(node.left);
            Console.Write(node.list + " ");
            _printSymLeftRight(node.right);
        }
        
        private void _printSymRightLeft(RBNode node)
        {
            if (node == null) return;
            _printSymRightLeft(node.right);
            Console.Write(node.list + " ");
            _printSymRightLeft(node.left);
        }
        
        /// <summary> Обход дерева в симметричном порядке по возрастанию </summary>
        public void PrintSymLeftRight() => _printSymLeftRight(_root);

        /// <summary> Обход дерева в симметричном порядке по убыванию </summary>
        public void PrintSymRightLeft() => _printSymRightLeft(_root);

        private void _printFwd(RBNode node)
        {
            if (node == null) return;
            Console.Write(node.list + " ");
            _printFwd(node.left);
            _printFwd(node.right);
        }

        /// <summary> Обход дерева в прямом порядке </summary>
        public void PrintFwd() => _printFwd(_root);
        
        private void _printRvrs(RBNode node)
        {
            if (node == null) return;
            _printRvrs(node.left);
            _printRvrs(node.right);
            Console.Write(node.list + " ");
        }

        /// <summary> Обход дерева в обратном порядке </summary>
        public void PrintRvrs() => _printRvrs(_root);
        
        public void Add(T val)
        {
            _root = _add(_root, val);
            if (_isRed(_root)) _root.Color = BLACK;
        }

        public void Remove(T val)
        {
            //Если данного значения нет в дереве - выходим
            if (!Contains(val)) return; 
            
            if (!_isRed(_root.left) && !_isRed(_root.right)) _root.Color = RED;
            _root = _delete(_root, val);
            if (_root != null) _root.Color = BLACK;
        }

        /// <summary> Поиск минимального значения в дереве </summary>
        public bool FindMin(out T minValue)
        {
            if (_root == null)
            {
                minValue = default;
                return false;
            }

            minValue = _findMin(_root).list.headValue;
            
            return true;
        }

        /// <summary> Содержит ли дерево указанное значение </summary>
        public bool Contains(T value)
        {
            var node = _root;
            while (node != null)
            {
                int res = node.PeekObj().CompareTo(value);
                if (res == 0) return true;
                node = res > 0 ? node.left : node.right;
            }

            return false;
        }

        /// <summary> Очистка дерева </summary>
        public void Clear()
        {
            _root = null;
            GC.Collect();
        }
    }
}
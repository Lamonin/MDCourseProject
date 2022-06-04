using System;

namespace FundamentalStructures
{
    public class LRBTree<TKey, TValue> where TKey:IComparable where TValue:IComparable
    {
        
        protected class ListNode
        {
            public ListNode prev, next;
            public TValue value { get; }

            public ListNode(TValue value)
            {
                this.value = value;
                prev = next = null;
            }

            public bool EqualsValue(TValue val)
            {
                return value.Equals(val);
            }
        }

        protected class BiList
        {
            private ListNode _head;
            
            private static ListNode InsertBetween(ListNode insert, ListNode left, ListNode right)
            {
                left.next = right.prev = insert;
                insert.prev = left;
                insert.next = right;
                return insert;
            }

            public BiList() { _head = null; }

            public void AddLast(TValue val)
            {
                var insertListNode = new ListNode(val);

                if (IsEmpty)
                {
                    _head = InsertBetween(insertListNode, insertListNode, insertListNode);
                }
                else
                {
                    var temp = _head;
                    do
                    {
                        if (temp.EqualsValue(val))
                        {
                            //throw new Exception($"Value {val} exist!");
                            return;
                        }
                        temp = temp.next;
                    } while (temp != _head);

                    InsertBetween(insertListNode, _head.prev, _head);
                }
            }

            public void Remove(TValue val)
            {
                if (IsEmpty) return;

                if (_head == _head.next)
                {
                    if (_head.EqualsValue(val)) _head = null;
                }
                else if (_head.EqualsValue(val))
                {
                    _head.next.prev = _head.prev;
                    _head.prev.next = _head.next;
                    _head = _head.next;
                }
                else
                {
                    var temp = _head.next;
                    do
                    {
                        if (temp.EqualsValue(val))
                        {
                            temp.next.prev = temp.prev;
                            temp.prev.next = temp.next;
                            break;
                        }
                        
                        temp = temp.next;
                    } while (temp != _head);
                }
            }

            public bool Find(TValue val)
            {
                if (IsEmpty) return false;
                
                var temp = _head;
                while (temp.next != _head)
                    if (temp.EqualsValue(val))
                        return true;

                return false;
            }

            public void Clear() => _head = null;
            public bool IsEmpty => _head == null;
        }
        
        private const bool BLACK = false;
        private const bool RED = true;

        protected class RBNode //Узел дерева
        {
            public RBNode left;
            public RBNode right;
            
            public BiList list;

            public RBNode(TKey key)
            {
                this.key = key;
                list = new BiList();
                left = right = null;
            }
            
            public void Add(TValue val) => list.AddLast(val);
            public void Remove(TValue val) => list.Remove(val);

            public bool Color = RED; //По умолчанию цвет нового узла - красный

            public TKey key;
        }

        private RBNode _root; //Корень дерева

        private static bool _isRed(RBNode node) //Красный ли узел
        {
            if (node == null) return BLACK;
            return node.Color;
        }

        private static RBNode _rotateLeft(RBNode node)
        {
            RBNode temp = node.right;
            node.right = temp.left;
            temp.left = node;
            temp.Color = node.Color;
            node.Color = RED;
            return temp;
        }

        private static RBNode _rotateRight(RBNode node)
        {
            RBNode temp = node.left;
            node.left = temp.right;
            temp.right = node;
            temp.Color = node.Color;
            node.Color = RED;
            return temp;
        }

        private static void _flipColors(RBNode node)
        {
            node.Color = !node.Color;
            node.left.Color = !node.left.Color;
            node.right.Color = !node.right.Color;
        }

        private static RBNode _moveRedLeft(RBNode node)
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
        
        private static RBNode _moveRedRight(RBNode node)
        {
            _flipColors(node);
            if (_isRed(node.left.left))
            {
                node = _rotateRight(node);
                _flipColors(node);
            }

            return node;
        }
        
        private static RBNode _balance(RBNode node)
        {
            if (_isRed(node.right) && !_isRed(node.left)) node = _rotateLeft(node);
            if (_isRed(node.left) && _isRed(node.left.left)) node = _rotateRight(node);
            if (_isRed(node.left) && _isRed(node.right)) _flipColors(node);
            
            return node;
        }

        private static RBNode _add(RBNode node, TKey key)
        {
            if (node == null) return new RBNode(key);

            int res = node.key.CompareTo(key);

            if (res > 0)
                node.left = _add(node.left, key);
            else
                node.right = _add(node.right, key);
            
            return _balance(node); //Балансировка дерева при вставке
        }

        //Вспомогательная, удаление минимального в дереве
        private static RBNode _deleteMin(RBNode node)
        {
            if (node.left == null) return null;

            if (!_isRed(node.left) && !_isRed(node.left.left)) node = _moveRedLeft(node);
            node.left = _deleteMin(node.left);
            return _balance(node);
        }

        private static RBNode _delete(RBNode node, TKey key)
        {
            int res = node.key.CompareTo(key);

            if (res > 0)
            {
                if (!_isRed(node.left) && !_isRed(node.left.left)) node = _moveRedLeft(node);
                node.left = _delete(node.left, key);
            }
            else
            {
                if (_isRed(node.left)) node = _rotateRight(node);

                //Этот элемент единственный узел дерева
                res = node.key.CompareTo(key);
                if (res == 0 && node.right == null)
                    return null;

                if (!_isRed(node.right) && !_isRed(node.right.left))
                    node = _moveRedRight(node);
                
                res = node.key.CompareTo(key);
                if (res == 0)
                {
                    var minNode = _findMin(node.right);
                    node.list = minNode.list;
                    node.key = minNode.key;
                    
                    node.right = _deleteMin(node.right);
                }
                else node.right = _delete(node.right, key);
            }

            return _balance(node);
        }
        
        private static RBNode _findMin(RBNode node) //Вспомогательная. Поиск узла с минимальным значением в дереве.
        {
            while (node.left != null)
                node = node.left;
            return node;
        }

        public LRBTree() => _root = null;

        private static void _printSymLeftRight(RBNode node)
        {
            if (node == null) return;
            _printSymLeftRight(node.left);
            Console.Write(node.list + " ");
            _printSymLeftRight(node.right);
        }
        
        private static void _printSymRightLeft(RBNode node)
        {
            if (node == null) return;
            _printSymRightLeft(node.right);
            Console.Write(node.list + " ");
            _printSymRightLeft(node.left);
        }
        
        private static void _printFwd(RBNode node)
        {
            if (node == null) return;
            Console.Write(node.list + " ");
            _printFwd(node.left);
            _printFwd(node.right);
        }
        
        private static void _printRvrs(RBNode node)
        {
            if (node == null) return;
            _printRvrs(node.left);
            _printRvrs(node.right);
            Console.Write(node.list + " ");
        }
        
        /// <summary> Обход дерева в симметричном порядке по возрастанию </summary>
        public void PrintSymLeftRight() => _printSymLeftRight(_root);

        /// <summary> Обход дерева в симметричном порядке по убыванию </summary>
        public void PrintSymRightLeft() => _printSymRightLeft(_root);

        /// <summary> Обход дерева в прямом порядке </summary>
        public void PrintFwd() => _printFwd(_root);

        /// <summary> Обход дерева в обратном порядке </summary>
        public void PrintRvrs() => _printRvrs(_root);

        private RBNode _findNodeByKey(TKey key)
        {
            var node = _root;
            while (node != null)
            {
                int res = node.key.CompareTo(key);
                if (res == 0) break;
                node = res > 0 ? node.left : node.right;
            }

            return node;
        }
        
        /// <summary> Добавляет в дерево значение по указанному ключу </summary>
        public void Add(TKey key, TValue val)
        {
            var node = _findNodeByKey(key);
            if (node != null) //Key already exist
            {
                node.Add(val); //Just add val
            }
            else
            {
                _root = _add(_root, key);
                _root.Add(val); //Add value to list
                if (_isRed(_root)) _root.Color = BLACK;
            }
        }

        /// <summary> Удаляет из дерева указанный ключ </summary>
        public void Remove(TKey key)
        {
            //Если данного значения нет в дереве - выходим
            if (!Contains(key)) return; 
            
            if (!_isRed(_root.left) && !_isRed(_root.right)) _root.Color = RED;
            _root = _delete(_root, key);
            if (_root != null) _root.Color = BLACK;
        }

        /// <summary> Удаляет из дерева значение по указанному ключу </summary>
        public void Remove(TKey key, TValue val)
        {
            var node = _findNodeByKey(key);
            if (node != null)
            {
                node.Remove(val);
                if (node.list.IsEmpty)
                {
                    Remove(key);
                }
            }
        }

        /// <summary> Содержит ли дерево указанный ключ </summary>
        public bool Contains(TKey key)
        {
            return _findNodeByKey(key) != null;
        }
        
        /// <summary> Содержит ли дерево значение по указанному ключу </summary>
        public bool Contains(TKey key, TValue val)
        {
            var node = _findNodeByKey(key);
            if (node == null) return false;

            return node.list.Find(val);
        }

        /// <summary> Очистка дерева </summary>
        public void Clear()
        {
            _root = null;
        }
        
        private void _treeBlackHeight(RBNode node, int h)
        {
            if (node != null && !_isRed(node)) h++;
            if (node == null)
            {
                Console.Write(h + " "); return;
            }
            _treeBlackHeight(node.left, h);
            _treeBlackHeight(node.right, h);
        }

        public void TreeBlackHeight() //Считает черную высоту каждого поддерева
        {
            _treeBlackHeight(_root, 0);
            Console.WriteLine();
        }
    }
}
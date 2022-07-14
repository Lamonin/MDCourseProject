using System;

namespace FundamentalStructures
{
    public class LRBTree<TKey, TValue> where TKey:IComparable<TKey> where TValue:IComparable<TValue>
    {
        private const bool BLACK = false;
        private const bool RED = true;

        protected class LRBNode //Узел дерева
        {
            public LRBNode(TKey key, TValue val)
            {
                Key = key;
                List = new DoubleCircularLinkedList<TValue>(){val};
                Left = Right = null;
            }
            
            public void Add(TValue val) => List.Add(val);
            public void Remove(TValue val) => List.Remove(val);
            
            public LRBNode Left;
            public LRBNode Right;
            public TKey Key;
            public DoubleCircularLinkedList<TValue> List;
            public bool Color = RED; //По умолчанию цвет нового узла - красный
        }

        private LRBNode _root; //Корень дерева

        private static bool _isRed(LRBNode node) //Красный ли узел
        {
            if (node == null) return BLACK;
            return node.Color;
        }

        private static LRBNode _rotateLeft(LRBNode node)
        {
            LRBNode temp = node.Right;
            node.Right = temp.Left;
            temp.Left = node;
            temp.Color = node.Color;
            node.Color = RED;
            return temp;
        }

        private static LRBNode _rotateRight(LRBNode node)
        {
            LRBNode temp = node.Left;
            node.Left = temp.Right;
            temp.Right = node;
            temp.Color = node.Color;
            node.Color = RED;
            return temp;
        }

        private static void _flipColors(LRBNode node)
        {
            node.Color = !node.Color;
            node.Left.Color = !node.Left.Color;
            node.Right.Color = !node.Right.Color;
        }

        private static LRBNode _moveRedLeft(LRBNode node)
        {
            _flipColors(node);
            if (_isRed(node.Right.Left))
            {
                node.Right = _rotateRight(node.Right);
                node = _rotateLeft(node);
                _flipColors(node);
            }

            return node;
        }
        
        private static LRBNode _moveRedRight(LRBNode node)
        {
            _flipColors(node);
            if (_isRed(node.Left.Left))
            {
                node = _rotateRight(node);
                _flipColors(node);
            }

            return node;
        }
        
        private static LRBNode _balance(LRBNode node)
        {
            if (_isRed(node.Right) && !_isRed(node.Left)) node = _rotateLeft(node);
            if (_isRed(node.Left) && _isRed(node.Left.Left)) node = _rotateRight(node);
            if (_isRed(node.Left) && _isRed(node.Right)) _flipColors(node);
            
            return node;
        }

        private static LRBNode _add(LRBNode node, TKey key, TValue val)
        {
            if (node == null) return new LRBNode(key, val);

            int res = node.Key.CompareTo(key);

            if (res > 0)
                node.Left = _add(node.Left, key, val);
            else
                node.Right = _add(node.Right, key, val);
            
            return _balance(node); //Балансировка дерева при вставке
        }

        //Вспомогательная, удаление минимального в дереве
        private static LRBNode _deleteMin(LRBNode node)
        {
            if (node.Left == null) return null;

            if (!_isRed(node.Left) && !_isRed(node.Left.Left)) node = _moveRedLeft(node);
            node.Left = _deleteMin(node.Left);
            return _balance(node);
        }

        private static LRBNode _delete(LRBNode node, TKey key)
        {
            int res = node.Key.CompareTo(key);

            if (res > 0)
            {
                if (!_isRed(node.Left) && !_isRed(node.Left.Left)) node = _moveRedLeft(node);
                node.Left = _delete(node.Left, key);
            }
            else
            {
                if (_isRed(node.Left)) node = _rotateRight(node);

                //Этот элемент единственный узел дерева
                res = node.Key.CompareTo(key);
                if (res == 0 && node.Right == null)
                    return null;

                if (!_isRed(node.Right) && !_isRed(node.Right.Left))
                    node = _moveRedRight(node);
                
                res = node.Key.CompareTo(key);
                if (res == 0)
                {
                    var minNode = _findMin(node.Right);
                    node.List = minNode.List;
                    node.Key = minNode.Key;
                    
                    node.Right = _deleteMin(node.Right);
                }
                else node.Right = _delete(node.Right, key);
            }

            return _balance(node);
        }
        
        private static LRBNode _findMin(LRBNode node) //Вспомогательная. Поиск узла с минимальным значением в дереве.
        {
            while (node.Left != null)
                node = node.Left;
            return node;
        }
        
        private void print_Tree(LRBNode p, int level, ref string output)
        {
            if (p == null) return;
            print_Tree(p.Right,level + 1, ref output);
            for(int i = 0; i < level; i++) output += "      ";
            output += p.Key + (p.Color ? "-К\n":"-Ч\n");
            print_Tree(p.Left,level + 1, ref output);
        }

        public LRBTree() => _root = null;

        /*
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
        */

        private LRBNode _findNodeByKey(TKey key)
        {
            var node = _root;
            while (node != null)
            {
                int res = node.Key.CompareTo(key);
                if (res == 0) break;
                node = res > 0 ? node.Left : node.Right;
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
                _root = _add(_root, key, val);
                if (_isRed(_root)) _root.Color = BLACK;
            }
        }

        /// <summary> Удаляет из дерева указанный ключ </summary>
        public void Remove(TKey key)
        {
            //Если данного значения нет в дереве - выходим
            if (!Contains(key)) return; 
            
            if (!_isRed(_root.Left) && !_isRed(_root.Right)) _root.Color = RED;
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
                if (node.List.Count()==0)
                {
                    Remove(key);
                }
            }
        }

        public bool TryGetValuesList(TKey key, out DoubleCircularLinkedList<TValue> list)
        {
            return TryGetValuesList(key, out list, out _);
        }

        public bool TryGetValuesList(TKey key, out DoubleCircularLinkedList<TValue> list, out int stepsToFind)
        {
            stepsToFind = 0;
            list = default;

            var node = _root;
            while (node != null)
            {
                stepsToFind += 1;
                
                int res = node.Key.CompareTo(key);
                if (res == 0) break;
                node = res > 0 ? node.Left : node.Right;
            }

            if (node is null) return false;
            
            list = node.List;
            return true;
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

            return node.List.Find(val);
        }

        /// <summary> Очистка дерева </summary>
        public void Clear()
        {
            _root = null;
        }

        public string PrintTree()
        {
            string output = String.Empty;
            if (_root != null) print_Tree(_root, 0, ref output);
            return output;
        }
    }
}
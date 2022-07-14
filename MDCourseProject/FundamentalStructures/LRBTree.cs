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

        private static bool IsRed(LRBNode node) //Красный ли узел
        {
            if (node == null) return BLACK;
            return node.Color;
        }

        private static LRBNode RotateLeft(LRBNode node)
        {
            LRBNode temp = node.Right;
            node.Right = temp.Left;
            temp.Left = node;
            temp.Color = node.Color;
            node.Color = RED;
            return temp;
        }

        private static LRBNode RotateRight(LRBNode node)
        {
            LRBNode temp = node.Left;
            node.Left = temp.Right;
            temp.Right = node;
            temp.Color = node.Color;
            node.Color = RED;
            return temp;
        }

        private static void FlipColors(LRBNode node)
        {
            node.Color = !node.Color;
            node.Left.Color = !node.Left.Color;
            node.Right.Color = !node.Right.Color;
        }

        private static LRBNode MoveRedLeft(LRBNode node)
        {
            FlipColors(node);
            if (IsRed(node.Right.Left))
            {
                node.Right = RotateRight(node.Right);
                node = RotateLeft(node);
                FlipColors(node);
            }

            return node;
        }
        
        private static LRBNode MoveRedRight(LRBNode node)
        {
            FlipColors(node);
            if (IsRed(node.Left.Left))
            {
                node = RotateRight(node);
                FlipColors(node);
            }

            return node;
        }
        
        private static LRBNode Balance(LRBNode node)
        {
            if (IsRed(node.Right) && !IsRed(node.Left)) node = RotateLeft(node);
            if (IsRed(node.Left) && IsRed(node.Left.Left)) node = RotateRight(node);
            if (IsRed(node.Left) && IsRed(node.Right)) FlipColors(node);
            
            return node;
        }

        private static LRBNode AddHelper(LRBNode node, TKey key, TValue val)
        {
            if (node == null) return new LRBNode(key, val);

            int res = node.Key.CompareTo(key);
            if (res == 0)
            {
                node.Add(val);
                return node;
            }
            if (res > 0)
                node.Left = AddHelper(node.Left, key, val);
            else
                node.Right = AddHelper(node.Right, key, val);
            
            return Balance(node); //Балансировка дерева при вставке
        }

        //Вспомогательная, удаление минимального в дереве
        private static LRBNode DeleteMin(LRBNode node)
        {
            if (node.Left == null) return null;

            if (!IsRed(node.Left) && !IsRed(node.Left.Left)) node = MoveRedLeft(node);
            node.Left = DeleteMin(node.Left);
            return Balance(node);
        }

        private static LRBNode DeleteHelper(LRBNode node, TKey key)
        {
            int res = node.Key.CompareTo(key);

            if (res > 0)
            {
                if (!IsRed(node.Left) && !IsRed(node.Left.Left)) node = MoveRedLeft(node);
                node.Left = DeleteHelper(node.Left, key);
            }
            else
            {
                if (IsRed(node.Left)) node = RotateRight(node);

                //Этот элемент единственный узел дерева
                res = node.Key.CompareTo(key);
                if (res == 0 && node.Right == null)
                    return null;

                if (!IsRed(node.Right) && !IsRed(node.Right.Left))
                    node = MoveRedRight(node);
                
                res = node.Key.CompareTo(key);
                if (res == 0)
                {
                    var minNode = FindMin(node.Right);
                    node.List = minNode.List;
                    node.Key = minNode.Key;
                    
                    node.Right = DeleteMin(node.Right);
                }
                else node.Right = DeleteHelper(node.Right, key);
            }

            return Balance(node);
        }
        
        private static LRBNode FindMin(LRBNode node) //Вспомогательная. Поиск узла с минимальным значением в дереве.
        {
            while (node.Left != null)
                node = node.Left;
            return node;
        }
        
        private void PrintTreeHelper(LRBNode p, int level, ref string output)
        {
            if (p == null) return;
            PrintTreeHelper(p.Right,level + 1, ref output);
            for(int i = 0; i < level; i++) output += "      ";
            output += p.Key + (p.Color ? "-К\n":"-Ч\n");
            PrintTreeHelper(p.Left,level + 1, ref output);
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

        private LRBNode FindNodeByKey(TKey key)
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
            _root = AddHelper(_root, key, val);
            if (IsRed(_root)) _root.Color = BLACK;
        }

        /// <summary> Удаляет из дерева указанный ключ </summary>
        public void Remove(TKey key)
        {
            //Если данного значения нет в дереве - выходим
            if (!Contains(key)) return; 
            
            if (!IsRed(_root.Left) && !IsRed(_root.Right)) _root.Color = RED;
            _root = DeleteHelper(_root, key);
            if (_root != null) _root.Color = BLACK;
        }

        /// <summary> Удаляет из дерева значение по указанному ключу </summary>
        public void Remove(TKey key, TValue val)
        {
            var node = FindNodeByKey(key);
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
            return FindNodeByKey(key) != null;
        }
        
        /// <summary> Содержит ли дерево значение по указанному ключу </summary>
        public bool Contains(TKey key, TValue val)
        {
            var node = FindNodeByKey(key);
            if (node == null) return false;

            return node.List.Find(val);
        }

        /// <summary> Очистка дерева </summary>
        public void Clear() => _root = null;

        public string PrintTree()
        {
            string output = String.Empty;
            if (_root != null) PrintTreeHelper(_root, 0, ref output);
            return output;
        }
    }
}
using System;


namespace FundamentalStructures
{
    public class RRBTree<TKey, TValue> where TKey : IComparable where TValue : IComparable
    {
        /// <summary>
        /// Узел двухсвязного кольцевого списка
        /// </summary>
        private class ListNode
    {
        public ListNode prev;
        public ListNode next;
        private TValue _value;

        public ListNode()
        {
            (prev, next) = (null, null);
        }
        public ListNode(TValue value)
        {
            (prev, next) = (this, this);
            _value = value;
        }

        public TValue GetValue()
        {
            return _value;
        }
    } 
    
        /// <summary>
        /// Двухсвязный список
        /// </summary>
        private class List
    {
        private ListNode _head;

        private static bool IsEmpty(ListNode node)
        {
            return node == null;
        }

        public List() => _head = null;

        public void Add(TValue value)
        {
            var elem = new ListNode(value);
            if (IsEmpty(_head)) _head = elem;
            else
            {
                (elem.prev, elem.next) = (_head.prev, _head);
                (_head.prev, _head.prev.next) = (elem, elem);
            }
        }

        public ListNode Find(TValue key)
        {
            if (IsEmpty(_head)) throw new Exception("List is empty");
            _head.prev.next = null;
            var curr = _head;
            while (!IsEmpty(curr))
            {
                if (curr.GetValue().CompareTo(key) == 0)
                {
                    _head.prev.next = _head;
                    return curr;
                }
                curr = curr.next;
            }
            return null;
        }

        public void Remove(TValue elem)
        {
            if (IsEmpty(_head)) throw new Exception("List is empty");
            var dElem = Find(elem);
            if(IsEmpty(dElem)) throw new Exception("Element don't exist");
            dElem.prev.next = dElem.next;
            dElem.next.prev = dElem.prev;
            dElem = null;
        }
    }

        /// <summary>
        /// Черный узел
        /// </summary>
        private static bool BLACK = false;
    
        /// <summary>
        /// Красный узел
        /// </summary>
        private static bool RED = true;

        /// <summary>
        /// Узел дерева
        /// </summary>
        private class TreeNode
        {
            public TreeNode LBranch;
            public TreeNode RBranch;
            public TKey Key;
            public List Values;
            public bool Color;

            public TreeNode()
            {
                (LBranch, RBranch, Values) = (null, null, null);
                Color = BLACK;
            }

            public TreeNode(TKey key, TValue value)
            {
                Color = RED;
                Values = new List();
                Values.Add(value);
                Key = key;
                (LBranch, RBranch) = (null, null);
            }
        }

        /// <summary>
        /// Корень дерева
        /// </summary>
        private TreeNode _root = null;
    }
}
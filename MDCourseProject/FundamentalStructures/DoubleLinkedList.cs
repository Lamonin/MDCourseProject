using System;

namespace FundamentalStructures
{
    public class DoubleLinkedList<TValue> where TValue: IComparable
    {
       /// <summary>
       /// Узел двухсвязного кольцевого списка
       /// </summary>
       public class ListNode {
           /// <summary>
           /// Ссылка на предыдущий элемент
           /// </summary>
           public ListNode Prev;
           /// <summary>
           /// Ссылка на последующий элемент
           /// </summary>
           public ListNode Next;
           /// <summary>
           /// Хранит значения узла
           /// </summary>
           private TValue _value;

           /// <summary>
           /// Создает пустой узел
           /// </summary>
           public ListNode()
           {
               (Prev, Next) = (null, null);
           }
           /// <summary>
           /// Создает узел со значением value
           /// </summary>
           /// <param name="value"></param>
           public ListNode(TValue value)
           {
               (Prev, Next) = (this, this);
               _value = value;
           }

           /// <summary>
           /// Возвращает значения узла
           /// </summary>
           /// <returns></returns>
           public TValue GetValue()
           {
               return _value;
           }

           /// <summary>
           /// Перегрузка метода ToString()
           /// </summary>
           /// <returns>Возвращает строковый тип значения узла</returns>
           public override string ToString()
           {
               return _value.ToString();
           }
       } 
       private ListNode _head;
       private static bool IsEmpty(ListNode node)
       {
           return node == null;
       }

       public DoubleLinkedList() => _head = null;

       public void Add(TValue value)
       {
           if (!IsEmpty(FindHelper(value))) return;
           var elem = new ListNode(value);
           if (IsEmpty(_head)) _head = elem;
           else
           {
               (elem.Prev, elem.Next) = (_head.Prev, _head);
               (_head.Prev, _head.Prev.Next) = (elem, elem);
           }
       }

       private ListNode FindHelper(TValue key)
       {
           if (IsEmpty(_head)) return null;
           _head.Prev.Next = null;
           var curr = _head;
           while (!IsEmpty(curr))
           {
               if (curr.GetValue().CompareTo(key) == 0)
               {
                   _head.Prev.Next = _head;
                   return curr;
               }
               curr = curr.Next;
           }
           _head.Prev.Next = _head;
           return null;
       }

       public void Remove(TValue elem)
       {
           if (IsEmpty(_head)) return;
           var dElem = FindHelper(elem);
           if(IsEmpty(dElem)) return;
           if(dElem == _head)
               if (_head.Next == _head)
                   _head = null;
               else
               {
                   _head.Prev.Next = _head.Next;
                   _head.Next.Prev = _head.Prev;
                   _head = _head.Next;
               }
           else
           {
               dElem.Prev.Next = dElem.Next;
               dElem.Next.Prev = dElem.Prev;
           }
       }

       public bool Find(TValue value)
       {
           return !IsEmpty(FindHelper(value));
       }
       
       public void ChangeHead(ListNode node)
       {
           _head = null;
           _head = node;
       }

       public ListNode GetHead() => _head;

       public int Count()
       {
           var count = 0;
           _head.Prev.Next = null;
           var tmp = _head;
           while (!IsEmpty(tmp))
           {
               ++count;
               tmp = tmp.Next;
           }
           _head.Prev.Next = _head;
           return count;
       }

       public override string ToString()
       {
           var str = "";
           _head.Prev.Next = null;
           var tmp = _head;
           while (!IsEmpty(tmp))
           {
               str += tmp + " ";
               tmp = tmp.Next;
           }
           return str;
       }
   }
}

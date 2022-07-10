using System;
using System.Collections;
using System.Collections.Generic;

namespace FundamentalStructures
{
    /// <summary>
    /// Двухсвязный кольцевой список
    /// </summary>
    /// <typeparam name="TValue">Тип значаений узлов в списке</typeparam>
    public class DoubleCircularLinkedList<TValue>:IEnumerable<TValue> where TValue: IComparable<TValue>
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
           public ListNode(TValue value)
           {
               (Prev, Next) = (this, this);
               _value = value;
           }

           /// <summary>
           /// Возвращает значения узла
           /// </summary>
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
       
       /// <summary>
       /// Голова списка
       /// </summary>
       private ListNode _head;
       
       /// <summary>
       /// Проверяет, является ли узел node null
       /// </summary>
       private static bool IsEmpty(ListNode node)
       {
           return node == null;
       }

       /// <summary>
       /// Создает пустой список
       /// </summary>
       public DoubleCircularLinkedList() => _head = null;

       
       /// <summary>
       /// Добавляет в конец списка узел со значением value 
       /// </summary>
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

       /// <summary>
       /// Находит узел со значением key
       /// </summary>
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

       /// <summary>
       /// Удаляет узел со значением elem
       /// </summary>
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
       
       /// <summary>
       /// Проверяет, есть ли в списке значение value
       /// </summary>
       public bool Find(TValue value)
       {
           return !IsEmpty(FindHelper(value));
       }
       
       /// <summary>
       /// Меняет голову спика на node
       /// </summary>
       public void ChangeHead(ListNode node)
       {
           _head = node;
       }

       /// <summary>
       /// Возвращает голову списка
       /// </summary>
       public ListNode GetHead() => _head;

       
        /// <summary>
        ///  Считает кол-во элементов в списке
        /// </summary>
        public int Count()
       {
           var count = 0;

           if (_head is null) return count;
           
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
        
        /// <summary>
        /// Переводит список в строку
        /// </summary>
        /// <returns></returns>
       public override string ToString()
       {
           var str = "";
           _head.Prev.Next = null;
           var tmp = _head;
           while (!IsEmpty(tmp))
           {
               str += "|" + tmp + "|";
               tmp = tmp.Next;
           }
           return str;
       }
        
       private class DoubleCircularLinkedListEnumerator:IEnumerator<TValue>
       {
           private ListNode head;
           private ListNode currentNode;
           private int count;
           private int i;
           
           public DoubleCircularLinkedListEnumerator(ListNode head, int count)
           {
               this.head = head;
               this.count = count;
               currentNode = head;
           }
           
           public bool MoveNext()
           {
               if (head is null) return false;
               
               Current = currentNode.GetValue();
               currentNode = currentNode.Next;
               
               i += 1;
               if (i > count) return false;
               return true;
           }

           public void Dispose() { }
           public void Reset() { currentNode = head; i = 0; }

           public TValue Current { get; private set; }

           object IEnumerator.Current => Current;
       }

       public IEnumerator<TValue> GetEnumerator()
       {
           return new DoubleCircularLinkedListEnumerator(_head, Count());
       }
       
       IEnumerator IEnumerable.GetEnumerator()
       {
           return GetEnumerator();
       }
    }
}

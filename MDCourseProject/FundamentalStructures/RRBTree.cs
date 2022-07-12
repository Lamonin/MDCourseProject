using System;

namespace FundamentalStructures
{
       /// <summary>
       /// Красно-Черное дерево (рекурсивная реализация). Представляется в виде "ключ - значение". Ключи могут быть не уникальными.
       /// Значения хранятся в двусвязном кольцевом списке.
       /// </summary>
       /// <typeparam name="TKey">Тип ключа дерева</typeparam>
       /// <typeparam name="TValue">Тип значения узла списка дерева</typeparam>
       public class RRBTree<TKey, TValue> where TKey : IComparable<TKey> where TValue : IComparable<TValue>
       {
           /// <summary>
           /// Черный цвет узла
           /// </summary>
           private const bool BLACK = false;

           /// <summary>
           /// Красный цвет узла
           /// </summary>
           private const bool RED = true;

           /// <summary>
           /// Узел дерева
           /// </summary>
           protected class TreeNode
           {
               /// <summary>
               /// Левый потомок узла
               /// </summary>
               public TreeNode LBranch;
               
               /// <summary>
               /// Правый потомок узла
               /// </summary>
               public TreeNode RBranch;
               
               /// <summary>
               /// Ключ узла
               /// </summary>
               public TKey Key;
               
               /// <summary>
               /// Список значений узла
               /// </summary>
               private DoubleCircularLinkedList<TValue> values;
               
               /// <summary>
               /// Цвет узла
               /// </summary>
               public bool Color;
   
               /// <summary>
               /// Создает красный узел по умолчанию
               /// </summary>
               public TreeNode()
               {
                   (LBranch, RBranch, values) = (null, null, null);
                   Color = RED;
               }
   
               /// <summary>
               /// Создает красный узел с ключом key и со значением value
               /// </summary>
               public TreeNode(TKey key, TValue value)
               {
                   Color = RED;
                   values = new DoubleCircularLinkedList<TValue>();
                   values.Add(value);
                   Key = key;
                   (LBranch, RBranch) = (null, null);
               }
               
               /// <summary>
               /// Добавляет в список значение value
               /// </summary>
               public void AddNodeToList(TValue value) => values.Add(value);
               
               /// <summary>
               /// Удаляет из спиcка значение value
               /// </summary>
               public void Remove(TValue value) => values.Remove(value);

               /// <summary>
               /// Проверяет на наличие в списке значение value
               /// </summary>
               public bool Find(TValue value)
               {
                   return values.Find(value);
               }
               
               /// <summary>
               /// Проверяет, является ли узел красным
               /// </summary>
               public bool IsRed() => Color == RED;
   
               /// <summary>
               /// Возвращает голову списка
               /// </summary>
               public DoubleCircularLinkedList<TValue>.ListNode GetHead() => values.GetHead();
   
               /// <summary>
               /// Возвращает список значений узла
               /// </summary>
               public DoubleCircularLinkedList<TValue> GetList() => values;
   
               /// <summary>
               /// Меняет голову списка значений на node
               /// </summary>
               public void Change(DoubleCircularLinkedList<TValue>.ListNode node) => values.ChangeHead(node);
   
               /// <summary>
               /// Считает количество элементов в списке
               /// </summary>
               public int CountValues() => values.Count();
           }
   
           /// <summary>
           /// Корень дерева
           /// </summary>
           private TreeNode _root;
   
           /// <summary>
           /// Проверяет, является ли узел node null
           /// </summary>
           private bool IsEmpty(TreeNode node) => node == null;
   
           /// <summary>
           /// Проверяет, является ли узел node красным
           /// </summary>
           private bool IsRed(TreeNode node) => !IsEmpty(node) && node.IsRed();
   
           /// <summary>
           /// Поворачивает узел node налево
           /// </summary>
           private void LeftRotate(ref TreeNode node)
           {
               var tmp = node.RBranch;
               node.RBranch = tmp.LBranch;
               tmp.LBranch = node;
               node = tmp;
           }
           
           /// <summary>
           /// Поворачивает узел node направо
           /// </summary>
           private void RightRotate(ref TreeNode node)
           {
               var tmp = node.LBranch;
               node.LBranch = tmp.RBranch;
               tmp.RBranch = node;
               node = tmp;
           }
           
           /// <summary>
           /// Находит в поддереве с корнем node узел по ключу key
           /// </summary>
           private TreeNode FindKeyHelper(TreeNode node, TKey key, ref int step)
           {
               if (IsEmpty(node)) return null;
               step += 1;
               switch (key.CompareTo(node.Key))
               {
                   case 0:
                       return node;
                   case -1:
                       return FindKeyHelper(node.LBranch, key, ref step);
                   default:
                       return FindKeyHelper(node.RBranch, key, ref step);
               }
           }
   
           /// <summary>
           /// Находит минимальное значение у поддерева с корнем node
           /// </summary>
           private TreeNode FindMin(TreeNode node)
           {
               if (IsEmpty(node)) return null;
               return IsEmpty(node.LBranch) ? node : FindMin(node.LBranch);
           }
           
           /// <summary>
           /// Меняет ключ и значение у узлов node и минимального значения в поддереве, в котором node.RBranch является корнем
           /// </summary>
           private TreeNode Transplant1(ref TreeNode node)
           {
               var nodeMin = FindMin(node.RBranch);
               var listNode = node.GetHead();
               var key = node.Key;
               node.Key = nodeMin.Key;
               node.Change(nodeMin.GetHead());
               nodeMin.Key = key;
               nodeMin.Change(listNode);
               return nodeMin;
           }
   
           /// <summary>
           /// Меняет ключ и значение у узлов node и node.RBranch
           /// </summary>
           private TreeNode Transplant2(ref TreeNode node)
           {
               var listNode = node.GetHead();
               var key = node.Key;
               node.Key = node.RBranch.Key;
               node.Change(node.RBranch.GetHead());
               node.RBranch.Change(listNode);
               node.RBranch.Key = key;
               return node.RBranch;
           }
   
           /// <summary>
           /// Меняет ключ и значение у узлов node и node.LBranch
           /// </summary>
           private TreeNode Transplant3(ref TreeNode node)
           {
               var listNode = node.GetHead();
               var key = node.Key;
               node.Key = node.LBranch.Key;
               node.Change(node.LBranch.GetHead());
               node.LBranch.Change(listNode);
               node.LBranch.Key = key;
               return node.LBranch;
           }
   
           /// <summary>
           /// Балансирует поддерево по первому случаю при удалении
           /// </summary>
           /// <param name="node">Корень поддерева</param>
           /// <param name="check">Дочерний узел поддерева, для которого проверяется, нужна ли балансировка</param>
           /// <param name="rotate">Отвечает за поворот в поддереве</param>>
           /// <param name="direction">Отвечает за направление удаления</param>
           private void DeleteBalance1(ref TreeNode node, TreeNode check, ref bool rotate, bool direction)
           {
               if (IsRed(check))
               {
                   node.Color = RED;
                   check.Color = BLACK;
                   if (direction) RightRotate(ref node);
                   else LeftRotate(ref node);
                   rotate = true;
               }
           }
   
           /// <summary>
           /// Балансирует поддерево по второму случаю при удалении
           /// </summary>
           /// <param name="node">Корень поддерева</param>
           /// <param name="check">Дочерний узел поддерева, для которого проверяется, нужна ли балансировка</param>>
           /// <param name="needToBalance">Отвечает за остановку балансировки</param>>
           /// <param name="ret">Если условия для балансировки второго случая выполняются, то переход на проверку для случаев 1, 2, 3, 4</param>
           private void DeleteBalance2(TreeNode node, TreeNode check, ref bool needToBalance, ref bool ret)
           {
               if (!IsRed(check) && !IsRed(check.RBranch) && !IsRed(check.LBranch))
               {
                   ret = true;
                   check.Color = RED;
                   if (IsRed(node))
                   {
                       node.Color = BLACK;
                       needToBalance = false;
                   }
               }
           }
           
           /// <summary>
           /// Балансирует поддерево по третьему случаю при удалении
           /// </summary>
           /// <param name="node">Корень поддерева</param>
           private void DeleteBalance3(ref TreeNode node, bool direction)
           {
               if (direction)
               {
                   if (!IsRed(node) && IsRed(node.RBranch) && !IsRed(node.LBranch))
                   {
                       node.Color = RED;
                       node.RBranch.Color = BLACK;
                       LeftRotate(ref node);
                   }
               }
               else
               {   if (!IsRed(node) && !IsRed(node.RBranch) && IsRed(node.LBranch))
                   {
                       node.Color = RED;
                       node.LBranch.Color = BLACK;
                       RightRotate(ref node);
                   }
               }
               
           }
           
           /// <summary>
           /// Балансирует поддерево по четвертому случаю при удалении
           /// </summary>
           /// <param name="node">Корень поддерева</param>
           /// <param name="direction">Отвечает за направление удаления</param>
           private void DeleteBalance4(ref TreeNode node, bool direction)
           {
               if (direction)
               {
                   node.LBranch.Color = node.Color;
                   node.Color = BLACK;
                   node.LBranch.LBranch.Color = BLACK;
                   RightRotate(ref node);
               }
               else
               {
                   node.RBranch.Color = node.Color;
                   node.Color = BLACK;
                   node.RBranch.RBranch.Color = BLACK;
                   LeftRotate(ref node);
               }
           }
   
           /// <summary>
           /// Балансировка, выполняющая балансировки для случаев 2, 3, 4, при удалении в поддереве
           /// </summary>
           /// <param name="node1">Корень поддерева</param>
           /// <param name="node2">Дочерний узел узла node1 </param>
           /// <param name="needToBalance">Отвечает за остановку балансировки</param>
           /// <param name="direction">Отвечает за направление удаления</param>
           private void DeleteBalanceHelper(ref TreeNode node1, ref TreeNode node2, ref bool needToBalance, bool direction)
           {
               var ret = false;
               DeleteBalance2(node1, node2, ref needToBalance, ref ret);
               if (ret) return;
               DeleteBalance3(ref node2, direction);
               DeleteBalance4(ref node1,  direction);
               needToBalance = false;
           }
           
           /// <summary>
           /// Балансировка левой части поддерева
           /// </summary>
           /// <param name="node">Корень поддерева</param>
           /// <param name="needToBalance">Отвечает за остановку балансировки</param>
           private void DeleteBalanceLeft(ref TreeNode node, ref bool needToBalance)
           {
               var rotate = false;
               DeleteBalance1(ref node, node.LBranch, ref rotate, true);
               if (rotate)
                   DeleteBalanceHelper(ref node.RBranch, ref node.RBranch.LBranch, ref needToBalance, true);
               else
                   DeleteBalanceHelper(ref node, ref node.LBranch, ref needToBalance, true);
           }
   
           /// <summary>
           /// Балансировка правой части поддерева
           /// </summary>
           /// <param name="node">Корень поддерева</param>
           /// <param name="needToBalance">Отвечает за остановку балансировки</param>
           private void DeleteBalanceRight(ref TreeNode node, ref bool needToBalance)
           {
               var rotate = false;
               DeleteBalance1(ref node, node.RBranch, ref rotate, false);
               if (rotate)
                   DeleteBalanceHelper(ref node.LBranch, ref node.LBranch.RBranch, ref needToBalance, false);
               else
                   DeleteBalanceHelper(ref node, ref node.RBranch, ref needToBalance, false);
               
           }
   
           /// <summary>
           /// Балансировка поддерева
           /// </summary>
           /// <param name="node">Корень поддерва</param>
           /// <param name="needToBalance">Отвечает за остановку балансировки</param>
           /// <param name="direction">Отвечает за направление удаления</param>
           private void DeleteBalance(ref TreeNode node, ref bool needToBalance, bool direction)
           {
               if(needToBalance)
                   if(direction)
                       DeleteBalanceLeft(ref node, ref needToBalance);
                   else
                       DeleteBalanceRight(ref node, ref needToBalance);
           }
           
           /// <summary>
           /// Удаляет в поддереве узел deleteNode
           /// </summary>
           /// <param name="node">Корень поддерва</param>
           /// <param name="deleteNode">Удаляемый узел</param>
           /// <param name="needToBalance">Отвечает за остановку балансировки</param>
           private void DeleteAndBalance(ref TreeNode node, ref TreeNode deleteNode, ref bool needToBalance)
           {
               var direction = true;
               if (node == deleteNode)
               {
                   if (!IsEmpty(node.LBranch) && !IsEmpty(node.RBranch))
                   {
                       deleteNode = Transplant1(ref node);
                       DeleteAndBalance(ref node.RBranch, ref deleteNode, ref needToBalance);
                   }
                   else if (IsEmpty(node.LBranch) && !IsEmpty(node.RBranch))
                   {
                       deleteNode = Transplant2(ref node);
                       DeleteAndBalance(ref node.RBranch, ref deleteNode, ref needToBalance);
                   }
                   else if (!IsEmpty(node.LBranch) && IsEmpty(node.RBranch))
                   {
                       deleteNode = Transplant3(ref node);
                       DeleteAndBalance(ref node.LBranch, ref deleteNode, ref needToBalance);
                       direction = false;
                   }
                   else
                   {
                       if (IsRed(node)) needToBalance = false;
                       node = null;
                       return;
                   }
               }
               else if (node.Key.CompareTo(deleteNode.Key) == -1)
                   DeleteAndBalance(ref node.RBranch, ref deleteNode, ref needToBalance);
               else
               {
                   DeleteAndBalance(ref node.LBranch, ref deleteNode, ref needToBalance);
                   direction = false;
               }
               DeleteBalance(ref node, ref needToBalance, direction);
           }
           
           private void AddBalance1(TreeNode node)
           {
               if (IsRed(node.LBranch) && IsRed(node.RBranch))
               {
                   node.Color = RED;
                   node.LBranch.Color = BLACK;
                   node.RBranch.Color = BLACK;
               }
           }
   
           private void AddBalance2(ref TreeNode node, bool colorOfInsert)
           {
               if(IsRed(node) && IsRed(node.RBranch) && colorOfInsert == BLACK)
                   LeftRotate(ref node);
               if (IsRed(node.RBranch) && IsRed(node.RBranch.RBranch))
               {
                   LeftRotate(ref node);
                   node.Color = BLACK;
                   node.LBranch.Color = RED;
               }
           }
           
           private void AddBalance3(ref TreeNode node, bool colorOfInsert)
           {
               if(IsRed(node) && IsRed(node.LBranch) && colorOfInsert == RED)
                   RightRotate(ref node);
               if (IsRed(node.LBranch) && IsRed(node.LBranch.LBranch))
               {
                   RightRotate(ref node);
                   node.Color = BLACK;
                   node.RBranch.Color = RED;
               }
           }
   
           private void AddAndBalance(ref TreeNode node, TreeNode add, bool colorOfInsert)
           {
               if (IsEmpty(node))
               {
                   node = add;
                   return;
               }
               AddBalance1(node);
               if (node.Key.CompareTo(add.Key) == -1)
               {
                   AddAndBalance(ref node.RBranch, add, RED);
                   AddBalance2(ref node, colorOfInsert);
               }
               else
               {
                   AddAndBalance(ref node.LBranch, add, BLACK);
                   AddBalance3(ref node, colorOfInsert);
               }
           }
           
           public RRBTree()
           {
               _root = null;
           }
   
           public void Add(TKey key, TValue value)
           {
               var _ = 0;
               if(ContainKey(key)) FindKeyHelper(_root, key, ref _).AddNodeToList(value);
               else
               {
                   var node = new TreeNode(key, value);
                   AddAndBalance(ref _root, node, BLACK);
               }
               _root.Color = BLACK;
           }
   
           public void Delete(TKey key, TValue value)
           {
               var _ = 0;
               var deleteNode = FindKeyHelper(_root, key, ref _);
               if (deleteNode != null && deleteNode.Find(value))
                   if (deleteNode.CountValues() > 1)
                       deleteNode.Remove(value);
                   else
                   {
                       var flag = true;
                       DeleteAndBalance(ref _root, ref deleteNode, ref flag);
                   }
               if (!IsEmpty(_root)) _root.Color = BLACK;
           }
           
           public bool ContainKey(TKey key)
           {
               var _ = 0;
               var node = FindKeyHelper(_root, key, ref _);
               return !IsEmpty(node);
           }

           public bool Contains(TKey key, TValue value)
           {
               var _ = 0;
               var node = FindKeyHelper(_root, key, ref _);
               return !IsEmpty(node) && node.Find(value);
           }

           public DoubleCircularLinkedList<TValue> GetValue(TKey key)
           {
               var _ = 0;
               var find = FindKeyHelper(_root, key, ref _);
               return !IsEmpty(find) ? find.GetList() : null;
           }
           
           private void Print(TreeNode root, int height, ref string output)
           {
               if (!IsEmpty(root))
               {
                   Print(root.RBranch, height+4, ref output);
                   for(var i = 1; i<height; i++) output += "    ";
                   output += root.Key+ " (" + root.GetList() + ")" + (root.Color ? "-RED" : "-BLACK") + '\n' + '\n';
                   Print(root.LBranch, height+4, ref output);
               }
           }

           public DoubleCircularLinkedList<TValue> Find(TKey key, out int step)
           {
               var steps = 0;
               var find = FindKeyHelper(_root, key, ref steps);
               step = steps;
               return !IsEmpty(find) ? find.GetList() : null;
           }
   
           public string PrintTree()
           {
               var output = string.Empty;
               Print(_root, 0, ref output);
               return output;
           }
       }
}
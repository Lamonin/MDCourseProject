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
           protected class TreeNode
           {
               public TreeNode LBranch;
               public TreeNode RBranch;
               public TKey Key;
               private DoubleLinkedList<TValue> values;
               public bool Color;
   
               public TreeNode()
               {
                   (LBranch, RBranch, values) = (null, null, null);
                   Color = RED;
               }
   
               public TreeNode(TKey key, TValue value)
               {
                   Color = RED;
                   values = new DoubleLinkedList<TValue>();
                   values.Add(value);
                   Key = key;
                   (LBranch, RBranch) = (null, null);
               }
   
               public void AddNodeToList(TValue value) => values.Add(value);
   
               public void Remove(TValue value) => values.Remove(value);

               public bool Find(TValue value)
               {
                   return values.Find(value);
               }
               public bool IsRed() => Color == RED;
   
               public DoubleLinkedList<TValue>.ListNode GetHead() => values.GetHead();
   
               public DoubleLinkedList<TValue> GetList() => values;
   
               public void Change(DoubleLinkedList<TValue>.ListNode node) => values.ChangeHead(node);
   
               public int CountValues() => values.Count();
           }
   
           /// <summary>
           /// Корень дерева
           /// </summary>
           private TreeNode _root;
   
           private bool IsEmpty(TreeNode node) => node == null;
   
           private bool IsRed(TreeNode node) => !IsEmpty(node) && node.IsRed();
   
           private void LeftRotate(ref TreeNode node)
           {
               var tmp = node.RBranch;
               node.RBranch = tmp.LBranch;
               tmp.LBranch = node;
               node = tmp;
           }
   
           private void RightRotate(ref TreeNode node)
           {
               var tmp = node.LBranch;
               node.LBranch = tmp.RBranch;
               tmp.RBranch = node;
               node = tmp;
           }
           
           private TreeNode FindKeyHelper(TreeNode node, TKey key)
           {
               if (IsEmpty(node)) return null;
               switch (key.CompareTo(node.Key))
               {
                   case 0:
                       return node;
                   case -1:
                       return FindKeyHelper(node.LBranch, key);
                   default:
                       return FindKeyHelper(node.RBranch, key);
               }
           }
   
           private TreeNode FindMin(TreeNode node)
           {
               if (IsEmpty(node)) return null;
               return IsEmpty(node.LBranch) ? node : FindMin(node.LBranch);
           }
   
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
   
           private void DeleteBalance2(TreeNode node, TreeNode check, ref bool needToBalance, ref bool ret)
           {
               ret = !IsRed(check) && !IsRed(check.RBranch) && !IsRed(check.LBranch);
               if (ret)
               {
                   check.Color = RED;
                   if (IsRed(node))
                   {
                       node.Color = BLACK;
                       needToBalance = false;
                   }
               }
           }
   
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
   
           private void DeleteBalanceHelper(ref TreeNode node1, ref TreeNode node2, ref bool needToBalance, bool direction)
           {
               var ret = true;
               DeleteBalance2(node1, node2, ref needToBalance, ref ret);
               if (ret) return;
               DeleteBalance3(ref node2, direction);
               DeleteBalance4(ref node1,  direction);
               needToBalance = false;
           }
           
           private void DeleteBalanceLeft(ref TreeNode node, ref bool needToBalance)
           {
               var rotate = false;
               DeleteBalance1(ref node, node.LBranch, ref rotate, true);
               if (rotate)
                   DeleteBalanceHelper(ref node.RBranch, ref node.RBranch.LBranch, ref needToBalance, true);
               else
                   DeleteBalanceHelper(ref node, ref node.LBranch, ref needToBalance, true);
           }
   
           private void DeleteBalanceRight(ref TreeNode node, ref bool needToBalance)
           {
               var rotate = false;
               DeleteBalance1(ref node, node.RBranch, ref rotate, false);
               if (rotate)
                   DeleteBalanceHelper(ref node.LBranch, ref node.LBranch.RBranch, ref needToBalance, false);
               else
                   DeleteBalanceHelper(ref node, ref node.RBranch, ref needToBalance, false);
               
           }
   
           private void DeleteBalance(ref TreeNode node, ref bool needToBalance, bool direction)
           {
               if(needToBalance)
                   if(direction)
                       DeleteBalanceLeft(ref node, ref needToBalance);
                   else
                       DeleteBalanceRight(ref node, ref needToBalance);
           }
           
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
               {
                   DeleteAndBalance(ref node.RBranch, ref deleteNode, ref needToBalance);
               }
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
                   AddAndBalance(ref node.RBranch,add,RED);
                   AddBalance2(ref node, colorOfInsert);
               }
               else
               {
                   AddAndBalance(ref node.LBranch,add,BLACK);
                   AddBalance3(ref node,colorOfInsert);
               }
           }
           
           public RRBTree()
           {
               _root = null;
           }
   
           public void Add(TKey key, TValue value)
           {
               if (FindKeyHelper(_root, key) != null && FindKeyHelper(_root, key).Find(value)) return;
               if(FindKeyHelper(_root, key) != null) FindKeyHelper(_root, key).AddNodeToList(value);
               else
               {
                   var node = new TreeNode(key, value);
                   AddAndBalance(ref _root, node, BLACK);
               }
               _root.Color = BLACK;
           }
   
           public void Delete(TKey key, TValue value)
           {
               var deleteNode = FindKeyHelper(_root, key);
               if (deleteNode != null && deleteNode.Find(value))
                   if (deleteNode.CountValues() > 1)
                       deleteNode.Remove(value);
                   else
                   {
                       var flag = true;
                       DeleteAndBalance( ref _root, ref deleteNode, ref flag);
                   }
               if (!IsEmpty(_root)) _root.Color = BLACK;
           }

           public void RemoveKey(TKey key)
           {
               var deleteNode = FindKeyHelper(_root, key);
               if (deleteNode != null)
               {
                   var flag = true;
                   DeleteAndBalance(ref _root, ref deleteNode, ref flag);
               }
               if (!IsEmpty(_root)) _root.Color = BLACK;
           }
           public bool ContainKey(TKey key)
           {
               var node = FindKeyHelper(_root, key);
               return !IsEmpty(node);
           }

           public bool Contains(TKey key, TValue value) => FindKeyHelper(_root, key) != null && FindKeyHelper(_root, key).Find(value);

           public DoubleLinkedList<TValue> GetValue(TKey key)
           {
               var find = FindKeyHelper(_root, key);
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
   
           public string PrintTree()
           {
               var output = string.Empty;
               Print(_root, 0, ref output);
               return output;
           }
       }
}
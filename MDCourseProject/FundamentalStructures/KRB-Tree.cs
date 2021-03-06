using System;

//TODO Как по мне, надо в первую очередь нейминг привести в порядок
namespace FundamentalStructures
{
    public class RB_Tree<TKey,TValue> where TKey:IComparable<TKey> where TValue:IComparable<TValue>
    {
        public class leaf
        {
            public class list
            {
                public class Node
                {
                    public Node pPrev;
                    public Node pNext;

                    public TValue pData;

                    public Node(TValue data, Node prev = null, Node next = null)
                    {
                        pData = data;
                        pNext = next;
                        pPrev = prev;
                    }
                }

                public Node head;
                public Node last;

                public list()
                {
                    head = null;
                    last = null;
                }

                public void AddNode(TValue data)
                {
                    if (head == null)
                    {
                        head = new Node(data);
                        last = head;
                    }
                    else
                    {
                        last.pNext = new Node(data);
                        last.pNext.pPrev = last;
                        last = last.pNext;
                    }
                }

                public int ReturnCountOfElements()
                {
                    if (head == null) return 0;
                    
                    int count = 1;
                    Node cur = head;
                    while (cur != null)
                    {
                        count++;
                        cur = cur.pNext;
                    }

                    return count;
                }

                public void DelNode(TValue val)
                {
                    Node cur = head;
                    bool deleted = false;
                    while (cur != null && deleted == false)
                    {
                        if (cur.pData.CompareTo(val) == 0)
                        {
                            if (cur == head && last == head)
                            {
                                head = null;
                                last = null;
                            }
                            else if (cur == head && last!=head)
                            {
                                Node del = cur;
                                head = cur.pNext;
                                head.pPrev = null;
                                del = null;
                                deleted = true;
                            }
                            else
                            {
                                cur.pPrev.pNext = cur.pNext;
                                cur.pNext.pPrev = cur.pPrev;
                                cur = null;
                            }
                        }
                        cur = cur.pNext;
                    }
                }
            }

            public leaf Lleaf;
            public leaf Rleaf;
            public leaf parent;

            public bool IsRed = false;
            
            //DATA
            public TKey key;

            public list valList;

            public leaf(){}
            
            public leaf(TKey key)
            {
                this.key = key;
            }
            public leaf(leaf Lf)
            {
                key = Lf.key;
                IsRed = Lf.IsRed;
            }
        }

        public leaf Tnil;
        public leaf m_root;

        public RB_Tree()
        {
            m_root = null;
            Tnil = new leaf();
        }

        public leaf GetLeaf(leaf root,TKey key)
        {
            if(root != null)
            {
                if (isEqual(root, Tnil)) return null; // Никогда не заходит в это условие
                if (root.key.CompareTo(key) < 0) return GetLeaf(root.Rleaf, key);
                if (root.key.CompareTo(key) > 0) return GetLeaf(root.Lleaf, key);
                return root;
            }
            return null;
        }
        public bool IsKeyExist(TKey key)
        {
            return GetLeaf(m_root, key) != null;
        }
        
        private void LeftRotate(leaf lf)
        {
            leaf lf_p = lf.Rleaf;
            lf.Rleaf = lf_p.Lleaf;

            if (!isEqual(lf_p.Lleaf,Tnil)) lf_p.Lleaf.parent = lf;
            lf_p.parent = lf.parent;
            if (isEqual(lf.parent,Tnil)) m_root = lf_p;
            else if (isEqual(lf,lf.parent.Lleaf)) lf.parent.Lleaf = lf_p;
            else lf.parent.Rleaf = lf_p;

            lf_p.Lleaf = lf;
            lf.parent = lf_p;
        }
        
        private void RightRotate(leaf lf)
        {
            leaf lf_p = lf.Lleaf;
            lf.Lleaf = lf_p.Rleaf;

            if (!isEqual(lf_p.Rleaf,Tnil)) lf_p.Rleaf.parent = lf;
            lf_p.parent = lf.parent;
            if (isEqual(lf.parent,Tnil)) m_root = lf_p;
            else if (isEqual(lf,lf.parent.Rleaf)) lf.parent.Rleaf = lf_p;
            else lf.parent.Lleaf = lf_p;

            lf_p.Rleaf = lf;
            lf.parent = lf_p;
        }

        private void RBInsertFixup(leaf InsertLeaf)
        {
            while (InsertLeaf.parent.IsRed == true)
            {
                if (isEqual(InsertLeaf.parent,InsertLeaf.parent.parent.Lleaf))
                {
                    leaf LfInsPPR = InsertLeaf.parent.parent.Rleaf;
                    if (LfInsPPR.IsRed == true)
                    {
                        InsertLeaf.parent.IsRed = false;
                        LfInsPPR.IsRed = false;
                        InsertLeaf.parent.parent.IsRed = true;
                        InsertLeaf = InsertLeaf.parent.parent;
                    }
                    else
                    {
                        if(isEqual(InsertLeaf,InsertLeaf.parent.Rleaf))
                        {
                            InsertLeaf = InsertLeaf.parent;
                            LeftRotate(InsertLeaf);
                        }

                        InsertLeaf.parent.IsRed = false;
                        InsertLeaf.parent.parent.IsRed = true;
                        RightRotate(InsertLeaf.parent.parent);
                    }
                }
                else
                {
                    leaf LfInsPPL = InsertLeaf.parent.parent.Lleaf;
                    if (LfInsPPL.IsRed == true)
                    {
                        InsertLeaf.parent.IsRed = false;
                        LfInsPPL.IsRed = false;
                        InsertLeaf.parent.parent.IsRed = true;
                        InsertLeaf = InsertLeaf.parent.parent;
                    }
                    else
                    {
                        if (isEqual(InsertLeaf,InsertLeaf.parent.Lleaf))
                        {
                            InsertLeaf = InsertLeaf.parent;
                            RightRotate(InsertLeaf);
                        }

                        InsertLeaf.parent.IsRed = false;
                        InsertLeaf.parent.parent.IsRed = true;
                        LeftRotate(InsertLeaf.parent.parent);
                    }
                }
            }
            m_root.IsRed = false;
        }

        public void RBAddLeaf(TKey key, TValue val)
        {
            bool addRepeatKey = false;
            if (m_root == null)
            {
                Tnil.IsRed = false;
                m_root = new leaf(key);
                
                m_root.valList = new leaf.list();
                m_root.valList.AddNode(val);
                
                m_root.Lleaf = new leaf(Tnil);
                m_root.Rleaf = new leaf(Tnil);
                m_root.parent = new leaf(Tnil);
            }
            else
            {
                leaf prev_current = new leaf(Tnil);
                leaf current = m_root;
                leaf tmp = new leaf(key);
                bool added = false;
                while (!isEqual(current,Tnil) && added != true)
                {
                    prev_current = current;
                    if(current.key.CompareTo(tmp.key)>0) current = current.Lleaf;
                    else if(current.key.CompareTo(tmp.key)<0) current = current.Rleaf;
                    else
                    {
                        if (current.valList == null) current.valList = new leaf.list();
                        current.valList.AddNode(val);
                        added = true;
                        addRepeatKey = true;
                    }
                }

                tmp.parent = prev_current;
                if (prev_current.key.CompareTo(tmp.key) > 0) prev_current.Lleaf = tmp;
                else if (prev_current.key.CompareTo(tmp.key) < 0) prev_current.Rleaf = tmp;
                tmp.Lleaf = Tnil;
                tmp.Rleaf = Tnil;
                tmp.IsRed = true;
                
                if (addRepeatKey == false)
                {
                    tmp.valList = new leaf.list();
                    tmp.valList.AddNode(val);
                    RBInsertFixup(tmp);
                }
            }
        }

        private void RBTransplant(leaf DelLeaf, leaf DelLeaf_Child)
        {
            if (isEqual(DelLeaf.parent,Tnil)) m_root = DelLeaf_Child;
            else if (isEqual(DelLeaf,DelLeaf.parent.Lleaf)) DelLeaf.parent.Lleaf = DelLeaf_Child;
            else DelLeaf.parent.Rleaf = DelLeaf_Child;
            DelLeaf_Child.parent = DelLeaf.parent;
        }

        private void RBDelFixUp(leaf FixLeaf)
        {
            leaf SecondChildFixLeafParrent;
            while (!isEqual(FixLeaf,m_root) && FixLeaf.IsRed == false)
            {
                if (isEqual(FixLeaf,FixLeaf.parent.Lleaf))
                {
                    SecondChildFixLeafParrent = FixLeaf.parent.Rleaf;
                    if (SecondChildFixLeafParrent.IsRed == true)
                    {
                        SecondChildFixLeafParrent.IsRed = false;
                        FixLeaf.parent.IsRed = true;
                        LeftRotate(FixLeaf.parent);
                        SecondChildFixLeafParrent = FixLeaf.parent.Rleaf;
                    }

                    if (SecondChildFixLeafParrent.Lleaf.IsRed == false && SecondChildFixLeafParrent.Rleaf.IsRed == false)
                    {
                        SecondChildFixLeafParrent.IsRed = true;
                        FixLeaf = FixLeaf.parent;
                    }

                    else
                    {
                        if (SecondChildFixLeafParrent.Rleaf.IsRed == false)
                        {
                            SecondChildFixLeafParrent.Lleaf.IsRed = false;
                            SecondChildFixLeafParrent.IsRed = true;
                            RightRotate(SecondChildFixLeafParrent);
                            SecondChildFixLeafParrent = FixLeaf.parent.Rleaf;
                        }
                        SecondChildFixLeafParrent.IsRed = FixLeaf.parent.IsRed;
                        FixLeaf.parent.IsRed = false;
                        SecondChildFixLeafParrent.Rleaf.IsRed = false;
                        LeftRotate(FixLeaf.parent);
                        FixLeaf = m_root;
                    }
                }
                else
                {
                    SecondChildFixLeafParrent = FixLeaf.parent.Lleaf;
                    if (SecondChildFixLeafParrent.IsRed == true)
                    {
                        SecondChildFixLeafParrent.IsRed = false;
                        FixLeaf.parent.IsRed = true;
                        RightRotate(FixLeaf.parent);
                        SecondChildFixLeafParrent = FixLeaf.parent.Lleaf;
                    }

                    if (SecondChildFixLeafParrent.Rleaf.IsRed == false && SecondChildFixLeafParrent.Lleaf.IsRed == false)
                    {
                        SecondChildFixLeafParrent.IsRed = true;
                        FixLeaf = FixLeaf.parent;
                    }
                    
                    else
                    {
                        if (SecondChildFixLeafParrent.Lleaf.IsRed == false)
                        {
                            SecondChildFixLeafParrent.Rleaf.IsRed = false;
                            SecondChildFixLeafParrent.IsRed = true;
                            LeftRotate(SecondChildFixLeafParrent);
                            SecondChildFixLeafParrent = FixLeaf.parent.Lleaf;
                        }
                        SecondChildFixLeafParrent.IsRed = FixLeaf.parent.IsRed;
                        FixLeaf.parent.IsRed = false;
                        SecondChildFixLeafParrent.Lleaf.IsRed = false;
                        RightRotate(FixLeaf.parent);
                        FixLeaf = m_root;
                    }
                }
            }
            FixLeaf.IsRed = false;
        }

        public void RBDelete(TKey key, TValue val)
        {
            leaf DelLeaf = GetLeaf(m_root, key);
            if (DelLeaf != null)
            {
                leaf DelLeafChild;
                leaf TempDelLeaf = DelLeaf;
                bool TempDelLeafOriginalColor = TempDelLeaf.IsRed;
                
                if (DelLeaf.valList != null && DelLeaf.valList.ReturnCountOfElements() > 0) DelLeaf.valList.DelNode(val);
                //тут убрал else
                if (DelLeaf.valList == null || (DelLeaf.valList != null && DelLeaf.valList.ReturnCountOfElements() == 0))
                {
                    if (isEqual(DelLeaf.Lleaf,Tnil))
                    {
                        DelLeafChild = DelLeaf.Rleaf;
                        RBTransplant(DelLeaf,DelLeaf.Rleaf);
                    }
                    else if (isEqual(DelLeaf.Rleaf,Tnil))
                    {
                        DelLeafChild = DelLeaf.Lleaf;
                        RBTransplant(DelLeaf,DelLeafChild);
                    }
                    else
                    {
                        leaf current = DelLeaf.Lleaf;
                        while (!isEqual(current.Rleaf,Tnil)) current = current.Rleaf;
                        TempDelLeaf = current;

                        TempDelLeafOriginalColor = TempDelLeaf.IsRed;
                        DelLeafChild = TempDelLeaf.Lleaf;
                        
                        if (isEqual(TempDelLeaf.parent, DelLeaf)) DelLeafChild.parent = TempDelLeaf;
                        else
                        {
                            RBTransplant(TempDelLeaf,TempDelLeaf.Lleaf);
                            TempDelLeaf.Lleaf = DelLeaf.Lleaf;
                            TempDelLeaf.Lleaf.parent = TempDelLeaf;
                        }
                        RBTransplant(DelLeaf,TempDelLeaf);
                        TempDelLeaf.Rleaf = DelLeaf.Rleaf;
                        TempDelLeaf.Rleaf.parent = TempDelLeaf;
                        TempDelLeaf.IsRed = DelLeaf.IsRed;
                    }
                    if (TempDelLeafOriginalColor == false) RBDelFixUp(DelLeafChild);
                }
            }
        }

        public void Clear()
        {
            m_root = null;
        }
        
        private void print_Tree(leaf p, int level, ref string output)
        {
            if(p != null)
            {
                print_Tree(p.Rleaf,level + 1, ref output);
                for(int i = 0;i< level;i++) output += "      ";
                output += p.key + (p.IsRed ? "-К\n":"-Ч\n");
                print_Tree(p.Lleaf,level + 1, ref output);
            }
        }
        
        public string PrintTree()
        {
            string output = String.Empty;
            if (m_root != null) print_Tree(m_root, 0, ref output);
            return output;
        }

        public bool isEqual(leaf leaf1, leaf leaf2)
        {
            if ((leaf1 == null && leaf2 != null) || (leaf1 != null && leaf2 == null)) return false;
            if (leaf1.key is null || leaf1.IsRed == leaf2.IsRed && leaf1.key.CompareTo(leaf2.key) == 0 &&
                leaf1.Lleaf == leaf2.Lleaf && leaf1.Rleaf == leaf2.Rleaf) return true;
            return false;
        }
    }
}
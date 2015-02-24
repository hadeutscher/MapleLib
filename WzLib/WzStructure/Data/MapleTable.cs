using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapleLib.WzLib.WzStructure.Data
{
    //read "MapleTable" out loud, lulz are guaranteed
    public class MapleTable<T> : Hashtable
    {
        public T this[string id]
        {
            get
            {
                return (T)this[(object)id];
            }
            set
            {
                this[(object)id] = value;
            }
        }
    }

    public class MapleTable<T1, T2> : Hashtable
    {
        public T2 this[T1 id]
        {
            get
            {
                return (T2)this[(object)id];
            }
            set
            {
                this[(object)id] = value;
            }
        }
    }
}

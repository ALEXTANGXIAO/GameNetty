using System.Collections.Generic;

namespace GameServer
{
    /// <summary>
    /// 无序重用HashSet
    /// </summary>
    public class UnOrderMultiMapSet<TKey, K>: Dictionary<TKey, HashSet<K>>
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="t"></param>
        public new HashSet<K> this[TKey t]
        {
            get
            {
                HashSet<K> set;
                if (!this.TryGetValue(t, out set))
                {
                    set = new HashSet<K>();
                }
                return set;
            }
        }
        
        public Dictionary<TKey, HashSet<K>> GetDictionary()
        {
            return this;
        }
        
        public void Add(TKey t, K k)
        {
            HashSet<K> set;
            this.TryGetValue(t, out set);
            if (set == null)
            {
                set = new HashSet<K>();
                base[t] = set;
            }
            set.Add(k);
        }

        public bool Remove(TKey t, K k)
        {
            HashSet<K> set;
            this.TryGetValue(t, out set);
            if (set == null)
            {
                return false;
            }
            if (!set.Remove(k))
            {
                return false;
            }
            if (set.Count == 0)
            {
                this.Remove(t);
            }
            return true;
        }

        public bool Contains(TKey t, K k)
        {
            HashSet<K> set;
            this.TryGetValue(t, out set);
            if (set == null)
            {
                return false;
            }
            return set.Contains(k);
        }

        public new int Count
        {
            get
            {
                int count = 0;
                foreach (KeyValuePair<TKey,HashSet<K>> kv in this)
                {
                    count += kv.Value.Count;
                }
                return count;
            }
        }
    }
}
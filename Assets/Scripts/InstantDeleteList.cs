using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    /**
     * basically uses generic list and overloads the remove & removesAt method with O(1) algorithm
     * DOES NOT KEEP ORDER ON REMOVE!
     */
    public class InstantDeleteList<T>
    {
        List<T> data;

        public InstantDeleteList()
        {
            data = new List<T>();
        }

        public InstantDeleteList(int reserve)
        {
            data = new List<T>(reserve);
        }

        public void add(T item)
        {
            data.Add(item);
        }

        /**
         * O(n) removal because of iterative search
         * still faster than List.Remove()
         */
        public void remove(T elem)
        {
            // find element
            int ind = data.FindIndex(x => x.Equals(elem));
            if (ind != -1)
                removeAt(ind);
        }

        /**
         * O(1) removal
         */
        public void removeAt(int ind)
        {
            if (ind >= data.Count)
                throw new IndexOutOfRangeException();

            // O(1): swap delete
            data[ind] = data[data.Count - 1]; // override item at delete pos with last element
            data.RemoveAt(data.Count - 1);    // delete last element
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIP
{
    class GenericLists<T>
    {
        List<T> list = new List<T>();

        public void Add(T p)
        {
            list.Add(p);
        }

        public void Delete(T p)
        {
            list.Remove(p);
        }

        public List<T> Get()
        {
            return list;
        }
    }
}

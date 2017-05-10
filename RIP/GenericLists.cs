using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIP
{
    class GenericLists<T>
    {
        private List<T> _list = new List<T>();

        public void Add(T p)
        {
            _list.Add(p);
        }

        public void Delete(T p)
        {
            _list.Remove(p);
        }

        public List<T> Get()
        {
            return _list;
        }

        public void Set(List<T> list)
        {
            _list = list;
        }
    }
}

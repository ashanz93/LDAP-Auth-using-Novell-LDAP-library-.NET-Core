using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreLDAPAuth.LDAP.Extensions
{
    public static class CollectionUtils
    {
        public static Collection<T> ToCollection<T>(this IEnumerable<T> data)
        {
            return new Collection<T>(data.ToList());
        }
    }
}

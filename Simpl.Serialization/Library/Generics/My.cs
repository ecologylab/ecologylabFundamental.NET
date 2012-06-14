using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Generics
{
    public class My<T> where T : struct
	{

        [SimplComposite]
        public T v;

		[SimplScalar]
		public int n;

        [SimplScalar]
		public Int32 o; 

		public List<My<T>> l;
        public Type ads = typeof(T);
	}
}

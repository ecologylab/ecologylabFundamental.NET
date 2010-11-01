using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ecologylab.attributes
{
    /// <summary>
    ///     Defines a field is represented as XML leaf when marshalled.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class simpl_hints : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
         private Hint[] hints;

        /// <summary>
        /// 
        /// </summary>
         /// <param name="p_hints"></param>
         public simpl_hints(Hint[] p_hints)
        {
            this.hints = p_hints;
        }

        /// <summary>
        /// 
        /// </summary>
        public Hint[] Value
        {
            get
            {
                return hints;
            }
        }
    }
}

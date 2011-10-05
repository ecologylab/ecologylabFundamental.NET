using System;

namespace Simpl.Serialization.Attributes
{
    /// <summary>
    ///     Defines a field is represented as XML leaf when marshalled.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SimplHints : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
         private readonly Hint[] hints;

        /// <summary>
        /// 
        /// </summary>
         /// <param name="pHints"></param>
         public SimplHints(Hint[] pHints)
        {
            this.hints = pHints;
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

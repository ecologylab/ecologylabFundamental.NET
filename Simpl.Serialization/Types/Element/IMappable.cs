using System;

namespace Simpl.Serialization.Types.Element
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMappable<out TK>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        TK Key();
    }
}

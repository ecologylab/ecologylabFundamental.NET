using System;

namespace Simpl.Serialization.Attributes
{
    /// <summary>
    ///     Takes a translation scope which binds classes to their XML
    ///     representation. This is used for polymorphic types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SimplScope : Attribute
    {
        private readonly String _translationScope;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="translationScope"></param>
        public SimplScope(String translationScope)
        {
            this._translationScope = translationScope;
        }

        /// <summary>
        /// 
        /// </summary>
        public String TranslationScope
        {
            get
            {
                return _translationScope;
            }
        }
    }
}

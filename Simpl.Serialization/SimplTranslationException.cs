using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.Serialization
{
    class SimplTranslationException : Exception
    {
        public SimplTranslationException(String message, Exception innerException) : base (message, innerException)
        {
            
        }
    }
}

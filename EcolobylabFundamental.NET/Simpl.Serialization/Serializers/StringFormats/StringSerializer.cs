﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Simpl.Serialization.Context;

namespace Simpl.Serialization.Serializers.StringFormats
{
    public class StringSerializer : FormatSerializer
    {
        public override void Serialize(object obj, StreamWriter streamWriter, TranslationContext translationContext)
        {
            throw new NotImplementedException();
        }

        public override void Serialize(object obj, FileStream fileStream, TranslationContext translationContext)
        {
            throw new NotImplementedException();
        }
    }
}

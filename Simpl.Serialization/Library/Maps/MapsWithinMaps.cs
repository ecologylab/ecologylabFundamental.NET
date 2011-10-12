using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Fundamental.Generic;

namespace Simpl.Serialization.Library.Maps
{
    public class MapsWithinMaps
    {
        public static TranslationS CreateObject()
        {
            TranslationS trans = new TranslationS();

            ClassDes cd1 = new ClassDes("cd1");

            cd1.fieldDescriptorsByTagName.Put("fd1_cd1", new FieldDes("fd1_cd1"));
            cd1.fieldDescriptorsByTagName.Put("fd2_cd1", new FieldDes("fd2_cd1"));
            cd1.fieldDescriptorsByTagName.Put("fd3_cd1", new FieldDes("fd3_cd1"));

            ClassDes cd2 = new ClassDes("cd2");
            cd2.fieldDescriptorsByTagName.Put("fd1_cd2", new FieldDes("fd1_cd2"));
            cd2.fieldDescriptorsByTagName.Put("fd2_cd2", new FieldDes("fd2_cd2"));
            cd2.fieldDescriptorsByTagName.Put("fd3_cd2", new FieldDes("fd3_cd2"));

            trans.entriesByTag.Put("cd1", cd1);
            trans.entriesByTag.Put("cd2", cd2);

            return trans;
        }
    }
}

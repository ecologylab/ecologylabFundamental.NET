//
//  CfCollaborationGetSurrogate.cs
//  s.im.pl serialization
//
//  Generated by DotNetTranslator on 04/09/11.
//  Copyright 2011 Interface Ecology Lab. 
//

using System;
using Simpl.Serialization.Attributes;
using Ecologylab.Collections;

namespace Simpl.OODSS.Messages
{
    /// <summary>
    /// missing java doc comments or could not find the source file.
    /// </summary>
    [SimplInherit]
    public class CfCollaborationGetSurrogate : RequestMessage
    {
        /// <summary>
        /// missing java doc comments or could not find the source file.
        /// </summary>
        [SimplScalar] 
        [SimplHints(new[] {Hint.XmlLeafCdata})]
        private String _surrogateSetString;

        public String SurrogateSetString
        {
            get { return _surrogateSetString; }
            set { _surrogateSetString = value; }
        }

        public override ResponseMessage PerformService(Scope<object> clientSessionScope)
        {
            throw new NotImplementedException();
        }
    }
}
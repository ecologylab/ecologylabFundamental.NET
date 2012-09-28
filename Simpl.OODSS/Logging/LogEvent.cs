//
//  LogEvent.cs
//  s.im.pl serialization
//
//  Generated by DotNetTranslator on 04/09/11.
//  Copyright 2011 Interface Ecology Lab. 
//

using System.Text;
using Simpl.OODSS.Messages;
using Simpl.Serialization.Attributes;
using Ecologylab.Collections;

namespace Simpl.OODSS.Logging
{
    /// <summary>
    /// missing java doc comments or could not find the source file.
    /// </summary>
    [SimplInherit]
    public class LogEvent : RequestMessage
    {
        /// <summary>
        /// missing java doc comments or could not find the source file.
        /// </summary>
        [SimplScalar] 
        [SimplHints(new[] {Hint.XmlLeafCdata})] 
        private StringBuilder _bufferToLog;

        public StringBuilder BufferToLog
        {
            get { return _bufferToLog; }
            set { _bufferToLog = value; }
        }

        public override ResponseMessage PerformService(Scope<object> clientSessionScope)
        {
            throw new System.NotImplementedException();
        }
    }
}
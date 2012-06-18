using System;

namespace Simpl.OODSS.Exceptions
{
    internal class MessageTooLargeException : Exception
    {
        //private const long SerialVersionUID = 1732834475978273620L;

        public int MaxMessageSize { get; private set; }

        public int ActualMessageSize { get; private set; }

        public MessageTooLargeException(int maxMessageSize, int actualMessageSize) :
            this("", maxMessageSize, actualMessageSize)
        {
        }

        public MessageTooLargeException(string message, int maxMessageSize, int actualMessageSize) :
            base(message)
        {
            MaxMessageSize = maxMessageSize;
            ActualMessageSize = actualMessageSize;
        }

        public MessageTooLargeException(Exception exception):
            base("", exception)
        {
        }
    }
}

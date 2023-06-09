using System;

namespace Cotur.BlockBee.Contracts
{
    public class BlockBeeClientException : Exception
    {
        public BlockBeeClientException(string message)
            : base(message)
        {
            
        }

        public BlockBeeClientException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }
    }
}
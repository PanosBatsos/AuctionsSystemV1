using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Exceptions
{
    public class PropertyAlreadyInUseException : Exception
    {
        public string FieldName { get; }
        public PropertyAlreadyInUseException(string fieldName, string message) : base(message) 
        {
            FieldName = fieldName;
        }
    }
}

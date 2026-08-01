using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Exceptions
{
    public class ConflictException : Exception
    {
        public string FieldName { get; }
        public ConflictException(string fieldName, string message) : base(message) 
        {
            FieldName = fieldName;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public InputType InputType { get; }

        public NotFoundException(string v)
        {
        }

        public NotFoundException(string message, InputType inputType)
            : base(message)
        {
            InputType = inputType;
        }

        public NotFoundException(string message, InputType inputType, Exception inner)
            : base(message, inner)
        {
            InputType = inputType;
        }
    }
    public enum InputType
    {
        User,
        Tenant,
        Supplier,
        Customer,
        Category,
        Product

    }
}

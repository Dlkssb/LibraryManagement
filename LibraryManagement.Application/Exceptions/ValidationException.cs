using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public List<string> ValdationErrors { get; set; }

        //public ValidationException(ValidationResult validationResult)
        //{
        //    ValdationErrors = new List<string>();

        //    foreach (var validationError in validationResult.Errors)
        //    {
        //        ValdationErrors.Add(validationError.ErrorMessage);
        //    }
        //}
        public ValidationException(List<ValidationFailure> failures)
        {
            ValdationErrors = new List<string>();

            foreach (var validationError in failures)
            {
                ValdationErrors.Add(validationError.ErrorMessage);
            }
        }


    }
}


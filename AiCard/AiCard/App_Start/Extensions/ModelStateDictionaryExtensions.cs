using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard
{
    public static class ModelStateDictionaryExtensions
    {

        public static string FirstErrorMessage(this System.Web.Mvc.ModelStateDictionary m)
        {
            return m.FirstOrDefault(s => s.Value.Errors.Count > 0).Value?.Errors[0].ErrorMessage;
        }

    }
}
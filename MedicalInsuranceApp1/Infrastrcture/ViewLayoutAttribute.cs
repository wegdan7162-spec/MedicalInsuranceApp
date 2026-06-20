using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MedicalInsuranceApp1.Infrastrcture
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ViewLayoutAttribute : ActionFilterAttribute
    {
        private readonly string _layout;

        public ViewLayoutAttribute(string layout)
        {
            _layout = layout;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ViewResult viewResult)
            {
                viewResult.ViewData["Layout"] = _layout;
            }
        }
    }
}

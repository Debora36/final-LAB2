using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace final_LAB2.Filters
{
    //sirve para capturar excepciones globales y redirigir a la página anterior con un mensaje de error
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        public GlobalExceptionFilter(ITempDataDictionaryFactory tempDataFactory)
        {
            _tempDataFactory = tempDataFactory;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is InvalidOperationException || context.Exception is ArgumentException)
            {
                var tempData = _tempDataFactory.GetTempData(context.HttpContext);
                tempData["ErrorMessage"] = context.Exception.Message;
                //intenta redirigir a la página anterior
                var request = context.HttpContext.Request;
                var returnUrl = request.Headers["Referer"].ToString();
                
                if (!string.IsNullOrEmpty(returnUrl))
                    context.Result = new RedirectResult(returnUrl);
                else
                    context.Result = new RedirectToActionResult("Index", "Home", null);
                
                context.ExceptionHandled = true; 
            }
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeSpace.Extension
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PreventDuplicateRequestAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Request["__RequestVerificationToken"] == null)
                return;

            var currentToken = HttpContext.Current.Request["__RequestVerificationToken"].ToString();

            if (HttpContext.Current.Session["LastProcessedToken"] == null)
            {
                HttpContext.Current.Session["LastProcessedToken"] = currentToken;
                return;
            }

            lock (HttpContext.Current.Session["LastProcessedToken"])
            {
                var lastToken = HttpContext.Current.Session["LastProcessedToken"].ToString();

                if (lastToken == currentToken)
                {
                    filterContext.Controller.ViewData.ModelState.AddModelError("", "Looks like you accidentally tried to double post.");
                    return;
                }

                HttpContext.Current.Session["LastProcessedToken"] = currentToken;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using SimpleHttpServer.Enums;
using SimpleHttpServer.Models;
using SimpleMVC.App.Extensions;
using SimpleMVC.App.MVC.Attributes.Methods;
using SimpleMVC.App.MVC.Controllers;
using SimpleMVC.App.MVC.Interfaces;

namespace SimpleMVC.App.MVC.Routes
{
    public class ControllerRouter : IHandleable
    {
        public ControllerRouter()
        {
            this.getParams = new Dictionary<string, string>();
            this.postParams = new Dictionary<string, string>();
        }

        private Dictionary<string, string> getParams;
        private Dictionary<string, string> postParams;
        private string requestMethod;
        private string controllerName;
        private string actionName;
        private object[] methodParams;

        private string[] controllerActionParams;
        private string[] controllerAction;

        public HttpResponse Handle(HttpRequest request)
        {
            this.ParseInput(request);

            var method = this.GetMethod();
            var controller = this.GetController();
            IInvocable result =
                (IInvocable)method
                .Invoke(controller, this.methodParams);

            string content = result.Invoke();
            var response = new HttpResponse()
            {
                ContentAsUTF8 = content,
                StatusCode = ResponseStatusCode.OK
            };

            this.ClearRequestParameters();
            return response;

        }
        private void ClearRequestParameters()
        {
            this.postParams = new Dictionary<string, string>();
            this.getParams = new Dictionary<string, string>();
        }

        private void InitRequestMethod(HttpRequest request)
        {
            this.requestMethod = request.Method.ToString();
        }

        private void InitControllerName()
        {
            this.controllerName = this.controllerAction[this.controllerAction.Length - 2].ToUpperFirst() + MvcContext.Current.ControllersSuffix;
        }

        private void InitActionName()
        {
            this.actionName = this.controllerAction[this.controllerAction.Length - 1].ToUpperFirst();
        }

        private void ParseInput(HttpRequest request)
        {
            string uri = WebUtility.UrlDecode(request.Url);
            string queryString = string.Empty;
            if (request.Url.Contains('?'))
            {
                queryString = request.Url.Split('?')[1];
            }

            this.controllerActionParams = uri.Split('?');
            this.controllerAction = controllerActionParams[0].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            this.controllerActionParams = queryString.Split('&');

            //GET Method parameters
            if (this.controllerActionParams.Length >= 1)
            {
                foreach (var pair in this.controllerActionParams)
                {
                    if (pair.Contains("="))
                    {
                        string[] keyValue = pair.Split('=');
                        this.getParams.Add(keyValue[0], keyValue[1]);
                    }
                }
            }

            //POST Method parameters
            string postParameters = request.Content;
            if (postParameters != null)
            {
                postParameters = WebUtility.UrlDecode(postParameters);
                string[] pairs = postParameters.Split('&');
                foreach (var pair in pairs)
                {
                    string[] keyValue = pair.Split('=');
                    this.postParams.Add(keyValue[0], keyValue[1]);
                }
            }

            this.InitRequestMethod(request);
            this.InitControllerName();
            this.InitActionName();

            MethodInfo method = this.GetMethod();

            if (method == null)
            {
                throw new NotSupportedException("No such method");
            }

            IEnumerable<ParameterInfo> parameters
                = method.GetParameters();

            this.methodParams
                = new object[parameters.Count()];


            int index = 0;
            foreach (ParameterInfo param in parameters)
            {
                if (param.ParameterType.IsPrimitive)
                {
                    object value = this.getParams[param.Name];
                    this.methodParams[index] = Convert.ChangeType(
                        value,
                        param.ParameterType
                        );
                    index++;
                }
                else
                {
                    Type bindingModelType = param.ParameterType;
                    object bindingModel =
                        Activator.CreateInstance(bindingModelType);

                    IEnumerable<PropertyInfo> properties
                        = bindingModelType.GetProperties();

                    foreach (PropertyInfo property in properties)
                    {
                        property.SetValue(
                            bindingModel,
                            Convert.ChangeType(
                                postParams[property.Name],
                                property.PropertyType
                                )
                            );
                    }

                    this.methodParams[index] = Convert.ChangeType(
                        bindingModel,
                        bindingModelType
                        );
                    index++;
                }
            }
        }

        private IEnumerable<MethodInfo> GetSuitableMethods()
        {
            return this.GetController()
                .GetType()
                .GetMethods()
                .Where(m => m.Name == this.actionName);
        }

        private Controller GetController()
        {
            var controllerType =
                $"{MvcContext.Current.AssemblyName}.{MvcContext.Current.ControllersFolder}.{this.controllerName}";

            var controller =
                (Controller)Activator.CreateInstance(Type.GetType(controllerType));
            return controller;
        }

        private MethodInfo GetMethod()
        {
            MethodInfo method = null;
            foreach (MethodInfo methodInfo in this.GetSuitableMethods())
            {
                IEnumerable<Attribute> attributes = methodInfo
                    .GetCustomAttributes()
                    .Where(a => a is HttpMethodAttribute);

                if (!attributes.Any())
                {
                    return methodInfo;
                }

                foreach (HttpMethodAttribute attribute in attributes)
                {
                    if (attribute.IsValid(this.requestMethod))
                    {
                        return methodInfo;
                    }
                }
            }

            return method;
        }
    }
}

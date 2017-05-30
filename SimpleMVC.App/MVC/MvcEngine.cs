
using System;
using System.Reflection;
using SimpleHttpServer.Models;

namespace SimpleMVC.App.MVC
{
    public class MvcEngine
    {
        public static void Run(HttpServer server)
        {
            RegisterAssemblyName();
            RegisterControllers();
            RegisterViews();
            RegisterModels();

            try
            {
                server.Listen();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void RegisterModels()
        {
            MvcContext.Current.ModelsFolder = "Models";
        }

        private static void RegisterViews()
        {
            MvcContext.Current.ViewsFolder = "Views";
        }

        private static void RegisterControllers()
        {
            MvcContext.Current.ControllersSuffix = "Controller";
            MvcContext.Current.ControllersFolder = "Controllers";
        }

        private static void RegisterAssemblyName()
        {
            MvcContext.Current.AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        }
    }
}

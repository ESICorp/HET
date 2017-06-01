using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using Het.Common;

namespace Het.Backend
{
    public sealed class AssemblyHelper 
    {
        public static Application[] GetApplications(string path)
        {
            var library = Path.Combine(path, "library");
            var repository = Path.Combine(path, "repository");

            Application[] applications = null;

            try
            {
                var directories = new List<string>(Directory.GetDirectories(repository));

                if (Directory.GetFiles(repository, "*.dll").Length > 0)
                {
                    directories.Add(repository);
                }

                applications = new Application[directories.Count];

                for (int i=0; i<applications.Length; i++)
                {
                    var name = new DirectoryInfo(directories[i]).Name;

                    var setup = new AppDomainSetup(); 
                    setup.ApplicationName = name;
                    setup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                    setup.PrivateBinPath = directories[i];

                    var configFile = Path.Combine(directories[i], name + ".config");
                    if (File.Exists(configFile))
                    {
                        setup.ConfigurationFile = configFile;
                    }

                    var appDomain = AppDomain.CreateDomain(name, AppDomain.CurrentDomain.Evidence, setup);

                    applications[i] = appDomain.CreateInstanceAndUnwrap(
                        typeof(Application).Assembly.FullName, typeof(Application).FullName) as Application;

                    applications[i].Init(library, directories[i]);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Couldn't find directory on {0}: {1}", repository, e.Message);
            }

            return applications;
        }

        private static List<Assembly> GetAssemblies(string path)
        {
            var assemblies = new List<Assembly>();
            string[] files = new string[0];

            try
            {
                files = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly);
            }
            catch (Exception e)
            {
                Trace.TraceError("Couldn't find dll on {0}: {1}", path, e.Message);
            }

            foreach (var file in files)
            {
                try
                {
                    assemblies.Add(Assembly.LoadFile(file));
                }
                catch (Exception e)
                {
                    Trace.TraceError("Couldn't load {0}: {1}", file, e.Message);
                }
            }
            
            return assemblies;
        }

        private static List<Type> GetComponents<T>(List<Assembly> list) where T : Attribute
        {
            var types = new List<Type>();

            foreach (var assembly in list)
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.GetCustomAttribute<T>() != null)
                        {
                            types.Add(type);
                        }
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError("Couldn't get types from {0}: {1}", assembly.FullName, e.Message);
                }
            }

            return types;
        }

        public static object[] GetComponents<T>(object[] list) where T : Attribute
        {
            var objects = new List<object>();

            foreach (var @object in list)
            {
                try
                {
                    var type = @object.GetType();

                    if (type.GetCustomAttribute<T>() == null)
                    {
                        foreach (var methodInfo in type.GetMethods())
                        {
                            if (methodInfo.GetCustomAttribute<T>() != null )
                            {
                                objects.Add(@object);

                                break;
                            }
                        }
                    }
                    else
                    {
                        objects.Add(@object);
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError("Couldn't get object from {0}: {1}", @object.GetType().FullName, e.Message);
                }
            }

            return objects.ToArray();
        }

        public static object[] GetInstances<T>(string path) where T : Attribute
        {
            var assemblies = GetAssemblies(path);
            var types = GetComponents<T>(assemblies);

            var objects = new object[types.Count];

            for (int i=0; i < types.Count; i++)
            {
                try
                {
                    objects[i] = Activator.CreateInstance(types[i]);

                    Trace.TraceInformation("Create instance {0}", types[i].FullName);

                }
                catch (Exception e)
                {
                    Trace.TraceError("Couldn't create instance {0}: {1}", types[i].FullName, e.Message);
                }
            }

            return objects;
        }

        public static void Wire(object[] list)
        {
            foreach (var @object in list)
            {
                foreach (var propertyInfo in @object.GetType().GetProperties())
                {
                    if (propertyInfo.GetCustomAttribute<AutowiredAttribute>() != null )
                    {
                        foreach (var @value in list)
                        {
                            if (propertyInfo.PropertyType.IsAssignableFrom(@value.GetType()))
                            {
                                try
                                {
                                    propertyInfo.SetValue(@object, @value);

                                    break;
                                }
                                catch (Exception e)
                                {
                                    Trace.TraceError("Couldn't wire {0}.{1}: {2}", @object.GetType().FullName, propertyInfo.Name, e.Message);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void WireInternal(object[] list, Application application)
        {
            foreach (var @object in list)
            {
                foreach (var propertyInfo in @object.GetType().GetProperties())
                {
                    if (propertyInfo.GetCustomAttribute<AutowiredAttribute>() != null)
                    {
                        if (propertyInfo.PropertyType.IsAssignableFrom(typeof(Application)))
                        {
                            try
                            {
                                propertyInfo.SetValue(@object, application);

                                break;
                            }
                            catch (Exception e)
                            {
                                Trace.TraceError("Couldn't wire {0}.{1}: {2}", @object.GetType().FullName, propertyInfo.Name, e.Message);
                            }
                        }
                    }
                }
            }
        }

        public static void InvokePostConstruct(object[] list)
        {
            foreach (var @object in list)
            {
                foreach (var methodInfo in @object.GetType().GetMethods())
                {
                    if (methodInfo.GetCustomAttribute<PostConstructAttribute>() != null)
                    {
                        try
                        {
                            methodInfo.Invoke(@object, null);
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError("Couldn't invoke post construct {0}.{1}: {2}", methodInfo.GetType().FullName, methodInfo.Name, e.Message);
                        }
                    }
                }
            }
        }
        public static Tuple<MethodInfo, T> GetMethodInfoAttribute<T>(object @object) where T : Attribute
        {
            foreach (var methodInfo in @object.GetType().GetMethods())
            {
                var attribute = methodInfo.GetCustomAttribute<T>();

                if ( attribute != null ) 
                {
                    return new Tuple<MethodInfo, T>(methodInfo, attribute);
                }
            }
            return null;
        }
    }
}

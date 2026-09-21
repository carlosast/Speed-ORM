namespace Speed.Data.Util
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    public static class AppAssembly
    {
        static Assembly _explicit;

        public static void Set(Assembly assembly)
        {
            if (IsUseful(assembly))
                _explicit = assembly;
        }

        public static string GetFullName()
        {
            var a = Get();
            return a != null ? a.FullName : null;
        }

        public static Assembly Get()
        {
            if (IsUseful(_explicit))
                return _explicit;

            var entry = Assembly.GetEntryAssembly();
            if (IsUseful(entry) && !IsFrameworkOrThisLib(entry))
                return entry;

            var fromHost = TryGetFromWcfHost();
            if (IsUseful(fromHost) && !IsFrameworkOrThisLib(fromHost))
                return fromHost;

            return FindNewestModifiedAssembly();
        }

        static Assembly FindNewestModifiedAssembly()
        {
            var candidates = FindAppAssemblies();
            Assembly best = null;
            var bestTime = DateTime.MinValue;

            for (var i = 0; i < candidates.Count; i++)
            {
                var a = candidates[i];
                var written = GetWriteTime(a);
                if (best == null || written > bestTime)
                {
                    best = a;
                    bestTime = written;
                }
            }

            return IsUseful(best) ? best : Assembly.GetExecutingAssembly();
        }

        static DateTime GetWriteTime(Assembly a)
        {
            try
            {
                var path = GetOriginalPath(a);
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    return DateTime.MinValue;

                return File.GetLastWriteTimeUtc(path);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        static List<Assembly> FindAppAssemblies()
        {
            var result = new List<Assembly>();
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (var i = 0; i < assemblies.Length; i++)
            {
                var a = assemblies[i];
                if (!IsUseful(a) || IsFrameworkOrThisLib(a))
                    continue;

                var path = GetOriginalPath(a);
                if (path.Length == 0 || IsTempAspNetPath(path))
                    continue;

                if (path.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase))
                    result.Add(a);
            }

            return result;
        }

        static string GetOriginalPath(Assembly a)
        {
            try
            {
                if (a == null || a.IsDynamic)
                    return "";

                var codeBase = a.CodeBase;
                if (!string.IsNullOrEmpty(codeBase) &&
                    codeBase.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
                {
                    var uri = new Uri(codeBase);
                    if (uri.IsFile && !IsTempAspNetPath(uri.LocalPath))
                        return uri.LocalPath;
                }
            }
            catch
            {
            }

            var location = SafeLocation(a);
            if (IsTempAspNetPath(location))
                return "";

            return location;
        }

        static bool IsTempAspNetPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            return path.IndexOf("Temporary ASP.NET Files", StringComparison.OrdinalIgnoreCase) >= 0
                || path.IndexOf("Temp ASP.NET Files", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        static Assembly TryGetFromWcfHost()
        {
            try
            {
                var ocType = FindType(
                    "System.ServiceModel.OperationContext",
                    "CoreWCF.OperationContext");
                if (ocType == null)
                    return null;

                var currentProp = ocType.GetProperty("Current", BindingFlags.Public | BindingFlags.Static);
                if (currentProp == null)
                    return null;

                var current = currentProp.GetValue(null, null);
                if (current == null)
                    return null;

                var host = GetProp(current.GetType(), current, "Host");
                if (host == null)
                    return null;

                var description = GetProp(host.GetType(), host, "Description");
                if (description == null)
                    return null;

                var serviceType = GetProp(description.GetType(), description, "ServiceType") as Type;
                return serviceType != null ? serviceType.Assembly : null;
            }
            catch
            {
                return null;
            }
        }

        static object GetProp(Type type, object target, string name)
        {
            var p = type.GetProperty(name);
            return p != null ? p.GetValue(target, null) : null;
        }

        static Type FindType(params string[] fullNames)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (var i = 0; i < assemblies.Length; i++)
            {
                var asm = assemblies[i];
                if (asm.IsDynamic)
                    continue;

                for (var j = 0; j < fullNames.Length; j++)
                {
                    var t = asm.GetType(fullNames[j], false, false);
                    if (t != null)
                        return t;
                }
            }
            return null;
        }

        static string SafeLocation(Assembly a)
        {
            try
            {
                if (a == null || a.IsDynamic)
                    return "";

                return a.Location ?? "";
            }
            catch
            {
                return "";
            }
        }

        static bool IsUseful(Assembly a)
        {
            if (a == null || a.IsDynamic)
                return false;

            return !string.IsNullOrEmpty(a.FullName);
        }

        static bool IsFrameworkOrThisLib(Assembly a)
        {
            if (a == null || a.IsDynamic)
                return true;

            if (a == typeof(AppAssembly).Assembly)
                return true;

            var n = a.GetName().Name ?? "";

#if NETFRAMEWORK
            if (a.GlobalAssemblyCache)
                return true;
#endif

            return n.StartsWith("System", StringComparison.OrdinalIgnoreCase)
                || n.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase)
                || n.StartsWith("App_", StringComparison.OrdinalIgnoreCase)
                || string.Equals(n, "mscorlib", StringComparison.OrdinalIgnoreCase)
                || string.Equals(n, "netstandard", StringComparison.OrdinalIgnoreCase)
                || n.StartsWith("CoreWCF", StringComparison.OrdinalIgnoreCase)
                || n.StartsWith("SMDiagnostics", StringComparison.OrdinalIgnoreCase);
        }
    }
}

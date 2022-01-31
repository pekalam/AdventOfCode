using System;
using System.Reflection;

static class ProjectRunner
{
    public static Action GetMain(string projectName, string dll)
    {
        var assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(dll));
        var ep = assembly.EntryPoint;
        var r = ep.ReturnType;

        // for fsharp
        if (!projectName.Contains("Cs"))
        {
            var main = assembly.GetTypes()[0].GetMethod("main");
            var del = (Func<string[], int>)Delegate.CreateDelegate(typeof(Func<string[], int>), main, true);
            return () => del(Array.Empty<string>());
        }


        var delegateType = TryMatchDelegate(ep);

        if (delegateType == typeof(Action))
        {
            var del = (Action)Delegate.CreateDelegate(delegateType, null, ep, true);
            return () => del();
        }
        else if (delegateType == typeof(Action<string[]>))
        {
            var del = (Action<string[]>)Delegate.CreateDelegate(delegateType, ep, true);
            return () => del(Array.Empty<string>());
        }

        return () => throw new NotImplementedException();
    }

    private static Type TryMatchDelegate(MethodInfo methodInfo)
    {
        var returnType = methodInfo.ReturnType;
        var args = methodInfo.GetParameters();

        return (returnType.Name, args.Length) switch
        {
            ("Void", 0) => typeof(Action),
            ("Void", 1) => typeof(Action<string[]>),
            ("Int32", 1) => typeof(Func<string[], int>),
        };
    }
}

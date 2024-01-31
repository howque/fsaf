So there's really aint no way to do this?  Why did you told me it would have worked in the beginning then? You probably were confusing because you have though that the code was in a seperate file than the game executable Like

Superfighters Deluxe.exe

and 

Superfighters Deluxe.dll

Correct?

Heres the code

using System;
using System.Linq;
using System.Reflection;
using dnlib.DotNet;
using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using dnlib.DotNet.Emit;
using System.Reflection.Emit;


public class Program
{
    private static Harmony _harmony;

    public static void Main(string[] args)
    {
        string processName = "Superfighters Deluxe";

        while (true)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. List classes in namespace 'SFD'");
            Console.WriteLine("2. List methods in class 'SFD.Player'");
            Console.WriteLine("3. Read code inside 'SFD.Player.JumpValue' method");
            Console.WriteLine("4. Inject code into 'SFD.Player.JumpValue' method");
            Console.WriteLine("5. Exit");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    ListClassesInNamespace("SFD", processName);
                    break;
                case "2":
                    ListMethodsInClass("SFD.Player", processName);
                    break;
                case "3":
                    ReadMethodCode("JumpValue", "SFD.Player", processName);
                    break;
                case "4":
                    InjectILInstructions("JumpValue", processName);
                    break;
                case "5":
                    return; // Exit the program
                default:
                    Console.WriteLine("Invalid option. Please enter '1', '2', '3', '4', or '5'.");
                    break;
            }

            // Prompt to go back to the option menu
            Console.WriteLine("Press Enter to go back to the option menu...");
            Console.ReadLine();
            Console.Clear(); // Clear the console for the next iteration
        }
    }

    public static void ListClassesInNamespace(string namespaceName, string processName)
    {
        try
        {
            var process = System.Diagnostics.Process.GetProcessesByName(processName).FirstOrDefault();
            if (process != null)
            {
                using (var module = ModuleDefMD.Load(process.MainModule.FileName))
                {
                    var types = module.Types
                        .Where(t => t.Namespace == namespaceName)
                        .ToList();

                    if (types.Count > 0)
                    {
                        Console.WriteLine($"Classes in namespace '{namespaceName}':");
                        foreach (var type in types)
                        {
                            Console.WriteLine($"  {type.Name}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No classes found in namespace '{namespaceName}'.");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Process {processName} not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public static void ListMethodsInClass(string className, string processName)
    {
        try
        {
            var process = System.Diagnostics.Process.GetProcessesByName(processName).FirstOrDefault();
            if (process != null)
            {
                using (var module = ModuleDefMD.Load(process.MainModule.FileName))
                {
                    var type = module.Types.FirstOrDefault(t => t.FullName == className);
                    if (type != null)
                    {
                        var methods = type.Methods.ToList();
                        if (methods.Count > 0)
                        {
                            Console.WriteLine($"Methods in class '{className}':");
                            foreach (var method in methods)
                            {
                                Console.WriteLine($"  {method.Name}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"No methods found in class '{className}'.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Class '{className}' not found.");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Process {processName} not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public static void ReadMethodCode(string methodName, string className, string processName)
    {
        try
        {
            var process = System.Diagnostics.Process.GetProcessesByName(processName).FirstOrDefault();
            if (process != null)
            {
                using (var module = ModuleDefMD.Load(process.MainModule.FileName))
                {
                    var method = FindMethod(module, className, methodName);
                    if (method != null)
                    {
                        Console.WriteLine($"Code in method '{methodName}' of class '{className}':");
                        foreach (var instruction in method.Body.Instructions)
                        {
                            Console.WriteLine($"{instruction}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"Process {processName} not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public static void InjectILInstructions(string methodName, string processName)
    {
        try
        {
            _harmony = new Harmony("YourUniqueIdentifier");

            // Get the running process
            var processes = System.Diagnostics.Process.GetProcessesByName(processName);
            if (processes.Length == 0)
            {
                Console.WriteLine($"Process {processName} not found.");
                return;
            }

            var process = processes[0];

            // Get the assembly of the target process
            Assembly assembly = Assembly.LoadFrom(process.MainModule.FileName);

            // Get the type and method using reflection
            Type targetType = assembly.GetType("SFD.Player");
            MethodInfo targetMethod = targetType.GetMethod(methodName);

            // Apply the patch using Harmony
            _harmony.Patch(targetMethod, new HarmonyMethod(typeof(JumpValuePatch).GetMethod("Prefix")));

            Console.WriteLine($"IL instructions injected into method '{methodName}'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    [HarmonyPatch(typeof(SFD.Player))]
    [HarmonyPatch("JumpValue")]
    public static class JumpValuePatch
    {
        public static bool Prefix(SFD.Player __instance, ref float __result)
        {
            // Your injection logic here
            Console.WriteLine("Code injected into JumpValue method.");

            // Call the original JumpValue logic
            return true;
        }
    }

    // Update the FindMethod method to consider only the specified class and method name
    private static MethodDef FindMethod(ModuleDef module, string className, string methodName)
    {
        var type = module.Types.FirstOrDefault(t => t.FullName == className);
        if (type != null)
        {
            var method = type.Methods.FirstOrDefault(m => m.Name == methodName);
            if (method != null)
            {
                return method;
            }
            else
            {
                Console.WriteLine($"Method '{methodName}' not found in class '{className}'.");
            }
        }
        else
        {
            Console.WriteLine($"Class '{className}' not found.");
        }

        return null;
    }
}

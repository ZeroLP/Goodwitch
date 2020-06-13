using System;
using System.Reflection;

namespace GWTest
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Goodwitch.Main.Test();

            /*Assembly ASM = Assembly.Load(System.IO.File.ReadAllBytes(@"C:\Users\stubl\Desktop\Project Goodwitch\Goodwitch\Goodwitch\bin\Debug\Goodwitch.dll"));

            Type StubType = ASM.GetType("Goodwitch.Main"); //Gets the class
            MethodInfo StubMethod = StubType.GetMethod("Test"); //Gets the method
            object StubClassInstance = Activator.CreateInstance(StubType); //Create an instance to the class

            StubMethod.Invoke(StubClassInstance, null);*/

            Console.Read();
        }
    }
}

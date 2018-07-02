using System;
using System.Collections.Generic;
using System.Text;
using TG.Common;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            MyClass c = new MyClass();
            c.Collect.Add("Hello World");
            c.Dict.Add("Hello", "World");
            c.MyProperty = 3443;

            MyClass c2 = Miscellaneous.CloneObject<MyClass>(c);
            
            //LogManager.WriteToLog("Test");
        }

        static void Test(MyClass mc)
        {

        }
    }

    public class MyClass
    {
        public int MyProperty { get; set; }

        public List<string> Collect { get;  } = new List<string>();

        public Dictionary<string, string> Dict { get; } = new Dictionary<string, string>();
    }
}

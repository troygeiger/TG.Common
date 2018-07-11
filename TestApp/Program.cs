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
            c.Collect.Add(new ChildClass() { Z = 55 });
            c.Dict.Add("Hello", "World");
            c.MyProperty = 3443;

            MyClass c2 = Miscellaneous.CloneObject<MyClass>(c, false);
            c2.Collect[0].Z = 99;
            //LogManager.WriteToLog("Test");
        }

        static void Test(MyClass mc)
        {

        }
    }

    public class MyClass
    {
        public int MyProperty { get; set; }

        public List<ChildClass> Collect { get;  } = new List<ChildClass>();

        public Dictionary<string, string> Dict { get; } = new Dictionary<string, string>();
    }

    public class ChildClass
    {
        public int Z { get; set; }
    }
}

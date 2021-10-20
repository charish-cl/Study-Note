using System;

namespace ConsoleApp2
{
    class Program
    {
        [Flags]
        enum MyEnum
        {
             a=1
             ,b =2     
             ,c=3
        }
        static void Main(string[] args)
        {
            MyEnum myEnuma = MyEnum.a;
            MyEnum myEnumb = MyEnum.b;
            if ((myEnuma & MyEnum.b)== MyEnum.b)
            {
                Console.WriteLine("存在");
            }

            myEnuma |= myEnumb;
             myEnuma ^= myEnumb;
            if ((myEnuma & MyEnum.b)== MyEnum.b)
            {
                Console.WriteLine("存在");
            }

            
        }
    }
}
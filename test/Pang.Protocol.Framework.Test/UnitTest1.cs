using System;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Pang.Protocol.Framework.Test
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void AssTest()
        {
            var t = new Test(1);
            var types = t
                .GetType().GetInterfaces();

            foreach (var type in types)
            {
                if(type.GetGenericTypeDefinition() == typeof(ITest1<>))
                    _testOutputHelper.WriteLine(type.Name);
            }

            //_testOutputHelper.WriteLine(typeof(ITest2<>).IsAssignableFrom(typeof(Test)).ToString());
            //_testOutputHelper.WriteLine(typeof(ITest1<>).IsSubclassOf(typeof(ITest2<>)).ToString());
        }
    }


    interface ITest1<T>
    {
        T Add();
    }

    interface ITest2<T>: ITest1<T>
    {
        

    }

    class Test: ITest2<int>
    {
        private int i;
        public Test(int c)
        {
            i = c;
        }

        public int Add()
        {
            return 1;
        }
    }
}
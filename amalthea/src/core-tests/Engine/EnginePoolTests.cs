using SoundByte.Core.Engine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SoundByte.Core.Tests.Engine
{
    public class EnginePoolTests
    {
        [Fact]
        public void TestCreateDefault()
        {
            var pool = new EnginePool();
            //pool.Dispose();
        }

        [Fact]
        public void TestCreateCustom()
        {
            var pool = new EnginePool(new EnginePool.EnginePoolConfig(e =>
            {
                e.SetValue("a", 5);
                e.SetValue("sleep", new Action<int>(x => Thread.Sleep(x)));
            }));

            for (var i = 0; i < 100; i++)
            {
                using var engine = pool.GetEngine();
                var result = engine.Execute<double>("a;");
                Assert.Equal(5, result);
            }

            Parallel.For(0, 100, i =>
            {
                using var engine = pool.GetEngine();
                var result = engine.Execute<double>("a;");
                engine.CallFunction("sleep", 50);
                Assert.Equal(5, result);
            });
        }

        [Fact]
        public void TestFunctionCalling()
        {
            var pool = new EnginePool(new EnginePool.EnginePoolConfig(e =>
            {
                e.SetValue("print", new Action<string>(s => Console.WriteLine(s)));
                e.SetValue("add", new Func<double, double, double>((a, b) => a + b));
            }));

            using var engine = pool.GetEngine();

            engine.CallFunction("print");
            Assert.Equal(15, engine.CallFunction<double>("add", 5, 10));
        }

        [Fact]
        public void TestExecute()
        {
            var pool = new EnginePool(new EnginePool.EnginePoolConfig(e =>
            {
                e.SetValue("print", new Action<string>(s => Console.WriteLine(s)));
                e.SetValue("add", new Func<double, double, double>((a, b) => a + b));
            }));

            using var engine = pool.GetEngine();

            engine.Execute("print();");
            Assert.Equal(15, engine.Execute<double>("add(10, 5);"));
        }

        [Fact]
        public void TestEngineCount()
        {
            var pool = new EnginePool();
            Assert.Equal(5, pool.EngineCount);

            var engine1 = pool.GetEngine();
            Assert.Equal(4, pool.EngineCount);
            engine1.Dispose();
            Assert.Equal(5, pool.EngineCount);

            var engines = new List<EnginePool.JSEngine>();
            for (var i = 0; i <= 5; i++)
            {
                engines.Add(pool.GetEngine());
            }

            Assert.Equal(0, pool.EngineCount);

            foreach (var e in engines)
            {
                e.Dispose();
            }

            Assert.Equal(5, pool.EngineCount);
        }
    }
}
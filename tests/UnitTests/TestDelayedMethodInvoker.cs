using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using TG.Common;

namespace UnitTests
{
    public class TestDelayedMethodInvoker
    {
        Stopwatch invocationTimer = new Stopwatch();
        bool methodInvoked = false;

        [SetUp]
        public void Setup()
        {
            methodInvoked = false;
        }

        [Test]
        public void TestExecutionAndTiming()
        {
            var invoker = new DelayedMethodInvoker(new Action(MethodToInvoke), 200);
            invocationTimer.Restart();
            invoker.Invoke();

            Thread.Sleep(250);

            
        }

        [Test]
        public void TestExecutionWithParameters()
        {
            var invoker = new DelayedMethodInvoker(new Action<int>(MethodWithParams), 100);
            invocationTimer.Restart();
            invoker.Invoke(72);

            Thread.Sleep(200);
        }

        [TearDown]
        public void TearDownMethodInvokedTrue()
        {
            Assert.True(methodInvoked);
            Console.WriteLine($"Executed in {invocationTimer.ElapsedMilliseconds}ms");
        }

        private void MethodToInvoke()
        {
            invocationTimer.Stop();
            methodInvoked = true;
            Assert.GreaterOrEqual(invocationTimer.ElapsedMilliseconds, 200);
        }

        private void MethodWithParams(int value)
        {
            invocationTimer.Stop();
            methodInvoked = true;
            Assert.AreEqual(72, value);
            Assert.GreaterOrEqual(invocationTimer.ElapsedMilliseconds, 100);
        }
    }
}
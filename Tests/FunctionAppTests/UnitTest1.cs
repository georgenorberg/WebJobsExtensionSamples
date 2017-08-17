using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FunctionApp;
using SampleExtension;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Collections.Generic;

namespace FunctionAppTests
{
    [TestClass]
    public class UnitTest1
    {
        // Demonstrate a unit test for our writer function
        [TestMethod]
        public void TestMethod1()
        {
            var testCollector = new Collector<SampleItem>();
            var traceWriter = new TestTraceWriter(TraceLevel.Info);
            WriterFunction.Run(null, testCollector, traceWriter);

            Assert.AreEqual(1, testCollector._list.Count);
            Assert.AreEqual("tom", testCollector._list[0].Name);
            Assert.AreEqual("hello", testCollector._list[0].Contents);
        }
    }

    public class Collector<T> : ICollector<T>
    {
        public List<T> _list = new List<T>();

        public void Add(T item)
        {
            _list.Add(item);
        }
    }

    public class TestTraceWriter : TraceWriter
    {
        public Collection<TraceEvent> Traces = new Collection<TraceEvent>();
        private object _syncLock = new object();

        public TestTraceWriter(TraceLevel level) : base(level)
        {
        }

        public override void Trace(TraceEvent traceEvent)
        {
            lock (_syncLock)
            {
                Traces.Add(traceEvent);
            }
        }
    }
}

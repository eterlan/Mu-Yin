using NUnit.Framework;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;

namespace MuYin.Tests
{
    public class TestNewArrayInJob :ECSTestsFixture
    {
        [Test]
        public void New_Array_In_Job_Test()
        {
            var handle = new NewArrayInJob().Schedule();
            handle.Complete();
            m_Manager.CompleteAllJobs();
        }
        public struct NewArrayInJob : IJob
        {
            public void Execute()
            {
                var array = new NativeArray<int>(1, Allocator.Temp);
                array.Dispose();
            }

        }
    }
}


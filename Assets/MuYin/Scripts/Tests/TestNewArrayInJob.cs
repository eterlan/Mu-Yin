using NUnit.Framework;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;

namespace MuYin.Tests
{
    public class TestNewArrayInJob :ECSTestsFixture
    {
        [Test]
        public void _0_New_NativeArray_In_Job_Test()
        {
            var handle = new NewArrayInJob().Schedule();
            handle.Complete();
            m_Manager.CompleteAllJobs();
        }

        [Test]
        public void _1_Array_Pass_To_Job_Test()
        {
            var handle = new PassToArray{array = new []{1,2}}.Schedule();
            handle.Complete();
            m_Manager.CompleteAllJobs();
        }

        [Test]
        public void _2_New_Struct_In_Job()
        {
            var handle = new NewStructInArrayJob().Schedule();
            handle.Complete();
            m_Manager.CompleteAllJobs();
        }

        
        public struct NewArrayInJob : IJob
        {
            public void Execute()
            {
                var nativeArray = new NativeArray<int>(1, Allocator.Temp);
                var array = new int[1,2,3];
                nativeArray.Dispose();
            }
        }

        public struct PassToArray : IJob
        {
            public int[] array;
            public void Execute()
            {

            }
        }

        public struct NewStructInArrayJob : IJob
        {
            public void Execute()
            {
                new aaa();
            }
        }
        private struct aaa
        {
            
        }
    }
}


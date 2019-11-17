using NUnit.Framework;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using System;
using System.Collections;
using Unity.Jobs;
using Unity.Burst;

namespace MuYin.Tests
{
    public class TestIndexerInComponent :ECSTestsFixture
    {
        [Test]
        public void _0_Test_Indexer()
        {
            var e           = m_Manager.CreateEntity(typeof(EcsTestData));
            m_Manager.AddComponentData(e, new Indexer{a = 2, b = 4});
            var target = m_Manager.GetComponentData<Indexer>(e)[0];
            Assert.AreEqual(2, target);
        }

        [Test]
        public void _1_Iterate_Indexer()
        {
            var e           = m_Manager.CreateEntity(typeof(EcsTestData));
            m_Manager.AddComponentData(e, new Indexer{a = 2, b = 4});
            var indexerComponent = m_Manager.GetComponentData<Indexer>(e);
            var target = -1;
            for (int i = 0; i < 2; i++)
            {
                if (i != 1) continue;
                target = indexerComponent[i];
            }
            Assert.AreEqual(4, target);
        }

        [Test]
        public void _2_Iterate_Indexer_In_Job()
        {
            var e           = m_Manager.CreateEntity(typeof(EcsTestData));
            var array = new NativeArray<int>(1, Allocator.TempJob);
            m_Manager.AddComponentData(e, new Indexer{a = 2, b = 4});
            var target = -1;
            var handle = new IndexerInJob{array = array}.Schedule(EmptySystem);
            handle.Complete();
            m_Manager.CompleteAllJobs();
            target = array[0];
            array.Dispose();
            Assert.AreEqual(4, target);
        }

        // Bug Use return in method in for loop lead to jump out of loop;
        // #fix Use continue instead.
        [BurstCompile]
        public struct IndexerInJob : IJobForEach<Indexer>
        {
            public NativeArray<int> array;
            public void Execute(ref Indexer c0)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (i != 1) continue;
                    array[0] = c0[i];
                }
            }
        }

        public struct Indexer : IComponentData
        {
            public int this[int index]
            {
                get 
                {   
                    switch (index)
                    {
                        case 0 : return a;
                        case 1 : return b;
                        default : throw new IndexOutOfRangeException();
                    } 

                }
                set {  }
            }
            public int Count;
            public int a;
            public int b;
        }
    }
}


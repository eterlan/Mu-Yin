using NUnit.Framework;
using Unity.Collections;

namespace MuYin.Tests
{
    public class ToComponentDataCannotWriteBack :ECSTestsFixture
    {
        [Test]
        public void Test_To_Component_Data_Cannot_Write_Back()
        {
            var e           = m_Manager.CreateEntity(typeof(EcsTestData));
            var filter      = m_Manager.CreateEntityQuery(typeof(EcsTestData));
            var array       = filter.ToComponentDataArray<EcsTestData>(Allocator.TempJob);
            var ecsTestData =array[0];
            ecsTestData.value = 1;
            array.Dispose();
            Assert.AreEqual(0, m_Manager.GetComponentData<EcsTestData>(e).value);
        }
    }
}


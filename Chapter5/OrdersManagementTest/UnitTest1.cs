using OrdersManagement;

namespace OrdersManagementTest
{
    // 测试OrderDetail
    [TestClass]
    public class UnitTestOrderDetail
    {
        internal static OrderDetail
            det1 = new OrderDetail("Tony"),
            det2 = new OrderDetail("Tony", "Axis"),
            det3 = new OrderDetail("Tony", "Axis", 1),
            det4 = new OrderDetail("Tony", "Axis", 1, 2); // 测试用实例，与OrderTest共享

        /*[ClassInitialize]
        public void Construct()
        {
            det1 = new OrderDetail("a");
            det2 = new OrderDetail("a");
            det3 = new OrderDetail("a", "b", 1);
        }*/

        [TestCleanup]
        public void Restore()
        {
            det1 = new OrderDetail("Tony");
            det2 = new OrderDetail("Tony", "Axis");
            det3 = new OrderDetail("Tony", "Axis", 1);
            det4 = new OrderDetail("Tony", "Axis", 1, 2);
        }

        // 测试继承重载方法
        [TestMethod]
        public void OpTest() // 测试相等判断
        {
            Assert.IsTrue(det3 == det4);
            Assert.IsTrue(det1 != det2);
            Assert.IsFalse(det2 == det3);
        }
        [TestMethod]
        public void EqualsTest()
        {
            Assert.AreEqual(det3, det4);
            Assert.AreNotEqual(det1, det2);
            Assert.AreNotEqual(det2, det3);
        }
        [TestMethod]
        public void TestToString()
        {
            string tmp = "item: Tony / client: Axis / price: 1 / cnt: 2";
            Assert.AreEqual(tmp, det4.ToString());
        }

    }
    [TestClass]
    public class UnitTestOrder
    {
        internal static Order
            ord1 = new Order(1),
            ord2 = new Order(1, new List<OrderDetail>()),
            ord3 = new Order(2, new List<OrderDetail>(){
                UnitTestOrderDetail.det1,
                UnitTestOrderDetail.det2,
                UnitTestOrderDetail.det3 }); // 测试用实例，与OrderServiceTest共享

        /*[ClassInitialize]
        public void Construct()
        {
            ord1 = new Order();
            ord2 = new Order();
            ord3 = new Order();
        }*/

        [TestCleanup]
        public void Restore() // 恢复实例值
        {
            ord1 = new Order(1);
            ord2 = new Order(1, new List<OrderDetail>());
            ord3 = new Order(2, new List<OrderDetail>(){
                UnitTestOrderDetail.det1,
                UnitTestOrderDetail.det2,
                UnitTestOrderDetail.det3 });
            //Console.WriteLine(ord3.ToString());
        }

        [TestMethod]
        public void TestEquals()
        {
            Assert.AreEqual(ord1, ord1);
            Assert.AreEqual(ord1, ord2);
            Assert.AreNotEqual(ord1, ord3);
        }
        [TestMethod]
        public void TestInsert()
        {
            try
            {
                ord3.InsertDetail(new OrderDetail("Tom", "Apple", 2));
            }
            catch (InvalidOperationException) { Assert.Fail(); }

            Assert.AreEqual(ord3.totAmount, 3); // 测试总金额是否更新

            try
            {
                ord3.InsertDetail(UnitTestOrderDetail.det4); // 尝试插入一个重复元素
                Assert.Fail();
            }
            catch (InvalidOperationException) { } // Good exception
        }
        [TestMethod]
        public void TestRemove()
        {
            try
            {
                ord3.RemoveDetail(x => (x == UnitTestOrderDetail.det3));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Assert.Fail();
            }
            Assert.AreEqual(ord3.totAmount, 0); // 测试总金额是否更新

            try
            {
                ord3.RemoveDetail((x) => x == new OrderDetail("Tom", "Apple", 2));
                //Assert.Fail();
            }
            catch (ArgumentNullException) { /* Good Exception */ }
            //catch (InvalidOperationException) { Assert.Fail(); }
        }
        [TestMethod]
        public void TestModify()
        {
            OrderDetail tmp = UnitTestOrderDetail.det3;
            tmp.cnt++; // 假设增加一件商品
            try
            {
                ord3.ModifyDetail(det => det == UnitTestOrderDetail.det3, tmp);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            Assert.AreEqual(ord3.totAmount, 2); // 测试总金额是否更新

            // 根据客户和货物再次尝试检索并修改原条目
            try
            {
                ord3.ModifyDetail(det => det == UnitTestOrderDetail.det3, tmp);
            }
            catch (Exception) { Assert.Fail(); } // 没检索到说明有问题

            try
            {
                ord3.ModifyDetail(det => det == new OrderDetail("c", "d", 1), tmp);
                Assert.Fail();
            }
            catch (InvalidOperationException) { Assert.Fail(); }
            catch (ArgumentNullException) { /* Good Exception */ }
        }
    }
    [TestClass]
    public class UnitTestOrderService
    {
        static OrderService ordServe = new OrderService();

        [TestInitialize]
        public void Init() // 生成初始序列
        {
            for (int i = 0; i < 4; ++i)
            {
                Order tmp = new Order();
                for (int j = 'A'+i*3; j <= 'A'+i*3 + 2; ++j)
                {
                    tmp.InsertDetail(new OrderDetail(((char)j).ToString(), ((char)(j+1)).ToString(), i+1));
                }
                ordServe.AddOrder(OrderService.CreateNextOrder(tmp));
            }
            //Console.WriteLine(ordServe.ToString());
        }
        [TestCleanup]
        public void Clean()
        {
            ordServe.Clear();
        }

        [TestMethod]
        public void TestQuery() // 确认各个查询方法的工作情况
        {
            var query1 = ordServe.QueryByItem("A"); 
            Assert.AreEqual(query1.Count(), 1);

            var query2 = ordServe.QueryByClient("B");
            CollectionAssert.AreEqual(query1.ToArray(), query2.ToArray());

            var query3 = ordServe.QueryLowerAmount(3);
            CollectionAssert.AreEqual(query1.ToArray(), query3.ToArray());

            var query4 = ordServe.QueryHigherAmount(7);
            Assert.AreEqual(query4.Count(), 2);
        }
       [TestMethod]
        public void TestRemove()
        {
            try
            {
                ordServe.RemoveOrder(ord => ord.id == 1);
                ordServe.RemoveOrder_ByDetail(det => det.price == 2);
                ordServe.RemoveOrderDetail(ord => ord.id == 3, det => det.price == 3);
                ordServe.RemoveOrderDetail(ord => ord.id == 4, det => det.item == "J");
            }
            catch (Exception) { Assert.Fail(); }

            Assert.IsFalse(ordServe.Query(ord => ord.id == 1).Any());
            Assert.IsFalse(ordServe.Query(ord => ord.id == 2).Any());
            Assert.IsFalse(ordServe.Query(ord => ord.id == 3).First().detList.Any());
            Assert.AreEqual(2, ordServe.Query(ord => ord.id == 4).First().detList.Count());
        }
        [TestMethod]
        public void TestModify()
        {
            try
            {
                Order tmp = UnitTestOrder.ord3;
                ordServe.ModifyOrder(ord => ord.id == 1, tmp);
                ordServe.ModifyOrder_ByDetail(det => det.item == "D", tmp);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Assert.Fail();
            }

            try
            {
                ordServe.ModifyOrderDetail(
                    ord => ord.id == 1,
                    det => det == UnitTestOrderDetail.det3,
                    UnitTestOrderDetail.det4);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Assert.Fail();
            }

            try // 试图把某个订单项的所有明细改成同一个
            {
                ordServe.ModifyOrderDetail(
                    ord => ord.id == 3,
                    det => det.price == 3,
                    UnitTestOrderDetail.det4);

                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
                Assert.Fail();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message); // Good exception
            } 

            try // 试图检索不存在的订单
            {
                ordServe.ModifyOrderDetail(
                    ord => false,
                    det => true,
                    UnitTestOrderDetail.det4);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message); // Good exception
            } 
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e.Message);
                Assert.Fail();
            } 
        }
        [TestMethod]
        public void TestSort()
        {
            ordServe.Sort(); // id正序
            Console.WriteLine(ordServe);

            ordServe.Sort(new OrderComparer((a, b) => b.id - a.id)); // id倒序
            Console.WriteLine(ordServe);

            ordServe.Sort(new OrderComparer((a, b) => (int)(100 * (a.totAmount - b.totAmount)))); // 总金额正序，精确到小数点后两位（分）
            Console.WriteLine(ordServe);

        }
    }
}

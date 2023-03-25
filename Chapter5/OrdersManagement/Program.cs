using System;
using System.Collections;

namespace OrdersManagement
{
    public struct OrderDetail // 订单明细定义为结构（值类型），重载相等号与Equals意义相同
    {
        public string item;
        public string client;
        public double price; // 单价
        public int cnt;

        public OrderDetail(string item = "default", string client = "default", double price = 0, int cnt = 1)
        {
            this.item = item;
            this.client = client;
            this.price = price;
            this.cnt = cnt;
        }

        public static bool operator == (OrderDetail a, OrderDetail b) 
        {
            // 认定商品、客户和价格均相同的条目为同一明细条目，同价格购买多件商品不允许增加条目而以cnt记录
            return (a.item == b.item && a.client == b.client && a.price == b.price);
        }
        public static bool operator != (OrderDetail a, OrderDetail b)
        {
            return !(a == b);
        }
        public override bool Equals(Object? b)
        {
            return b is OrderDetail && (OrderDetail)b == this;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return $"item: {item} / client: {client} / price: {price} / cnt: {cnt}";
        }

    }
    public class Order // 订单类
    {
        public int id;
        public double totAmount
        {
            get
            {
                double ret = 0;
                foreach (var x in detList)
                {
                    ret += x.price * x.cnt;
                }
                return ret;
            }
        }
        public List<OrderDetail> detList; // 明细

        // 构造函数
        public Order(int id, List<OrderDetail> list)
        {
            this.id = id;
            detList = new List<OrderDetail>(list); // 深拷贝
        }
        public Order(int id) : this(id, new List<OrderDetail>()) { }
        public Order() : this(0) { }
        public Order(Order x) // 深拷贝
        {
            this.id = x.id;
            this.detList = new List<OrderDetail>(x.detList);
        }

        // 继承方法重载
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object? obj) // 认为id相等的为同一订单，避免序号冲突
        {
            return obj is Order && ((Order)obj).id == this.id;
        }
        public override string ToString() // 使用StirngBuilder提高循环构建效率
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(
                $"-------------------------\nOrder {id}:\n-------------------------\n");
            foreach (OrderDetail x in detList)
            {
                sb.Append(x.ToString());
                sb.Append('\n');
            }
            return sb.ToString();
        }

        // 增删改方法
        public void InsertDetail(OrderDetail det) // 插入明细项
        {
            foreach (var x in detList)
            {
                if (x == det)
                {
                    throw new InvalidOperationException("无法插入该项，条目重复！");
                }
            }   
            detList.Add(det);
        }

        public void RemoveDetail(Func<OrderDetail, bool> F) // 按指定规则删除明细项
        {
            List<OrderDetail> tmp = new List<OrderDetail>();
            detList.ForEach(det => //调用foreach方法访问列表
            {
                if (F(det))
                {
                    tmp.Add(det); // 记录待删除元素，foreach退出后逐个remove
                }
            });
            if (!tmp.Any())
            {
                throw new ArgumentNullException("不存在相应的订单明细项！");
            }
            foreach (var det in tmp)
            {
                detList.Remove(det);
            }
        }

        public void ModifyDetail(Func<OrderDetail, bool> F, OrderDetail newDet) // 修改指定明细项，必须保证被修改的明细只有一条且修改后列表不重复
        {
            int cnt = 0;
            Order tmp = new Order(this);
            foreach (var det in detList)
            {
                if (F(det))
                {
                    ++cnt;
                }
            }
            if (cnt == 0)
            {
                throw new ArgumentNullException("不存在相应的订单明细项！");
            }
            if (cnt > 1)
            {
                throw new InvalidOperationException("不能将多个匹配的明细项修改为相同的值");
            }

            tmp.RemoveDetail(F); // 此时确信满足F的明细只有一条

            try
            {
                tmp.InsertDetail(newDet);
                this.detList = tmp.detList; // 把改好的明细表拷贝回来
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("条目重复，无法完成修改！");
            }
            
        }
    }
    public class OrderService
    {
        private static int ordCnt = 0;
        private List<Order> ordList;

        public OrderService()
        {
            ordList = new List<Order>();
        }
        public void Clear()
        {
            ordCnt = 0;
            ordList.Clear();
        }

        // 基本信息方法
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(
                "--------------------The current order list--------------------\n");
            foreach (var x in ordList)
            {
                sb.Append(x.ToString());
            }
            sb.Append("-------------------------------------------------------------\n");
            return sb.ToString();
        }

        // 插入元素，组合使用
        public static Order CreateNextOrder(Order ord) // 以order计数器自增值作为下一个订单编号
        {
            ++ordCnt;
            ord.id = ordCnt; // 默认编号
            return ord;
        }
        public void AddOrder(Order ord) // 添加一条委托
        {
            foreach (var x in ordList) // 检验id是否重复
            {
                if (x.Equals(ord))
                {
                    throw new InvalidOperationException();
                }
            }
            ordList.Add(ord);
        }

        // 查询，select条件以lambda表达式传入
        public IEnumerable<Order> Query(Func<Order, bool> F) 
        {
            var query = ordList.Where(F)
                .OrderBy(x => x.totAmount); // 按总金额排序
            return query;
        }
        public IEnumerable<Order> QueryByItem(string item) // 按订单明细中的商品查询
        {
            return Query(ord =>
            {
                return (ord.detList.Where(det => det.item == item)).Any();
            });
        }
        public IEnumerable<Order> QueryByClient(string client) // 按订单明细中的客户查询
        {
            return Query(ord =>
            {
                return (ord.detList.Where(det => det.client == client)).Any();
            });
        }
        public IEnumerable<Order> QueryLowerAmount(double totAmount) // 总金额小于某值
        {
            return Query(x =>
            {
                return x.totAmount <= totAmount;
            });
        }
        public IEnumerable<Order> QueryHigherAmount(double totAmount) // 总金额大于某值
        {
            return Query(x =>
            {
                return x.totAmount >= totAmount;
            });
        }

        // 删除方法
        public void RemoveOrder(Func<Order, bool> F) // 调用foreach方法删除
        {
            List<Order> tmp = new List<Order>();
            ordList.ForEach(ord =>
            {
                if (F(ord))
                {
                    tmp.Add(ord);
                }
            });

            if (!tmp.Any())
                throw new ArgumentNullException("不存在相应的订单项！");
            foreach (var ord in tmp)
            {
                ordList.Remove(ord);
            }
        }
        public void RemoveOrder_ByDetail(Func<OrderDetail, bool> G)
        {
            RemoveOrder(ord => ord.detList.FirstOrDefault(G) != default(OrderDetail));
        }
        public void RemoveOrderDetail(Func<Order, bool> OrderF, Func<OrderDetail, bool> DetailF) // 删除指定订单的指定明细
        {
            bool f1 = false, f2 = false;
            ordList.ForEach(ord =>
            {
                if (OrderF(ord))
                {
                    f1 = true;
                    try
                    {
                        ord.RemoveDetail(DetailF);
                        f2 = true;
                    }
                    catch (ArgumentNullException) { } // 某个订单项没有相应明细也没关系
                }
            });
            if (!f1) throw new ArgumentNullException("不存在相应的订单项！");
            if (!f2) throw new InvalidOperationException("检索到的订单项中均不含有相应的明细项！");
        }

        // 修改方法
        public void ModifyOrder(Func<Order, bool> F, Order newOrd) 
        {
            bool f = false;
            for (int i = 0; i < ordList.Count(); ++i)
            {
                if (F(ordList[i]))
                {
                    f = true;
                    newOrd.id = ordList[i].id; // 强行转换，保证id不重复
                    ordList[i] = new Order(newOrd);
                }
            }
            if (!f)
                throw new ArgumentNullException("不存在相应的订单项！");
        }
        public void ModifyOrder_ByDetail(Func<OrderDetail, bool> G, Order newOrd)
        {
            ModifyOrder((ord => ord.detList.FirstOrDefault(G) != default(OrderDetail)), newOrd);
        }
        public void ModifyOrderDetail(Func<Order, bool> OrderF, Func<OrderDetail, bool> DetailF, OrderDetail newDet) // 删除指定订单的指定明细
        {
            bool f1 = false, f2 = false;
            ordList.ForEach(ord =>
            {
                if (OrderF(ord))
                {
                    f1 = true;
                    try
                    {
                        ord.ModifyDetail(DetailF, newDet);
                        f2 = true;
                    }
                    catch (ArgumentNullException) { } // 没有相应明细也没关系
                }
            });
            if (!f1) throw new ArgumentNullException("不存在相应的订单项");
            if (!f2) throw new InvalidOperationException("检索到的订单项中均不含有相应的明细项");
        }

        // 排序方法
        public void Sort(OrderComparer? OrdCmp = null)
        {
            if (ordList.Count == 0) // 判空
                throw new InvalidOperationException();

            if (OrdCmp is null)
            {
                OrdCmp = new OrderComparer(); // 默认为null规则，按id排序
            }
            ordList.Sort(OrdCmp);
        }
    }
    public class OrderComparer : IComparer<Order> // 与Order.Sort()配套的比较器类
    {
        Func<Order, Order, int>? Cmp; // 比较规则

        public OrderComparer(Func<Order, Order, int>? Cmp) // 构造时传入规则
        {
            this.Cmp = Cmp;
        }
        public OrderComparer() : this(null) { }

        public int Compare(Order? a, Order? b)
        {
            if (a is null || b is null)
            {
                throw new ArgumentNullException(); // 参数为空，触发例外
            }

            if (Cmp is null)
                return a.id - b.id; // 默认按id比较

            return Cmp(a, b);
        }
    }
    public class Program
    {
        public static void Main()
        {

        }
    }
}
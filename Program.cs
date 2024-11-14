using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSystem
{
    public abstract class Delivery
    {
        public virtual string address { get; set; }
        public abstract void DisplayDeliveryInfo();
    }

    public class HomeDelivery : Delivery
    {
        public override string address { get; set; }
        public int id_courier { get; set; }

        public HomeDelivery(string address, int id_courier)
        {
            this.address = address;
            this.id_courier = id_courier;
        }

        public override void DisplayDeliveryInfo()
        {
            Console.WriteLine($"Доставка на дом: {address}, курьер: {id_courier}");
        }
    }

    public class PickPointDelivery : Delivery
    {
        public string point_name { get; set; }
        public int id_point { get; set; }

        public PickPointDelivery(string point_name, int id_point)
        {
            this.point_name = point_name;
            this.id_point = id_point;
        }

        public override void DisplayDeliveryInfo()
        {
            Console.WriteLine($"Доставка в пункт выдачи: {point_name}, ID: {id_point}");
        }
    }

    public class ShopDelivery : Delivery
    {
        public string shop_name { get; set; }

        public ShopDelivery(string shop_name)
        {
            this.shop_name = shop_name;
        }

        public override void DisplayDeliveryInfo()
        {
            Console.WriteLine($"Самовывоз из магазина: {shop_name}");
        }
    }

    public class Product
    {
        public string name { get; set; }
        public int price { get; set; }

        public Product(string name, int price)
        {
            this.name = name;
            this.price = price;
        }
    }

    public class Order<TDelivery, TId> where TDelivery : Delivery
    {
        public TDelivery delivery { get; set; }
        public Product product { get; set; }
        public bool isPaid { get; set; }

        public Order(TDelivery delivery, Product product, bool isPaid)
        {
            this.delivery = delivery;
            this.product = product;
            this.isPaid = isPaid;
        }

        public TId this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        if (delivery is HomeDelivery homeDelivery)
                        {
                            return (TId)(object)homeDelivery.id_courier;
                        }
                        else if (delivery is PickPointDelivery pickPointDelivery)
                        {
                            return (TId)(object)pickPointDelivery.id_point;
                        }
                        break;
                    case 1:
                        return (TId)(object)product.price;
                    default:
                        throw new IndexOutOfRangeException();
                }
                return default;
            }
        }

        public static Order<TDelivery, TId> operator +(Order<TDelivery, TId> order1, Order<TDelivery, TId> order2)
        {
            return new Order<TDelivery, TId>(order1.delivery, order1.product, order1.isPaid);
        }
    }

    public class SpecialOrder<TDelivery, TId> : Order<TDelivery, TId> where TDelivery : Delivery
    {
        public string specialNote { get; set; }

        public SpecialOrder(TDelivery delivery, Product product, bool isPaid, string specialNote) : base(delivery, product, isPaid)
        {
            this.specialNote = specialNote;
        }
    }

    public static class DeliveryExtensions
    {
        public static void DisplayDeliveryInfo(this Delivery delivery)
        {
            delivery.DisplayDeliveryInfo();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Product product = new Product("Телефон", 1000);
            HomeDelivery homeDelivery = new HomeDelivery("ул. Ленина, д. 1", 123);
            PickPointDelivery pickPointDelivery = new PickPointDelivery("Пункт выдачи № 456", 456);
            ShopDelivery shopDelivery = new ShopDelivery("Магазин");

            Order<HomeDelivery, int> order1 = new Order<HomeDelivery, int>(homeDelivery, product, true);
            Order<PickPointDelivery, int> order2 = new Order<PickPointDelivery, int>(pickPointDelivery, product, false);
            Order<ShopDelivery, int> order3 = new Order<ShopDelivery, int>(shopDelivery, product, true);

            Console.WriteLine(order1[0]);
            Console.WriteLine(order1[1]);

            Console.WriteLine(order2[0]);
            Console.WriteLine(order2[1]);

            Console.WriteLine(order3[0]);
            Console.WriteLine(order3[1]);

            homeDelivery.DisplayDeliveryInfo();
            pickPointDelivery.DisplayDeliveryInfo();
            shopDelivery.DisplayDeliveryInfo();

            Order<HomeDelivery, int> combinedOrder = order1 + order1;
            Console.WriteLine(combinedOrder[0]);
            Console.WriteLine(combinedOrder[1]);

            Order<HomeDelivery, int> specialOrder = new Order<HomeDelivery, int>(homeDelivery, product, true);
            Console.WriteLine(specialOrder[0]);
            Console.WriteLine(specialOrder[1]);

            SpecialOrder<HomeDelivery, int> specialOrder1 = new SpecialOrder<HomeDelivery, int>(homeDelivery, product, true, "Доставка в праздничные дни");
            Console.WriteLine(specialOrder1.specialNote);
        }
    }
}

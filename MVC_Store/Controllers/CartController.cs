using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using MVC_Store.Models.Data;
using MVC_Store.Models.ViewModels.Cart;

namespace MVC_Store.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            // Объявляем лист типа CartVM
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Проверяем, пуста ли корзина
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty.";
                return View();
            }
            // Складываем сумму и записываем во ViewBag
            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            // Возвращаем лист в представление
            return View(cart);
        }

        public ActionResult CartPartial()
        {
            // Объявляем модель CartVM
            CartVM model = new CartVM();

            // Объявляем переменную количества
            int qty = 0;

            // Объявляем переменную цены
            decimal price = 0m;

            // Проверяем сессию корзины
            if (Session["cart"] != null)
            {
                // Получаем общее количество товаров и цену
                var list = (List<CartVM>) Session["cart"];

                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }

                model.Quantity = qty;
                model.Price = price;
            }
            else
            {
                // Или устанавливаем количество и цену в 0
                model.Quantity = 0;
                model.Price = 0m;
            }

            // Возвращаем частичное представление с моделью
            return PartialView("_CartPartial", model);
        }

        public ActionResult AddToCartPartial(int id)
        {
            // Объявляем лист, параметризированный типом CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Объявляем модель CartVM
            CartVM model = new CartVM();

            using (Db db = new Db())
            {
                // Получаем продукт
                ProductDTO product = db.Products.Find(id);

                // Проверяем, находится ли товар уже в корзине
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                // Если нет, то добавляем новый товар в корзину
                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                // Если да, добавляем единицу товара
                else
                {
                    productInCart.Quantity++;
                }
            }
            // Получаем общее количество, цену и добавляем данные в модель
            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            // Сохраняем состояние корзины в сессию
            Session["cart"] = cart;

            // Возвращаем частичное представление с моделью
            return PartialView("_AddToCartPartial", model);
        }

        // GET: /cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            // Объявляем лист cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                // Получаем модель CartVM из листа
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Добавляем количество
                model.Quantity++;

                // Сохраняем необходимые данные
                var result = new {qty = model.Quantity, price = model.Price};

                // Возвращаем JSON ответ с данными
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /cart/DecrementProduct
        public ActionResult DecrementProduct(int productId)
        {
            // Объявляем лист cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                // Получаем модель CartVM из листа
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Отнимаем количество
                if (model.Quantity > 1)
                    model.Quantity--;
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                // Сохраняем необходимые данные
                var result = new {qty = model.Quantity, price = model.Price};

                // Возвращаем JSON ответ с данными
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public void RemoveProduct(int productId)
        {
            // Объявляем лист cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                // Получаем модель CartVM из листа
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                cart.Remove(model);
            }
        }

        public ActionResult PaypalPartial()
        {
            // Получаем список товаров в корзине
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // Возвращаем частичное представление с листом
            return PartialView(cart);
        }

        // POST: /cart/PlaceOrder
        [HttpPost]
        public void PlaceOrder()
        {
            // Получаем список товаров в корзине
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // Получаем имя пользователя
            string userName = User.Identity.Name;

            // Объявляем переменную для orderId
            int orderId = 0;

            using (Db db = new Db())
            {
                // Объявляем модель OrderDTO
                OrderDTO orderDto = new OrderDTO();

                // Получаем ID пользователя
                var q = db.Users.FirstOrDefault(x => x.Username == userName);
                int userId = q.Id;

                // Заполняем модель OrderDTO данными и сохраняем
                orderDto.UserId = userId;
                orderDto.CreatedAt = DateTime.Now;

                db.Orders.Add(orderDto);
                db.SaveChanges();

                // Получаем orderId
                orderId = orderDto.OrderId;

                // Объявляем модель OrderDetailsDTO
                OrderDetailsDTO orderDetailsDto = new OrderDetailsDTO();

                // Добавляем в модель данные
                foreach (var item in cart)
                {
                    orderDetailsDto.OrderId = orderId;
                    orderDetailsDto.UserId = userId;
                    orderDetailsDto.ProductId = item.ProductId;
                    orderDetailsDto.Quantity = item.Quantity;

                    db.OrderDetails.Add(orderDetailsDto);
                    db.SaveChanges();
                }
            }
            // Отправляем письмо о заказе на почту администратора
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("ee053ae36467a9", "8bf25933605f02"),
                EnableSsl = true
            };
            client.Send("shop@example.com", "admin@example.com", "New Order", $"You have a new order. Order number: {orderId}");

            // Обноляем сессию
            Session["cart"] = null;
        }
    }
}
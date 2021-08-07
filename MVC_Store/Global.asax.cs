using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MVC_Store.Models.Data;

namespace MVC_Store
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        // Создаём метод обработки запросов аутентификации
        protected void Application_AuthenticateRequest()
        {
            // Проверяем, авторизован ли пользователь
            if (User == null)
                return;

            // Получаем имя пользователя
            string userName = Context.User.Identity.Name;

            // Объявляем массив ролей
            string[] roles = null;

            using (Db db = new Db())
            {
                // Заполняем роли
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);

                if (dto == null)
                    return;

                roles = db.UserRoles.Where(x => x.UserId == dto.Id).Select(x => x.Role.Name).ToArray();
            }
            // Создаём объект интерфейса IPrincipal
            IIdentity userIdentity = new GenericIdentity(userName);
            IPrincipal newUserObj = new GenericPrincipal(userIdentity, roles);

            // Объявляем и инициализируем данными Context.User
            Context.User = newUserObj;
        }
    }
}

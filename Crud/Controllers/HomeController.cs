using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApiWeb;
using Crud.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using PagedList;

namespace Crud
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string cedula)
        {
            var httpClient = _httpClientFactory.CreateClient("Api");
            var response = await httpClient.GetAsync($"/api/Usuarios/login?username={username}&cedula={cedula}");

            if (response.IsSuccessStatusCode)
            {
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
      new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, CookieAuthenticationDefaults.AuthenticationScheme)),
      new AuthenticationProperties());

                return RedirectToAction(nameof(Index));
            }

            ViewData["Error"] = "Invalid username or password";
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AccesDenied()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UserActivo(string Idmenu, string sortOrder, string searchNombre, string searchCedula, string CurrentSort, string pageSize, int page = 1)
        {
            var httpClient = _httpClientFactory.CreateClient("Api");

            var response = await httpClient.GetAsync($"/api/Usuarios/activos");
            var content = await response.Content.ReadAsStringAsync();

            int pag = 10;

            if (pageSize != null)
                pag = Convert.ToInt16(pageSize);
            ViewBag.pageSize = pag;
            ViewBag.searchNombre = searchNombre;

            ViewBag.searchCedula = searchCedula;


            IPagedList<UsuarioActivo>? list2 = null;
            try
            {
                List<UsuarioActivo>? list = JsonConvert.DeserializeObject<List<UsuarioActivo>>(content);

                if (searchNombre != null)
                {
                    list = list.Where(p => p.nombreCompleto.ToUpper().Contains(searchNombre.ToUpper())).ToList();
                }
                if (searchCedula != null)
                {
                    list = list.Where(p => p.cedula.ToUpper().Contains(searchNombre.ToUpper())).ToList();
                }


                sortOrder = String.IsNullOrEmpty(sortOrder) ? "NombreCompleto" : sortOrder;



                switch (sortOrder)
                {
                    case "NombreCompleto":
                        if (sortOrder.Equals(CurrentSort))
                        {

                            list2 = list.OrderByDescending
                                    (m => m.nombreCompleto).ToPagedList(page, pag);

                            CurrentSort = null;
                        }

                        else
                        {
                            ViewBag.CurrentSort = sortOrder;
                            list2 = list.OrderBy
                                    (m => m.nombreCompleto).ToPagedList(page, pag);
                        }

                        break;
                    case "UserName":
                        if (sortOrder.Equals(CurrentSort))
                        {
                            list2 = list.OrderByDescending
                                    (m => m.alias).ToPagedList(page, pag);
                            CurrentSort = null;
                        }
                        else
                        {
                            ViewBag.CurrentSort = sortOrder;
                            list2 = list.OrderBy
                                    (m => m.alias).ToPagedList(page, pag);
                        }

                        break;
                    case "Estado":
                        if (sortOrder.Equals(CurrentSort))
                        {

                            list2 = list.OrderByDescending
                                    (m => m.estado).ToPagedList(page, pag);
                            CurrentSort = null;
                        }
                        else
                        {
                            ViewBag.CurrentSort = sortOrder;
                            list2 = list.OrderBy
                                    (m => m.estado).ToPagedList(page, pag);
                        }

                        break;

                    case "FechaCreacion":
                        if (sortOrder.Equals(CurrentSort))
                        {

                            list2 = list.OrderByDescending
                                    (m => m.fechaCreacion).ToPagedList(page, pag);
                            CurrentSort = null;
                        }
                        else
                        {
                            ViewBag.CurrentSort = sortOrder;
                            list2 = list.OrderBy
                                    (m => m.fechaCreacion).ToPagedList(page, pag);
                        }

                        break;

                }
            }
            catch
            {

            }
            return View(list2);
        }


        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(UsuarioModels datos)
        {
            datos.FechaCreacion = DateTime.Now;
            var httpClient = _httpClientFactory.CreateClient("Api");


            var response = await httpClient.PostAsJsonAsync("/api/Usuarios", datos);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("UserActivo", "Home");
            }
            else
            {
                return View(datos);
            }
        }




        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            var httpClient = _httpClientFactory.CreateClient("Api");
            var response = await httpClient.GetAsync($"/api/Usuarios/{id}");

            var content = await response.Content.ReadAsStringAsync();
            UsuarioModels usuario = JsonConvert.DeserializeObject<UsuarioModels>(content);

            return View(usuario);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(UsuarioModels datos)
        {
            // crea un objeto Usuario con los nuevos valores

            var usuario = new Usuario { Id = datos.Id, Nombre =datos.Nombre , Apellido = datos.Apellido, Estado = datos.Estado, Cedula = datos.Cedula, UserName = datos.UserName };      
            var requestUri = $"/api/Usuarios/{datos.Id}";
            var json = JsonConvert.SerializeObject(usuario);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpClient = _httpClientFactory.CreateClient("Api");
            var response = await httpClient.PutAsync(requestUri, content);

            if (response.IsSuccessStatusCode)
            {
                // el usuario se actualizó correctamente
                return RedirectToAction("UserActivo"); 
            }
            else
            {
                // hubo un error al actualizar el usuario
                var errorMessage = await response.Content.ReadAsStringAsync();
                // maneja el error de acuerdo a tus necesidades
                return View(usuario);
            }


        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            // crea un objeto Usuario con los nuevos valores

            var requestUri = $"/api/Usuarios/{id}";
            var httpClient = _httpClientFactory.CreateClient("Api");
            var response = await httpClient.DeleteAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                // el usuario se actualizó correctamente
                return RedirectToAction("UserActivo");
            }
            else
            {
                // hubo un error al actualizar el usuario
                var errorMessage = await response.Content.ReadAsStringAsync();
                // maneja el error de acuerdo a tus necesidades
                return View();
            }


        }


    }
}

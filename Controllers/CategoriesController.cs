using System;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using StoreCatalogueDA.DataObject;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace StoreCatalogue.Controllers
{
    public class CategoriesController : Controller
    {
        HttpClient client;
        string url = ConfigurationManager.AppSettings["ApiURL"] + "Categories";

        public CategoriesController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // GET: Categories
        public async Task<ActionResult> Index()
        {
            List<Category> categories = null;
            HttpResponseMessage responseMessage = await client.GetAsync(url);
            if (responseMessage.IsSuccessStatusCode)
            {
                String categoryString = responseMessage.Content.ReadAsStringAsync().Result;
                categories = JsonConvert.DeserializeObject<List<Category>>(categoryString);
            }
            return View(categories);
        }

        // GET: Categories/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            Category category = null;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HttpResponseMessage responseMessage = await client.GetAsync(url + "/" + id);
            if (responseMessage.IsSuccessStatusCode)
            {
                String categoryString = responseMessage.Content.ReadAsStringAsync().Result;
                category = JsonConvert.DeserializeObject<Category>(categoryString);
                if (category == null)
                {
                    return HttpNotFound();
                }
            }
            
            return View(category);
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.Id = Guid.NewGuid();
                var jsonCategory = JsonConvert.SerializeObject(category);
                var buffer = System.Text.Encoding.UTF8.GetBytes(jsonCategory);
                var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                await client.PostAsync(url, byteContent);
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            Category category = null;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HttpResponseMessage responseMessage = await client.GetAsync(url + "/" + id);
            if (responseMessage.IsSuccessStatusCode)
            {
                String categoryString = responseMessage.Content.ReadAsStringAsync().Result;
                category = JsonConvert.DeserializeObject<Category>(categoryString);
                if (category == null)
                {
                    return HttpNotFound();
                }
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                var jsonCategory = JsonConvert.SerializeObject(category);
                var buffer = System.Text.Encoding.UTF8.GetBytes(jsonCategory);
                var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                await client.PutAsync(url + "/" + category.Id, byteContent);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            Category category = null;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HttpResponseMessage responseMessage = await client.GetAsync(url + "/" + id);
            if (responseMessage.IsSuccessStatusCode)
            {
                String categoryString = responseMessage.Content.ReadAsStringAsync().Result;
                category = JsonConvert.DeserializeObject<Category>(categoryString);
                if (category == null)
                {
                    return HttpNotFound();
                }
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            HttpResponseMessage responseMessage = await client.DeleteAsync(url + "/" + id);
            TempData["Message"] = responseMessage.ReasonPhrase;
            return RedirectToAction("Index");
        }

        
    }
}

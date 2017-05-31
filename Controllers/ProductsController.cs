using System;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using StoreCatalogueDA.DataObject;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Configuration;
using System.Collections.Generic;

namespace StoreCatalogue.Controllers
{
    public class ProductsController : Controller
    {
        HttpClient client;
        string url = ConfigurationManager.AppSettings["ApiURL"] + "Products";
        string subCategoryURL = ConfigurationManager.AppSettings["ApiURL"] + "SubCategories";

        public ProductsController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // GET: Products
        public async Task<ActionResult> Index()
        {
            List<Product> products = null;
            HttpResponseMessage responseMessage = await client.GetAsync(url);
            if (responseMessage.IsSuccessStatusCode)
            {
                String productsString = responseMessage.Content.ReadAsStringAsync().Result;
                products = JsonConvert.DeserializeObject<List<Product>>(productsString);
                await GetSubCategories(null);
            }
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            Product product = null;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HttpResponseMessage responseMessage = await client.GetAsync(url + "/" + id);
            if (responseMessage.IsSuccessStatusCode)
            {
                String productString = responseMessage.Content.ReadAsStringAsync().Result;
                product = JsonConvert.DeserializeObject<Product>(productString);
                if (product == null)
                {
                    return HttpNotFound();
                }
                await GetSubCategory(product.SubCategoryId);
            }

            return View(product);
        }

        // GET: Products/Create
        public async Task<ActionResult> Create()
        {
            await GetSubCategories(null);
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,SubCategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.Id = Guid.NewGuid();
                var jsonproduct = JsonConvert.SerializeObject(product);
                var buffer = System.Text.Encoding.UTF8.GetBytes(jsonproduct);
                var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                await client.PostAsync(url, byteContent);
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            Product product = null;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HttpResponseMessage responseMessage = await client.GetAsync(url + "/" + id);
            if (responseMessage.IsSuccessStatusCode)
            {
                String productString = responseMessage.Content.ReadAsStringAsync().Result;
                product = JsonConvert.DeserializeObject<Product>(productString);
                if (product == null)
                {
                    return HttpNotFound();
                }
                await GetSubCategories(product.SubCategoryId);
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,SubCategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                var jsonproduct = JsonConvert.SerializeObject(product);
                var buffer = System.Text.Encoding.UTF8.GetBytes(jsonproduct);
                var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                await client.PutAsync(url + "/" + product.Id, byteContent);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            Product product = null;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HttpResponseMessage responseMessage = await client.GetAsync(url + "/" + id);
            if (responseMessage.IsSuccessStatusCode)
            {
                String productString = responseMessage.Content.ReadAsStringAsync().Result;
                product = JsonConvert.DeserializeObject<Product>(productString);
                if (product == null)
                {
                    return HttpNotFound();
                }
            }
            return View(product);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            await client.DeleteAsync(url + "/" + id);
            return RedirectToAction("Index");
        }

        private async Task GetSubCategories(Guid? selectedId)
        {
            List<SelectListItem> subCategories = new List<SelectListItem>();
            HttpResponseMessage responseMessage = await client.GetAsync(subCategoryURL);
            if (responseMessage.IsSuccessStatusCode)
            {
                String subCategoryString = responseMessage.Content.ReadAsStringAsync().Result;
                var subCategoryList = JsonConvert.DeserializeObject<List<SubCategory>>(subCategoryString);
                foreach (var subCat in subCategoryList)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = subCat.Name;
                    item.Value = subCat.Id.ToString();
                    if (selectedId.HasValue && selectedId.Value == subCat.Id)
                        item.Selected = true;
                    subCategories.Add(item);
                }
            }
            if(subCategories.Count == 0)
                TempData["Message"] = "Please add atleast one Sub category to proceed.";
            ViewBag.SubCategories = subCategories;
        }

        private async Task GetSubCategory(Guid id)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(subCategoryURL + "/" + id);
            if (responseMessage.IsSuccessStatusCode)
            {
                String subCategoryString = responseMessage.Content.ReadAsStringAsync().Result;
                var subCat = JsonConvert.DeserializeObject<SubCategory>(subCategoryString);
                ViewBag.SubCategory = subCat.Name;
            }
        }
    }
}


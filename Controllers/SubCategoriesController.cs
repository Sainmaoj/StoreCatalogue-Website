using Newtonsoft.Json;
using StoreCatalogueDA.DataObject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace StoreSubCatalogue.Controllers
{
    public class SubCategoriesController : Controller
    {
        HttpClient client;
        string url = ConfigurationManager.AppSettings["ApiURL"] + "SubCategories";
        string categoryURL = ConfigurationManager.AppSettings["ApiURL"] + "Categories";

        public SubCategoriesController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // GET: SubCategories
        public async Task<ActionResult> Index()
        {
            List<SubCategory> subCategories = null;
            HttpResponseMessage responseMessage = await client.GetAsync(url);
            if (responseMessage.IsSuccessStatusCode)
            {
                String categoryString = responseMessage.Content.ReadAsStringAsync().Result;
                subCategories = JsonConvert.DeserializeObject<List<SubCategory>>(categoryString);
            }
            await GetCategories(null);
            return View(subCategories);
        }

        // GET: SubCategories/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            SubCategory SubCategory = null;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HttpResponseMessage responseMessage = await client.GetAsync(url + "/" + id);
            if (responseMessage.IsSuccessStatusCode)
            {
                String SubCategoryString = responseMessage.Content.ReadAsStringAsync().Result;
                SubCategory = JsonConvert.DeserializeObject<SubCategory>(SubCategoryString);
                if (SubCategory == null)
                {
                    return HttpNotFound();
                }
                await GetCategory(SubCategory.CategoryId);
            }

            return View(SubCategory);
        }

        // GET: SubCategories/Create
        public async Task<ActionResult> Create()
        {
            await GetCategories(null);
            return View();
        }

        // POST: SubCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,CategoryId")] SubCategory SubCategory)
        {
            if (ModelState.IsValid)
            {
                SubCategory.Id = Guid.NewGuid();
                var jsonSubCategory = JsonConvert.SerializeObject(SubCategory);
                var buffer = System.Text.Encoding.UTF8.GetBytes(jsonSubCategory);
                var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                await client.PostAsync(url, byteContent);
                return RedirectToAction("Index");
            }

            return View(SubCategory);
        }

        // GET: SubCategories/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            SubCategory SubCategory = null;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HttpResponseMessage responseMessage = await client.GetAsync(url + "/" + id);
            if (responseMessage.IsSuccessStatusCode)
            {
                String SubCategoryString = responseMessage.Content.ReadAsStringAsync().Result;
                SubCategory = JsonConvert.DeserializeObject<SubCategory>(SubCategoryString);
                if (SubCategory == null)
                {
                    return HttpNotFound();
                }
                await GetCategories(SubCategory.CategoryId);
            }
            return View(SubCategory);
        }

        // POST: SubCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,CategoryId")] SubCategory SubCategory)
        {
            if (ModelState.IsValid)
            {
                var jsonSubCategory = JsonConvert.SerializeObject(SubCategory);
                var buffer = System.Text.Encoding.UTF8.GetBytes(jsonSubCategory);
                var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                await client.PutAsync(url + "/" + SubCategory.Id, byteContent);
                return RedirectToAction("Index");
            }
            return View(SubCategory);
        }

        // GET: SubCategories/Delete/5
        public async Task<ActionResult> Delete(Guid? id)
        {
            SubCategory SubCategory = null;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HttpResponseMessage responseMessage = await client.GetAsync(url + "/" + id);
            if (responseMessage.IsSuccessStatusCode)
            {
                String SubCategoryString = responseMessage.Content.ReadAsStringAsync().Result;
                SubCategory = JsonConvert.DeserializeObject<SubCategory>(SubCategoryString);
                if (SubCategory == null)
                {
                    return HttpNotFound();
                }
            }
            return View(SubCategory);
        }

        // POST: SubCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            await client.DeleteAsync(url + "/" + id);
            return RedirectToAction("Index");
        }

        private async Task GetCategories(Guid? selectedId)
        {
            List<SelectListItem> categories = new List<SelectListItem>();
            HttpResponseMessage responseMessage = await client.GetAsync(categoryURL);
            if (responseMessage.IsSuccessStatusCode)
            {
                String categoryString = responseMessage.Content.ReadAsStringAsync().Result;
                var categoryList = JsonConvert.DeserializeObject<List<Category>>(categoryString);
                foreach (var cat in categoryList)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = cat.Name;
                    item.Value = cat.Id.ToString();
                    if (selectedId.HasValue && selectedId.Value == cat.Id)
                        item.Selected = true;
                    categories.Add(item);
                }
            }
            if (categories.Count == 0)
                TempData["Message"] = "Please add atleast one category to proceed.";
            ViewBag.Categories = categories;
        }

        private async Task GetCategory(Guid id)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(categoryURL + "/" + id);
            if (responseMessage.IsSuccessStatusCode)
            {
                String categoryString = responseMessage.Content.ReadAsStringAsync().Result;
                var cat = JsonConvert.DeserializeObject<Category>(categoryString);
                ViewBag.Category = cat.Name;
            }
        }
    }
}


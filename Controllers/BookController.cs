using Microsoft.AspNetCore.Mvc;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Library.Models;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Library.Controllers
{
    public class BookController : Controller
    {

        // Needs updating to Admin SDK
        readonly IFirebaseConfig config = new FirebaseConfig { AuthSecret = "nnCSNoZVjDfwLrRZjJaWWVErbOjJrm2NBToOMS7d", BasePath = "https://library-fcc0a-default-rtdb.europe-west1.firebasedatabase.app" };
        private IFirebaseClient? client;
        public IActionResult Index()
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Books");
                dynamic? data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<Book?>();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        Book? book = JsonConvert.DeserializeObject<Book>(((JProperty)item).Value.ToString());
                        if (book != null && book.UnavailableUntil < DateTime.Now)
                        {
                            book.IsAvailable = true;
                            SetResponse setResponse = client.Set("Books/" + book.Id, book);
                        }
                        list.Add(book);
                    }
                }

                return View(list);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Book book)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                PushResponse response = client.Push("Books/", book);
                book.Id = response.Result.name;
                SetResponse setResponse = client.Set("Books/" + book.Id, book);
                if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ModelState.AddModelError(string.Empty, "Added Succesfully");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong!!");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View();
        }

        public IActionResult Details(Book book)
        {
            return View(book);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Books/" + id);
                Book? data = JsonConvert.DeserializeObject<Book>(response.Body);
                return View(data);
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
        }

        [HttpPost]
        public IActionResult Edit(Book book)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                SetResponse response = client.Set("Books/" + book.Id, book);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ModelState.AddModelError(string.Empty, "Edited Succesfully");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong!!");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View();

        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Delete("Books/" + id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(Book book)
        {
            return View(book);
        }

        public IActionResult ReserveBook(Book book)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                if(book.IsAvailable == true)
                {
                    book.IsAvailable = false; 
                    book.UnavailableUntil = DateTime.Now.AddDays(1);
                    SetResponse setResponse = client.Set("Books/" + book.Id, book);
                }

            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return RedirectToAction("Index");
        }

        public IActionResult ReturnedBook(Book book)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                book.IsAvailable = true;
                SetResponse setResponse = client.Set("Books/" + book.Id, book);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return RedirectToAction("Index");
        }


        public IActionResult BorrowBook(Book book)
        {
            return View(book);
        }

        [HttpPost]
        public IActionResult BorrowBook(string borrowUntil, string id)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Books/" + id);
                Book? data = JsonConvert.DeserializeObject<Book>(response.Body);
                if(data == null)
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong!!");
                    return View();
                }
                if(borrowUntil == null)
                {
                    return View(data);
                }
                if (DateTime.Parse(borrowUntil) <= DateTime.Now.AddMonths(3))
                {
                    data.IsAvailable = false;
                    data.UnavailableUntil = DateTime.Parse(borrowUntil);
                    SetResponse setResponse = client.Set("Books/" + data.Id, data);
                    if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ModelState.AddModelError(string.Empty, "Borrowed Succesfully");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Something went wrong!!");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "You can only borrow for up to 3 months");
                }
                return View(data);

            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
            
        }

        public IActionResult SearchBook()
        {
            return View();
        }

        // possible bug on asp.net side, bool isAvailable always false, using string instead
        public IActionResult SearchBookResults(string title, string author, string publisher, string publishingDateStart, string publishingDateEnd, string genre, string isbn, string isAvailable)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("Books");
                dynamic? data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<Book?>();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        Book? book = JsonConvert.DeserializeObject<Book>(((JProperty)item).Value.ToString());
                        if (book != null && book.UnavailableUntil < DateTime.Now)
                        {
                            book.IsAvailable = true;
                            SetResponse setResponse = client.Set("Books/" + book.Id, book);
                        }
                        list.Add(book);
                    }
                }
                list.RemoveAll(item => item == null);
                if(title != null)
                {
                    list.RemoveAll(item => item.Title.Contains(title));
                }
                if (author != null)
                {
                    list.RemoveAll(item => item.Author.Contains(author));
                }
                if (publisher != null)
                {
                    list.RemoveAll(item => item.Publisher.Contains(publisher));
                }
                if (genre != null)
                {
                    list.RemoveAll(item => item.Genre.Contains(genre));
                }
                if (isbn != null)
                {
                    list.RemoveAll(item => item.Isbn.Contains(isbn));
                }
                // May be bug on aps.net side, when trying to get bool isAvailable it is always false. Using string, which is null if isAvailable was not checked and not null when it was checked
                list.RemoveAll(item => item.IsAvailable != (isAvailable != null));
                if (publishingDateStart != null)
                {
                    list.RemoveAll(item => item.PublishingDate >= DateTime.Parse(publishingDateStart));
                }
                if (publishingDateEnd != null)
                {
                    list.RemoveAll(item => item.PublishingDate >= DateTime.Parse(publishingDateEnd));
                }
                return View(list);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
            
        }
    }
 
}

using Microsoft.AspNetCore.Mvc;
using FireSharp.Response;
using Library.Models;
using Library.Logic;

namespace Library.Controllers
{
    public class BookController : Controller
    {

        private readonly FirebaseDataManager firebaseDataManager;

        public BookController()
        {
            firebaseDataManager = new FirebaseDataManager();
        }
        public IActionResult Index()
        {
            try
            {
                UpdateUser();
                return View(firebaseDataManager.GetAllBooks());
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
            if (HttpContext.Session.GetString("email") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            UpdateUser();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Book book)
        {
            try
            {
                UpdateUser();
                SetResponse setResponse = firebaseDataManager.AddBook(book);
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
            UpdateUser();
            return View(book);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            try
            {
                if (HttpContext.Session.GetString("email") == null)
                {
                    return RedirectToAction("Login", "Auth");
                }
                UpdateUser();
                return View(firebaseDataManager.GetBook(id));
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
                UpdateUser();
                SetResponse response = firebaseDataManager.SetBook(book);
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
                var response = firebaseDataManager.DeleteBook(id);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    UpdateUser();
                    ModelState.AddModelError(string.Empty, "Something went wrong!!");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(Book book)
        {
            
            if (HttpContext.Session.GetString("email") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            UpdateUser();
            return View(book);
        }

        public IActionResult ReserveBook(Book book)
        {
            if(HttpContext.Session.GetString("email") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            try
            {
                book.TakenBy = HttpContext.Session.GetString("email");
                firebaseDataManager.ReserveBook(book);
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return RedirectToAction("Index");
        }

        public IActionResult ReturnedBook(Book book)
        {
            if (HttpContext.Session.GetString("email") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            try
            {
                book.IsAvailable = true;
                firebaseDataManager.SetBook(book);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return RedirectToAction("Index");
        }


        public IActionResult BorrowBook(Book book)
        {
            if (HttpContext.Session.GetString("email") == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            UpdateUser();
            return View(book);
        }

        [HttpPost]
        public IActionResult BorrowBook(string borrowUntil, string id)
        {
            try
            {
                UpdateUser();
                Book? book = firebaseDataManager.GetBook(id);
                if(book == null)
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong!!");
                    return View();
                }
                if(borrowUntil == null)
                {
                    return View(book);
                }
                book.TakenBy = HttpContext.Session.GetString("email");
                ModelState.AddModelError(string.Empty, firebaseDataManager.BorrowBook(borrowUntil, book));
                return View(book);

            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
            
        }

        public IActionResult SearchBook()
        {
            UpdateUser();
            return View();
        }

        // possible bug on asp.net side, bool isAvailable always false, using string instead
        public IActionResult SearchBookResults(string? title, string? author, string? publisher, string? publishingDateStart, string? publishingDateEnd, string? genre, string? isbn, string? availability)
        {
            try
            {
                UpdateUser();
                // May be bug on aps.net side, when trying to get bool isAvailable it is always false. Using string, which is null if isAvailable was not checked and not null when it was checked
                return View(firebaseDataManager.SearchBook(title, author, publisher, publishingDateStart, publishingDateEnd, genre, isbn, availability == "all" ? null : availability));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View();
            }
            
        }

        private void UpdateUser()
        {
            ViewData["Email"] = HttpContext.Session.GetString("email");
            ViewData["Role"] = HttpContext.Session.GetString("role");
        }
    }
 
}

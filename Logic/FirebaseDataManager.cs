using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Library.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Library.Logic
{
    public class FirebaseDataManager
    {
        // Needs updating to Admin SDK
        readonly IFirebaseConfig config;
        readonly private IFirebaseClient client;

        public FirebaseDataManager()
        {
            config = new FirebaseConfig { AuthSecret = "nnCSNoZVjDfwLrRZjJaWWVErbOjJrm2NBToOMS7d", BasePath = "https://library-fcc0a-default-rtdb.europe-west1.firebasedatabase.app" };
            client = new FireSharp.FirebaseClient(config);
        }

        public SetResponse AddBook(Book book)
        {
            PushResponse response = client.Push("Books/", book);
            book.Id = response.Result.name;
            return SetBook(book);
        }

        public Book? GetBook(string id)
        {
            FirebaseResponse response = client.Get("Books/" + id);
            return JsonConvert.DeserializeObject<Book>(response.Body);
        }

        public SetResponse SetBook(Book book)
        {
            return client.Set("Books/" + book.Id, book);
        }

        public FirebaseResponse DeleteBook(string id)
        {
            return client.Delete("Books/" + id);
        }

        public SetResponse? ReserveBook(Book book)
        {
            if (book.IsAvailable == true)
            {
                book.IsAvailable = false;
                book.UnavailableUntil = DateTime.Now.AddDays(1);
                SetResponse setResponse = client.Set("Books/" + book.Id, book);
            }
            return null;
        }

        public string BorrowBook(string borrowUntil, Book book)
        {

            if (DateTime.Parse(borrowUntil) < DateTime.Now)
            {
                return "Invalid date: borrow date cannot be earlier than today";
            }
            if (DateTime.Parse(borrowUntil) <= DateTime.Now.AddMonths(3))
            {
                book.IsAvailable = false;
                book.UnavailableUntil = DateTime.Parse(borrowUntil);
                SetResponse setResponse = SetBook(book);
                if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return "Borrowed Succesfully";
                }
                else
                {
                    return "Something went wrong!!";
                }
            }
            else
            {
                return "You can only borrow for up to 3 months";
            }
        }

        public List<Book?> GetAllBooks()
        {
            FirebaseResponse response = client.Get("Books");
            dynamic? data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Book?>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    Book? book = JsonConvert.DeserializeObject<Book>(((JProperty)item).Value.ToString());
                    list.Add(book);
                }
                
            }
            return list;
        }

        public List<Book?> SearchBook(string? title, string? author, string? publisher, string? publishingDateStart, string? publishingDateEnd, string? genre, string? isbn, bool isAvailable)
        {
            List<Book?> books = GetAllBooks();
            books.RemoveAll(item => item == null);
            #pragma warning disable CS8602 // Dereference of a possibly null reference. item cannot be null, all of null cases were removed in line above(line 108)
            if (title != null)
            {
                books.RemoveAll(item => item.Title == null || !item.Title.Contains(title));
            }
            if (author != null)
            {
                books.RemoveAll(item => item.Author == null || !item.Author.Contains(author));
            }
            if (publisher != null)
            {
                books.RemoveAll(item => item.Publisher == null || !item.Publisher.Contains(publisher));
            }
            if (genre != null)
            {
                books.RemoveAll(item => item.Genre == null || !item.Genre.Contains(genre));
            }
            if (isbn != null)
            {
                books.RemoveAll(item => item.Isbn == null || !item.Isbn.Contains(isbn));
            }
            books.RemoveAll(item => item.IsAvailable != isAvailable);
            if (publishingDateStart != null)
            {
                books.RemoveAll(item => !(item.PublishingDate >= DateTime.Parse(publishingDateStart)));
            }
            if (publishingDateEnd != null)
            {
                books.RemoveAll(item => !(item.PublishingDate >= DateTime.Parse(publishingDateEnd)));
            }
            #pragma warning restore CS8602 // Dereference of a possibly null reference.
            return books;
        }
    }
}

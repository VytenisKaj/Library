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

        // May not be needed if better authrization solution will be implemented
        public SetResponse AddUser(User user)
        {
            user.Id = PushObject("Users/", user).Result.name;
            return SetUser(user);
        }

        public SetResponse SetUser(User user)
        {
            return client.Set("Users/" + user.Id, user);
        }

        public User? FindUserByEmail(string email)
        {
            FirebaseResponse response = client.Get("Users");
            dynamic? data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            if (data != null)
            {
                foreach (var item in data)
                {
                    User? user = JsonConvert.DeserializeObject<User>(((JProperty)item).Value.ToString());
                    if(user != null && user.Email == email)
                    {
                        return user;
                    }
                }

            }
            return null;
            
        }

        public SetResponse AddBook(Book book)
        {
            PushResponse response = PushObject("Books/", book);
            book.Id = response.Result.name;
            return SetBook(book);
        }

        public PushResponse PushObject<T>(string location, T obj)
        {
            return client.Push(location, obj);
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

        public List<Book?> SearchBook(string? title, string? author, string? publisher, string? publishingDateStart, string? publishingDateEnd, string? genre, string? isbn, string? availability)
        {
            List<Book?> books = GetAllBooks();
            books.RemoveAll(item => item == null);
            #pragma warning disable CS8602 // Dereference of a possibly null reference. item cannot be null, all of null cases were removed in line above(line 108)
            if (title != null)
            {
                books.RemoveAll(item => item.Title == null || !item.Title.ToLower().Contains(title.ToLower()));
            }
            if (author != null)
            {
                books.RemoveAll(item => item.Author == null || !item.Author.ToLower().Contains(author.ToLower()));
            }
            if (publisher != null)
            {
                books.RemoveAll(item => item.Publisher == null || !item.Publisher.ToLower().Contains(publisher.ToLower()));
            }
            if (genre != null)
            {
                books.RemoveAll(item => item.Genre == null || !item.Genre.ToLower().Contains(genre.ToLower()));
            }
            if (isbn != null)
            {
                books.RemoveAll(item => item.Isbn == null || !item.Isbn.ToLower().Contains(isbn.ToLower()));
            }
            if(availability != null)
            {
                if(availability == "available")
                {
                    books.RemoveAll(item => item.IsAvailable == false);
                }
                else
                {
                    books.RemoveAll(item => item.IsAvailable == true);
                }
            }
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

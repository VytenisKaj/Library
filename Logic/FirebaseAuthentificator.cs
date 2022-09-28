using Firebase.Auth;


namespace Library.Logic
{
    public class FirebaseAuthentificator
    {
        const string API_KEY = "AIzaSyCXb8fAYgmQ3E7ErQvbhDkq2kdtvJaabyo";
        readonly FirebaseAuthProvider auth;
        FirebaseDataManager dataManager;

        public FirebaseAuthentificator()
        {
            auth = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));
            dataManager = new FirebaseDataManager();
        }

        public async Task<bool> CreateUser(Models.User user, HttpContext httpContext)
        {
            await auth.CreateUserWithEmailAndPasswordAsync(user.Email, user.Password);
            var result = dataManager.AddUser(user);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await LoginUser(user.Email ?? "", user.Password ?? "", httpContext);
            }
            return false;
        }

        public async Task<bool> LoginUser(string email, string password, HttpContext httpContext)
        {
            FirebaseAuthLink firebaseAuthLink = await auth.SignInWithEmailAndPasswordAsync(email, password);
            string token = firebaseAuthLink.FirebaseToken;
            if (token != null)
            {
                Models.User? user = dataManager.FindUserByEmail(email);
                if (user == null)
                {
                    return false;
                }
                httpContext.Session.SetString("email", user.Email);
                if (user.IsAdmin)
                {
                    httpContext.Session.SetString("role", "admin");
                }
                else
                {
                    httpContext.Session.Remove("role");
                }
                return true;
            }
            return false;
        }

    }

}

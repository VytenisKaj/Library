using Firebase.Auth;


namespace Library.Logic
{
    public class FirebaseAuthentificator
    {
        const string API_KEY = "AIzaSyCXb8fAYgmQ3E7ErQvbhDkq2kdtvJaabyo";
        readonly FirebaseAuthProvider auth;

        public FirebaseAuthentificator()
        {
            auth = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));
        }

        public async Task CreateUser(string email, string password, HttpContext httpContext)
        {
            await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            await LoginUser(email, password, httpContext);
        }

        public async Task LoginUser(string email, string password, HttpContext httpContext)
        {
            FirebaseAuthLink firebaseAuthLink = await auth.SignInWithEmailAndPasswordAsync(email, password);
            string token = firebaseAuthLink.FirebaseToken;
            if (token != null)
            {
                httpContext.Session.SetString("userToken", token);
            }
        }

    }

}

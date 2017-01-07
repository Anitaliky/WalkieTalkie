using System.Text.RegularExpressions;

namespace WalkieTalkieServer
{
    public static class Validator
    {
        private const int min_UsernameLen = 4;
        private const int max_UsernameLen = 10;
        private const int min_PasswordLen = 8;
        private const int max_PasswoedLen = 20;
        private const string pattern = @"^\W+$";     //any non-word or non-number character

        public static bool IsValidUsername(string username)
        {
            Regex rgx = new Regex(pattern);
            return (username.Length >= min_UsernameLen && username.Length <= max_UsernameLen) &&
                !rgx.IsMatch(username);
        }

        public static bool IsValidPassword(string password)
        {
            Regex rgx = new Regex(pattern);
            return (password.Length >= min_PasswordLen && password.Length <= max_PasswoedLen) &&
                !rgx.IsMatch(password);
        }
    }
}

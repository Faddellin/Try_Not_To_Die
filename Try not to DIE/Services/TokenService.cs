namespace Try_not_to_DIE.Services
{
    public class TokenService
    {
        private static readonly HashSet<string> _blacklist = new HashSet<string>();

        public void BlacklistToken(string token)
        {
            _blacklist.Add(token);
        }

        public bool IsTokenValid(string token)
        {
            return !_blacklist.Contains(token);
        }
    }
}

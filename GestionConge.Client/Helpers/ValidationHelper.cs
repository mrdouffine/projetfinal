namespace GestionConge.Client.Helpers
{
    public static class ValidationHelper
    {
        /// <summary>
        /// Valide un email
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Valide la complexité d'un mot de passe
        /// </summary>
        public static (bool IsValid, List<string> Errors) ValidatePassword(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Le mot de passe est requis");
                return (false, errors);
            }

            if (password.Length < 8)
                errors.Add("Le mot de passe doit contenir au moins 8 caractères");

            if (!password.Any(char.IsUpper))
                errors.Add("Le mot de passe doit contenir au moins une majuscule");

            if (!password.Any(char.IsLower))
                errors.Add("Le mot de passe doit contenir au moins une minuscule");

            if (!password.Any(char.IsDigit))
                errors.Add("Le mot de passe doit contenir au moins un chiffre");

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                errors.Add("Le mot de passe doit contenir au moins un caractère spécial");

            return (errors.Count == 0, errors);
        }
    }

}

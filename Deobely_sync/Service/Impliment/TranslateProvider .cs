using Auto_Api.Service.Interface;
using Google.Cloud.Translate.V3;
using Nito.AsyncEx;

namespace Auto_Api.Service.Impliment
{
    public class TranslateProvider :ITranslateProvider
    {

        private static readonly AsyncLazy<TranslationServiceClient> _client = new(
            () => TranslationServiceClient.CreateAsync());

        public async Task<string> ExecuteAsync(
            string text, string language)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }
            string credential_path = @".\Service\Impliment\key.json";
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);
            if (string.IsNullOrWhiteSpace(language))
            {
                throw new ArgumentException();
            }

            TranslationServiceClient client = await _client;
            var response = await client.TranslateTextAsync(
                $"projects/upbeat-medley-378009", language, new string[] { text });

            // Will always contain a single entry
            return response.Translations.ToString();
        }

    }
}

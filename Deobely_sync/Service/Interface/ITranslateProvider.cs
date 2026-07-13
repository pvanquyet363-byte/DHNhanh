namespace Auto_Api.Service.Interface
{
    public interface ITranslateProvider
    {
        Task<string> ExecuteAsync(
            string text, string language);
    }
}

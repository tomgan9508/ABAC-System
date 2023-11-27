namespace Common.Application.Clients
{
    /// <summary>
    /// An extension of the HttpClient that allows
    /// for http requests to be made.
    /// </summary>
    public interface IHttpClientExtension
    {
        Task<T> PostAsync<T>(Uri uri, string serializedBody);
    }
}

namespace Bars.Linq.Async
{
    public interface IAsyncEnumerable<T>
    {
        IAsyncEnumerator<T> GetAsyncEnumerator();
    }
}
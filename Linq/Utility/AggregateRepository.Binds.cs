namespace BarsUp.Designer.Workspace.NuGet.Utility
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class AggregateRepository
    {
        /// <summary>
        /// Функтор агрегатор коллекций в одну
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collections"></param>
        /// <returns></returns>
		protected IEnumerable<T> Collection<T>(IEnumerable<IEnumerable<T>> collections)
        {
            var list = new List<T>();
            foreach (var collection in collections)
            {
                list.AddRange(collection);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Функтор агрегатор предикатов в успех при хотя бы одном успешном результате
        /// </summary>
        /// <param name="vs"></param>
        /// <returns></returns>
        protected bool OneIsSuccess(IEnumerable<bool> vs)
        {
            return vs.Any(x => x == true);
        }

        /// <summary>
        /// Функтор агрегатор берёт первый результат
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        protected T First<T>(IEnumerable<T> ts)
        {
            return ts.FirstOrDefault();
        }

        /// <summary>
        /// Синхронно
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        protected T Sync<T>(Task<T> task)
        {
            task.Wait();
            return task.Result;
        }
    }
}

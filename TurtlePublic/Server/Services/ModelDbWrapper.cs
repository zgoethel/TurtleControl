using Generated;

namespace TurtlePublic.Services;

public class ModelDbWrapper : IModelDbWrapper
{
    public async Task ExecuteAsync(Func<Task> impl)
    {
        //TODO
        await impl();
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> impl)
    {
        //TODO
        return await impl();
    }
}

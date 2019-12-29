using System.Threading.Tasks;

namespace FeatureServices.Interfaces
{
    public interface IFeature
    {
        Task Load(object request, IRouter router);
        Task Unload();
    }

    public interface IFeature<TFeatureRequest> : IFeature
    {
        Task Load(TFeatureRequest request, IRouter router);
    }
}

using System.Threading.Tasks;

namespace FeatureServices.Interfaces
{
    public interface IRouter
    {
        void AddFeature<TRequest, TFeatureRequestHandler>() where TFeatureRequestHandler : IFeature<TRequest>;

        Task<IFeature> Navigate<TRequest>(TRequest request);
        Task<IFeature> ReplaceCurrent<TRequest>(TRequest request);
        Task<IFeature> Back();

        void ClearHistory();

        void SaveState<TFeatureRequestHandler, TState>(TState state) where TFeatureRequestHandler : IFeature;
        TState GetState<TFeatureRequestHandler, TState>() where TFeatureRequestHandler : IFeature;
    }
}

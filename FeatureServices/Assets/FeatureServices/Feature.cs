using FeatureServices.Interfaces;
using System;
using System.Threading.Tasks;

namespace FeatureServices
{
    public class Feature : IFeature
    {
        public virtual Task Load(object request, IRouter router)
        {
            throw new NotImplementedException();
        }

        public virtual Task Unload()
        {
            return Task.CompletedTask;
        }
    }

    public class Feature<TRequest> : Feature, IFeature<TRequest>
    {
        public override Task Load(object request, IRouter router)
        {
            return this.Load((TRequest)request, router);
        }

        public virtual Task Load(TRequest request, IRouter router)
        {
            throw new NotImplementedException();
        }
    }
}

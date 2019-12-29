using MicroDI.Interfaces;
using FeatureServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeatureServices
{
    public class Router : IRouter
    {
        private readonly Dictionary<Type, Type> _featureHandlerBindings = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, object> _featureStates = new Dictionary<Type, object>();
        private readonly IContainer _container;

        private readonly Stack<object> _requestStack = new Stack<object>();
        private IFeature _currentFeature;

        public Router(IContainer container)
        {
            _container = container;
        }

        public void AddFeature<TRequest, TFeatureRequestHandler>() where TFeatureRequestHandler : IFeature<TRequest>
        {
            _featureHandlerBindings[typeof(TRequest)] = typeof(TFeatureRequestHandler);
        }

        public Task<IFeature> Navigate<TRequest>(TRequest request)
        {
            return OnRoute(request);
        }

        private async Task<IFeature> OnRoute(object request)
        {
            if (!_featureHandlerBindings.ContainsKey(request.GetType()))
            {
                throw new Exception("Router: Request type not bound!");
            }

            if(_currentFeature != null)
            {
                await _currentFeature.Unload();
            }

            _requestStack.Push(request);
            _currentFeature = (IFeature)_container.Resolve(_featureHandlerBindings[request.GetType()]);
            await _currentFeature.Load(request, this);
            return _currentFeature;
        }

        public async Task<IFeature> Back()
        {
            // Unload and pop the current...
            if (_currentFeature != null)
            {
                await _currentFeature.Unload();
                _currentFeature = null;
                _requestStack.Pop();
            }

            // Peek and load the current...
            if (_requestStack.Count > 0)
            {
                var request = _requestStack.Peek();
                return await OnRoute(request);
            }

            return null;
        }

        public async Task<IFeature> ReplaceCurrent<TRequest>(TRequest request)
        {
            // Unload and pop the current...
            if(_currentFeature != null)
            {
                await _currentFeature.Unload();
                _currentFeature = null;
                _requestStack.Pop();
            }

            return await OnRoute(request);
        }

        public void ClearHistory()
        {
            _requestStack.Clear();

            // Stack should retain current feature...
            if(_currentFeature != null)
            {
                _requestStack.Push(_currentFeature);
            }
        }

        #region States
        public void SaveState<TFeatureRequestHandler, TState>(TState state) where TFeatureRequestHandler : IFeature
        {
            _featureStates[typeof(TFeatureRequestHandler)] = state;
        }

        public TState GetState<TFeatureRequestHandler, TState>() where TFeatureRequestHandler : IFeature
        {
            if (!_featureStates.ContainsKey(typeof(TFeatureRequestHandler)))
            {
                throw new Exception("Router: Request type not bound!");
            }

            return (TState)_featureStates[typeof(TFeatureRequestHandler)];
        }
        #endregion
    }
}

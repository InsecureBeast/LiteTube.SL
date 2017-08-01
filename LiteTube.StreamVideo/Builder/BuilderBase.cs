using System;
using System.Threading;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.Builder
{
    public class BuilderBase<TBuild> : BuilderBase, IBuilder<TBuild>, IBuilder, IDisposable
    {
        private IBuilderHandle<TBuild> _handle;

        protected BuilderBase(params IModule[] modules)
        {
            if (modules == null || modules.Length <= 0)
                return;
            foreach (IModule module in modules)
                ModuleRegistrationExtensions.RegisterModule(this.ContainerBuilder, module);
        }

        public TBuild Create()
        {
            if (null != this._handle)
                throw new InvalidOperationException("The builder is in use");
            BuilderHandle<TBuild> builderHandle = new BuilderHandle<TBuild>(this.Container.BeginLifetimeScope((object)"builder-scope"));
            if (null != Interlocked.CompareExchange<IBuilderHandle<TBuild>>(ref this._handle, (IBuilderHandle<TBuild>)builderHandle, (IBuilderHandle<TBuild>)null))
            {
                DisposeExtensions.DisposeSafe((IDisposable)builderHandle);
                throw new InvalidOperationException("The builder is in use");
            }
            return builderHandle.Instance;
        }

        public void Destroy(TBuild instance)
        {
            IBuilderHandle<TBuild> builderHandle = Interlocked.Exchange<IBuilderHandle<TBuild>>(ref this._handle, (IBuilderHandle<TBuild>)null);
            if (null == builderHandle)
                throw new InvalidOperationException("No handle");
            if (!object.ReferenceEquals((object)instance, (object)builderHandle.Instance))
                throw new InvalidOperationException("Wrong instance");
            DisposeExtensions.DisposeSafe((IDisposable)builderHandle);
        }
    }

    public abstract class BuilderBase : IBuilder, IDisposable
    {
        private readonly ContainerBuilder _containerBuilder = new ContainerBuilder();
        private readonly object _lock = new object();
        private IContainer _container;
        private bool _isDirty;
        private int _isDisposed;

        public ContainerBuilder ContainerBuilder
        {
            get
            {
                if (null != this._container)
                    throw new InvalidOperationException("The builder is in use");
                return this._containerBuilder;
            }
        }

        protected IContainer Container
        {
            get
            {
                IContainer container1 = (IContainer)null;
                while (true)
                {
                    bool lockTaken = false;
                    object obj = null;
                    try
                    {
                        Monitor.Enter(obj = this._lock, ref lockTaken);
                        IContainer container2 = this._container;
                        if (this._isDirty)
                        {
                            this.LockedCleanupContainer();
                            container2 = (IContainer)null;
                        }
                        if (null != container2)
                        {
                            if (null != container1)
                                DisposeExtensions.DisposeSafe((IDisposable)container1);
                            return container2;
                        }
                        if (null != container1)
                        {
                            this._container = container1;
                            return container1;
                        }
                    }
                    finally
                    {
                        if (lockTaken)
                            Monitor.Exit(obj);
                    }
                    container1 = this.ContainerBuilder.Build(ContainerBuildOptions.None);
                }
            }
        }

        public void Register<TService, TImplementation>() where TImplementation : TService
        {
            this.ChangeBuilder();
            Autofac.RegistrationExtensions.RegisterType<TImplementation>(this.ContainerBuilder).As<TService>();
        }

        public void RegisterSingleton<TService, TImplementation>() where TImplementation : TService
        {
            this.ChangeBuilder();
            Autofac.RegistrationExtensions.RegisterType<TImplementation>(this.ContainerBuilder).As<TService>().SingleInstance();
        }

        public void RegisterSingleton<TService>(TService instance) where TService : class
        {
            this.ChangeBuilder();
            Autofac.RegistrationExtensions.RegisterInstance<TService>(this.ContainerBuilder, instance).ExternallyOwned();
        }

        public void RegisterSingletonFactory<TService>(Func<TService> factory)
        {
            this.ChangeBuilder();
            Autofac.RegistrationExtensions.Register<TService>(this.ContainerBuilder, (Func<IComponentContext, TService>)(ctx => factory())).SingleInstance();
        }

        public void RegisterTransientFactory<TService>(Func<TService> factory)
        {
            this.ChangeBuilder();
            Autofac.RegistrationExtensions.Register<TService>(this.ContainerBuilder, (Func<IComponentContext, TService>)(ctx => factory())).ExternallyOwned();
        }

        public void Dispose()
        {
            if (0 != Interlocked.Exchange(ref this._isDisposed, 0))
                return;
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        private void LockedCleanupContainer()
        {
            this._isDirty = false;
            IContainer container = this._container;
            if (null == container)
                return;
            this._container = (IContainer)null;
            DisposeExtensions.DisposeSafe((IDisposable)container);
        }

        private void ChangeBuilder()
        {
            if (0 != this._isDisposed)
                throw new ObjectDisposedException(this.GetType().FullName);
            bool lockTaken = false;
            object obj = null;
            try
            {
                Monitor.Enter(obj = this._lock, ref lockTaken);
                if (null != this._container)
                    throw new InvalidOperationException("The builder is in use");
                this._isDirty = true;
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(obj);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            bool lockTaken = false;
            object obj = null;
            IContainer container;
            try
            {
                Monitor.Enter(obj = this._lock, ref lockTaken);
                container = this._container;
                this._container = (IContainer)null;
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(obj);
            }
            using (container)
                ;
        }
    }
}

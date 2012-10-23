﻿using System;
using FubuCore;
using FubuCore.Binding;


namespace StoryTeller.Engine
{
    public interface IExecutionContext : IDisposable
    {
        IServiceLocator Services { get; }
        BindingRegistry BindingRegistry { get; }
    }

    public class SimpleExecutionContext : IExecutionContext
    {
        public SimpleExecutionContext()
        {
            Services = new InMemoryServiceLocator();
            BindingRegistry = new BindingRegistry();
        }

        void IDisposable.Dispose()
        {
            
        }

        IServiceLocator IExecutionContext.Services { get { return Services; } }

        public InMemoryServiceLocator Services { get; private set; }


        public BindingRegistry BindingRegistry { get; private set; }
    }

    public interface ISystem : IDisposable
    {
        IExecutionContext CreateContext();
        void Recycle();
    }
}
using System;

namespace DSerfozo.RpcBindings.Contract
{
    public class ResolvingBoundObjectArgs : EventArgs
    {
        public string Name { get; }

        public object Object { get; set; }

        public bool Disposable { get; set; } = true;

        public ResolvingBoundObjectArgs(string name)
        {
            Name = name;
        }
    }
}

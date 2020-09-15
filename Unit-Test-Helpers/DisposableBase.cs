using System;
namespace UnitTestHelpers
{
    public abstract class DisposableBase : IDisposable
    {
        public bool Disposed { get; private set; }

        public void Dispose()
            => Dispose(true);

        protected void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                Disposed = true;

                if (disposing)
                {
                    DisposeExplicit();
                }

                DisposeImplicit();

                GC.SuppressFinalize(this);
            }
        }

        //Called when Disposed by calling Dispose()
        protected virtual void DisposeExplicit() { }
        //Called when automatically disposed by GC
        protected virtual void DisposeImplicit() { }

        ~DisposableBase()
        {
            Dispose(false);
        }
    }
}

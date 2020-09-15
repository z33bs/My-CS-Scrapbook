namespace UnitTestHelpers
{
        class ClassThatsDisposable : DisposableBase
        {
            private Action _onExplicitDispose;
            private Action _onImplicitDispose;

            public ClassThatsDisposable(Action onExplicitDispose, Action onImplicitDispose)
            {
                _onExplicitDispose = onExplicitDispose;
                _onImplicitDispose = onImplicitDispose;
            }

            protected override void DisposeExplicit()
                => _onExplicitDispose?.DynamicInvoke();
            protected override void DisposeImplicit()
                => _onImplicitDispose?.DynamicInvoke();
        }
}

using Kirinji.ReactiveTree.TreeStructures;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace Kirinji.ReactiveTree
{
    public class ReactiveDirectoryValue : IReactiveDirectoryValue<KeyOrIndex<string>>
    {
        private ICollection<KeyArray<KeyOrIndex<string>>> directories;
        private IReadOnlyDictionary<KeyArray<KeyOrIndex<string>>, IReadOnlyTreeElement<string, IDataObject>> injected;

        public TValue GetValue<TValue>(KeyArray<KeyOrIndex<string>> directory)
        {
            if (Mode == ReactiveDirectoryValueMode.GetDirectoryMode)
            {
                this.directories.Add(directory);
                return default(TValue);
            }
            else if (Mode == ReactiveDirectoryValueMode.ConvertingMode)
            {
                if (injected == null) throw new InvalidOperationException("Injected == null");
                IReadOnlyTreeElement<string, IDataObject> t;
                if (!injected.TryGetValue(directory, out t)) return default(TValue);
                if (t == null || t.Type != ElementType.Leaf) return default(TValue);
                return t.LeafValue.CastOrDefault<TValue>();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.directories == null ^ this.Mode == ReactiveDirectoryValueMode.GetDirectoryMode);
            Contract.Invariant(this.injected == null ^ this.Mode == ReactiveDirectoryValueMode.ConvertingMode);
        }

        public IDisposable EnterGetDirectoryMode(ICollection<KeyArray<KeyOrIndex<string>>> directories)
        {
            Contract.Requires<ArgumentNullException>(directories != null);

            this.directories = directories;
            this.Mode = ReactiveDirectoryValueMode.GetDirectoryMode;
            return System.Reactive.Disposables.Disposable.Create(() =>
            {
                this.Mode = ReactiveDirectoryValueMode.None;
                this.directories = null;
            });
        }

        public IDisposable EnterConvertingMode(IReadOnlyDictionary<KeyArray<KeyOrIndex<string>>, IReadOnlyTreeElement<string, IDataObject>> injected)
        {
            Contract.Requires<ArgumentNullException>(injected != null);

            this.injected = injected;
            this.Mode = ReactiveDirectoryValueMode.ConvertingMode;
            return System.Reactive.Disposables.Disposable.Create(() =>
            {
                this.Mode = ReactiveDirectoryValueMode.None;
                this.injected = null;
            });
        }

        public ReactiveDirectoryValueMode Mode { get; private set; }

        public TValue GetValue<TValue>(IEnumerable<KeyOrIndex<string>> directory)
        {
            return GetValue<TValue>(new KeyArray<KeyOrIndex<string>>(directory));
        }

        public TValue GetValue<TValue>(params KeyOrIndex<string>[] directory)
        {
            return GetValue<TValue>(directory.AsEnumerable());
        }
    }

    public enum ReactiveDirectoryValueMode
    {
        None,
        GetDirectoryMode,
        ConvertingMode,
    }

    public class ReactiveDirectory : IReactiveDirectory<KeyOrIndex<string>>
    {
        readonly ISimpleReactiveDictionary<KeyArray<KeyOrIndex<string>>, IReadOnlyTreeElement<string, IDataObject>> inner;

        public ReactiveDirectory(ISimpleReactiveDictionary<KeyArray<KeyOrIndex<string>>, IReadOnlyTreeElement<string, IDataObject>> inner)
        {
            Contract.Requires<ArgumentNullException>(inner != null);

            this.inner = inner;
        }

        public IObservable<T> ObserveChanges<T>(Func<IReactiveDirectoryValue<KeyOrIndex<string>>, T> selector)
        {
            var m = new ReactiveDirectoryValue();

            IObservable<IReadOnlyDictionary<KeyArray<KeyOrIndex<string>>, IReadOnlyTreeElement<string, IDataObject>>> obs;
            var directories = new List<KeyArray<KeyOrIndex<string>>>();
            using (m.EnterGetDirectoryMode(directories))
            {
                var t = selector(m);
                obs = inner.ValuesChanged(directories);
            }

            return obs
                .Select(x =>
                {
                    using (m.EnterConvertingMode(x))
                    {
                        return selector(m);
                    }
                });
        }

        public TValue GetValue<TValue>(KeyArray<KeyOrIndex<string>> directory)
        {
            var t = inner.Value(directory).Value;
            if (t == null || t.Type != ElementType.Leaf) return default(TValue);
            return t.LeafValue.CastOrDefault<TValue>();
        }

        public TValue GetValue<TValue>(IEnumerable<KeyOrIndex<string>> directory)
        {
            return GetValue<TValue>(new KeyArray<KeyOrIndex<string>>(directory));
        }

        public TValue GetValue<TValue>(params KeyOrIndex<string>[] directory)
        {
            return GetValue<TValue>(directory.AsEnumerable());
        }

        public IObservable<T> ObserveChanges<T>(Func<IReactiveDictionaryValue<KeyArray<KeyOrIndex<string>>>, T> selector)
        {
            return ObserveChanges(selector);
        }
    }
}

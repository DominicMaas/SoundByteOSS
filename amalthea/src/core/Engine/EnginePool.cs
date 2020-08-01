using System;
using System.Collections.Concurrent;

namespace SoundByte.Core.Engine
{
    /// <summary>
    ///     Provides a pool of engines for a music provider to use
    /// </summary>
    public class EnginePool : IDisposable
    {
        private readonly ConcurrentQueue<JSEngine> _cache = new ConcurrentQueue<JSEngine>();
        private readonly EnginePoolConfig _config;

        public int EngineCount => _cache.Count;

        public EnginePool(EnginePoolConfig? config = null)
        {
            _config = config ?? new EnginePoolConfig();

            for (var i = 0; i < _config.StartEngines; i++)
            {
                _cache.Enqueue(new JSEngine(this));
            }
        }

        public void Dispose()
        {
            while (_cache.TryDequeue(out var engine))
            {
                engine.Dispose();
            }
        }

        public JSEngine GetEngine()
        {
            // Try get an existing engine
            if (_cache.TryDequeue(out JSEngine engine))
            {
                return engine;
            }

            // No items in queue, check if we can add more
            if (_cache.Count < _config.MaxEngines)
            {
                return new JSEngine(this);
            }

            // At max items
            throw new Exception("No more engines left in engine pool");
        }

        public class JSEngine : IDisposable
        {
            private Jint.Engine _engine;
            private readonly EnginePool _parent;

            public JSEngine(EnginePool parent)
            {
                _parent = parent;
                _engine = new Jint.Engine();
                parent._config.Initializer.Invoke(_engine);
            }

            public T Execute<T>(string source)
            {
                return (T)_engine.Execute(source).GetCompletionValue().ToObject();
            }

            public void Execute(string source)
            {
                _engine.Execute(source);
            }

            public T CallFunction<T>(string function, params object[] arguments)
            {
                return (T)_engine.Invoke(function, arguments).ToObject();
            }

            public void CallFunction(string function, params object[] arguments)
            {
                _engine.Invoke(function, arguments);
            }

            public void Dispose()
            {
                // The engine pool is above max count, don't return this engine
                if (_parent._cache.Count >= _parent._config.StartEngines)
                {
                    _engine.ResetCallStack();
                    _engine.ResetConstraints();
                    _engine = null;
                }
                else
                {
                    // Return the engine
                    _parent._cache.Enqueue(this);
                }
            }
        }

        public class EnginePoolConfig
        {
            /// <summary>
            ///     Gets or sets the number of engines to initially start when a pool is created.
            ///     Defaults to <c>5</c>.
            /// </summary>
            public int StartEngines { get; set; }

            /// <summary>
            ///     Gets or sets the maximum number of engines that will be created in the pool.
            ///     Defaults to <c>10</c>.
            /// </summary>
            public int MaxEngines { get; set; }

            /// <summary>
            ///     Gets or sets the code to run when a new engine is created. This should configure
            ///     the environment and set up any required JavaScript libraries.
            /// </summary>
            public Action<Jint.Engine> Initializer { get; set; }

            public EnginePoolConfig(Action<Jint.Engine>? initializer = null, int startEngines = 5, int maxEngines = 10)
            {
                if (initializer == null)
                    Initializer = engine => { };
                else
                    Initializer = initializer;

                StartEngines = startEngines;
                MaxEngines = maxEngines;
            }
        }
    }
}
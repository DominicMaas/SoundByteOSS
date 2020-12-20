using System;
using System.Collections.Concurrent;

namespace SoundByte.App.Uwp.Extensions.Core
{
    public class EnginePool
    {
        private readonly ConcurrentQueue<JSEngine> _cache = new ConcurrentQueue<JSEngine>();
        private readonly EnginePoolConfig _config;

        public EnginePool(EnginePoolConfig config)
        {
            _config = config;

            for (var i = 0; i < 5; i++)
            {
                _cache.Enqueue(new JSEngine(this));
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

            public T CallFunction<T>(string function, params object[] arguments) where T : class
            {
                return _engine.Invoke(function, arguments).ToObject() as T;
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
            ///     Defaults to <c>10</c>.
            /// </summary>
            public int StartEngines { get; set; }

            /// <summary>
            ///     Gets or sets the maximum number of engines that will be created in the pool.
            ///     Defaults to <c>25</c>.
            /// </summary>
            public int MaxEngines { get; set; }

            /// <summary>
            ///     Gets or sets the code to run when a new engine is created. This should configure
            ///     the environment and set up any required JavaScript libraries.
            /// </summary>
            public Action<Jint.Engine> Initializer { get; set; }

            public EnginePoolConfig(Action<Jint.Engine> initializer = null, int startEngines = 5, int maxEngines = 10)
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
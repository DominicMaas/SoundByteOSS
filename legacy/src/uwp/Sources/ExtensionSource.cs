using GalaSoft.MvvmLight.Ioc;
using Jint.Runtime;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Sources;
using SoundByte.App.Uwp.ServicesV2;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SoundByte.App.Uwp.Extensions.Core;

namespace SoundByte.App.Uwp.Sources
{
    /// <summary>
    ///     This source acts as a wrapper around extension sources.
    /// </summary>
    public class ExtensionSource : ISource
    {
        private Dictionary<string, object> _parameters;

        private readonly Extension _extension;
        private readonly string _functionName;

        public ExtensionSource(Extension extension, string functionName)
        {
            _extension = extension;
            _functionName = functionName;
        }

        public override Dictionary<string, object> GetParameters() => _parameters;

        public override void ApplyParameters(Dictionary<string, object> data) => _parameters = data;

        public override async Task<SourceResponse> GetItemsAsync(int count, string token, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (_parameters == null)
                    _parameters = new Dictionary<string, object> { { "filter", "all-music" } };

                var response = await Task.Run(() => _extension.CallFunction<SourceResponse>(_functionName, count, token, _parameters));
                if (response == null)
                    return new SourceResponse(null, null, false, "Error running extension", "The extension did not return any information. Please contact the extension author.");

                return response;
            }
            catch (JavaScriptException jsex)
            {
                SimpleIoc.Default.GetInstance<ITelemetryService>().TrackException(jsex);
                throw new SoundByteException("Error running extension", $"Error running JavaScript: {jsex.Error} at {jsex.LineNumber}:{jsex.Column}, {jsex.StackTrace}", jsex);
            }
            catch (Exception ex)
            {
                SimpleIoc.Default.GetInstance<ITelemetryService>().TrackException(ex);
                throw new SoundByteException("Error running extension", $"Error running General: {ex.Message} at {ex.StackTrace}", ex);
            }
        }
    }
}
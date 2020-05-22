using System;

namespace SoundByte.Core.Exceptions
{
    /// <summary>
    ///     Thrown when trying to access a service which does not exist
    /// </summary>
    [Serializable]
    public class ServiceNotExistException : Exception
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int ServiceType { get; set; }

        public ServiceNotExistException(int service) : base("An error occurred while trying to access the following service, it does not exist: " + service)
        {
            ServiceType = service;
        }
    }
}
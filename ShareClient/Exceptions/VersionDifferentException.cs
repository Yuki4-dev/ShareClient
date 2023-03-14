using System;

namespace ShareClient.Exceptions
{
    public class VersionDifferentException : Exception
    {
        public string ObjectName { get; }
        public int ThisVersion { get; }
        public int OtherVersion { get; }

        public VersionDifferentException(Type objType, int ver, int otherVer, Exception innerException) : base($"This Version : {ver} / Other Version : {otherVer}", innerException)
        {
            ObjectName = objType != null ? objType.ToString() : "";
            ThisVersion = ver;
            OtherVersion = otherVer;
        }

        public VersionDifferentException(Type objType, int ver, int otherVer) : base($"This Version : {ver} / Other Version : {otherVer}")
        {
            ObjectName = objType != null ? objType.ToString() : "";
            ThisVersion = ver;
            OtherVersion = otherVer;
        }
    }
}

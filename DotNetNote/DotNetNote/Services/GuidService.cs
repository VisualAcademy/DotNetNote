using System;

namespace DotNetNote.Services
{
    public class GuidService : IGuidService
    {
        public Guid GetGuid() => Guid.NewGuid();
    }

    public interface IGuidService
    {
        Guid GetGuid();
    }

    public interface ITransientGuidService : IGuidService
    {

    }
    public interface IScopedGuidService : IGuidService
    {

    }
    public interface ISingletonGuidService : IGuidService
    {

    }

    public class TransientGuidService : GuidServiceBase, ITransientGuidService
    {

    }
    public class ScopedGuidService : GuidServiceBase, IScopedGuidService
    {

    }
    public class SingletonGuidService : GuidServiceBase, ISingletonGuidService
    {

    }
    public class GuidServiceBase : IGuidService
    {
        private Guid _guid;
        public GuidServiceBase()
        {
            _guid = Guid.NewGuid(); // 개체 생성 시점의 Guid값 반환
        }
        public Guid GetGuid()
        {
            return _guid;
        }
    }
}

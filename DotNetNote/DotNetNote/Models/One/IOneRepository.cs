using System.Collections.Generic;

namespace DotNetNote.Models
{
    public interface IOneRepository
    {
        One Add(One model);
        List<One> GetAll();
    }
}

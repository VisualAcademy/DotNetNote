using System.Collections.Generic;

namespace DotNetNote.Models
{
    public interface IVariableRepository
    {
        List<Variable> GetAll();
    }
}
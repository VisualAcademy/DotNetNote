using System.Collections.Generic;

namespace DotNetNote.Models
{
    public class VariableRepositoryInMemory : IVariableRepository
    {
        public List<Variable> GetAll()
        {
            List<Variable> variables = new List<Variable>()
            {
                new Variable { Id = 1, Text = "한국어", Value = "KO" },
                new Variable { Id = 2, Text = "영어", Value = "EN" }
            };
            return variables; 
        }
    }
}

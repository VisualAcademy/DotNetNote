using System.Collections.Generic;

namespace DotNetNote.Models
{
    public interface IFluctuationStateRepository
    {
        List<FluctuationStateDto> GetFluctuationStates();
    }
}
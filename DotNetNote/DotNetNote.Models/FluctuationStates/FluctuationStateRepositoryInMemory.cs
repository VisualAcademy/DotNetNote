using System.Collections.Generic;

namespace DotNetNote.Models;

public class FluctuationStateRepositoryInMemory : IFluctuationStateRepository
{
    public List<FluctuationStateDto> GetFluctuationStates()
    {
        List<FluctuationStateDto> r = new List<FluctuationStateDto>()
        {
            new FluctuationStateDto { Text = "증가", Value = "0"},
            new() { Text = "감소", Value = "1"},
            new FluctuationStateDto { Text = "고정", Value = "2"},
            new FluctuationStateDto { Text = "예측", Value = "3"},
        };

        return r;
    }
}

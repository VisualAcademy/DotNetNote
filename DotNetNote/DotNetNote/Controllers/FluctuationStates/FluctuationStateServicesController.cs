namespace DotNetNote.Controllers;

public class FluctuationStateServicesController : Controller
{
    private readonly IFluctuationStateRepository _repository;

    public FluctuationStateServicesController() => _repository = new FluctuationStateRepositoryInMemory();

    [HttpPost]
    [Route("api/FluctuationStateServices/GetFluctuationState")]
    public List<FluctuationStateDto> GetFluctuationState()
    {
        #region +
        //List<FluctuationStateDto> r = new List<FluctuationStateDto>()
        //{
        //    new FluctuationStateDto { Text = "증가", Value = "0"},
        //    new FluctuationStateDto { Text = "감소", Value = "1"},
        //    new FluctuationStateDto { Text = "고정", Value = "2"},
        //};

        //return r; 
        #endregion
        return _repository.GetFluctuationStates();
    }
}

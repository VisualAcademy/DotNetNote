using DotNetNote.Models.Buyers;

namespace DotNetNote.Controllers;

public class BuyerTestController : Controller
{
    private readonly IBuyerRepository buyerRepository;

    public BuyerTestController(IBuyerRepository buyerRepository) => this.buyerRepository = buyerRepository;

    public IActionResult Index()
    {
        var buyers = buyerRepository.GetBuyers();
        return View(buyers);
    }

    public IActionResult Details(string buyerId)
    {
        var buyer = buyerRepository.GetBuyer(buyerId);
        return View(buyer);
    }
}

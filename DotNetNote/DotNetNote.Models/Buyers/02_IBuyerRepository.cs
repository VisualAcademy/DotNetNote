using System.Collections.Generic;

namespace DotNetNote.Models.Buyers;

public interface IBuyerRepository
{
    List<Buyer> GetBuyers();
    Buyer GetBuyer(string buyerId);
}

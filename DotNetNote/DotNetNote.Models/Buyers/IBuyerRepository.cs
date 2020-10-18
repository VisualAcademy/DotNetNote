using System.Collections.Generic;

namespace DotNetNote.Models
{
    public interface IBuyerRepository
    {
        List<Buyer> GetBuyers();
        Buyer GetBuyer(string buyerId);
    }
}

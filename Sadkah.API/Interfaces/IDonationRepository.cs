using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.API.Interfaces
{
    public interface IDonationRepository
    {
        Task<PagedResult<Donation>> GetAllDonationsAsync(QueryObject query);
        Task<Donation?> GetDonationByIdAsync(Guid id);
        Task<Donation> CreateDonationAsync(Donation donation);
        Task<Donation?> UpdateAnonymousDonationAsync(Guid id, bool isAnonymous);
    }
}
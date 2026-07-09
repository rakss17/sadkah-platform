using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.Web.Interfaces
{
    public interface IDonationService
    {
        Task<ServiceResult<DonationModel>> CreateDonationAsync(DonationModel campaign);
    }
}
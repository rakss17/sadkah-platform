using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.API.Repository
{
    public class DonationRepository : IDonationRepository
    {
        private readonly ApplicationDBContext _context;
        public DonationRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Donation>> GetAllDonationsAsync(QueryObject query)
        {
            var donationsQuery = _context.Donations.Include(d => d.Donor).AsQueryable();

            var totalCount = await donationsQuery.CountAsync();

            var pageSize = query.PageSize > 0 ? query.PageSize : 10;
            var pageNumber = query.PageNumber > 0 ? query.PageNumber : 1;

            var items = await donationsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Donation>
            {
                Items = items,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<Donation?> GetDonationByIdAsync(Guid id)
        {
            return await _context.Donations.Include(d => d.Donor).FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Donation> CreateDonationAsync(Donation donation)
        {
            await _context.Donations.AddAsync(donation);
            await _context.SaveChangesAsync();

            var createdDonation = await _context.Donations.Include(d => d.Donor).FirstOrDefaultAsync(d => d.Id == donation.Id);

            return createdDonation!;
        }

        public async Task<Donation?> UpdateAnonymousDonationAsync(Guid id, bool isAnonymous)
        {
            var donationModel = await _context.Donations.FirstOrDefaultAsync(d => d.Id == id);

            if (donationModel == null) return null;

            donationModel.IsAnonymous = isAnonymous;
            await _context.SaveChangesAsync();
            return donationModel;
        }
    }
}
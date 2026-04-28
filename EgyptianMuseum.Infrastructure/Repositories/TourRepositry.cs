using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Infrastructure.Repositories
{
    public class TourRepository : ITourRepository
    {
        private readonly AppDbContext _context;
        public TourRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Tour>> GetAllAsync()
        {
            return await _context.Tours
               .Include(t => t.RoomTours)
                   .ThenInclude(rt => rt.Room)
               .ToListAsync();
        }
        public async Task<Tour?> GetByIdAsync(int id)
        {
            return await _context.Tours
                .Include(t => t.RoomTours)
                    .ThenInclude(rt => rt.Room)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        public async Task<UserTour?> GetUserTourAsync(int tourId, string userId)
        {
            return await _context.UserTours
                .FirstOrDefaultAsync(ut => ut.TourId == tourId && ut.UserId == userId);
        }

        public async Task AddUserTourAsync(UserTour userTour)
        {
            await _context.UserTours.AddAsync(userTour);
        }

        public void DeleteUserTour(UserTour userTour)
        {
            _context.UserTours.Remove(userTour);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Tour>> GetFilteredToursAsync(double estimatedTime, int rooms)
        {
            var estimatedMinutes = estimatedTime * 60;

            return await _context.Tours
                .Include(t => t.RoomTours)
                .ThenInclude(rt => rt.Room)
                .Where(t =>
                    EF.Functions.DateDiffMinute(t.StartTime, t.EndTime) <= estimatedMinutes &&
                    t.RoomTours.Count <= rooms
                )
                .OrderByDescending(t => t.RoomTours.Count) // 
                .ThenByDescending(t =>
                    EF.Functions.DateDiffMinute(t.StartTime, t.EndTime)
                ) 
                .ToListAsync();
        }
    }
}

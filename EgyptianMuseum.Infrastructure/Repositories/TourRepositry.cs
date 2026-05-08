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
            await _context.SaveChangesAsync();
        }

        public void DeleteUserTour(UserTour userTour)
        {
            _context.UserTours.Remove(userTour);
            _context.SaveChanges();
        }



        public async Task<List<Tour>> GetFilteredToursAsync(int estimatedTime, int rooms, string category)
        {
            if (string.IsNullOrEmpty(category))
                throw new ArgumentException("Category is required");

            return await _context.Tours
                  .Include(t => t.RoomTours)
                  .ThenInclude(rt => rt.Room)
                    .Where(t =>
                        t.Category == category &&
                        t.DurationInMinutes <= estimatedTime &&
                        t.RoomTours.Count <= rooms
                     )
                  .OrderByDescending(t => t.RoomTours.Count)
                  .ThenByDescending(t => t.DurationInMinutes)
                  .ToListAsync();
        }
        public async Task AddAsync(Tour tour)
        {
            await _context.Tours.AddAsync(tour);
            await _context.SaveChangesAsync();
        }
    }
}

     
    
 

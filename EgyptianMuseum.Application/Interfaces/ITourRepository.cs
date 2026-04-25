using EgyptianMuseum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface ITourRepository
    {
        Task<List<Tour>> GetAllAsync();
        Task<Tour?> GetByIdAsync(int id);
        Task<UserTour?> GetUserTourAsync(int tourId, string userId);
        Task AddUserTourAsync(UserTour userTour);
        void DeleteUserTour(UserTour userTour);
        Task<List<Tour>> GetFilteredToursAsync(double estimatedTime, int rooms);
        Task SaveChangesAsync();
    }
}

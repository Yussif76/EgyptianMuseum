using EgyptianMuseum.Application.DTOs.DtoTour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface ITourService
    {
        Task<List<TourDto>> GetAllToursAsync(int estimatedTime, int rooms, string category);
        Task<TourDto?> GetTourByIdAsync(int id);
        Task<bool> StartTourAsync(int tourId, string userId);
        Task<bool> CancelTourAsync(int tourId, string userId);
        Task<int> CreateTourAsync(CreateTourDto dto);
    
    }
}

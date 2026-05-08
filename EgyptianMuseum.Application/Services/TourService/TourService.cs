using EgyptianMuseum.Application.DTOs.DtoRoom;
using EgyptianMuseum.Application.DTOs.DtoTour;
using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Application.Services.TourService
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;

        public TourService(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;
        }

        public async Task<List<TourDto>> GetAllToursAsync(int estimatedTime, int rooms, string category)
        {



            var tours = await _tourRepository.GetFilteredToursAsync(estimatedTime, rooms, category);

            return tours.Select(t => new TourDto
            {
                Id = t.Id,
                Description = t.Description,
                Category = t.Category,
                DurationInMinutes = t.DurationInMinutes,
                Rooms = t.RoomTours.Select(rt => new RoomTourDto
                {
                    Id = rt.Room.Id,
                    Name = rt.Room.Name

                }).ToList()



            }).ToList();
        
        }


        public async Task<TourDto?> GetTourByIdAsync(int id)
        {
            var tour = await _tourRepository.GetByIdAsync(id);

            if (tour == null) return null;

            return new TourDto
            {
                Id = tour.Id,
                Category = tour.Category,
                Description = tour.Description,
               DurationInMinutes = tour.DurationInMinutes,
                Rooms = tour.RoomTours.Select(rt => new RoomTourDto
                {
                    Id = rt.Room.Id,
                    Name = rt.Room.Name
                   
                }).ToList()
            };
        }
       

        public async Task<bool> StartTourAsync(int tourId, string userId)
        {
            var existing = await _tourRepository.GetUserTourAsync(tourId, userId);
            if (existing != null) return false;
            var tour = await _tourRepository.GetByIdAsync(tourId);
            if (tour == null) return false;

            var userTour = new UserTour
            {
                TourId = tourId,
                UserId = userId
            };

            await _tourRepository.AddUserTourAsync(userTour);

            return true;
        }

        public async Task<bool> CancelTourAsync(int tourId, string userId)
        {
            var userTour = await _tourRepository.GetUserTourAsync(tourId, userId);
            if (userTour == null) return false;

            _tourRepository.DeleteUserTour(userTour);

            return true;
        }
        public async Task<int> CreateTourAsync(CreateTourDto dto)
        {
            var tour = new Tour
            {
                Description = dto.Description,
                Category = dto.Category,
                DurationInMinutes= dto.DurationInMinutes,
                RoomTours = new List<RoomTour>()
            };

            // 🔥 ربط الغرف
            foreach (var roomId in dto.RoomIds)
            {
                tour.RoomTours.Add(new RoomTour
                {
                    RoomId = roomId
                });
            }

            await _tourRepository.AddAsync(tour);
           

            return tour.Id;
        }
    }
}

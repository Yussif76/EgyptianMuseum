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

        public async Task<List<TourDto>> GetAllToursAsync(double estimatedTime, int rooms)
        {



            var tours = await _tourRepository.GetFilteredToursAsync(estimatedTime, rooms);

            return tours.Select(t => new TourDto
            {
                Id = t.Id,
                Description = t.Description,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                Rooms = t.RoomTours.Select(rt => new RoomDto
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
                Description = tour.Description,
                StartTime = tour.StartTime,
                EndTime = tour.EndTime,
                Rooms = tour.RoomTours.Select(rt => new RoomDto
                {
                    Id = rt.Room.Id,
                    Name = rt.Room.Name
                }).ToList()
            };
        }

        public async Task<bool> StartTourAsync(int tourId, string userId)
        {
            var tour = await _tourRepository.GetByIdAsync(tourId);
            if (tour == null) return false;

            var userTour = new UserTour
            {
                TourId = tourId,
                UserId = userId
            };

            await _tourRepository.AddUserTourAsync(userTour);
            await _tourRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelTourAsync(int tourId, string userId)
        {
            var userTour = await _tourRepository.GetUserTourAsync(tourId, userId);
            if (userTour == null) return false;

            _tourRepository.DeleteUserTour(userTour);
            await _tourRepository.SaveChangesAsync();
            return true;
        }
    }
}

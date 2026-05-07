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

        public async Task<List<TourDto>> GetAllToursAsync(double estimatedTime, int rooms, string category)
        {



            var tours = await _tourRepository.GetFilteredToursAsync(estimatedTime, rooms, category);

            var rankedTours = tours
                .Select(t => new
                {
                    Tour = t,
                    TimeDiff = Math.Abs((t.EndTime - t.StartTime).TotalMinutes - estimatedTime * 60),
                    RoomDiff = Math.Abs(t.RoomTours.Count - rooms)
                })
                .OrderBy(x => x.TimeDiff)
                .ThenBy(x => x.RoomDiff)
                .Select(x => x.Tour)
                .ToList();

            return rankedTours.Select(t => new TourDto
            {
                Id = t.Id,
                Description = t.Description,
                Category = t.Category,
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
                Category = tour.Category,
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
    }
}

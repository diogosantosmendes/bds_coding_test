using Application.Inventory.DTOs;
using AutoMapper;
using Domain.Inventory.Entities;

namespace Application.Inventory.mappingProfiles
{
    public class VehicleProfile : Profile
    {
        public VehicleProfile()
        {
            CreateMap<AddVehicleDto, Hatchback>(MemberList.Source)
                .BeforeMap((src, dst) =>
                {
                    if (!src.Doors.HasValue) throw new ArgumentNullException("Doors is required in Hatchback");
                });
            CreateMap<AddVehicleDto, Sudan>()
                .BeforeMap((src, dst) =>
                {
                    if (!src.Doors.HasValue) throw new ArgumentNullException("Doors is required in Sudan");
                });
            CreateMap<AddVehicleDto, Suv>(MemberList.Source)
                .BeforeMap((src, dst) =>
                {
                    if (!src.Seats.HasValue) throw new ArgumentNullException("Seats is required in SUV");
                });
            CreateMap<AddVehicleDto, Truck>()
                .BeforeMap((src, dst) =>
                {
                    if (!src.LoadCapacity.HasValue) throw new ArgumentNullException("LoadCapacity is required in Truck");
                });
        }
    }
}

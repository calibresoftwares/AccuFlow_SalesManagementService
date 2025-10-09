using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using AutoMapper;
using System.Text;
using System.Threading.Tasks;
using SalesManagementService.Domain.Entities;
using SalesManagementService.Domain.DTOs.Customer;

namespace SalesManagementService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<CustomerDto, Customer>();
            CreateMap<CreateCustomerDto, Customer>();
          


        }
    }
}

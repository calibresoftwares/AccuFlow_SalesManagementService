using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using AutoMapper;
using System.Text;
using System.Threading.Tasks;
using SalesManagementService.Domain.Entities;
using SalesManagementService.Domain.DTOs.Customer;
using SalesManagementService.Domain.DTOs.SalesOrder;

namespace SalesManagementService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<CustomerDto, Customer>();
            CreateMap<CreateCustomerDto, Customer>();
            
            // SalesOrder mappings
            CreateMap<SalesOrder, SalesOrderDto>().ReverseMap();
            CreateMap<SalesOrderDto, SalesOrder>();
            CreateMap<CreateSalesOrderDto, SalesOrder>();
            CreateMap<SalesOrderLineItem, SalesOrderLineItemDto>().ReverseMap();
            CreateMap<SalesOrderLineItemDto, SalesOrderLineItem>();
            CreateMap<CreateSalesOrderLineItemDto, SalesOrderLineItem>();
          


        }
    }
}

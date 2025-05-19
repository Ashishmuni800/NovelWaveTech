using Application.DTO;
using Application.ViewModel;
using AutoMapper;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.AppMapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<RegisterModel, RegisterViewModel>();
            CreateMap<RegisterModel, RegisterDTO>();
            CreateMap<RegisterDTO, RegisterModel>();
        }
    }
}

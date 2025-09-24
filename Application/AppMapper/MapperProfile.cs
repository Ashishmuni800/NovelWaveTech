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
            CreateMap<ChangePasswordModel, ChangePasswordDTO>();
            CreateMap<ChangePasswordDTO, ChangePasswordModel>();

            CreateMap<PasswordChangeHistory, PasswordChangeHistoryDTO>();
            CreateMap<PasswordChangeHistoryDTO, PasswordChangeHistory>();

            CreateMap<GenerateCaptchaCode, GenerateCaptchaCodeViewModel>();
            CreateMap<GenerateCaptchaCode, GenerateCaptchaCodeDTO>();
            CreateMap<GenerateCaptchaCodeDTO, GenerateCaptchaCode>();

            CreateMap<AuthorizationData, AuthorizationDataViewModel>();
            CreateMap<AuthorizationData, AuthorizationDataDTO>();
            CreateMap<AuthorizationDataDTO, AuthorizationData>();

            CreateMap<UserPermission, PermissionMatrixViewModel>();
            CreateMap<UserPermission, UserPermissionDTO>();
            CreateMap<UserPermissionDTO, UserPermission>();

            CreateMap<UpdatePermissionsRequest, PermissionMatrixViewModel>();
            CreateMap<UpdatePermissionsRequest, UpdatePermissionsRequestDTO>();
            CreateMap<UpdatePermissionsRequestDTO, UpdatePermissionsRequest>();

            CreateMap<Product, ProductViewModel>();
            CreateMap<Product, ProductDTO>();
            CreateMap<ProductDTO, Product>();

            CreateMap<ProductSummary, ProductSummaryDTO>();
            //CreateMap<ProductSummary, ProductSummaryDTO>();
            //CreateMap<ProductSummaryDTO, ProductSummary>();

            CreateMap<Transactions, TransactionViewModel>();
            CreateMap<Transactions, TransactionDTO>();
            CreateMap<TransactionDTO, Transactions>();

            CreateMap<Customer, CustomerViewModel>();
            CreateMap<Customer, CustomerDTO>();
            CreateMap<CustomerDTO, Customer>();

            CreateMap<Reminder, ReminderViewModel>();
            CreateMap<Reminder, ReminderDTO>();
            CreateMap<ReminderDTO, Reminder>();
        }
    }
}

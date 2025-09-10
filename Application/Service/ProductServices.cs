using Application.DTO;
using Application.ServiceInterface;
using Application.ViewModel;
using AutoMapper;
using Domain.Model;
using Domain.RepositoryInterface;
using Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.Aztec.Internal;

namespace Application.Service
{
    public class ProductServices : IProductServices
    {
        private readonly IServiceInfraRepo _ProductRepository;
        private readonly IMapper _Mapp;
        public ProductServices(IServiceInfraRepo ProductRepository, IMapper Mapp)
        {
            _ProductRepository=ProductRepository;
            _Mapp=Mapp;
        }
        public async Task<ProductDTO> CreateByProductAsync(ProductDTO product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (_Mapp == null)
                throw new InvalidOperationException("_Mapp is not initialized.");

            if (_ProductRepository?.ProductRepo == null)
                throw new InvalidOperationException("_ProductRepository.AuthRepo is not initialized.");

            var model = _Mapp.Map<Product>(product);

            var result = await _ProductRepository.ProductRepo
                .CreateByProductAsync(model)
                .ConfigureAwait(false);

            var dto = _Mapp.Map<ProductDTO>(result);
            return dto;
        }

        public async Task<bool> DeleteByIdAsync(int Id)
        {
            var result = await _ProductRepository.ProductRepo.DeleteByIdAsync(Id).ConfigureAwait(false);
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public async Task<ProductDTO> EditByProductAsync(ProductDTO product, int Id)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (_Mapp == null)
                throw new InvalidOperationException("_Mapp is not initialized.");

            if (_ProductRepository?.ProductRepo == null)
                throw new InvalidOperationException("_ProductRepository.AuthRepo is not initialized.");

            var model = _Mapp.Map<Product>(product);

            var result = await _ProductRepository.ProductRepo
                .EditByProductAsync(model,Id)
                .ConfigureAwait(false);

            var dto = _Mapp.Map<ProductDTO>(result);
            return dto;
        }

        public async Task<List<ProductViewModel>> GetAsync()
        {
            var result = await _ProductRepository.ProductRepo.GetAsync().ConfigureAwait(false);

            if (result != null)
            {
                var dtoList = _Mapp.Map<List<ProductViewModel>>(result);
                return dtoList;
            }

            return null;
        }

        public async Task<List<ProductViewModel>> GetByUserIdAsync(string UserId)
        {
            var result = await _ProductRepository.ProductRepo.GetByUserIdAsync(UserId).ConfigureAwait(false);

            if (result != null)
            {
                var dtoList = _Mapp.Map<List<ProductViewModel>>(result);
                return dtoList;
            }

            return null;
        }
        public async Task<ProductViewModel> GetProductById(int Id)
        {
            var result = await _ProductRepository.ProductRepo.GetProductById(Id).ConfigureAwait(false);

            if (result != null)
            {
                var dtoList = _Mapp.Map<ProductViewModel>(result);
                return dtoList;
            }

            return null;
        }
    }
}

using Application.DTO;
using Application.ServiceInterface;
using Application.ViewModel;
using AutoMapper;
using Domain.Model;
using Domain.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class TransactionsService : ITransactionsService
    {
        private readonly IServiceInfraRepo _transactionsRepository;
        private readonly IMapper _mapper;

        public TransactionsService(IServiceInfraRepo transactionsRepository, IMapper mapper)
        {
            _transactionsRepository = transactionsRepository;
            _mapper = mapper;
        }

        public async Task<TransactionDTO> CreateTransactionAsync(TransactionDTO transactionDTO)
        {
            var entity = _mapper.Map<Transactions>(transactionDTO);

            var created = await _transactionsRepository.TransactionsRepo.CreateTransactionsAsync(entity);

            return _mapper.Map<TransactionDTO>(created);
        }

        public async Task<bool> DeleteTransactionAsync(Guid customerId, Guid id)
        {
            return await _transactionsRepository.TransactionsRepo.DeleteTransactionsAsync(customerId, id);
        }

        public async Task<IEnumerable<TransactionViewModel>> GetAllTransactionAsync()
        {
            var entities = await _transactionsRepository.TransactionsRepo.GetAllTransactionsAsync();
            return _mapper.Map<IEnumerable<TransactionViewModel>>(entities);
        }

        public async Task<CustomerBalanceDTO> GetBalanceBycustomerIdAsync(Guid customerId)
        {
            var entity = await _transactionsRepository.TransactionsRepo.GetBalanceBycustomerIdAsync(customerId);
            return _mapper.Map<CustomerBalanceDTO>(entity);
        }
        public async Task<CustomerBalanceDTO> GetBalanceAsync()
        {
            var entity = await _transactionsRepository.TransactionsRepo.GetBalanceAsync();
            return _mapper.Map<CustomerBalanceDTO>(entity);
        }

        public async Task<List<TransactionViewModel>> GetTransactionBycustomerIdAsync(Guid customerId)
        {
            var entity = await _transactionsRepository.TransactionsRepo.GetTransactionsBycustomerIdAsync(customerId);
            return _mapper.Map<List<TransactionViewModel>>(entity);
        }

        public async Task<TransactionViewModel> GetTransactionByIdAsync(Guid id)
        {
            var entity = await _transactionsRepository.TransactionsRepo.GetTransactionsByIdAsync(id);
            return _mapper.Map<TransactionViewModel>(entity);
        }

        public async Task<TransactionDTO> UpdateTransactionAsync(TransactionDTO transactionDTO, Guid customerId, Guid id)
        {
            var updatedEntity = _mapper.Map<Transactions>(transactionDTO);

            var updated = await _transactionsRepository.TransactionsRepo.UpdateTransactionsAsync(updatedEntity, customerId, id);

            if (updated == null)
                return null;

            return _mapper.Map<TransactionDTO>(updated);
        }
    }

}

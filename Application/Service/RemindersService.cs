using Application.DTO;
using Application.ServiceInterface;
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
    public class RemindersService : IRemindersService
    {
        private readonly IServiceInfraRepo _remindersRepository;
        private readonly IMapper _mapper;

        public RemindersService(IServiceInfraRepo remindersRepository, IMapper mapper)
        {
            _remindersRepository = remindersRepository;
            _mapper = mapper;
        }

        public async Task<ReminderDTO> CreateReminderAsync(ReminderDTO reminderDTO)
        {
            var entity = _mapper.Map<Reminder>(reminderDTO);
            var created = await _remindersRepository.RemindersRepo.CreateReminderAsync(entity);
            return _mapper.Map<ReminderDTO>(created);
        }

        public async Task<bool> DeleteReminderAsync(Guid customerId, Guid id)
        {
            return await _remindersRepository.RemindersRepo.DeleteReminderAsync(customerId, id);
        }

        public async Task<IEnumerable<ReminderDTO>> GetAllRemindersAsync()
        {
            var list = await _remindersRepository.RemindersRepo.GetAllRemindersAsync();
            return _mapper.Map<IEnumerable<ReminderDTO>>(list);
        }

        public async Task<ReminderDTO> GetReminderByCustomerIdAsync(Guid customerId)
        {
            var reminder = await _remindersRepository.RemindersRepo.GetReminderByCustomerIdAsync(customerId);
            return _mapper.Map<ReminderDTO>(reminder);
        }

        public async Task<ReminderDTO> GetReminderByIdAsync(Guid id)
        {
            var reminder = await _remindersRepository.RemindersRepo.GetReminderByIdAsync(id);
            return _mapper.Map<ReminderDTO>(reminder);
        }

        public async Task<ReminderDTO> UpdateReminderAsync(ReminderDTO reminderDTO, Guid customerId, Guid id)
        {
            var entity = _mapper.Map<Reminder>(reminderDTO);
            var updated = await _remindersRepository.RemindersRepo.UpdateReminderAsync(entity, customerId, id);

            if (updated == null)
                return null;

            return _mapper.Map<ReminderDTO>(updated);
        }
    }

}

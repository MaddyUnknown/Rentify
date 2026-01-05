using Rentify.Core.Entities;
using Rentify.Core.Exceptions;
using Rentify.Core.Utils;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.Core.UnitOfWork;
using Rentify.Event.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Dispatcher.Worker
{
    public class EventDispatcherProcessor
    {
        private readonly ILogger<EventDispatcherProcessor> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<EventOutbox> _eventOutboxCRUDRepository;
        private readonly IEventOutboxService _eventOutboxService;

        public EventDispatcherProcessor(ILogger<EventDispatcherProcessor> logger, IUnitOfWork unitOfWork, IRepository<EventOutbox> eventOutboxCRUDRepository, IEventOutboxService eventOutboxService) {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _eventOutboxCRUDRepository = eventOutboxCRUDRepository;
            _eventOutboxService = eventOutboxService;
        }

        public async Task ExecuteAsync()
        {
            var events = await _eventOutboxCRUDRepository.GetAllAsync();
            if (events.Count() > 0) _logger.LogInformation("Total events polled for processing: {eventCount}", events.Count());

            foreach (var @event in events)
            {
                try
                {
                    await _eventOutboxService.PublishEventsAsync(@event);
                    _eventOutboxCRUDRepository.Remove(@event);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (EventException ex)
                {
                    _logger.LogError(ex, "Event exception when processing event id: {eventId}, json: {eventJson}. Removing queued event", @event.Id, @event.EventData);
                    _eventOutboxCRUDRepository.Remove(@event);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception while processing event id: {eventId}", @event.Id);
                }
            }
        }
    }
}

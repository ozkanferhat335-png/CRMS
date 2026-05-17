using System;
using CRMS.Entity.Models;

namespace CRMS.DataAccess.Repositories
{
    public class UnitOfWork
    {
        private readonly string _connectionString;
        private UserRepository _userRepository;
        private CustomerRepository _customerRepository;
        private RoleRepository _roleRepository;
        private TaskRepository _taskRepository;
        private SaleRepository _saleRepository;
        private OfferRepository _offerRepository;
        private TicketRepository _ticketRepository;
        private LogRepository _logRepository;
        private MeetingRepository _meetingRepository;
        private NotificationRepository _notificationRepository;
        private EventRepository _eventRepository;
        private SettingsRepository _settingsRepository;

        public UnitOfWork(string connectionString)
        {
            _connectionString = connectionString;
        }

        public UserRepository Users
        {
            get
            {
                if (_userRepository == null)
                    _userRepository = new UserRepository(_connectionString);
                return _userRepository;
            }
        }

        public CustomerRepository Customers
        {
            get
            {
                if (_customerRepository == null)
                    _customerRepository = new CustomerRepository(_connectionString);
                return _customerRepository;
            }
        }

        public RoleRepository Roles
        {
            get
            {
                if (_roleRepository == null)
                    _roleRepository = new RoleRepository(_connectionString);
                return _roleRepository;
            }
        }

        public TaskRepository Tasks
        {
            get
            {
                if (_taskRepository == null)
                    _taskRepository = new TaskRepository(_connectionString);
                return _taskRepository;
            }
        }

        public SaleRepository Sales
        {
            get
            {
                if (_saleRepository == null)
                    _saleRepository = new SaleRepository(_connectionString);
                return _saleRepository;
            }
        }

        public OfferRepository Offers
        {
            get
            {
                if (_offerRepository == null)
                    _offerRepository = new OfferRepository(_connectionString);
                return _offerRepository;
            }
        }

        public TicketRepository Tickets
        {
            get
            {
                if (_ticketRepository == null)
                    _ticketRepository = new TicketRepository(_connectionString);
                return _ticketRepository;
            }
        }

        public LogRepository Logs
        {
            get
            {
                if (_logRepository == null)
                    _logRepository = new LogRepository(_connectionString);
                return _logRepository;
            }
        }

        public MeetingRepository Meetings
        {
            get
            {
                if (_meetingRepository == null)
                    _meetingRepository = new MeetingRepository(_connectionString);
                return _meetingRepository;
            }
        }

        public NotificationRepository Notifications
        {
            get
            {
                if (_notificationRepository == null)
                    _notificationRepository = new NotificationRepository(_connectionString);
                return _notificationRepository;
            }
        }

        public EventRepository Events
        {
            get
            {
                if (_eventRepository == null)
                    _eventRepository = new EventRepository(_connectionString);
                return _eventRepository;
            }
        }

        public SettingsRepository Settings
        {
            get
            {
                if (_settingsRepository == null)
                    _settingsRepository = new SettingsRepository(_connectionString);
                return _settingsRepository;
            }
        }

        public void SaveChanges()
        {
            // Changes are saved immediately per operation
        }

        public void Dispose()
        {
            // Clean up resources if needed
        }
    }
}

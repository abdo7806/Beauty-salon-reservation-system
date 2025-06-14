using BookingSystem.Application.DTOs.Appointments;
using BookingSystem.Application.Interfaces.Repositories;
using BookingSystem.Application.Interfaces.Services;
using BookingSystem.Domain.Entites;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookingSystem.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IAvailabilityRepository availabilityRepository,
            IServiceRepository serviceRepository,
            ILogger<AppointmentService> logger)
        {
            _appointmentRepository = appointmentRepository;
            _availabilityRepository = availabilityRepository;
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        private string GetShiftLabel(TimeSpan time)
        {
            if (time.Hours >= 6 && time.Hours < 12)
                return "Morning";
            else if (time.Hours >= 12 && time.Hours < 18)
                return "Afternoon";
            else if (time.Hours >= 18 && time.Hours <= 22)
                return "Evening";
            else
                return "OutOfShift";
        }

        public async Task<AppointmentDto?> CreateAsync(CreateAppointmentDto appointmentDto)
        {
            try
            {
                _logger.LogInformation("Creating new appointment");

                // التحقق من توفر الموظف في اليوم المطلوب
                var staffAvailability = await _availabilityRepository.GetByStaffIdAndDayAsync(
                    appointmentDto.StaffId, appointmentDto.Date.DayOfWeek);

                if (staffAvailability == null)
                {
                    _logger.LogWarning("No availability for staff {StaffId} on {Day}", appointmentDto.StaffId, appointmentDto.Date.DayOfWeek);
                    return null;
                }



                // 🟨 تحديد الفترة صباح/مساء/مساء متأخر
                var shift = GetShiftLabel(appointmentDto.StartTime);
                _logger.LogInformation("Requested shift: {Shift}", shift);
                if (shift == "OutOfShift")
                {
                    _logger.LogWarning("Appointment request is outside allowed shifts");
                    return null;
                }




                // الحصول على مدة الخدمة من جدول الخدمات
                var service = await _serviceRepository.GetByIdAsync(appointmentDto.ServiceId);
                if (service == null) return null;

                 var endTime = appointmentDto.StartTime + TimeSpan.FromMinutes(service.Duration);


                // التحقق أن الوقت المطلوب داخل ساعات العمل
                if (appointmentDto.StartTime < staffAvailability.StartTime ||
                    endTime > staffAvailability.EndTime)
                {
                    _logger.LogWarning("Appointment time is outside available hours");
                    return null;
                }
             

                // التحقق من عدم وجود موعد بنفس الوقت
                var existingAppointments = await _appointmentRepository.GetByStaffIdAndDateAsync(
                    appointmentDto.StaffId, appointmentDto.Date);

                bool isOverlapping = existingAppointments.Any(a =>
     (appointmentDto.StartTime < a.EndTime && endTime > a.StartTime));

                if (isOverlapping)
                {
                    _logger.LogWarning("Time conflict with another appointment");
                    return null;
                }

                var appointment = new Appointment
                {
                    ClientId = appointmentDto.ClientId,
                    StaffId = appointmentDto.StaffId,
                    ServiceId = appointmentDto.ServiceId,
                    Date = appointmentDto.Date,
                    StartTime = appointmentDto.StartTime,
                    EndTime = endTime,
                    Status = "Pending"
                };

                var createdAppointment = await _appointmentRepository.AddAsync(appointment);

                if (createdAppointment == null)
                {
                    _logger.LogWarning("Failed to create appointment");
                    return null;
                }

                _logger.LogInformation("Appointment {AppointmentId} created", createdAppointment.Id);
                return MapToDto(createdAppointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return null;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
            

                _logger.LogInformation("Deleting appointment {AppointmentId}", id);

                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    _logger.LogWarning("Appointment {AppointmentId} not found for deletion", id);
                    return false;
                }

                var result = await _appointmentRepository.DeleteAsync(appointment);
                if (result)
                {
                    _logger.LogInformation("Appointment {AppointmentId} deleted successfully", id);
                }
                else
                {
                    _logger.LogWarning("Failed to delete appointment {AppointmentId}", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment {AppointmentId}", id);
                return false;
            }

        }

        public async Task<List<AppointmentDto>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all appointments");
                var appointments = await _appointmentRepository.GetAllAsync();
                return appointments.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointments");
                return new List<AppointmentDto>();
            }
        }

        public async Task<List<AppointmentDto>> GetByClientIdAsync(int clientId)
        {
            try
            {
                _logger.LogDebug("Fetching appointment By clientId: {clientId}", clientId);
                var appointments = await _appointmentRepository.GetByClientIdAsync(clientId);
                if (appointments == null)
                {
                    _logger.LogWarning("Appointment By clientId: {clientId} not found", clientId);
                    return null;
                }
                return appointments.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointment By clientId: {clientId}", clientId);
                return null;
            }
        }

        public async Task<AppointmentDto?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogDebug("Fetching appointment {AppointmentId}", id);
                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    _logger.LogWarning("Appointment {AppointmentId} not found", id);
                    return null;
                }
                return MapToDto(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointment {AppointmentId}", id);
                return null;
            }
        }

        public async Task<List<AppointmentDto>> GetByStaffIdAsync(int staffId)
        {
            try
            {
                _logger.LogDebug("Fetching appointment By staffId: {staffId}", staffId);
                var appointments = await _appointmentRepository.GetByStaffIdAsync(staffId);
                if (appointments == null)
                {
                    _logger.LogWarning("Appointment By staffId: {staffId} not found", staffId);
                    return null;
                }
                return appointments.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointment By staffId: {staffId}", staffId);
                return null;
            }
        }

        public async Task<AppointmentDto?> UpdateStatusAsync(int id, string status)
        {
            try
            {
                _logger.LogInformation("Updating appointment {AppointmentId}", id);

                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    _logger.LogWarning("Appointment {AppointmentId} not found for update", id);
                    return null;
                }

                appointment.Status = status;
                if (status == "Pending")
                {
                    appointment.EndTime = new TimeSpan();
                }
                else
                {
                    appointment.EndTime = DateTime.Now.TimeOfDay;
                }
                

                var updatedAppointment = await _appointmentRepository.UpdateAsync(appointment);
                if (updatedAppointment == null)
                {
                    _logger.LogWarning("Failed to update appointment {AppointmentId}", id);
                    return null;
                }

                _logger.LogInformation("Appointment {AppointmentId} updated successfully", id);
                return MapToDto(updatedAppointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment {AppointmentId}", id);
                return null;
            }
        }

   

        private AppointmentDto MapToDto(Appointment appointment)
        {
            return new AppointmentDto
            {
                Id = appointment.Id,
                ClientId = appointment.ClientId,
                StaffId = appointment.StaffId,
                ServiceId = appointment.ServiceId,
                Date = appointment.Date,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status
            };
        }
    }
}
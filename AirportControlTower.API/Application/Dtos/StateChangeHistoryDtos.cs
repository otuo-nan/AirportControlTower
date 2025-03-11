﻿using AirportControlTower.API.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportControlTower.API.Application.Dtos
{
    public class StateChangeHistoryDto
    {
        public Guid Id { get; set; }

        public AirlineState FromState { get; set; }
        public AirlineStateTrigger Trigger { get; set; }

        public Guid AirlineId { get; set; }
        public required string AirlineName { get; set; }
        public required string AirlineCallSign { get; set; }
        public HistoryStatus Status { get; set; }
        public DateTime CreatedOn { get; set; } 
    }
}

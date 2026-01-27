using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vikalp.Models.DTO
{

    public class HSactivityDto
    {
    }
    public class HsActivityCreateDto
    {
        /* =========================
           A. GENERAL INFORMATION
           ========================= */

        [Required]
        [Display(Name = "Name of person collected this information")]
        public string CollectedBy { get; set; }

        [Required]
        [Display(Name = "Activity Type")]
        public string ActivityType { get; set; }

        [Display(Name = "Other Activity specified")]
        public string? OtherActivityType { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string District { get; set; }

        [Required]
        [Display(Name = "Name of Training Venue")]
        public string VenueName { get; set; }

        [Required]
        [Display(Name = "Training Type")]
        public string TrainingType { get; set; }   // Clinical / Non-Clinical / Both

        [Required]
        [Display(Name = "Activity Format")]
        public string ActivityFormat { get; set; } // In-person / Virtual / Hybrid

        [Required]
        [Range(1, 10000)]
        [Display(Name = "Total number of Participants")]
        public int TotalParticipants { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }


        /* =========================
           B. TRAINING CONTENT
           ========================= */

        // Used only to RENDER checkboxes
        public Dictionary<string, bool> ClinicalTopics { get; set; }
            = new Dictionary<string, bool>
            {
            { "MPA-IM", false },
            { "MPA-SC", false },
            { "IUCD", false },
            { "PPIUCD", false },
            { "Implant", false },
            { "OCP", false }
            };

        public Dictionary<string, bool> NonClinicalTopics { get; set; }
            = new Dictionary<string, bool>
            {
            { "VCAT - FP", false },
            { "FPLMIS", false },
            { "Supply chain of Implant and MPA-SC", false },
            { "Counselling", false }
            };

        // These bind POSTED checkbox values
        public List<string> SelectedClinicalTopics { get; set; }
            = new();

        public List<string> SelectedNonClinicalTopics { get; set; }
            = new();


        /* =========================
           C. PARTICIPANTS BY CADRE
           ========================= */

        public List<CadreParticipantDto> Cadres { get; set; }
            = new()
            {
            new CadreParticipantDto { Id = 1, Name = "Providers- Specialists (Ob-Gyn)" },
            new CadreParticipantDto { Id = 2, Name = "Providers- General Practitioners (MBBS)" },
            new CadreParticipantDto { Id = 3, Name = "Providers - AYUSH" },
            new CadreParticipantDto { Id = 4, Name = "Providers- Nurses/ANM" },
            new CadreParticipantDto { Id = 5, Name = "Providers- CHO" },
            new CadreParticipantDto { Id = 6, Name = "Pharmacists" },
            new CadreParticipantDto { Id = 7, Name = "Store In-charge" },
            new CadreParticipantDto { Id = 8, Name = "DEO" },
            new CadreParticipantDto { Id = 9, Name = "Counsellors" },
            new CadreParticipantDto { Id = 10, Name = "Other non-clinical staff" },
            new CadreParticipantDto { Id = 11, Name = "Others" }
            };


        /* =========================
           REMARKS
           ========================= */

        public string? Remarks { get; set; }
    }
}
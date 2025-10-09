using Microsoft.AspNetCore.Mvc;

namespace GameFlow.Models.Admin;

public class ActionFormModel
{
    [FromForm(Name = "action-applicant-type")]
    public string ApplicantType { get; set; } = null!;
    [FromForm(Name = "action-applicant-name")]
    public string ApplicantName { get; set; } = null!;
    [FromForm(Name = "action-name")]
    public string Name { get; set; } = null!;
    [FromForm(Name = "action-description")]
    public string Description { get; set; } = null!;
    [FromForm(Name = "action-amount")]
    public int Amount { get; set; }
    [FromForm(Name = "action-startDate")]
    public DateTime StartDate { get; set; }
    [FromForm(Name = "action-endDate")]
    public DateTime EndDate { get; set; }
}
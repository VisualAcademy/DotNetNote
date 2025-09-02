using Microsoft.AspNetCore.Mvc;

namespace DotNetNote.Apis;

public record DrugPanelItem(
    string guid,
    string name,
    string? description,
    string type,          // "nondot" | "dot" | "nonDotAlcohol" | "dotAlcohol" 등
    string? drugsScreened,
    string? specimenType  // "Urine" | "Hair" | "Oral Fluid" | "Breath" 등
);

[ApiController]
[Route("v1/clients/{clientGuid}/products/collection/[controller]")]
public class PanelsController : ControllerBase
{
    private const string FixedJwtToken = "TestToken";

    // 3개 샘플
    private static readonly List<DrugPanelItem> Panels = new()
    {
        new DrugPanelItem(
            guid: "b1e9f7b6-3bde-4b6d-9c8f-4d1c5b6a2b11",
            name: "Essential 4 (No THC)",
            description: "Core screening without THC",
            type: "nondot",
            drugsScreened: "Amphetamines; Cocaine; Opiates; Phencyclidine",
            specimenType: "Urine"
        ),
        new DrugPanelItem(
            guid: "0a4c1f2d-0f6f-4a3a-8ba0-7f2f7db0a9d2",
            name: "Rapid 5 Oral Fluid",
            description: "Quick oral-fluid check",
            type: "nondot",
            drugsScreened: "Amphetamines; Cannabinoids; Cocaine; Opiates; Phencyclidine",
            specimenType: "Oral Fluid"
        ),
        new DrugPanelItem(
            guid: "7c2b8b9a-1b2e-4e1e-9c1c-8a7e2c5d4f33",
            name: "Comprehensive 9",
            description: "Expanded lab-based screening",
            type: "nondot",
            drugsScreened: "Amphetamines; Barbiturates; Benzodiazepines; Cannabinoids; Cocaine; Methadone; Opiates; Phencyclidine; Propoxyphene",
            specimenType: "Urine"
        )
    };

    [HttpGet]
    public IActionResult GetPanels(
        [FromHeader(Name = "Authorization")] string? authorization,
        [FromRoute] string clientGuid)
    {
        if (authorization is null || !authorization.StartsWith("Bearer "))
            return Unauthorized(new { message = "Authorization header missing" });

        var token = authorization["Bearer ".Length..];
        if (token != FixedJwtToken)
            return Unauthorized(new { message = "Invalid token" });

        // 학습용: clientGuid는 인증만 통과하면 단순히 3개 리스트를 반환
        return Ok(Panels);
    }
}

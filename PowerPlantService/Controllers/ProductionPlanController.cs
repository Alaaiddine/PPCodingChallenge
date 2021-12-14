using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PowerPlantService.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ProductionPlanController : ControllerBase
    {
        public IMediator Mediator { get; }
        public ProductionPlanController(IMediator mediator)
        {
            Mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        [ProducesResponseType(typeof(List<PowerPlantsCommitmentReponse>), 200)]
        public async Task<ActionResult<PowerPlantsCommitmentReponse>> CalculateUnitCommitmentAsync(PowerPlantsCommitmentRequest request)
        {
            var response = await Mediator.Send(request, CancellationToken.None);
            return Ok(response);
        }
    }
}

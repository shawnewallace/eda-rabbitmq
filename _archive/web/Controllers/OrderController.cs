using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using web.Models;

namespace web.Controllers
{
    public class OrderController : ApiController
    {
        [HttpPost]
        [Route("api/donate")]
        public async Task<IHttpActionResult> Post([FromBody] string content)
        {
            var r = new Random();
            var numItemsInOrder = r.Next(0, 20);

            var model = new NewOrderModel(numItemsInOrder);
            //if (!ModelState.IsValid) return BadRequest("Donation model is invalid.");

            //var newDonation = new NewDonationServiceModel { GiverId = Guid.NewGuid() };
            //newDonation = donation.ApplyTo(newDonation);

            //try
            //{
            //    var finalizedDonation = await DonationService.Create(newDonation);
            //    return CreatedAtRoute("DefaultApi", new { controller = "Donation", id = finalizedDonation.Id }, new CompletedDonationApiModel(finalizedDonation));
            //}
            //catch (Exception e)
            //{
            //    return InternalServerError(e);
            //}

            return Created("http://www.cnn.com", "DONE");
        }
    }
}

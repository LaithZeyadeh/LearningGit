using AllServices;
using Microsoft.AspNetCore.Mvc;
using Project.Models;
using DataModels;
using MyUtil;

namespace Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly IDatabase Database;

        public MainController(IDatabase _Database)
        {
            Database = _Database;
        }

        [HttpPost("Signup")]
        public async Task<ActionResult<RequestModel>> Signup([FromBody] RequestModelDTO RequestModelDTO) //  TO-DO: use DTO
        {
            if (!Database.IsEmpty()) return Problem("Entity set 'Mycontext.data'  is null.");
            if (RequestModelDTO == null) return Problem("empty data");
            if (Database.FindElement(RequestModelDTO, Element => Element.Email == RequestModelDTO.Email)) return BadRequest("Already registered ");
            if (RequestModelDTO.Password != RequestModelDTO.ConfirmPassword) return BadRequest("Passwords are not matched");

            // Doing this until we learn Mapping.
            RegisterModel Model = new RegisterModel();
            Model.Email = RequestModelDTO.Email;
            Model.ConfirmPassword = RequestModelDTO.ConfirmPassword;
            Model.Password = RequestModelDTO.Password;
            Model.Name = RequestModelDTO.Name;
            Model.Role = RequestModelDTO.Role;

            Boolean check = await Database.AddUser(Model);

            return Ok(RequestModelDTO.Password + " " + check);
        }

        [HttpPost("Signin")]
        public async Task<ActionResult<RequestModel>> Signin([FromBody] SignInModelDTO ModelDTO) //  TO-DO: use DTO
        {
            if (ModelDTO == null) return NotFound("empty data");

            string CheckPassword;

            // doing this until we learn about Mapping.
            SignInModel Model = new SignInModel();
            Model.Email = ModelDTO.Email;
            Model.Password = ModelDTO.Password;



            try
            {

                CheckPassword = await Database.GetPassword(Model);

            }
            catch (NullReferenceException Except)
            {

                return BadRequest("User Not Registered");
            }

            Boolean Check = false;

            try
            {
                Check = PasswordHashing.Verify(Model.Password, CheckPassword);

            }
            catch (NotSupportedException Except)
            {
                Console.WriteLine(Except.StackTrace);
            }

            if (Database.FindElement(Model, Element => Element.Email == Model.Email && Check))
                return Ok("Signed in!");

            else if (Database.FindElement(Model, Element => Element.Email == Model.Email) || Check)
                return BadRequest("Wrong credentials" + " " + CheckPassword);
            else
                return BadRequest("Not Registered");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequestModel([FromBody] RequestModelDTO Model)
        {
            if (!Database.IsEmpty())
            {
                return NotFound();
            }

            if (await Database.RemoveUser(Model.Id) == null)
            {
                return NotFound();
            }

            await Database.RemoveUser(Model.Id);
            return Ok("Removed User");
        }
    }
}


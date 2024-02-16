using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TestExerciseSK.DAL;
using TestExerciseSK.DTO;
using TestExerciseSK.Models;

namespace TestCode_SK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserRepository _userRepository;
        public const int RETIREMENT_AGE = 62;
        public const int UNDER_AGE_LIMIT = 18;
        public const int USERID_LENGTH = 7;
        public const String EMPTY_STRING = "";
        public const string USERID_IN_USE = "The used userID is already in use. User with this ID exists.";
        public const string NO_USERID = "UserID to insert cannot be empty or null.";
        public const string INVALID_USERID = "UserID cannot be longer than 7 characters.";
        public const string INVALID_BIRTHDATE = "Expected Birth Date format is YYYY-MM-DD";
        public const string BIRTHDATE_UNDERAGE = "The user you are trying to update is under 18 years, is forbidden in the system";
        public const string INVALID_EMAIL = "E-mail format is not correct.";

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpGet]
        [Route("GetUsers")]
        public IActionResult Get()
        {
            try { 
                var users = _userRepository.GetUsers();
                if (users.Count == 0)
                {
                    return NotFound("There is no user to display");
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetUserById")]
        public IActionResult Get(string id) 
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(NO_USERID);
                } else if (!isValidId(id))
                {
                    return BadRequest(INVALID_USERID);
                }
                var user = _userRepository.GetUserById(id);
                if (user == null) 
                {
                    return NotFound($"User {id} not found.");
                }
               
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateUser")]
        public IActionResult Post(UserDTO userRequest)
        {    
            
            if (userRequest != null)
            {
                if (userRequest.userID == null || userRequest.userID == EMPTY_STRING)
                {
                    return BadRequest(NO_USERID);
                }
                else if (!isValidId(userRequest.userID))
                {
                    return BadRequest(INVALID_USERID);
                }

                var userTemp = _userRepository.GetUserById(userRequest.userID);

                if (userTemp != null)
                {
                    return BadRequest(USERID_IN_USE);
                }

                if (!isValidEmail(userRequest.email))
                {
                    return BadRequest(INVALID_EMAIL);
                }

                if (!isValidDate(userRequest.birthDate))
                {
                    return BadRequest(INVALID_BIRTHDATE); 
                } else if (isUnderAge(getDateFromString(userRequest.birthDate)))
                {
                    return BadRequest(BIRTHDATE_UNDERAGE);
                }
                User user = getUserFromDTO(userRequest);

                _userRepository.Create(user);
                return Ok(user);
            }
            else
            {
                return BadRequest("User Request in null");
            }
        }


        [HttpPut]
        [Route("UpdateUser")]
        public IActionResult Put(UserDTO userRequest)
        {
            User userToUpdate = new User();
            if (userRequest.userID == null || userRequest.userID == EMPTY_STRING)
            {
                return BadRequest(NO_USERID);
            }
            else if (!isValidId(userRequest.userID))
            {
                return BadRequest(INVALID_USERID);
            } else
            {
                userToUpdate.userID = userRequest.userID;
            }
            if (userRequest.firstName != null && !EMPTY_STRING.Equals(userRequest.firstName))
            {
                userToUpdate.firstName = userRequest.firstName;
            }
            if (userRequest.lastName != null && !EMPTY_STRING.Equals(userRequest.lastName))
            {
                userToUpdate.lastName = userRequest.lastName;
            }
            if (userRequest == null || userRequest.userID == EMPTY_STRING)
            {
                return NotFound(NO_USERID);
            }

            if (userRequest.email != null && !isValidEmail(userRequest.email)) 
            {
                return BadRequest(INVALID_EMAIL);
            } else
            {
                userToUpdate.email = userRequest.email;
            }
            
            if (userRequest.birthDate != null && !EMPTY_STRING.Equals(userRequest.birthDate))
            {
                if (isValidDate(userRequest.birthDate))
                { 
                    DateTime birthDateRequest = getDateFromString(userRequest.birthDate);
                    if (isUnderAge(birthDateRequest))
                    {
                        return BadRequest(BIRTHDATE_UNDERAGE);
                    }
                    userToUpdate.birthDate = birthDateRequest;
                    userToUpdate.retirementDate = getRetirementDate(birthDateRequest);
                } else
                {
                    return BadRequest(INVALID_BIRTHDATE);
                }
            }

            bool updated = _userRepository.Update(userToUpdate);
            if (updated)
            {
                return Ok("User is updated");
            } else
            {
                return NotFound($"User not found with id {userRequest.userID}");
            }            
        }


        [HttpDelete]
        [Route("DeleteUserById")]
        public IActionResult Delete(String userId)
        {

            if (userId == null || EMPTY_STRING.Equals(userId))
            {
                return BadRequest(NO_USERID);
            } else if (!isValidId(userId))
            {
                return BadRequest(INVALID_USERID);
            } else
            {
                bool deleted = _userRepository.Delete(userId);

                if (deleted)
                {
                    return Ok($"User deleted with id {userId}");
                }
                else
                {
                    return NotFound($"User not found with id {userId}");

                }
            }
            
        }

        private bool isValidId(string userId)
        {
            return (userId != null && !EMPTY_STRING.Equals(userId) && userId.Length <= USERID_LENGTH);
        }

        private DateTime getRetirementDate(DateTime birthDate)
        {
            return birthDate.AddYears(RETIREMENT_AGE);

        }
        private bool isUnderAge(DateTime birthDate)
        {
            return (DateTime.Now.AddYears(-birthDate.Year).Year < UNDER_AGE_LIMIT);
        }

        private DateTime getDateFromString(String dateS)
        {
            try
            {
                return DateTime.ParseExact(dateS, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {     
               throw;   
            }
            
        }

        private bool isValidDate(string birthDate)
        {
            try
            {
                getDateFromString(birthDate);
                return true;
            }catch  (Exception)
            {
                return false;
            }
        }

        private bool isValidEmail(string email)
        {
            bool result = false;
            try
            {
                if(email != null && !email.Equals(EMPTY_STRING))
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    result=  true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        private User getUserFromDTO(UserDTO userDTO)
        {
            return new User
            {
                userID = userDTO.userID,
                firstName = userDTO.firstName,
                lastName = userDTO.lastName,
                email = userDTO.email,
                birthDate = getDateFromString(userDTO.birthDate),
                retirementDate = getRetirementDate(getDateFromString(userDTO.birthDate))
            };
        }
    }

    
}

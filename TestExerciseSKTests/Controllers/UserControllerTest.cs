using System;
using System.Globalization;
using System.Net;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestCode_SK.Controllers;
using TestExerciseSK.DAL;
using TestExerciseSK.DTO;
using TestExerciseSK.Models;
using TestExerciseUnitTestsSK.Context;

namespace TestExerciseSKTests.Controllers
{
    [TestClass]
    public class UsersControllerTest
    {
        public const String EMPTY_STRING = "";
        public const int RETIREMENT_DATE = 62;
        public const string USERID_IN_USE = "The used userID is already in use. User with this ID exists.";
        public const string NO_USERID = "UserID to insert cannot be empty or null.";
        public const string INVALID_USERID = "UserID cannot be longer than 7 characters.";
        public const string INVALID_BIRTHDATE = "Expected Birth Date format is YYYY-MM-DD";
        public const string BIRTHDATE_UNDERAGE = "The user you are trying to update is under 18 years, is forbidden in the system";
        public const string INVALID_EMAIL = "E-mail format is not correct.";
        private SqlLite_UnitTests sqlLiteUnitTests;
        private IUserRepository _userRepository;
        [TestInitialize]
        public void Init()
        {
            sqlLiteUnitTests = new SqlLite_UnitTests();
        }

        [TestMethod]
        public void CreateUser_CreateWithValidDataUser_ShouldReturnUserCreated()
        {
            var userDTO = new UserDTO { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = "1982-12-12" };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                
                var response = usersController.Post(userDTO) as ObjectResult;
                
                User resultUser = (User) response.Value;
                var responseStatus = GetHttpStatusCode(response);
                var birthDate = resultUser.birthDate;
                var retireDate = resultUser.retirementDate;

                Assert.AreEqual(resultUser.userID,userDTO.userID);
                Assert.AreEqual(resultUser.firstName, userDTO.firstName);
                Assert.AreEqual(resultUser.lastName, userDTO.lastName);
                Assert.AreEqual(resultUser.email, userDTO.email);
                Assert.AreEqual(resultUser.birthDate, getDateFromString(userDTO.birthDate));
                Assert.AreEqual(resultUser.birthDate.AddYears(RETIREMENT_DATE), resultUser.retirementDate);

                Assert.AreEqual(HttpStatusCode.OK, responseStatus);
            }
        }
        
        [TestMethod]
        public void CreateUser_CreateWithExistingId_ShouldReturnBadRequest()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(2010, 12, 12), retirementDate = new DateTime(2010, 12, 12).AddYears(62) };
            var userClone = new UserDTO { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = "1982-12-12" };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                _userRepository.Create(user);
                var response = usersController.Post(userClone) as ObjectResult;
                var responseStatus = GetHttpStatusCode(response);
                var resultMsg = response.Value;

                Assert.AreEqual(responseStatus, HttpStatusCode.BadRequest);
                Assert.AreEqual(USERID_IN_USE,resultMsg); 
            }
        }
        
        [TestMethod]
        public void CreateUser_CreateWithNullId_ShouldReturnBadRequest()
        {
            var userDTO = new UserDTO { userID = null, firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = "1982-12-12" };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                var response = usersController.Post(userDTO) as ObjectResult;
                
                var responseStatus = GetHttpStatusCode(response);
                var resultMsg = response.Value;

                Assert.AreEqual(HttpStatusCode.BadRequest,responseStatus);
                Assert.AreEqual(NO_USERID, resultMsg);
            }
        }

        [TestMethod]
        public void CreateUser_CreateWithEmptyId_ShouldReturnBadRequest()
        {
            var userDTO = new UserDTO { userID = "", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = "1982-12-12" };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                var response = usersController.Post(userDTO) as ObjectResult;

                var responseStatus = GetHttpStatusCode(response);
                var resultMsg = response.Value;

                Assert.AreEqual(HttpStatusCode.BadRequest, responseStatus);
                Assert.AreEqual(NO_USERID, resultMsg);
            }
        }

        [TestMethod]
        public void CreateUser_CreateWithInvalidID_ShouldReturnBadRequest()
        {
            var userDTO = new UserDTO { userID = "12345678", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = "1982-14-12" };
            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                var response = usersController.Post(userDTO) as ObjectResult;

                var responseStatus = GetHttpStatusCode(response);
                var resultMsg = response.Value;

                Assert.AreEqual(HttpStatusCode.BadRequest, responseStatus);
                Assert.AreEqual(INVALID_USERID, resultMsg);
            }
        }

        [TestMethod]
        public void CreateUser_CreateWithInvalidBirthDate_ShouldReturnBadRequest()
        {
            var userDTO = new UserDTO { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = "1982-14-12"};
            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                var response = usersController.Post(userDTO) as ObjectResult;

                var responseStatus = GetHttpStatusCode(response);
                var resultMsg = response.Value;

                Assert.AreEqual(HttpStatusCode.BadRequest, responseStatus);
                Assert.AreEqual(INVALID_BIRTHDATE, resultMsg);
            }
        }

        [TestMethod]
        public void CreateUser_CreateWithUnderage_ShouldReturnBadRequest()
        {
            var userDTO = new UserDTO { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = "2012-12-12" };
            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                var response = usersController.Post(userDTO) as ObjectResult;

                var responseStatus = GetHttpStatusCode(response);
                var resultMsg = response.Value;

                Assert.AreEqual(HttpStatusCode.BadRequest, responseStatus);
                Assert.AreEqual(BIRTHDATE_UNDERAGE, resultMsg);
            }
        }

        [TestMethod]
        public void CreateUser_CreateWithInvalidFormatEmail_ShouldReturnArgumentBadRequest()
        {
            var userDTO = new UserDTO { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email.com", birthDate = "1982-12-12" };
            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                var response = usersController.Post(userDTO) as ObjectResult;

                var responseStatus = GetHttpStatusCode(response);
                var resultMsg = response.Value;

                Assert.AreEqual(HttpStatusCode.BadRequest, responseStatus);
                Assert.AreEqual(INVALID_EMAIL, resultMsg);
            }
        }
        
        [TestMethod]
        public void GetUser_GetUserByExistingId_ShouldReturnSameUserCreatedWithId()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(2010, 12, 12), retirementDate = new DateTime(2010, 12, 12).AddYears(62) };
            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                _userRepository.Create(user);
                var response = usersController.Get(user.userID) as ObjectResult;
                var responseStatus = GetHttpStatusCode(response);
                var userConsulted = (User) response.Value;

                Assert.AreEqual(HttpStatusCode.OK, responseStatus);

                Assert.AreEqual(userConsulted.userID, user.userID);
                Assert.AreEqual(userConsulted.firstName, user.firstName);
                Assert.AreEqual(userConsulted.lastName, user.lastName);
                Assert.AreEqual(userConsulted.email, user.email);
                Assert.AreEqual(userConsulted.birthDate, user.birthDate);
                Assert.AreEqual(userConsulted.retirementDate, user.retirementDate);
            }
        }
        
        [TestMethod]
        public void GetUser_GetUsersCreated_ShouldReturnTrue()
        {
            var user_1 = new User { userID = "1", firstName = "testNameFN", lastName = "testLastName", email = "email@email.com", birthDate = new DateTime(2010, 12, 12), retirementDate = new DateTime(2010, 12, 12).AddYears(62) };
            var user_2 = new User { userID = "2", firstName = "testNameFN_2", lastName = "testLastName", email = "email@email.com", birthDate = new DateTime(2010, 12, 12), retirementDate = new DateTime(2010, 12, 12).AddYears(62) };
            var user_3 = new User { userID = "3", firstName = "testNameFN_3", lastName = "testLastName", email = "email@email.com", birthDate = new DateTime(2010, 12, 12), retirementDate = new DateTime(2010, 12, 12).AddYears(62) };
            var user_4 = new User { userID = "4", firstName = "testNameFN_4", lastName = "testLastName", email = "email@email.com", birthDate = new DateTime(2010, 12, 12), retirementDate = new DateTime(2010, 12, 12).AddYears(62) };
            var user_5 = new User { userID = "5", firstName = "testNameFN_5", lastName = "testLastName", email = "email@email.com", birthDate = new DateTime(2010, 12, 12), retirementDate = new DateTime(2010, 12, 12).AddYears(62) };
            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                var userInserted_1 = _userRepository.Create(user_1);
                var userInserted_2 = _userRepository.Create(user_2);
                var userInserted_3 = _userRepository.Create(user_3);
                var userInserted_4 = _userRepository.Create(user_4);
                var userInserted_5 = _userRepository.Create(user_5);
                
                var response = usersController.Get() as ObjectResult;
                var usersConsulted = (List<User>) response.Value;

                var responseStatus = GetHttpStatusCode(response);
                
                Assert.IsTrue(usersConsulted.Count == 5);
                Assert.AreEqual(HttpStatusCode.OK, responseStatus);
            }
        }
        
        [TestMethod]
        public void GetUser_GetUserByNotExistingId_ShouldReturnNoUser()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(2010, 12, 12), retirementDate = new DateTime(2010, 12, 12).AddYears(62) };
            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                _userRepository.Create(user);
                var response = usersController.Get("2") as ObjectResult;
                var responseStatus = GetHttpStatusCode(response);

                Assert.AreEqual(HttpStatusCode.NotFound, responseStatus);
            }
        }

        [TestMethod]
        public void GetUser_GetUserByNullId_ShouldReturBadRequest()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(2010, 12, 12), retirementDate = new DateTime(2010, 12, 12).AddYears(62) };
            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                _userRepository.Create(user);
                var response = usersController.Get(null) as ObjectResult;
                var responseStatus = GetHttpStatusCode(response);

                Assert.AreEqual(HttpStatusCode.BadRequest, responseStatus);
            }
        }

        [TestMethod]
        public void GetUser_GetUserByEmptyId_ShouldReturBadRequest()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(2010, 12, 12), retirementDate = new DateTime(2010, 12, 12).AddYears(62) };
            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                _userRepository.Create(user);
                var response = usersController.Get(EMPTY_STRING) as ObjectResult;
                var responseStatus = GetHttpStatusCode(response);

                Assert.AreEqual(HttpStatusCode.BadRequest, responseStatus);
            }
        }

        [TestMethod]
        public void UpdateUser_UpdateFirstname_ShouldReturnOK()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(2010, 12, 12), retirementDate = new DateTime(2010, 12, 12).AddYears(62) };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                _userRepository.Create(user);
                var userUpdate = new UserDTO { userID = "1", firstName = "UpdatedName" };
                usersController.Put(userUpdate);

                var response = usersController.Get("1") as ObjectResult;
                var updatedUser = (User)response.Value;
                var responseStatus = GetHttpStatusCode(response);
                
                Assert.AreNotEqual("testNameFN", updatedUser.firstName);
                Assert.AreEqual(HttpStatusCode.OK, responseStatus);
            }
        }
        
        [TestMethod]
        public void UpdateUser_UpdateUserWithValidDataUser_ShouldReturnTrueWhileComparassionFields()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(1982, 12, 12), retirementDate = new DateTime(2000, 12, 12) };
            var userUpdate = new UserDTO { userID = "1", firstName = "UpdatedName", lastName = "UpdatedLastname", email = "Updatedemail@email.com", birthDate = "2000-12-12" };
            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                _userRepository.Create(user);
                usersController.Put(userUpdate);
                
                var response = usersController.Get("1") as ObjectResult;
                var updatedUser = (User) response.Value;
                var responseStatus = GetHttpStatusCode(response);

                Assert.AreEqual(updatedUser.userID, "1");
                Assert.AreNotEqual(updatedUser.firstName, "testNameFN");
                Assert.AreNotEqual(updatedUser.lastName, "dd");
                Assert.AreNotEqual(updatedUser.email, "email@email.com");
                Assert.AreNotEqual(updatedUser.birthDate, new DateTime(1982, 12, 12));
                Assert.AreNotEqual(updatedUser.retirementDate, new DateTime(2000, 12, 12));
                Assert.AreEqual(HttpStatusCode.OK, responseStatus);
            }
        }
        
        [TestMethod]
        public void UpdateUser_UpdateWithNoPresentId_ShouldReturnUserNotFound()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(1982, 12, 12), retirementDate = new DateTime(2000, 12, 12) };
            var userUpdate = new UserDTO { userID = "2", firstName = "UpdatedName" };
            
            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                _userRepository.Create(user);
                
                var response = usersController.Put(userUpdate) as ObjectResult;
                var responseStatus = GetHttpStatusCode(response);

                Assert.AreEqual(HttpStatusCode.NotFound, responseStatus);
            }
        }

        [TestMethod]
        public void UpdateUser_UpdateInvalidId_ShouldReturnBadRequest()
        {
            var userUpdate = new UserDTO { userID = "12345678", firstName = "UpdatedName" };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);


                var response = usersController.Put(userUpdate) as ObjectResult;
                var responseStatus = GetHttpStatusCode(response);

                var resultMsg = response.Value;

                Assert.AreEqual(HttpStatusCode.BadRequest, responseStatus);
                Assert.AreEqual(INVALID_USERID, resultMsg);
            }
        }

        [TestMethod]
        public void UpdateUser_UpdateWithNullId_ShouldReturnBadRequest()
        {
            var userDTO = new UserDTO { userID = null, firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = "1982-12-12" };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                var response = usersController.Put(userDTO) as ObjectResult;

                var responseStatus = GetHttpStatusCode(response);
                var resultMsg = response.Value;

                Assert.AreEqual(HttpStatusCode.BadRequest, responseStatus);
                Assert.AreEqual(NO_USERID, resultMsg);
            }
        }

        
        [TestMethod]
        public void UpdateUser_UpdateWithInvalidBirthDate_ShouldReturnBadRequest()
        {
            var userDTO = new UserDTO { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = "1982-14-12" };
            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                var response = usersController.Put(userDTO) as ObjectResult;

                var responseStatus = GetHttpStatusCode(response);
                var resultMsg = response.Value;

                Assert.AreEqual(HttpStatusCode.BadRequest, responseStatus);
                Assert.AreEqual(INVALID_BIRTHDATE, resultMsg);
            }
        }
        
        [TestMethod]
        public void UpdateUser_UpdateWithUnderage_ShouldReturnBadRequest()
        {
            var userDTO = new UserDTO { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = "2012-12-12" };
            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                var response = usersController.Put(userDTO) as ObjectResult;

                var responseStatus = GetHttpStatusCode(response);
                var resultMsg = response.Value;

                Assert.AreEqual(HttpStatusCode.BadRequest, responseStatus);
                Assert.AreEqual(BIRTHDATE_UNDERAGE, resultMsg);
            }
        }
        
        [TestMethod]
        public void UpdateUser_UpdateWithInvalidEmail_ShouldReturnBadRequest()
        {
            var userDTO = new UserDTO { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email.com", birthDate = "1982-12-12" };
            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                var response = usersController.Put(userDTO) as ObjectResult;

                var responseStatus = GetHttpStatusCode(response);
                var resultMsg = response.Value;

                Assert.AreEqual(HttpStatusCode.BadRequest, responseStatus);
                Assert.AreEqual(INVALID_EMAIL, resultMsg);
            }
        }
        
        [TestMethod]
        public void DeleteUser_DeleteByValidId_ShouldReturnOK()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(1982, 12, 12), retirementDate = new DateTime(2000, 12, 12) };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                _userRepository.Create(user);

                var response = usersController.Delete(user.userID) as ObjectResult;
                var responseStatus = GetHttpStatusCode(response);
                
                Assert.AreEqual(HttpStatusCode.OK, responseStatus);

            }
        }
        
        [TestMethod]
        public void DeleteUser_DeleteNullId_ShouldReturnBadRequest()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(1982, 12, 12), retirementDate = new DateTime(2000, 12, 12) };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                _userRepository.Create(user);

                var response = usersController.Delete(null) as ObjectResult;
                var responseStatus = GetHttpStatusCode(response);

                Assert.AreEqual(HttpStatusCode.BadRequest, responseStatus);
            }
        }

        [TestMethod]
        public void DeleteUser_DeleteNotExistingUserId_ShouldReturnUserCreated()
        {
            var user = new User { userID = "123465798", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(1982, 12, 12), retirementDate = new DateTime(2000, 12, 12) };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                _userRepository.Create(user);

                var response = usersController.Delete("33") as ObjectResult;
                var responseStatus = GetHttpStatusCode(response);

                Assert.AreEqual(HttpStatusCode.NotFound, responseStatus);
            }
        }

        [TestMethod]
        public void DeleteUser_DeleteInvalidId_ShouldReturnUserCreated()
        {
            var user = new User { userID = "123465798", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(1982, 12, 12), retirementDate = new DateTime(2000, 12, 12) };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                IUserRepository _userRepository = new UserRepository(context);
                var usersController = new UsersController(_userRepository);
                _userRepository.Create(user);

                var response = usersController.Delete(user.userID) as ObjectResult;
                var responseStatus = GetHttpStatusCode(response);

                var resultMsg = response.Value;

                Assert.AreEqual(HttpStatusCode.BadRequest, responseStatus);
                Assert.AreEqual(INVALID_USERID, resultMsg);
            }
        }

        private static HttpStatusCode GetHttpStatusCode(IActionResult functionResult)
        {
            try
            {
                return (HttpStatusCode)functionResult
                    .GetType()
                    .GetProperty("StatusCode")
                    .GetValue(functionResult, null);
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
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
    }
}

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestCode_SK.Controllers;
using TestExerciseSK.DAL;
using TestExerciseSK.Models;
using TestExerciseUnitTestsSK.Context;

namespace TestExerciseSKTests.DAL
{
    [TestClass]
    public class UserRepositoryTest
    {
        private SqlLite_UnitTests sqlLiteUnitTests;
        [TestInitialize]
        public void Init()
        {
            sqlLiteUnitTests = new SqlLite_UnitTests();
        }

        [TestMethod]
        public void CreateUser_CreateWithValidDataUser_ShouldReturnUserCreated()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(1982, 12, 12), retirementDate = new DateTime(1982, 12, 12) };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                var userRepository = new UserRepository(context);
                var result = userRepository.Create(user);

                Assert.AreEqual(user, result);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateUser_CreateWithExistingId_ShouldReturnInvalidOperationException()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(1982, 12, 12), retirementDate = new DateTime(1982, 12, 12) };
            var userClone = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(1982, 12, 12), retirementDate = new DateTime(1982, 12, 12) };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                var userRepository = new UserRepository(context);
                var result = userRepository.Create(user);
                Assert.AreEqual(user, result);
                userRepository.Create(userClone);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateUser_CreateWithNullId_ShouldReturnInvalidOperationException()
        {
            var user = new User { userID = null, firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(1982, 12, 12), retirementDate = new DateTime(1982, 12, 12) };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                var userRepository = new UserRepository(context);
                var result = userRepository.Create(user);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateUser_CreateWithInvalidBirthDate_ShouldReturnArgumentOutOfRangeException()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(1982, 14, 12), retirementDate = new DateTime(1982, 12, 12).AddYears(62) };
            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                var userRepository = new UserRepository(context);
                var result = userRepository.Create(user);
            }
        }

        [TestMethod]
        public void GetUser_GetUserByExistingId_ShouldReturnSameUserCreatedWithId()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(2010, 12, 12), retirementDate = new DateTime(2010, 12, 12).AddYears(62) };
            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                var userRepository = new UserRepository(context);
                var userCreated = userRepository.Create(user);

                var userConsulted = userRepository.GetUserById(userCreated.userID);
                Assert.AreEqual(userConsulted, userCreated);
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
                var userRepository = new UserRepository(context);
                var userInserted_1 = userRepository.Create(user_1);
                var userInserted_2 = userRepository.Create(user_2);
                var userInserted_3 = userRepository.Create(user_3);
                var userInserted_4 = userRepository.Create(user_4);
                var userInserted_5 = userRepository.Create(user_5);

                List<User> usersConsulted = userRepository.GetUsers();
                Assert.IsTrue(usersConsulted.Count == 5);
            }
        }

        [TestMethod]
        public void GetUser_GetUserByNotExistingId_ShouldReturnNoUser()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(2010, 12, 12), retirementDate = new DateTime(2010, 12, 12).AddYears(62) };
            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                var userRepository = new UserRepository(context);
                var userCreated = userRepository.Create(user);

                var userConsulted = userRepository.GetUserById("2");
                Assert.IsNull(userConsulted);
            }
        }

        [TestMethod]
        public void UpdateUser_UpdateNameWithValidDataUser_ShouldReturnDataUserCreated()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(1982, 12, 12), retirementDate = new DateTime(1982, 12, 12) };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                var userRepository = new UserRepository(context);
                userRepository.Create(user);
                var userUpdate = new User { userID = "1", firstName = "UpdatedName" };
                userRepository.Update(userUpdate);
                var updatedUser = userRepository.GetUserById("1");
                Assert.AreEqual(updatedUser.userID, user.userID);
                Assert.AreNotEqual(updatedUser.firstName, "testNameFN");
            }
        }

        [TestMethod]
        public void UpdateUser_UpdateUserWithValidDataUser_ShouldReturnUserCreated()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(1982, 12, 12), retirementDate = new DateTime(1982, 12, 12) };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                var userRepository = new UserRepository(context);
                userRepository.Create(user);
                var userUpdate = new User { userID = "1", firstName = "UpdatedName", lastName = "UpdatedLastname", email = "Updatedemail@email.com", birthDate = new DateTime(2000, 12, 12), retirementDate = new DateTime(2000, 12, 12) };
                userRepository.Update(userUpdate);
                var updatedUser = userRepository.GetUserById("1");
                Assert.AreEqual(updatedUser.userID, "1");
                Assert.AreNotEqual(updatedUser.firstName, "testNameFN");
                Assert.AreNotEqual(updatedUser.lastName, "dd");
                Assert.AreNotEqual(updatedUser.email, "email@email.com");
                Assert.AreNotEqual(updatedUser.birthDate, new DateTime(1982, 12, 12));
                Assert.AreNotEqual(updatedUser.retirementDate, new DateTime(1982, 12, 12));
            }
        }

        [TestMethod]
        public void UpdateUser_UpdateInvalidId_ShouldReturnFalseInUpdate()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(1982, 12, 12), retirementDate = new DateTime(1982, 12, 12) };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                var userRepository = new UserRepository(context);
                userRepository.Create(user);
                var userUpdate = new User { userID = "2", firstName = "UpdatedName" };
                bool updatedRecords = userRepository.Update(userUpdate);
                var updatedUser = userRepository.GetUserById("1");
                Assert.AreEqual(updatedUser.userID, user.userID);
                Assert.IsFalse(updatedRecords);
            }
        }

        [TestMethod]
        public void DeleteUser_DeleteByValidId_ShouldReturnUserDeletedTrue()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(1982, 12, 12), retirementDate = new DateTime(1982, 12, 12) };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                var userRepository = new UserRepository(context);
                userRepository.Create(user);
                var createdUser = userRepository.GetUserById("1");
                bool isCreatedUser = createdUser != null;
                userRepository.Delete("1");
                var deletedUser = userRepository.GetUserById("1");
                Assert.IsTrue(isCreatedUser);
                Assert.IsNull(deletedUser);
            }
        }

        [TestMethod]
        public void DeleteUser_DeleteInvalidId_ShouldReturnUserDeletedFalse()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(1982, 12, 12), retirementDate = new DateTime(1982, 12, 12) };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                var userRepository = new UserRepository(context);
                userRepository.Create(user);
                bool deletedRecords = userRepository.Delete("2");
                Assert.IsFalse(deletedRecords);
            }
        }

        [TestMethod]
        public void DeleteUser_DeleteNullId_ShouldReturnUserDeletedFalse()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(1982, 12, 12), retirementDate = new DateTime(1982, 12, 12) };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                var userRepository = new UserRepository(context);
                userRepository.Create(user);
                bool deletedRecords = userRepository.Delete(null);
                Assert.IsFalse(deletedRecords);
            }
        }

        [TestMethod]
        public void DeleteUser_DeleteNotExistinglId_ShouldReturnUserDeletedFalse()
        {
            var user = new User { userID = "1", firstName = "testNameFN", lastName = "dd", email = "email@email.com", birthDate = new DateTime(1982, 12, 12), retirementDate = new DateTime(1982, 12, 12) };

            using (var context = sqlLiteUnitTests.GetDbContext())
            {
                var userRepository = new UserRepository(context);
                userRepository.Create(user);
                bool deletedRecords = userRepository.Delete("123456789");
                Assert.IsFalse(deletedRecords);
            }
        }
    }
}

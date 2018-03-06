using Exiger.JWT.Core.Data.EF;
using Exiger.JWT.Core.Models;
using Newtonsoft.Json;
using System;

namespace Exiger.JWT.AdminConsoleTool.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var action = String.Empty;
            string error;

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Action (add-client, add-user, list-users, list-clients):");
                action = Console.ReadLine();
            }

            if (String.IsNullOrEmpty(action))
            {
                return;
            }

            using(var unitOfWork = new UnitOfWorkFactory().Create())
            {
                switch (action)
                {
                    case "list-clients":
                        var clientAccounts = unitOfWork.ClientAccountRepository.GetAllAccounts(x => x);
                        if (clientAccounts != null)
                        {
                            foreach (var clientAccount in clientAccounts)
                            {
                                Console.WriteLine(JsonConvert.SerializeObject(clientAccount));
                            }
                        }
                        Console.WriteLine("Press any key to exit");
                        Console.ReadLine();
                        return;
                    case "list-users":
                        var users = unitOfWork.UserRepository.GetUsers(x => x);
                        if (users != null)
                        {
                            foreach (var user in users)
                            {
                                Console.WriteLine(JsonConvert.SerializeObject(user));
                            }
                        }
                        Console.WriteLine("Press any key to exit");
                        Console.ReadLine();
                        return;
                    case "add-user":
                        Console.WriteLine("New User name:");
                        var userName = Console.ReadLine();
                        Console.WriteLine("User email:");
                        var userEmail = Console.ReadLine();
                        Console.WriteLine("User password:");
                        var userPassword = Console.ReadLine();
                        if (!String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(userEmail) && !String.IsNullOrEmpty(userPassword))
                        {
                            error = CreateNewUser(unitOfWork, userName, userEmail, userPassword);
                            if (String.IsNullOrEmpty(error))
                            {
                                Console.WriteLine("Successfully created new user");
                                Console.ReadLine();
                            }
                            else
                            {
                                Console.WriteLine(error);
                                Console.ReadLine();
                            }
                        }
                        return;
                    case "add-client":
                        Console.WriteLine("New Client name:");
                        var clientName = Console.ReadLine();
                        if (!String.IsNullOrEmpty(clientName))
                        {
                            error = CreateNewClient(unitOfWork, clientName);
                            if (String.IsNullOrEmpty(error))
                            {
                                Console.WriteLine("Successfully created new account");
                                Console.ReadLine();
                            }
                            else
                            {
                                Console.WriteLine(error);
                                Console.ReadLine();
                            }
                        }
                        return;
                    default:
                        Console.WriteLine("Incorrect action argument. Press any key to exit");
                        Console.ReadLine();
                        return;
                }
            }

        }

        private static string CreateNewUser(IUnitOfWork unitOfWork, string userName, string userEmail, string userPassword)
        {
            var errorMsg = String.Empty;

            var checkUser = unitOfWork.UserRepository.GetUserByName(x => x.UserName, userName);
            if (!String.IsNullOrEmpty(checkUser))
            {
                return "User already exists";
            }
            var clientAccount = unitOfWork.ClientAccountRepository.GetAccountById(1);
            if (clientAccount == null)
            {
                return "Default client account doesn't exist";
            }
            var newUser = new ClientUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = userName,
                Email = userEmail,
                Client = clientAccount,
                Active = true,
            };
            try
            {
                unitOfWork.UserRepository.CreateUser(newUser, userPassword);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return errorMsg;
        }

        private static string CreateNewClient(IUnitOfWork unitOfWork, string clientName)
        {
            var errorMsg = String.Empty;

            var checkClient = unitOfWork.ClientAccountRepository.GetAccountByName(x => x.ClientName, clientName);
            if (!String.IsNullOrEmpty(checkClient))
            {
                return "Client already exists";
            }

            var newAccount = new ClientAccount
            {
                ClientName = clientName,
            };
            try
            {
                unitOfWork.ClientAccountRepository.CreateAccount(newAccount);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return errorMsg;
        }
    }
}

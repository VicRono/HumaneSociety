using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {

        internal static List<USState> GetStates()
        {
            HumaneSocietyDataContext  db = new HumaneSocietyDataContext();

            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }

        internal static Client GetClient(string userName, string password)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.AddressLine2 = null;
                newAddress.Zipcode = zipCode;
                newAddress.USStateId = stateId;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        public static List<Adoption> GetPendingAdoptions()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var pendingAdoptions = db.Adoptions.Where(m => m.ApprovalStatus.ToLower() == "Pending").ToList();
            return pendingAdoptions;
        }

        public delegate void AdminDelgate(Employee employee);

        public static void RunEmployeeQueries(Employee employee, string v)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            AdminDelgate adminDelgate;
            switch (v)
            {
                case "add":
                    adminDelgate = new AdminDelgate(AddEmployee);
                    adminDelgate(employee);
                    break;
                case "update":
                    adminDelgate = new AdminDelgate(UpdateEmployee);
                    adminDelgate(employee);
                    break;
                case "read":
                    adminDelgate = new AdminDelgate(ReadEmployee);
                    adminDelgate(employee);
                    break;
                case "delete":
                    adminDelgate = new AdminDelgate(DeleteEmployee);
                    adminDelgate(employee);
                    break;
            }
        }

        public static void AddEmployee(Employee employee)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            db.Employees.InsertOnSubmit(employee);
            db.SubmitChanges();
        }

        public static void UpdateEmployee(Employee employee)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var employeeInDb = db.Employees.Where(u => u.EmployeeNumber == employee.EmployeeNumber).Single();
            employeeInDb.FirstName = employee.FirstName;
            employeeInDb.LastName = employee.LastName;
            employeeInDb.UserName = employee.UserName;
            employeeInDb.Password = employee.Password;
            employeeInDb.Email = employee.Email;
            db.SubmitChanges();
        }

        public static void ReadEmployee(Employee employee)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var employeeInDb = db.Employees.Where(u => u.EmployeeNumber == employee.EmployeeNumber).Single();
            UserInterface.DisplayEmployeeInfo(employeeInDb);
        }

        public static void DeleteEmployee(Employee employee)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var deleteEmployeeInDb = db.Employees.Where(u => u.EmployeeNumber == employee.EmployeeNumber).Single();
            db.Employees.DeleteOnSubmit(deleteEmployeeInDb);
            db.SubmitChanges();
        }

        public static Animal GetAnimalByID(int id)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Animal animal = db.Animals.Where(c => c.AnimalId == id).Single();
            return animal;
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            // find corresponding Client from Db
            Client clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.AddressLine2 = null;
                newAddress.Zipcode = clientAddress.Zipcode;
                newAddress.USStateId = clientAddress.USStateId;

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }

        internal static List<Animal> SearchForAnimalByMultipleTraits(Dictionary<int, string> searchParameterDictionary)
        {

            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            List<Animal> defaultReturn = new List<Animal>();
            try
            {
                string dictionaryValue = searchParameterDictionary.Values.ElementAt(0);
                int dictionaryKey = searchParameterDictionary.Keys.ElementAt(0);


                switch (dictionaryKey)
                {
                    case 1:
                        var categorySearchResult = db.Animals.Where(a => a.Category.Name == dictionaryValue).ToList();
                        return categorySearchResult;
                    case 2:
                        var nameSearchResult = db.Animals.Where(a => a.Name == dictionaryValue).ToList();
                        return nameSearchResult;
                    case 3:
                        var ageSearchResult = db.Animals.Where(a => a.Age == Int32.Parse(dictionaryValue)).ToList();
                        return ageSearchResult;
                    case 4:
                        var demeanorSearchResult = db.Animals.Where(a => a.Demeanor == dictionaryValue).ToList();
                        return demeanorSearchResult;
                    case 5:
                        var kidFriendlySearchResult = db.Animals.Where(a => a.KidFriendly.ToString() == dictionaryValue).ToList();
                        return kidFriendlySearchResult;
                    case 6:
                        var petFriendlySearchResult = db.Animals.Where(a => a.PetFriendly.ToString() == dictionaryValue).ToList();
                        return petFriendlySearchResult;
                    case 7:
                        var weightSearchResult = db.Animals.Where(a => a.Weight == Int32.Parse(dictionaryValue)).ToList();
                        return weightSearchResult;
                    case 8:
                        var idSearchResult = db.Animals.Where(a => a.AnimalId == Int32.Parse(dictionaryValue)).ToList();
                        return idSearchResult;
                    case 9:

                        return SearchForAnimalByMultipleTraits(searchParameterDictionary);
                    default:
                        List<Animal> defaultRetrun = new List<Animal>();
                        return defaultReturn;

                }
            }

            catch
            {
                return defaultReturn;

            }
        }

        public static void Adopt(Animal animal, Client client)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var adoptionFromDb = db.Adoptions.Where(a => a.AnimalId == animal.AnimalId).SingleOrDefault();
            if (adoptionFromDb == null)
            {
                CreateAdoption(animal, client);
            }
            else if (adoptionFromDb.ClientId == client.ClientId)
            {
                UserInterface.DisplayUserOptions("You've request for adoption is being processed. You will be notified once its approved.");
                UserInterface.GetUserInput();
            }
            else if (adoptionFromDb.ApprovalStatus == "denied")
            {
                adoptionFromDb.ClientId = client.ClientId;
                UpdateAdoption(true, adoptionFromDb);
                UserInterface.DisplayUserOptions("Adoption request sent we will hold $75 adoption fee until processed");
                UserInterface.GetUserInput();
                return;
            }
            else
            {
                UserInterface.DisplayUserOptions("Already requested by someone else. Sorry !");
                UserInterface.GetUserInput();
            }
            db.SubmitChanges();
        }

        public static void CreateAdoption(Animal animal, Client client)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var animalFromDb = db.Animals.Where(c => c.AnimalId == animal.AnimalId).Single();
            Adoption adoption = new Adoption();
            adoption.AdoptionFee = 50;
            adoption.ClientId = client.ClientId;
            adoption.AnimalId = animal.AnimalId;
            adoption.ApprovalStatus = "pending";

            animalFromDb.AdoptionStatus = "requested";

            adoption.PaymentCollected = false;
            UserInterface.DisplayUserOptions("Adoption request sent we will hold $50 adoption fee until processed");
            UserInterface.GetUserInput();
            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();

        }

        public static Room GetRoom(int id)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var room = db.Rooms.Where(r => r.AnimalId == id).Single();
            return room;
        }

        public static List<Room> GetAnimalHousing()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var bookedRooms = db.Rooms.Where(c => c.AnimalId != null).ToList();
            return bookedRooms;
        }

        public static void UpdateAdoption(bool v, Adoption adoption)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var adoptionFromDb = db.Adoptions.Where(a => a.AdoptionId == adoption.AdoptionId).Single();
            if (v)
            {
                adoptionFromDb.ApprovalStatus = "Status is approved";
                adoptionFromDb.Animal.AdoptionStatus = "Adopted";
                adoptionFromDb.PaymentCollected = true;
            }
            else
            {
                adoptionFromDb.ApprovalStatus = "Status is denied";
                adoptionFromDb.Animal.AdoptionStatus = "Available";
            }

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if(employeeFromDb == null)
            {
                throw new NullReferenceException();            
            }
            else
            {
                return employeeFromDb;
            }            
        }

        public static void UpdateShot(string word, Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            var shotUpdateAnimal = db.AnimalShots.Where(s => s.AnimalId == animal.AnimalId).SingleOrDefault();
            var shotUpdateShot = db.AnimalShots.Where(f => f.Shot.Name == word && f.AnimalId == animal.AnimalId).ToArray();
            var shotz = shotUpdateShot;

            if (shotUpdateShot.Length > 0)
            {
                var thisShot = shotz[0];
                thisShot.DateReceived = DateTime.Now;
                db.SubmitChanges();
            }

            if (shotUpdateAnimal == null)
            {
                AnimalShot animalShot = new AnimalShot { AnimalId = animal.AnimalId, ShotId = 6, DateReceived = DateTime.Now };
                db.AnimalShots.InsertOnSubmit(animalShot);
                db.SubmitChanges();
            }
        }

        public static List<Room> GetAvailableRooms()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var availableRooms = db.Rooms.Where(c => c.AnimalId == null).ToList();
            return availableRooms;
        }

        public static List<AnimalShot> GetShots(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var AnimalShotinfo = db.AnimalShots.Where(s => s.AnimalId == animal.AnimalId).ToList();
            return AnimalShotinfo;
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }

        public static Animal EnterAnimalUpdate(Animal animal, Dictionary<int, string> updates)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var animalInDb = db.Animals.Where(c => c.AnimalId == animal.AnimalId).Single();
            for (int i = 0; i < updates.Count; i++)
            {
                int userInput = updates.Keys.ElementAt(i);
                string updatedValue = updates.Values.ElementAt(i);
                switch (userInput)
                {
                    case 1:
                        var updatedCategory = db.Categories.Where(c => c.Name == updatedValue).Single();
                        animalInDb.CategoryId = updatedCategory.CategoryId;
                        break;
                    case 2:
                        animalInDb.Name = updatedValue;
                        break;
                    case 3:
                        animalInDb.Age = Int32.Parse(updatedValue);
                        break;
                    case 4:
                        animalInDb.Gender = updatedValue;
                        break;
                    case 5:
                        animalInDb.Demeanor = updatedValue;
                        break;
                    case 6:
                        animalInDb.KidFriendly = AnimalBehaviour(animalInDb.KidFriendly);
                        break;
                    case 7:
                        animalInDb.PetFriendly = AnimalBehaviour(animalInDb.PetFriendly);
                        break;
                    case 8:
                        animalInDb.Weight = Int32.Parse(updatedValue);
                        break;
                }
            }

            db.SubmitChanges();
            return animalInDb;
        }

        private static bool? AnimalBehaviour(bool? userInput)
        {
            if (userInput ?? false)
            {
                return false;
            }
            return true;
        }

        internal static void AddUsernameAndPassword(Employee employee)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();

            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        public static void UpdateRoom(int number, int id)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            Room roomFromDb = db.Rooms.Where(r => r.RoomNumber == number).Single();
            if (roomFromDb.AnimalId != null)
            {
                if (UserInterface.GetBitData("Already Occupied! Do you still want to update?"))
                {
                    roomFromDb.AnimalId = id;
                }
            }
            else
            {
                roomFromDb.AnimalId = id;
            }
            db.SubmitChanges();
        }

        public static void RemoveAnimal(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var animalsToBeRemoved = db.Animals.Where(d => d.Name == animal.Name).ToList();
            ResetRoomFields(animal);

            if (db.AnimalShots.Where(s => s.AnimalId == animal.AnimalId) != null)
            {
                RemoveAnimalShotFields(animal);
            }

            if (db.Adoptions.Where(a => a.AnimalId == animal.AnimalId) != null)

            {
                RemoveAdoptionFields(animal);
            }

            db.Animals.DeleteAllOnSubmit(animalsToBeRemoved);
            db.SubmitChanges();
        }

        private static void RemoveAdoptionFields(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var resetAdoptions = db.Adoptions.Where(a => a.AnimalId == animal.AnimalId).SingleOrDefault();
            if (resetAdoptions != null)
            {
                db.Adoptions.DeleteOnSubmit(resetAdoptions);
                db.SubmitChanges();
            }
        }

        private static void RemoveAnimalShotFields(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var resetAnimalShots = db.AnimalShots.Where(s => s.AnimalId == animal.AnimalId).SingleOrDefault();
            if (resetAnimalShots != null)
            {
                db.AnimalShots.DeleteOnSubmit(resetAnimalShots);
                db.SubmitChanges();
            }
        }

        private static void ResetRoomFields(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var resetRooms = db.Rooms.Where(r => r.AnimalId == animal.AnimalId).SingleOrDefault();
            if (resetRooms != null)
            {
                db.Rooms.DeleteOnSubmit(resetRooms);
                db.SubmitChanges();
            }
        }

        public static int GetCategoryId()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            List<string> types = new List<string>() { "What is the Animal Type:", "1. Dog", "2. Cat", "3. Lizard", "4. Bird", "5. Rodent", "Enter the corresponding number." };
            UserInterface.DisplayUserOptions(types);
            int animalTypeId = Int32.Parse(UserInterface.GetUserInput());
            var category = db.Categories.Where(c => c.CategoryId == animalTypeId).Single();
            return category.CategoryId;
        }

        public static int GetDietPlanId()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            List<string> plans = new List<string>() { "Choose a Diet Plan:", "1. Canine", "2. Feline", "3. Avian", "4. Reptile", "5. Mammal", "Enter the corresponding number." };
            UserInterface.DisplayUserOptions(plans);
            int dietNameId = Int32.Parse(UserInterface.GetUserInput());
            var dietPlan = db.DietPlans.Where(m => m.DietPlanId == dietNameId).Single();
            return dietPlan.DietPlanId;
        }

        public static bool CheckDietPlanName(string name)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            try
            {
                DietPlan dietplanFromDB = db.DietPlans.Where(c => c.Name == name).Single();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public static void AddAnimal(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        public static void AddDietPlan(DietPlan dietplan)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            db.DietPlans.InsertOnSubmit(dietplan);
            db.SubmitChanges();
        }

        public static List<Category> GetAllCategories()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var allCategories = db.Categories.ToList();
            return allCategories;
        }

        public static List<DietPlan> GetAllDietPlans()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var allDietPlans = db.DietPlans.ToList();
            return allDietPlans;
        }

        public static void AddCategory(Category category)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            db.Categories.InsertOnSubmit(category);
            db.SubmitChanges();
        }

        public static bool CheckCategoryName(string name)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            try
            {
                Category categoryFromDb = db.Categories.Where(c => c.Name == name).Single();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public static DietPlan FindDietPlan(string nameOfPlan)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            DietPlan planToMod = db.DietPlans.Where(d => d.Name == nameOfPlan).Single();
            return planToMod;
        }
    }
}
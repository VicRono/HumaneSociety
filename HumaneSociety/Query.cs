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

        internal static void RunEmployeeQueries(Employee employee, string v)
        {
            throw new NotImplementedException();
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

        internal static void Adopt(object animal, Client client)
        {
            throw new NotImplementedException();
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



        internal static object SearchForAnimalByMultipleTraits()
        {
            throw new NotImplementedException();
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

        internal static void UpdateShot(string v, Animal animal)
        {
            throw new NotImplementedException();
        }

        public static List<Room> GetAvailableRooms()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var availableRooms = db.Rooms.Where(c => c.AnimalId == null).ToList();
            return availableRooms;
        }

        internal static object GetShots(Animal animal)
        {
            throw new NotImplementedException();
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

        internal static void EnterAnimalUpdate(Animal animal, Dictionary<int, string> updates)
        {
            throw new NotImplementedException();
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

        internal static void RemoveAnimal(object animal)
        {
            throw new NotImplementedException();
        }

        internal static int? GetCategoryId()
        {
            throw new NotImplementedException();
        }

        internal static int? GetDietPlanId()
        {
            throw new NotImplementedException();
        }

        public static void AddAnimal(Animal animal)
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        public static List<Category> GetAllCategories()
        {
            HumaneSocietyDataContext db = new HumaneSocietyDataContext();
            var allCategories = db.Categories.ToList();
            return allCategories;
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

    }
}
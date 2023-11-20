using System;


/*Program CeckList
    Inventory Manager
    Employee Manager
    Event Scheduler
    Day Record Tracking ie: Sales Numbers, Customer Tickets, Ticket Times, Menu Item Popularity....
    Notifications ie : Low Inventory, Event Reminders, Menu Update ....
    
    DataBase Intergration
*/
 

namespace PerkUp
{
    class Program
    {
       
        static void Main(string[] args)
        {
            InventoryManager inventoryManager = new InventoryManager();
            bool runningProgram = true;
        while(runningProgram)
        {

        
            Console.Write("Would you like to continue? (y) yes (n)no: ");
            string userInput = Console.ReadLine().ToLower();
            if(userInput == "n" || userInput == "no")
            {
                runningProgram = false;
            }
        }
           


        }
    }
}
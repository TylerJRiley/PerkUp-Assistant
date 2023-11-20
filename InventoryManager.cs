using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;


//Todo Notes: 
//add new user input class
//refactor
//make inventory manager testing class


namespace PerkUp;

public class InventoryManager
{
    private List<InventoryItem> inventoryItems;
    private List<int> freedIDs;
    private List<InventoryItem> itemsToReorder;
    private delegate List<InventoryItem> listMinipulation(List<InventoryItem> listToMinipulate);
    private delegate bool listCheckForItem(InventoryItem ItemCheck);
    
    public InventoryManager()
    {
        inventoryItems = new List<InventoryItem>();
        freedIDs = new List<int>();
        itemsToReorder = new List<InventoryItem>();
        
    }

    private List<InventoryItem> SortByID (List<InventoryItem> item) 
    {
        return item.OrderBy(i => i.itemID).ToList();
    } 
    private List<InventoryItem> SortByName(List<InventoryItem> item)
    {
        return item.OrderBy(i => i.itemName).ToList();
    }
    private bool CheckReorderList(InventoryItem itemToCompare)
    {
        if (itemsToReorder.Count != 0)
        {
            if (itemsToReorder.Contains(itemToCompare))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    private bool CheckInventoryList(InventoryItem itemToCompare)
    {
        if(inventoryItems.Count!=0)
        {
            if (inventoryItems.Contains(itemToCompare))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else 
        {
            return false;
        }
    }
    public InventoryItem InputStringToItem(List<InventoryItem> itemList)
    {
       
        InventoryItem convertedStringItem = null;
        
        while(convertedStringItem ==null)
        {
            string userInput = UserInputAsString("");
            if(!string.IsNullOrWhiteSpace(userInput))
            {
                convertedStringItem = itemList.FirstOrDefault(i => i.itemName.Equals(userInput, StringComparison.OrdinalIgnoreCase));
                if(convertedStringItem == null)
                {
                   Console.WriteLine("That item is not in the Inventory List please try again.");
                }
            }
            else
            {
                Console.WriteLine("Im sorry it seems you have have not entered a Item Name please try again.");
            }

        }
         return convertedStringItem;
    }
    private List<InventoryItem> FilterByCatigory(List<InventoryItem> listRef,string filterItem)
    {
        if (!string.IsNullOrWhiteSpace(filterItem))
        {
           
            return listRef.Where(i => i.itemCategory.ToLower() == filterItem.ToLower()).ToList();
        }
        else
        {
            Console.WriteLine($"List was not able to be Filterd by {filterItem}. Either that catigory does not exist or it was entred incorectly. Please check your selection again.");
            return listRef;
        }
    }

    public void VeiwByCloseToReorder()
    {
        if(itemsToReorder.Count !=0)
        {
            foreach(InventoryItem item in itemsToReorder)
            {
                Console.WriteLine($"{item.itemName} has {item.itemStockQuantity} in stock and is at or passed its ReorderLevel of {item.itemReorderLevel}");
            }
        }
        else
        {
            Console.WriteLine("There are Currently no items in Reorder Range.");
        }
    }

    public void ViewByCatigory()
    {
        Console.Write("What Catigory would you like to filter by: ");
        string userInput = UserInputAsString("");
        List<InventoryItem> filteredList = FilterByCatigory(inventoryItems,userInput);
        foreach (InventoryItem item in filteredList)
        {
            Console.WriteLine($"Item ID: {item.itemID} - Item name: {item.itemName} - Item Quantity: {item.itemStockQuantity} - Item Reorder Level: {item.itemReorderLevel} ");
        }
    }

    public void InspectItemByName(string itemName)
    {
        var inspectItem = inventoryItems.FirstOrDefault(i => i.itemName.ToLower() == itemName.ToLower());
        if(CheckInventoryList(inspectItem))
        {
            Console.WriteLine($"Item Information is: \n- Item ID: {inspectItem.itemID} \n- Item Name: {inspectItem.itemName} \n- Item Description: {inspectItem.itemDescription} \n- Item Catigory: {inspectItem.itemCategory} \n- Item Quantity in Scock: {inspectItem.itemStockQuantity} \n- Item Reorder Level: {inspectItem.itemReorderLevel}");
        }
        else
        {
            Console.WriteLine($"There is not Item ({itemName}) found in the Inventory List. Would you like to add one? (y) yes (n) no");
            string userInput = Console.ReadLine().ToLower();
            if(userInput == "y" || userInput == "yes")
            {
               AddItem(InventoryItemBuilder(itemName));
            }
        }

    }
    public void ViewAllList()
    { 
        if (inventoryItems.Count != 0)
        {
            List<InventoryItem> ListInOrderByID = SortByID(inventoryItems);
            foreach (InventoryItem item in ListInOrderByID)
            {
                Console.WriteLine($"Item ID: {item.itemID} - Name: {item.itemName} - Catigoriy: {item.itemCategory} - Quanity in stock: {item.itemStockQuantity} - Reorder Level: {item.itemReorderLevel}");
            }
        }
        else
        {
            Console.Write("There seems to be no items in your Invintory List would you like to add one? (y) yes (n) no: ");
             string userInput = Console.ReadLine().ToLower();
                if(userInput == "y" || userInput == "yes")
                {
                    AddItem(InventoryItemBuilder());
                }
        }
    }

    public void AddItem(InventoryItem item) //Add and item to the inventory list
    {   
        if(inventoryItems.Count != 0)
        {   
            if (item.itemID == 0) //If the new item did not recive an Id during user input
            {
                item.itemID = FindFirstAvalableID(inventoryItems); //Set the Id # of the New item to the next ID # in list
                inventoryItems.Add(item);
                Console.WriteLine($"New Item {item.itemName} was Added to your Inventory ");
            }
            else if (inventoryItems.FirstOrDefault(id => id.itemID == item.itemID) != null) //If the User Inputs an ID # Check to see if that number is taken
            {

                Console.WriteLine($"I'm sorry the ID Number: {item.itemID} is not Avalable and is taken by another item.\n");
                item.itemID = FindFirstAvalableID(inventoryItems); //Finds the first avalable Id in the list and sets the new items Id to the open Id #
                inventoryItems.Add(item);
                Console.WriteLine($"The new ID Number for {item.itemName} is {item.itemID}. This is the first avalable ID would you like to change it? (y) Yes (n) No)");
                string userInput = Console.ReadLine().ToLower();
                if(userInput == "y" || userInput == "yes")
                {
                    ChangeIDNumber(item);
                }
                Console.WriteLine($"New Item {item.itemName} was Added to your Inventory at the ID Number or {item.itemID} ");
            }
            else//If the user Set ID # is Avalable then add the Item as Input by the User
            {
               inventoryItems.Add(item);
               Console.WriteLine($"New Item {item.itemName} was Added to your Inventory ");
            }
        }
        else //Sets ID # to 1 starting the item list
        {
            item.itemID = 1;
            inventoryItems.Add(item);
            Console.WriteLine($"New Item {item.itemName} was Added to your Inventory ");
        }
    }

    public void AddToSockQuantity(string item, int quantityToAdd) //add an amoutn to the current stock amount
    {
        var modifiyItem = inventoryItems.FirstOrDefault(id => id.itemName.ToLower() == item.ToLower());
        if( modifiyItem != null)
        {
            modifiyItem.itemStockQuantity += quantityToAdd;
            if(CheckReorderList(modifiyItem) && modifiyItem.itemStockQuantity > modifiyItem.itemReorderLevel)
            {
                itemsToReorder.Remove(modifiyItem);
            }

        }
        else //if there is no item in the list prompt user if they would like to add the item to the inventory list
        {
            Console.WriteLine($"The item ({item}) is not listed. Would you like to add this item to your Inventory? (y) yes (n) no ");
            string userInput = Console.ReadLine().ToLower();
            if (userInput == "y" || userInput == "yes")
            {
                AddItem(InventoryItemBuilder(item));
            }
        }
    }

    public void RemoveFromStockQuantity(string item, int quantityToRemove) //deduct an amount from the quantity of the item in the list
    {
        var modifiyItem = inventoryItems.FirstOrDefault(id => id.itemName.ToLower() == item.ToLower());
        var restock = modifiyItem.itemReorderLevel;
        if(modifiyItem != null && modifiyItem.itemStockQuantity >= quantityToRemove) 
        {
            if (restock >= modifiyItem.itemStockQuantity - quantityToRemove) //if the quantity the user wants to remove is going to be under the reorder value then promt a warning and ask if the user would like to change the reorder value
            {
                modifiyItem.itemStockQuantity -= quantityToRemove;
                Console.WriteLine($"{quantityToRemove} as removed from Inventory");
                itemsToReorder.Add(modifiyItem);
                Console.WriteLine($"ATTENTION! The number of {modifiyItem.itemID} in stock is in/past the Item Reorder range of ({restock}) item(s) in iventory");
                Console.WriteLine("would you would like to change your Reorder range (y) yes (n) no");
                string userInput = Console.ReadLine().ToLower();
                if (userInput == "y" || userInput == "yes")
                {
                    ChangReorderRange(modifiyItem);
                }
            }
            else
            {
                modifiyItem.itemStockQuantity -= quantityToRemove;
                Console.WriteLine($"{quantityToRemove} as removed from Inventory");
            }
        }
        else if (modifiyItem == null) //if there is no item to remove quantity from alurt user that there is no item listed
        {
            Console.WriteLine($"There is no item ({item} listed in the inventory. Please check your selection again)");
        }
        else // if the quantity the user wants to remove is lower than the amount of quantity availabe in stock promps user if they want to enter a new quantity to the list
        {
            Console.WriteLine($"{modifiyItem.itemName} only has {modifiyItem.itemStockQuantity} in stock. Would you like to update the quantity of the {modifiyItem.itemName} quantity? (y) yes (n) No");
            string userInput = Console.ReadLine().ToLower();
            if (userInput == "y" || userInput == "yes")
            {
                ChangeItemQuantity(modifiyItem);
            }
        }
    }

    public void ChangeItemQuantity(InventoryItem item) //allows users to change the quanity of the item in the list
    {
        bool canChangeQuantity = false;
        if(item != null)
        {
            while(!canChangeQuantity)
            {
                Console.Write($"Current quantity avalable {item.itemStockQuantity}. Enter the new quantity amount: ");
                int newQuantity = UserInputAsInt(item.itemStockQuantity);
                if(newQuantity >= 0)
                {
                    item.itemStockQuantity = newQuantity;
                    canChangeQuantity = true;
                }
                else //if the ammount is les than 0 notifiy the user that thats not posable to have less than 0 of an item in stock
                {
                    Console.WriteLine("The ammount you entered must be grater than 0");
                }
            }
        }
    }

    public void ChangeItemCatigory(InventoryItem item)
    {
        bool canChangeCatigory = false;
        if(item != null)
        {
            while(!canChangeCatigory)
            {
                string oldCatigory = item.itemCategory;
                Console.Write($"The Current Catigory is: {oldCatigory}. Please Enter a new Catigory: ");
                string newCatigory = UserInputAsString(oldCatigory);
                if(newCatigory != oldCatigory)
                {
                    Console.WriteLine($"{oldCatigory} was updated to {newCatigory}");
                    item.itemCategory = newCatigory;
                    canChangeCatigory = true;
                }
                else
                {
                    Console.Write($"No Catigory was defined {oldCatigory} will remain is that Corect? (y) yes (n) no ");
                    string userInput = UserInputAsString("n");
                    if (userInput == "y" || userInput == "yes")
                    {
                        canChangeCatigory = true;
                    }
                    
                }
            }

        }
    }
    public void ChangeDescription(InventoryItem item)
    {
        bool canChangeDescrition = false;
        if(item != null)
        {
            while(!canChangeDescrition)
            {
                string oldDescription = item.itemDescription;
                Console.Write($"The Current Catigory is: {oldDescription}. Please Enter a new Catigory: ");
                string newDeisciption = UserInputAsString(oldDescription);
                if(newDeisciption != oldDescription)
                {
                    Console.WriteLine($"{oldDescription} was updated to {newDeisciption}");
                    item.itemCategory = newDeisciption;
                    canChangeDescrition = true;
                }
                else
                {
                    Console.Write($"No Catigory was defined {oldDescription} will remain is that Corect? (y) yes (n) no ");
                    string userInput = UserInputAsString("n");
                    if (userInput == "y" || userInput == "yes")
                    {
                        canChangeDescrition = true;
                    } 
                }
            }

        }

    }

    public void ChangReorderRange(InventoryItem item) //change the reorder value of an item
    {
        bool canChangeRange = false;
        if(item != null)
        {
            while(!canChangeRange)
            {
                Console.Write($"Current quantity avalable {item.itemReorderLevel}. Enter the new quantity amount: ");
                int newRange = UserInputAsInt(item.itemReorderLevel);
                if(newRange >= 1)
                {
                    item.itemReorderLevel = newRange;
                    canChangeRange = true;
                }
                else //notifies the user that they must have more than 1 in the reorder range
                {
                    Console.WriteLine("The ammount you entered must be grater than 1");
                }
            }
        }
    }
    public void RemoveItem(string item) //remove Items from list
    {
 
        if (inventoryItems.Count != 0)
        {
        var itemToRemove = inventoryItems.FirstOrDefault(i => i.itemName.ToLower() == item.ToLower());
        
            if (itemToRemove != null)
            {
                freedIDs.Add(itemToRemove.itemID);
                inventoryItems.Remove(itemToRemove);
                if (CheckReorderList(itemToRemove))
                {
                    itemsToReorder.Remove(itemToRemove);
                }
            }
            else
            {
                Console.WriteLine($"There is no item: {item}. Cannot remove somethithing that is not there. Please check your Options again and try again.");
            }
        }
        else
        {
            Console.WriteLine($"There are no items in this List. Canot remove sothing that is not there.");
        }
    }

    //Fids the First abalable Item Id in the InventoryItems list and retruns the avalable ID #
    private int FindFirstAvalableID(List<InventoryItem> listToSearch)
    {
        int avalableItemID = 0;
        if(listToSearch.Any() && !freedIDs.Any()) //Check to See if anything is curently in the list
        {
            List<InventoryItem> listInOrder = SortByID(inventoryItems); //Sorts the list by Item ID # In accending order
            bool gapFound = false; //Flag set to determan if there is a gap in the ID # in sequince

            
            foreach (InventoryItem item in listInOrder) //Iterates through the sorted list
            {
                int currentId = item.itemID; //Sets the Current ID # the loop is checking
                int checkId = currentId + 1; //Sets the ID # 1 number ahaed in the list to check
               
                // Checks to see if the number after the Current ID # has anything Assined to it
                if (listInOrder.FirstOrDefault(item => item.itemID == checkId) == null) //If there is no Item assined after the current ID # in loop it will Retrun Null
                {
                    avalableItemID = checkId; //Set the first Avalable ID # found to fill the Gap
                    gapFound = true; //Show that there is a gap between ID # in the list
                    break;
                }
            }
            if (gapFound) //If there is a gap in ID # Then set the Returns the Gap #
            {
                return avalableItemID;
            }
            else //If there is no gap in ID # get the last ID # and Return the number proceeding it
            {
                return avalableItemID = listInOrder.Last().itemID + 1;
            }
        }
        else if (freedIDs.Any())
        {
            int freeID = freedIDs.Last();
            freedIDs.Remove(freeID);
            return freeID;
        }
        else //If there is no item in the list to sort then retrun ID # 1 to start the Inventory List ID #'s
        {
            return 1;
        }
    }

    public void ChangeIDNumber(InventoryItem item)
    {
        bool isAvailableID = false;
        int newID = -1;
        while (!isAvailableID)
        {
            Console.Write("\nPlease Enter the new Item ID Number: ");
            newID = UserInputAsInt(0);
            var existingItem = inventoryItems.FirstOrDefault(i => i.itemID == newID);
            if ((item.itemID != 0 && existingItem == null) || existingItem == item)
            {  
                item.itemID = newID;
                isAvailableID = true;
            }
            else 
            {
                Console.WriteLine($"The ID Number you have chosen ({newID}) is not Available or is arlready take please try a new ID Number.");
            }
        }
    }

    public InventoryItem InventoryItemBuilder()
    {
        string newItemName = "New Item";
        string newItemCategory = "No Category";
        string newItemDescription = "No Description";
        int newItemStockQuantity = 1;
        int newItemReorderLevel = 1;
        int newItemID = 0;


        Console.Write("\nItem Name: ");
        newItemName = UserInputAsString(newItemName);
        Console.Write("Item Catigory ");
        newItemCategory = UserInputAsString(newItemCategory);
        Console.Write("Item Description: ");
        newItemDescription = UserInputAsString(newItemDescription);
        Console.Write("Item Starting Quantity:");
        newItemStockQuantity = UserInputAsInt(newItemStockQuantity);
        Console.Write("Item Reorder Level(Optional: 1 by defult):");
        newItemReorderLevel = UserInputAsInt(newItemReorderLevel);
        Console.Write("Item ID Number(Optinal: ID will be set Automaticly): ");
        newItemID = UserInputAsInt(newItemID);

        InventoryItem newInvtoryItem = new InventoryItem(newItemName, newItemCategory, newItemDescription, newItemStockQuantity,newItemReorderLevel,newItemID);
        
        return newInvtoryItem;
    }
    
    public InventoryItem InventoryItemBuilder(string item)
    {
        string newItemName = item;
        string newItemCategory = "No Category";
        string newItemDescription = "No Description";
        int newItemStockQuantity = 1;
        int newItemReorderLevel = 1;
        int newItemID = 0;


        Console.Write("Item Catigory ");
        newItemCategory = UserInputAsString(newItemCategory);
        Console.Write("Item Description: ");
        newItemDescription = UserInputAsString(newItemDescription);
        Console.Write("Item Starting Quantity:");
        newItemStockQuantity = UserInputAsInt(newItemStockQuantity);
        Console.Write("Item Reorder Level(Optional: 1 by defult):");
        newItemReorderLevel = UserInputAsInt(newItemReorderLevel);
        Console.Write("Item ID Number(Optinal: ID will be set Automaticly): ");
        newItemID = UserInputAsInt(newItemID);

        InventoryItem newInvtoryItem = new InventoryItem(newItemName, newItemCategory, newItemDescription, newItemStockQuantity,newItemReorderLevel,newItemID);
        
        return newInvtoryItem;
        
    }


    private string UserInputAsString(string defultValue)
    {
        string? input = Console.ReadLine();
        
        if (!string.IsNullOrEmpty(input))
        {
            return input;
        } 
        else 
        {
           return defultValue;
        }
    }

    private int UserInputAsInt(int defultValue)
    {
        int inputConverted = 0;
        bool canConvert = false;
        string? input = Console.ReadLine();
        if(!string.IsNullOrEmpty(input))
        {
            while(!canConvert)
            {
           
                if(Int32.TryParse(input, out inputConverted))
                {
                    canConvert = true;
                }
                else
                {
                Console.WriteLine("The Value you have entered is not a whole number. Please try again:");
                input = Console.ReadLine();
                }
            }
            return inputConverted;
        }
        else 
        {
            return defultValue;
        }
    }
}

public class InventoryItem
{
    public int itemID { get; set; }
    public string itemName { get; set; }
    public string itemCategory { get; set; }
    public string itemDescription { get; set; }
    public int itemStockQuantity { get; set; }
    public int itemReorderLevel { get; set; }


    public InventoryItem(string name, string category, string description, int stockQuantity, int reorderLevel = 1, int id = 0)
    {
        itemID = id;
        itemName = name;
        itemCategory = category;
        itemDescription = description;
        itemStockQuantity = stockQuantity;
        itemReorderLevel = reorderLevel;
    }
}

using System.Collections.Generic;

namespace PerkUp;

public class ListUtil
{


    public static bool CheckListForItem<T>(T refranceItem, List<T> listRefrance)
    {
        if (refranceItem != null)
        {
            if (listRefrance.Contains(refranceItem))
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
        else if (listRefrance.Count != 0)
        {
            return false;
        }
        else 
        {
            Console.WriteLine("No List Avalable. Retruning False");
            return false;

        }
    }
}

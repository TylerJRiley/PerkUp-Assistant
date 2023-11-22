using System.Collections.Generic;
using System.Linq.Expressions;

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

    public static List<T> SortListBy<T,TKey>(List<T> listRef, Func<T, TKey> keySelector)
    {
        
        return (List<T>)listRef.OrderBy(keySelector).ToList();
    }
}

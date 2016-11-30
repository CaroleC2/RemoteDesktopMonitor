using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDMWinPhone.Extensions
{
    public static class IListExtensions
    {
        /// <summary>
        /// Permet d'ajouter un élément à une IList<T> triée.
        /// </summary>
        /// <typeparam name="T">Type d'un élément de la collection</typeparam>
        /// <param name="collection">Collection dans laquelle l'élément de type T doit être ajouté</param>
        /// <param name="item">Element de type T qui doit être ajouté à la collection</param>
        /// <param name="orderDescending">Permet de définir l'ordre de tris pour ajouter l'élément (facultatif et ascendant par défaut)</param>
        /// <param name="comparer">Permet de définir un comparateur pour l'ajout (facultatif, comparateur du type T par défaut s'il implémente IComparable<T></param>
        public static void AddSorted<T>(this IList<T> collection, T item, bool orderDescending = false,
            IComparer<T> comparer = null)
        {
            if (comparer == null && !(item is IComparable<T>))
            {
                collection.Add(item);
            }
            else
            {
                if (comparer == null)
                {
                    comparer = Comparer<T>.Default;
                }

                int index = 0;

                if (!orderDescending)
                {
                    while (index < collection.Count && comparer.Compare(collection[index], item) < 0)
                    {
                        index++;
                    }
                }
                else
                {
                    while (index < collection.Count && comparer.Compare(collection[index], item) > 0)
                    {
                        index++;
                    }
                }

                collection.Insert(index, item);
            }
        }
    }
}

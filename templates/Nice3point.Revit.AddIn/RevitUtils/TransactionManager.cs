using System;
using Autodesk.Revit.DB;

namespace Nice3point.Revit.AddIn.RevitUtils
{
    public static class TransactionManager
    {
        /// <summary>
        /// Used to create a single transaction. Do not use this method if your code contains lambda expressions to avoid large memory usage.
        /// </summary>
        /// <param name="document">Revit document</param>
        /// <param name="transactionName">Transaction name</param>
        /// <param name="action">Action</param>
        public static void CreateTransaction(Document document, string transactionName, Action action)
        {
            using var transaction = new Transaction(document);
            transaction.Start(transactionName);
            try
            {
                action?.Invoke();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.RollBack();
            }
        }

        /// <summary>
        /// Used to create a group of transactions. Do not use this method if your code contains lambda expressions to avoid large memory usage.
        /// </summary>
        /// <param name="document">Revit document</param>
        /// <param name="transactionName">Transaction name</param>
        /// <param name="action">Action</param>
        public static void CreateGroupTransaction(Document document, string transactionName, Action action)
        {
            using var transaction = new TransactionGroup(document);
            transaction.Start(transactionName);
            try
            {
                action?.Invoke();
                transaction.Assimilate();
            }
            catch (Exception)
            {
                transaction.RollBack();
            }
        }
    }
}
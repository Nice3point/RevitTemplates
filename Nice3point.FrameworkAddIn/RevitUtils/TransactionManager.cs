using System;
using Autodesk.Revit.DB;

namespace Nice3point.FrameworkAddIn.RevitUtils
{
    public static class TransactionManager
    {
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
using System;
using System.Data.Entity;
using PolyPrint.Model;

namespace PolyPrint.AppData
{
    public static class DbHelper
    {
        public static bool SaveEntity<TEntity>(TEntity entity, Action<PolyPrintEntities, TEntity> persistAction, out string errorMessage)
            where TEntity : class
        {
            errorMessage = null;

            if (entity == null)
            {
                errorMessage = "Пустая сущность не может быть сохранена.";
                return false;
            }

            try
            {
                persistAction?.Invoke(App.db, entity);
                App.db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public static bool UpdateEntity<TEntity>(TEntity entity, out string errorMessage)
            where TEntity : class
        {
            errorMessage = null;

            if (entity == null)
            {
                errorMessage = "Пустая сущность не может быть обновлена.";
                return false;
            }

            try
            {
                App.db.Entry(entity).State = EntityState.Modified;
                App.db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public static bool DeleteEntity<TEntity>(TEntity entity, Action<PolyPrintEntities, TEntity> deleteAction, out string errorMessage)
            where TEntity : class
        {
            errorMessage = null;

            if (entity == null)
            {
                errorMessage = "Выбранная запись не найдена.";
                return false;
            }

            try
            {
                deleteAction?.Invoke(App.db, entity);
                App.db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }
    }
}

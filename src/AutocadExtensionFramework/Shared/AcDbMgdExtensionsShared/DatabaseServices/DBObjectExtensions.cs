﻿namespace Autodesk.AutoCAD.DatabaseServices
{
    /// <summary>
    /// Extension class for DBObject object
    /// </summary>
    public static class DBObjectExtensions
    {
        /// <summary>
        /// Determines whether [has extension dictionary].
        /// </summary>
        /// <param name="dbObj">The database object.</param>
        /// <returns></returns>
        public static bool HasExtensionDictionary(this DBObject dbObj)
        {
            return !dbObj.ExtensionDictionary.IsNull;
        }

        /// <summary>
        /// Gets the extension dictionary.
        /// </summary>
        /// <param name="dbObj">The database object.</param>
        /// <returns></returns>
        public static DBDictionary GetExtensionDictionary(this DBObject dbObj)
        {
            if (!HasExtensionDictionary(dbObj))
            {
                if (!dbObj.IsWriteEnabled)
                {
                    dbObj.UpgradeOpen();
                }
                dbObj.CreateExtensionDictionary();
            }
            return dbObj.ExtensionDictionary.GetDBObject<DBDictionary>();
        }

        /// <summary>
        /// Tries the get extension dictionary identifier.
        /// </summary>
        /// <param name="dbObj">The database object.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static bool TryGetExtensionDictionaryId(this DBObject dbObj, out ObjectId id)
        {
            id = dbObj.ExtensionDictionary;
            return !id.IsNull;
        }

        /// <summary>
        /// Gets the extension dictionary identifier.
        /// </summary>
        /// <param name="dbObj">The database object.</param>
        /// <returns></returns>
        public static ObjectId GetExtensionDictionaryId(this DBObject dbObj)
        {
            ObjectId id;
            if (!dbObj.TryGetExtensionDictionaryId(out id))
            {
                if (!dbObj.IsWriteEnabled)
                {
                    dbObj.UpgradeOpen();
                }

                dbObj.CreateExtensionDictionary();
                id = dbObj.ExtensionDictionary;
            }
            return id;
        }

        /// <summary>
        /// Extensions the dictionary contains.
        /// </summary>
        /// <param name="dbObj">The database object.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static bool ExtensionDictionaryContains(this DBObject dbObj, string name)
        {
            if (dbObj.HasExtensionDictionary())
            {
                return dbObj.GetExtensionDictionary().Contains(name);
            }
            return false;
        }
    }
}
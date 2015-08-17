﻿using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.AutoCAD.Runtime;
using Exception = Autodesk.AutoCAD.Runtime.Exception;
namespace Autodesk.AutoCAD.DatabaseServices
{
    public static class SymbolTableExtensions
    {
        public static IEnumerable<ObjectId> GetObjectIds(this SymbolTable st)
        {
            foreach (ObjectId id in st)
            {
                yield return id;
            }
        }

        internal static IEnumerable<T> GetSymbolTableRecords<T>(this SymbolTable symbolTbl, Transaction trx,
            OpenMode mode, SymbolTableRecordFilter filter, bool filterDependecyById) where T : SymbolTableRecord
        {
            if (trx == null)
            {
                throw new Exception(ErrorStatus.NoActiveTransactions);
            }

            bool includingErased = filter.IsSet(SymbolTableRecordFilter.IncludedErased);

            if (filter.IsSet(SymbolTableRecordFilter.IncludeDependent))
            {
                foreach (ObjectId id in includingErased ? symbolTbl.IncludingErased : symbolTbl)
                {
                    yield return (T) trx.GetObject(id, mode, includingErased, false);
                }
            }

            else
            {
                if (filterDependecyById)
                {
                    IntPtr dbIntPtr = symbolTbl.Database.UnmanagedObject;
                    foreach (ObjectId id in includingErased ? symbolTbl.IncludingErased : symbolTbl)
                    {
                        if (id.OriginalDatabase.UnmanagedObject == dbIntPtr)
                        {
                            yield return (T) trx.GetObject(id, mode, includingErased, false);
                        }
                    }
                }
                else
                {
                    foreach (ObjectId id in includingErased ? symbolTbl.IncludingErased : symbolTbl)
                    {
                        T current = (T) trx.GetObject(id, mode, includingErased, false);
                        if (!current.IsDependent)
                        {
                            yield return current;
                        }
                    }
                }
            }
        }


        public static ObjectId ImportSymbolTableRecord<T>(this Database targetDb, string sourceFile,
            DuplicateRecordCloning cloningStyle, string recordName)
            where T : SymbolTable
        {
            using (var sourceDb = new Database(false, true))
            {
                sourceDb.ReadDwgFile(sourceFile, FileShare.ReadWrite, false, "");
                ObjectId sourceTableId, targetTableId;
                switch (typeof (T).Name)
                {
                    case "BlockTable":
                        sourceTableId = sourceDb.BlockTableId;
                        targetTableId = targetDb.BlockTableId;
                        break;
                    case "DimStyleTable":
                        sourceTableId = sourceDb.DimStyleTableId;
                        targetTableId = targetDb.DimStyleTableId;
                        break;
                    case "LayerTable":
                        sourceTableId = sourceDb.LayerTableId;
                        targetTableId = targetDb.LayerTableId;
                        break;
                    case "LinetypeTable":
                        sourceTableId = sourceDb.LinetypeTableId;
                        targetTableId = targetDb.LinetypeTableId;
                        break;
                    case "RegAppTable":
                        sourceTableId = sourceDb.RegAppTableId;
                        targetTableId = targetDb.RegAppTableId;
                        break;
                    case "TextStyleTable":
                        sourceTableId = sourceDb.TextStyleTableId;
                        targetTableId = targetDb.TextStyleTableId;
                        break;
                    case "UcsTable":
                        sourceTableId = sourceDb.UcsTableId;
                        targetTableId = targetDb.UcsTableId;
                        break;
                    case "ViewTable":
                        sourceTableId = sourceDb.ViewportTableId;
                        targetTableId = targetDb.ViewportTableId;
                        break;
                    case "ViewportTable":
                        sourceTableId = sourceDb.ViewportTableId;
                        targetTableId = targetDb.ViewportTableId;
                        break;
                    default:
                        throw new ArgumentException("Requires a concrete type derived from SymbolTable");
                }

                using (Transaction tr = sourceDb.TransactionManager.StartTransaction())
                {
                    T sourceTable = (T) tr.GetObject(sourceTableId, OpenMode.ForRead);
                    if (!sourceTable.Has(recordName))
                    {
                        return ObjectId.Null;
                    }
                    var idCol = new ObjectIdCollection();
                    ObjectId sourceTableRecordId = sourceTable[recordName];
                    idCol.Add(sourceTableRecordId);
                    var idMap = new IdMapping();
                    sourceDb.WblockCloneObjects(idCol, targetTableId, idMap, cloningStyle, false);
                    tr.Commit();
                    return idMap[sourceTableRecordId].Value;
                }
            }
        }


        public static bool ImportSymbolTableRecords<T>(this Database targetDb, string sourceFile,
            DuplicateRecordCloning cloningStyle, params string[] recordNames)
            where T : SymbolTable
        {
            using (var sourceDb = new Database(false, true))
            {
                sourceDb.ReadDwgFile(sourceFile, FileShare.Read, false, "");
                ObjectId sourceTableId, targetTableId;
                switch (typeof (T).Name)
                {
                    case "BlockTable":
                        sourceTableId = sourceDb.BlockTableId;
                        targetTableId = targetDb.BlockTableId;
                        break;
                    case "DimStyleTable":
                        sourceTableId = sourceDb.DimStyleTableId;
                        targetTableId = targetDb.DimStyleTableId;
                        break;
                    case "LayerTable":
                        sourceTableId = sourceDb.LayerTableId;
                        targetTableId = targetDb.LayerTableId;
                        break;
                    case "LinetypeTable":
                        sourceTableId = sourceDb.LinetypeTableId;
                        targetTableId = targetDb.LinetypeTableId;
                        break;
                    case "RegAppTable":
                        sourceTableId = sourceDb.RegAppTableId;
                        targetTableId = targetDb.RegAppTableId;
                        break;
                    case "TextStyleTable":
                        sourceTableId = sourceDb.TextStyleTableId;
                        targetTableId = targetDb.TextStyleTableId;
                        break;
                    case "UcsTable":
                        sourceTableId = sourceDb.UcsTableId;
                        targetTableId = targetDb.UcsTableId;
                        break;
                    case "ViewTable":
                        sourceTableId = sourceDb.ViewportTableId;
                        targetTableId = targetDb.ViewportTableId;
                        break;
                    case "ViewportTable":
                        sourceTableId = sourceDb.ViewportTableId;
                        targetTableId = targetDb.ViewportTableId;
                        break;
                    default:
                        throw new ArgumentException("Requires a concrete type derived from SymbolTable");
                }

                using (var tr = sourceDb.TransactionManager.StartTransaction())
                {
                    var allImported = true;
                    var sourceTable = (T) tr.GetObject(sourceTableId, OpenMode.ForRead);
                    var idCol = new ObjectIdCollection();
                    foreach (var recordName in recordNames)
                    {
                        if (sourceTable.Has(recordName))
                        {
                            idCol.Add(sourceTable[recordName]);
                        }
                        else
                        {
                            allImported = false;
                        }
                    }

                    var idMap = new IdMapping();
                    sourceDb.WblockCloneObjects(idCol, targetTableId, idMap, cloningStyle, false);
                    tr.Commit();
                    return allImported;
                }
            }
        }
    }
}



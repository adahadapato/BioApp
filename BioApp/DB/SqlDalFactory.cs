using BioApp.DB.Dal;
using System;


namespace BioApp.DB
{
    public enum GrConnector
    {
        AccessSQLDal
        //AccessMySQLDal,
        //AccessUploadDal
    }

    public abstract class SQLDalFactory
    {
        public static IGRDal GetDal(GrConnector connector)
        {
            switch (connector)
            {
                case GrConnector.AccessSQLDal:
                    return (IGRDal)Activator.CreateInstance(typeof(SqlDataAccessLayer), true);

                default:
                    return null;
            }
        }


    }
}

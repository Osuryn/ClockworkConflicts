using MMTD_Client.Domain;

namespace MMTD_Client.Persistence
{
    //========================================================================
    //  			  CLASS DESCRIPTION : PersistenceController
    //========================================================================
    //- This class controls all functions of the Persistence layer
    //========================================================================
    class PersistenceController
    {
        #region Init
        private static PersistenceController persistenceController;
        private XMLHandler xmlHandler;

        private PersistenceController()
        {
            xmlHandler = new XMLHandler();
        }

        public static PersistenceController getInstance()
        {
            if (persistenceController == null)
            {
                persistenceController = new PersistenceController();
            }
            return persistenceController;
        }
        #endregion

        #region Extra Functions
        public Server getLoginServer()
        {
            return xmlHandler.getDataFromXML();
        }
        #endregion
    }
}

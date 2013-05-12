using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMTD_Client.Domain
{
    //========================================================================
    //  			  CLASS DESCRIPTION : Party
    //========================================================================
    //- This class holds all information about a party.
    //========================================================================
    class Party
    {
        public List<Account> userList { get; set; }
        public int partyId { get; set; }
        public int partyLeader;
        private DomainController domainController;

        public Party(Account user, int partyId)
        {
            userList = new List<Account>();
            this.userList.Add(user);
            partyLeader = user.accountId;
            this.partyId = partyId;
            domainController = DomainController.getInstance();
        }

        public void adduser(int userId)
        {
            userList.Add(DomainController.getInstance().getAccountByID(userId));
        }

        public void assignPartyLeader(int accountId)
        {
            this.partyLeader = accountId;
        }

        public bool removeUserFromParty(int userId)
        {
            foreach (Account account in userList)
            {
                if (account.accountId == userId)
                {
                    userList.Remove(account);
                    return true;
                }
            }
            return false;
        }
    }
}

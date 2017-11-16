﻿using Logging;
using System;
using Windows.Security.Credentials;
using XMPP_API.Classes.Network;

namespace Data_Manager.Classes
{
    public class Vault
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static readonly string VAULT_NAME_PREFIX = "XMPP LOGIN DATA VAULT_";

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 25/08/2017 Created [Fabian Sauter]
        /// </history>
        private Vault()
        {

        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public static void loadPassword(XMPPAccount account)
        {
            PasswordVault vault = new PasswordVault();
            string vaultName = VAULT_NAME_PREFIX + account.getIdAndDomain();
            PasswordCredential passwordCredential;
            try
            {
                 passwordCredential = vault.Retrieve(vaultName, account.user.userId);
            }
            catch (Exception e)
            {
                Logger.Error("Error during loadPassword - Vault", e);
                account.user.userPassword = "";
                return;
            }
            if(passwordCredential == null)
            {
                Logger.Warn("No password found for: " + account.user.getIdAndDomain());
                account.user.userPassword = "";
                return;
            }
            passwordCredential.RetrievePassword();
            account.user.userPassword = passwordCredential.Password;
        }

        public static void storePassword(XMPPAccount account)
        {
            PasswordVault vault = new PasswordVault();
            string vaultName = VAULT_NAME_PREFIX + account.getIdAndDomain();
            vault.Add(new PasswordCredential(vaultName, account.user.userId, account.user.userPassword));
        }

        public static void deletePassword(XMPPAccount account)
        {
            PasswordVault vault = new PasswordVault();
            string vaultName = VAULT_NAME_PREFIX + account.getIdAndDomain();
            PasswordCredential passwordCredential = vault.Retrieve(vaultName, account.user.userId);
            if(passwordCredential != null)
            {
                vault.Remove(passwordCredential);
            }
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}

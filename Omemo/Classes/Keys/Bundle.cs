﻿using System;
using System.Collections.Generic;

namespace Omemo.Classes.Keys
{
    public class Bundle
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The public part of the signed PreKey.
        /// </summary>
        public ECPubKey signedPreKey;
        /// <summary>
        /// The id of the signed PreKey.
        /// </summary>
        public uint signedPreKeyId;
        /// <summary>
        /// The signature of the signed PreKey.
        /// </summary>
        public byte[] preKeySignature;
        /// <summary>
        /// The public part of the identity key.
        /// </summary>
        public ECPubKey identityKey;
        /// <summary>
        /// A collection of public parts of the <see cref="PreKey"/>s and their ID.
        /// </summary>
        public List<Tuple<ECPubKey, uint>> preKeys = new List<Tuple<ECPubKey, uint>>();

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public Tuple<ECPubKey, uint> getRandomPreKey()
        {
            Random r = new Random();
            return preKeys[r.Next(0, preKeys.Count)];
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

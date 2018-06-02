﻿using System.Xml;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0363
{
    public class HTTPUploadResponseSlotMessage : IQMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public readonly HTTPUploadSlot SLOT;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 02/06/2018 Created [Fabian Sauter]
        /// </history>
        public HTTPUploadResponseSlotMessage(XmlNode node) : base(node)
        {
            XmlNode slotNode = XMLUtils.getChildNode(node, "slot", Consts.XML_XMLNS, Consts.XML_XEP_0363_NAMESPACE);
            if (slotNode != null)
            {
                this.SLOT = new HTTPUploadSlot(slotNode);
            }
            else
            {
                this.SLOT = null;
            }
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


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

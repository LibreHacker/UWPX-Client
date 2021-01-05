﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using XMPP_API.Classes.Network.XML.Messages.XEP_0082;
using XMPP_API.Classes.Network.XML.Messages.XEP_0420;

namespace XMPP_API.Classes.Network.XML.Messages.XEP_0384
{
    public class OmemoEncryptedMessage: EncryptedMessage
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The current state of the message. True in case the massage has been encrypted.
        /// </summary>
        public bool ENCRYPTED { get; private set; } = false;
        /// <summary>
        /// The device ID of the sender.
        /// </summary>
        public uint SID { get; private set; }
        public readonly List<OmemoKeys> KEYS = new List<OmemoKeys>();
        public string BASE_64_PAYLOAD { get; private set; }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public OmemoEncryptedMessage(string from, string to, string message, string type, bool reciptRequested) : base(from, to, message, type, reciptRequested)
        {
            includeBody = false;
        }

        public OmemoEncryptedMessage(XmlNode node, CarbonCopyType ccType) : base(node, ccType)
        {
            XmlNode encryptedNode = XMLUtils.getChildNode(node, "encrypted", Consts.XML_XMLNS, Consts.XML_XEP_0384_NAMESPACE);
            Debug.Assert(!(encryptedNode is null));
            XmlNode headerNode = XMLUtils.getChildNode(encryptedNode, "header");
            Debug.Assert(!(headerNode is null));
            SID = uint.Parse(headerNode.Attributes["sid"]?.Value);
            foreach (XmlNode keysNode in headerNode.ChildNodes)
            {
                if (string.Equals(keysNode.Name, "keys"))
                {
                    KEYS.Add(new OmemoKeys(keysNode));
                }
            }

            // Can be null for empty messages:
            XmlNode payloadNode = XMLUtils.getChildNode(encryptedNode, "payload");
            if (payloadNode is null)
            {
                MESSAGE = null;
            }
            else
            {
                BASE_64_PAYLOAD = payloadNode.Value;
            }
            ENCRYPTED = true;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public override XElement toXElement()
        {
            if (!ENCRYPTED)
            {
                throw new InvalidOperationException("Message not encrypted! Call encrypt(...) first.");
            }

            XElement msgNode = base.toXElement();

            XNamespace ns = Consts.XML_XEP_0384_NAMESPACE;
            XElement encryptedNode = new XElement(ns + "encrypted");

            // Header:
            XElement headerNode = new XElement("header");
            headerNode.Add(new XAttribute("sid", SID));
            foreach (OmemoKeys keys in KEYS)
            {
                headerNode.Add(keys.toXElement(ns));
            }
            encryptedNode.Add(headerNode);

            // Payload:
            if (!string.IsNullOrEmpty(BASE_64_PAYLOAD))
            {

                encryptedNode.Add(new XElement("payload", BASE_64_PAYLOAD));
            }

            return msgNode;
        }

        /// <summary>
        /// Encrypts the message and prepares everything for sending it.
        /// </summary>
        /// <param name="sid">The device ID of the sender.</param>
        public void encrypt(uint sid)
        {
            SID = sid;
            XElement contentNode = generateContent();
            byte[] contentData = Encoding.UTF8.GetBytes(contentNode.ToString());
            byte[] contentEnc = encryptContent(contentData);
            BASE_64_PAYLOAD = Convert.ToBase64String(contentEnc);
            ENCRYPTED = true;
        }

        public bool decrypt()
        {
            // Content can be empty:
            if (!string.IsNullOrEmpty(BASE_64_PAYLOAD))
            {
                byte[] contentEnc = Convert.FromBase64String(BASE_64_PAYLOAD);
                byte[] contentDec = decryptContent(contentEnc);
                string contentStr = Encoding.UTF8.GetString(contentDec);
                XDocument contentNode = XDocument.Parse(contentStr);
                parseContentNode(contentNode);
            }
            ENCRYPTED = false;
            return true;
        }

        private void parseContentNode(XDocument contentNode)
        {
            throw new NotImplementedException("TODO");
        }

        #endregion

        #region --Misc Methods (Private)--
        private XElement generateContent()
        {
            XNamespace ns = Consts.XML_XEP_0420_NAMESPACE;
            XElement contentNode = new XElement(ns + "content");

            // Payload:
            XElement payloadNode = new XElement("payload");
            XNamespace bodyNs = Consts.XML_CLIENT;
            payloadNode.Add(new XElement(bodyNs + "body", MESSAGE));
            contentNode.Add(payloadNode);

            // Padding
            contentNode.Add(generatePaddingNode(1, 200, ns));

            // From:
            XElement fromNode = new XElement("from");
            fromNode.Add(new XAttribute("jid", FROM));
            contentNode.Add(fromNode);

            // To:
            XElement toNode = new XElement("to");
            toNode.Add(new XAttribute("jid", FROM));
            contentNode.Add(toNode);

            // Time:
            timeStamp = DateTime.Now;
            XElement timeNode = new XElement("time");
            timeNode.Add(new XAttribute("stamp", DateTimeHelper.ToString(timeStamp)));
            contentNode.Add(timeNode);

            return contentNode;
        }

        private byte[] encryptContent(byte[] data)
        {
            throw new NotImplementedException();
        }

        private byte[] decryptContent(byte[] data)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
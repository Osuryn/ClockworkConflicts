using UnityEngine;

using System;
using System.Xml;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Text;

using MMTD_Client.Domain;
using MMTD_Client.Gui;


namespace MMTD_Client.Persistence
{
    //========================================================================
    //  			  CLASS DESCRIPTION : XMLHandler
    //========================================================================
    //- This class gets the servers from an online xml file
    //========================================================================
    class XMLHandler
    {
        #region Init
        string path;

        public XMLHandler()
        {
        }

        #endregion

        #region Data Reciever

        public Server getDataFromXML()
        {
            Server loginserver = null;
            try
            {
                //path = "http://loginapi.brokendiamond.org/servers.xml";
                path = "http://loginapi.clockworkconflicts.com/servers.xml";
                XmlTextReader reader = new XmlTextReader(path);

                string extIP = null;
                string intIP = null;
                int serverPort = 0;
                string serverStatus = null;

                while (reader.Read())
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            switch (reader.Name)
                            {
                                case "status":
                                    serverStatus = reader.ReadInnerXml();
                                    break;
                                case "ExternalIp":
                                    extIP = reader.ReadInnerXml();
                                    break;
                                case "InternalIp":
                                    intIP = reader.ReadInnerXml();
                                    break;
                                case "port":
                                    serverPort = Convert.ToInt32(reader.ReadInnerXml());
                                    break;
                            }
                        }
                    }
                }

                if (serverPort != 0)
                {
                    string ownExtIp = GetExternalIp();
                    string ownIntIp = getInternalIp();
                    int index = -1;
                    index = extIP.LastIndexOf('.');
                    string extIP2 = extIP.Substring(0, index);
                    index = -1;
                    index = ownExtIp.LastIndexOf('.');
                    string ownExtIp2 = ownExtIp.Substring(0, index);

                    if (extIP != ownExtIp)
                    {
                        Debug.Log("Connecting to: " + extIP);
                        loginserver = new Server(serverPort, "Loginserver", serverStatus, extIP);
                    }
                    else if (intIP != ownIntIp)
                    {
                        Debug.Log("Connecting to: " + intIP);
                        loginserver = new Server(serverPort, "Loginserver", serverStatus, intIP);
                    }
                    else
                    {
                        loginserver = new Server(serverPort, "Loginserver", serverStatus, "127.0.0.1");
                        Debug.Log("Connecting to: 127.0.0.1");
                    }
                    Debug.Log("Internal: " + ownIntIp + "/" + intIP + " External:" + ownExtIp + "/" + extIP);
                }

            }
            //Show the exeption in debugmode
            catch (Exception e)
            {
                GuiController.getInstance().SetDebugText("XMLHandler error: " + e.ToString());
                //loginserver = new Server(26800, "Loginserver", "Online", "127.0.0.1");
            }
            return loginserver;
        }

        public string GetExternalIp()
        {
            string externalIp = "";
            try
            {
                string url = "http://checkip.dyndns.org";
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                string response = sr.ReadToEnd().Trim();
                string[] a = response.Split(':');
                string a2 = a[1].Substring(1);
                string[] a3 = a2.Split('<');
                externalIp = a3[0];

                if (externalIp == "")
                {
                    string whatIsMyIp = "http://whatismyip.com";
                    string getIpRegex = @"(?<=<TITLE>.*)\d*\.\d*\.\d*\.\d*(?=</TITLE>)";
                    WebClient wc = new WebClient();
                    UTF8Encoding utf8 = new UTF8Encoding();
                    string requestHtml = "";
                    try
                    {
                        requestHtml = utf8.GetString(wc.DownloadData(whatIsMyIp));
                    }
                    catch (WebException we)
                    {
                        // do something with exception
                        Console.Write(we.ToString());
                    }
                    Regex r = new Regex(getIpRegex);
                    Match m = r.Match(requestHtml);
                    externalIp = "";
                    if (m.Success)
                    {
                        externalIp = m.Value;
                    }
                }
            }
            catch (Exception)
            {
                try
                {

                    string whatIsMyIp = "http://whatismyip.com";
                    string getIpRegex = @"(?<=<TITLE>.*)\d*\.\d*\.\d*\.\d*(?=</TITLE>)";
                    WebClient wc = new WebClient();
                    UTF8Encoding utf8 = new UTF8Encoding();
                    string requestHtml = "";
                    try
                    {
                        requestHtml = utf8.GetString(wc.DownloadData(whatIsMyIp));
                    }
                    catch (WebException we)
                    {
                        // do something with exception
                        Console.Write(we.ToString());
                    }
                    Regex r = new Regex(getIpRegex);
                    Match m = r.Match(requestHtml);
                    externalIp = "";
                    if (m.Success)
                    {
                        externalIp = m.Value;
                    }
                }
                catch (Exception)
                {

                }
            }
            return externalIp;
        }

        public string getInternalIp()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
        #endregion
    }
}

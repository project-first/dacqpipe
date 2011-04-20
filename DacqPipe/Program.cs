/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    Dacq.cs
 *  Desc:    Data acquisition pipeline (Dacq)
 *  Created: Feb-2011
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using System.IO;
using System.Threading;
using System.Web;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using Latino;
using Latino.Workflows;
using Latino.Workflows.TextMining;
using Latino.Workflows.Persistance;
using Latino.Web;
using Latino.Persistance;
using Latino.TextMining;

namespace Dacq
{
    class Program
    {
        static string TIME_FORMAT 
            = "yyyy-MM-dd HH:mm:ss";

        static void Main(string[] args)
        {
            DateTime startTime = DateTime.MinValue;
            Logger rootLogger = Logger.GetRootLogger();
            rootLogger.LocalLevel = Logger.Level.Debug;
            rootLogger.LocalOutputType = Logger.OutputType.Console | Logger.OutputType.Writer;
            rootLogger.LocalWriter = new StreamWriter("c:\\work\\dacqpipe\\log.txt");
            const string SOURCES_FILE_NAME = "c:\\work\\dacqpipe\\rsssources\\RssSourcesBig.txt";
            const string DB_CONNECTION_STRING = "Provider=SQLNCLI10;Server=(local);Database=DacqPipeTmp;Trusted_Connection=Yes";
            //rootLogger.LocalWriter = new StreamWriter(@"E:\Users\miha\Work\DacqPipeBig_4\log.txt");
            //const string SOURCES_FILE_NAME = @"E:\Users\miha\Work\DacqPipeBig_4\RssSourcesBig.txt";
            //const string DB_CONNECTION_STRING = "Provider=SQLNCLI10;Server=(local);Database=DacqPipeBig_4;Trusted_Connection=Yes";
            const int SLEEP_BETWEEN_INITS = 0;
            const int NUM_WRITERS = 10;
            const int SLEEP_BETWEEN_POLLS = 15 * 60 * 1000; // 15 minutes
            const string DACQ_VER = "1.0";
            ArrayList<RssFeedComponent> rssComponents = new ArrayList<RssFeedComponent>();
            ArrayList<StreamDataConsumer> dataConsumers = new ArrayList<StreamDataConsumer>();
            ArrayList<StreamDataConsumer> rssSubscribers = new ArrayList<StreamDataConsumer>();
            Dictionary<object, Guid> components = new Dictionary<object, Guid>();
            // init logging 
            Logger logger = Logger.GetLogger("Latino.Workflows.Dacq");        
            // start server
            bool exit = false;
            bool httpServerRunning = false;
            new Thread(new ThreadStart(delegate()
            {
                HttpListener listener = new HttpListener();
                listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                listener.Prefixes.Add("http://localhost/dacq/");
                listener.Prefixes.Add("http://first.ijs.si/dacq/");
                listener.Start();
                logger.Info("Main.HttpServer", "HTTP server started.");
                httpServerRunning = true;
                while (!exit)
                {
                    try
                    {
                        HttpListenerContext ctx = null;
                        listener.BeginGetContext(new AsyncCallback(delegate(IAsyncResult ar)
                        {
                            try { ctx = listener.EndGetContext(ar); } catch { }
                        }), /*state=*/null);
                        while (!exit && ctx == null) { Thread.Sleep(500); }
                        if (!exit)
                        {
                            // process requests one by one
                            ctx.Response.AppendHeader("Content-Type", "application/xml");
                            XmlWriterSettings settings = new XmlWriterSettings();
                            settings.Encoding = Encoding.UTF8;
                            settings.CheckCharacters = false;
                            settings.Indent = true;
                            XmlWriter w = XmlTextWriter.Create(ctx.Response.OutputStream, settings);
                            w.WriteStartElement("DacqResponse");
                            w.WriteElementString("DacqVersion", DACQ_VER);
                            w.WriteElementString("DacqStartTime", startTime.ToString(TIME_FORMAT));
                            w.WriteElementString("Request", ctx.Request.Url.ToString());
                            string command = GetHttpRequestCommand(ctx.Request.Url.ToString());
                            w.WriteElementString("Command", command);
                            w.WriteStartElement("ResponseBody");                            
                            if (command == "help")
                            {
                                WriteSupportedCommands(w);
                            }
                            else if (command == "components")
                            {                                
                                WriteComponentInfo(w, components);
                            }
                            else if (command == "sources")
                            {
                                WriteRssInfo(w, components);
                            }
                            else if (command == "throughput")
                            {
                                WriteThroughputInfo(w, components);
                            }
                            w.WriteEndElement(); // ResponseBody
                            w.WriteEndElement(); // DacqResponse
                            w.Close();
                            ctx.Response.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Warn("Main.HttpServer", e);
                    }
                }
                listener.Stop();
                logger.Info("Main.HttpServer", "HTTP server stopped.");
                httpServerRunning = false;
            })).Start();           
            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
            {
                logger.Info("Main", "*** Ctrl-C command received. ***");
                e.Cancel = true;
                exit = true;
            };            
            logger.Info("Main", "Starting Dacq ...");
            string[] sources = File.ReadAllLines(SOURCES_FILE_NAME);
            // initialize database writers
            for (int i = 0; i < NUM_WRITERS; i++)
            {
                DocumentCorpusWriterComponent c = new DocumentCorpusWriterComponent(DB_CONNECTION_STRING);
                BoilerplateRemoverComponent brc = new BoilerplateRemoverComponent();
                brc.Subscribe(c);
                dataConsumers.Add(c);
                dataConsumers.Add(brc);
                rssSubscribers.Add(brc);
            }                   
            // initialize RSS feed components
            int j = 0;
            RssFeedComponent rssComp = null;
            Set<string> sites = new Set<string>();
            foreach (string _url in sources)
            {
                string url = _url.Trim();
                if (url != "" && !url.StartsWith("#"))
                {
                    Match m;
                    if ((m = Regex.Match(url, @"^site\s*:(?<siteId>.*)$", RegexOptions.IgnoreCase)).Success)
                    {
                        DatabaseConnection historyDatabase = new DatabaseConnection();
                        historyDatabase.ConnectionString = DB_CONNECTION_STRING;
                        historyDatabase.Connect();
                        string siteId = m.Result("${siteId}").Trim();
                        if (sites.Contains(siteId)) { logger.Warn("Main", "Duplicated site identifier ({0}).", siteId); }
                        sites.Add(siteId);
                        rssComp = new RssFeedComponent(siteId);
                        rssComp.Name = siteId;
                        rssComp.TimeBetweenPolls = SLEEP_BETWEEN_POLLS;
                        rssComp.IncludeRssXml = true;
                        rssComp.HistoryDatabase = historyDatabase;
                        rssComp.LoadHistory();
                        rssComp.IncludeRawData = true;                      
                        rssComp.Subscribe(rssSubscribers[j % NUM_WRITERS]);
                        rssComponents.Add(rssComp);
                        j++;
                    }
                    else if (rssComp != null) 
                    {
                        rssComp.AddSource(url);
                    }
                }
            }
            foreach (RssFeedComponent c in rssComponents)
            {
                c.Start();
            }
            foreach (object obj in rssComponents) { components.Add(obj, Guid.NewGuid()); }
            foreach (object obj in dataConsumers) { components.Add(obj, Guid.NewGuid()); }
            startTime = DateTime.Now;
            while (!exit) { Thread.Sleep(500); }
            // shut down gracefully
            logger.Info("Main", "Please wait while shutting down ...");
            // wait for HTTP server shutdown
            while (httpServerRunning) { Thread.Sleep(500); }
            // stop RSS components
            foreach (RssFeedComponent c in rssComponents)
            {                
                c.Stop();
            }
            foreach (RssFeedComponent c in rssComponents)
            {
                while (c.IsRunning) { Thread.Sleep(500); }
                c.HistoryDatabase.Disconnect();
            }
            // wait for all data consumers to finish 
            foreach (StreamDataConsumer dataConsumer in dataConsumers)
            {
                if (dataConsumer.IsRunning) { while (!dataConsumer.IsSuspended) { Thread.Sleep(500); } }
                dataConsumer.Dispose(); 
            }
            logger.Info("Main", "Dacq successfully stopped.");
        }

        static string[] mCommands 
            = new string[] { 
                "help", 
                "components",
                "sources",
                "throughput"
            };

        static string GetHttpRequestCommand(string url)
        {
            foreach (string command in mCommands)
            {
                if (url.EndsWith(command)) { return command; }
            }
            return "help";
        }

        static void WriteSupportedCommands(XmlWriter w)
        {
            string[] desc = new string[] { 
                "Information about the supported commands.", 
                "Information about the pipeline components.",
                "Information about the RSS sources.",
                "Information about the pipeline throughput (not implemented)."
            };
            int i = 0;
            foreach (string command in mCommands)
            {
                w.WriteStartElement("SupportedCommand");
                w.WriteElementString("Identifier", command);
                w.WriteElementString("Description", desc[i++]);
                w.WriteEndElement(); // SupportedCommand
            }
        }

        static void WriteComponentInfo(XmlWriter w, Dictionary<object, Guid> components)
        {
            int c = 0;
            int maxMaxQueueSize = 0;
            string maxMaxQueueSizeRef = "";
            int maxCurrentQueueSize = 0;
            string maxCurrentQueueSizeRef = "";
            foreach (object component in components.Keys)
            {
                w.WriteStartElement("Component");
                w.WriteAttributeString("id", components[component].ToString("N"));
                string type = (component is IDataConsumer && component is IDataProducer) ? "Processor" : (component is IDataConsumer ? "Consumer" : "Producer");
                w.WriteElementString("Type", type);
                w.WriteElementString("Class", component.GetType().ToString());
                string name = null;
                if (component is StreamDataConsumer)
                {
                    StreamDataConsumer consumer = (StreamDataConsumer)component;
                    int currentQueueSize = consumer.QueueSize;
                    int maxQueueSize = consumer.MaxQueueSize;
                    string maxQueueSizeTime = consumer.MaxQueueSizeTime.ToString(TIME_FORMAT);
                    w.WriteElementString("CurrentQueueSize", currentQueueSize.ToString());                    
                    w.WriteElementString("MaxQueueSize", maxQueueSize.ToString());
                    if (maxQueueSize == maxMaxQueueSize)
                    {
                        maxMaxQueueSizeRef += components[component].ToString("N") + ",";
                    }
                    else if (maxQueueSize > maxMaxQueueSize)
                    {
                        maxMaxQueueSizeRef = components[component].ToString("N") + ",";
                        maxMaxQueueSize = maxQueueSize;
                    }
                    if (currentQueueSize == maxCurrentQueueSize)
                    {
                        maxCurrentQueueSizeRef += components[component].ToString("N") + ",";
                    }
                    else if (currentQueueSize > maxCurrentQueueSize)
                    {
                        maxCurrentQueueSizeRef = components[component].ToString("N") + ",";
                        maxCurrentQueueSize = currentQueueSize;
                    }
                    if (maxQueueSize > 0)
                    {
                        w.WriteElementString("MaxQueueSizeTime", maxQueueSizeTime);
                    }
                    name = consumer.Name;
                }
                if (component is StreamDataProducer)
                {
                    StreamDataProducer producer = (StreamDataProducer)component;
                    foreach (IDataConsumer consumer in producer.SubscribedConsumers)
                    {
                        w.WriteStartElement("SubscribedConsumer");
                        w.WriteAttributeString("ref", components[consumer].ToString("N"));
                        w.WriteEndElement(); // SubscribedConsumer
                    }
                    name = producer.Name;
                }
                if (name != null) { w.WriteElementString("Name", name); }
                else { w.WriteStartElement("Name"); w.WriteAttributeString("null", "true"); w.WriteEndElement(); }
                w.WriteEndElement(); // Component
                c++;
            }
            w.WriteElementString("ComponentCount", c.ToString());
            w.WriteStartElement("MaxMaxQueueSize");
            w.WriteAttributeString("ref", maxMaxQueueSizeRef.TrimEnd(','));
            w.WriteString(maxMaxQueueSize.ToString());
            w.WriteEndElement(); // MaxMaxQueueSize
            w.WriteStartElement("MaxCurrentQueueSize");
            w.WriteAttributeString("ref", maxCurrentQueueSizeRef.TrimEnd(','));
            w.WriteString(maxCurrentQueueSize.ToString());
            w.WriteEndElement(); // MaxCurrentQueueSize
        }

        static void WriteRssInfo(XmlWriter w, Dictionary<object, Guid> components)
        {
            int numberOfSites = 0;
            int totalNumberOfSources = 0;
            foreach (object component in components.Keys)
            {
                if (component is RssFeedComponent)
                {
                    RssFeedComponent rssComponent = (RssFeedComponent)component;
                    w.WriteStartElement("RssFeedComponent");
                    w.WriteAttributeString("id", components[component].ToString("N"));
                    if (rssComponent.SiteId != null) { w.WriteElementString("SiteId", rssComponent.SiteId); }
                    else { w.WriteStartElement("SiteId"); w.WriteAttributeString("null", "true"); w.WriteEndElement(); } 
                    w.WriteElementString("NumberOfSources", rssComponent.Sources.Count.ToString());
                    foreach (string sourceUrl in rssComponent.Sources)
                    {
                        w.WriteElementString("Source", sourceUrl);
                    }
                    totalNumberOfSources += rssComponent.Sources.Count;
                    w.WriteEndElement(); // RssFeedComponent
                    numberOfSites++;
                }
            }
            w.WriteElementString("TotalNumberOfSources", totalNumberOfSources.ToString());
            w.WriteElementString("NumberOfSites", numberOfSites.ToString());
        }

        static void WriteThroughputInfo(XmlWriter w, Dictionary<object, Guid> components)
        {
            // ...
        }
    }
}
﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4963
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

// 
// This source code was auto-generated by wsdl, Version=2.0.50727.42.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Web.Services.WebServiceBindingAttribute(Name="ServiceSoap", Namespace="http://tempuri.org/")]
public partial class DataService : System.Web.Services.Protocols.SoapHttpClientProtocol {
    
    private System.Threading.SendOrPostCallback PingOperationCompleted;
    
    private System.Threading.SendOrPostCallback GetDocRefsOperationCompleted;
    
    private System.Threading.SendOrPostCallback GetDocOperationCompleted;
    
    /// <remarks/>
    public DataService() {
        this.Url = "http://first.ijs.si/FIRSTDatasetService_live/Service.asmx";
    }
    
    /// <remarks/>
    public event PingCompletedEventHandler PingCompleted;
    
    /// <remarks/>
    public event GetDocRefsCompletedEventHandler GetDocRefsCompleted;
    
    /// <remarks/>
    public event GetDocCompletedEventHandler GetDocCompleted;
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/Ping", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    public string Ping() {
        object[] results = this.Invoke("Ping", new object[0]);
        return ((string)(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginPing(System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("Ping", new object[0], callback, asyncState);
    }
    
    /// <remarks/>
    public string EndPing(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((string)(results[0]));
    }
    
    /// <remarks/>
    public void PingAsync() {
        this.PingAsync(null);
    }
    
    /// <remarks/>
    public void PingAsync(object userState) {
        if ((this.PingOperationCompleted == null)) {
            this.PingOperationCompleted = new System.Threading.SendOrPostCallback(this.OnPingOperationCompleted);
        }
        this.InvokeAsync("Ping", new object[0], this.PingOperationCompleted, userState);
    }
    
    private void OnPingOperationCompleted(object arg) {
        if ((this.PingCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.PingCompleted(this, new PingCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetDocRefs", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    [return: System.Xml.Serialization.XmlArrayItemAttribute("ArrayOfString")]
    [return: System.Xml.Serialization.XmlArrayItemAttribute(NestingLevel=1)]
    public string[][] GetDocRefs(string sourceUrl, string timeStart, string timeEnd) {
        object[] results = this.Invoke("GetDocRefs", new object[] {
                    sourceUrl,
                    timeStart,
                    timeEnd});
        return ((string[][])(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginGetDocRefs(string sourceUrl, string timeStart, string timeEnd, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("GetDocRefs", new object[] {
                    sourceUrl,
                    timeStart,
                    timeEnd}, callback, asyncState);
    }
    
    /// <remarks/>
    public string[][] EndGetDocRefs(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((string[][])(results[0]));
    }
    
    /// <remarks/>
    public void GetDocRefsAsync(string sourceUrl, string timeStart, string timeEnd) {
        this.GetDocRefsAsync(sourceUrl, timeStart, timeEnd, null);
    }
    
    /// <remarks/>
    public void GetDocRefsAsync(string sourceUrl, string timeStart, string timeEnd, object userState) {
        if ((this.GetDocRefsOperationCompleted == null)) {
            this.GetDocRefsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetDocRefsOperationCompleted);
        }
        this.InvokeAsync("GetDocRefs", new object[] {
                    sourceUrl,
                    timeStart,
                    timeEnd}, this.GetDocRefsOperationCompleted, userState);
    }
    
    private void OnGetDocRefsOperationCompleted(object arg) {
        if ((this.GetDocRefsCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.GetDocRefsCompleted(this, new GetDocRefsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetDoc", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    public string GetDoc(string corpusId, string docId, string format, bool rmvRaw, bool changesOnly, string corpusTime) {
        object[] results = this.Invoke("GetDoc", new object[] {
                    corpusId,
                    docId,
                    format,
                    rmvRaw,
                    changesOnly,
                    corpusTime});
        return ((string)(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginGetDoc(string corpusId, string docId, string format, bool rmvRaw, bool changesOnly, string corpusTime, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("GetDoc", new object[] {
                    corpusId,
                    docId,
                    format,
                    rmvRaw,
                    changesOnly,
                    corpusTime}, callback, asyncState);
    }
    
    /// <remarks/>
    public string EndGetDoc(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((string)(results[0]));
    }
    
    /// <remarks/>
    public void GetDocAsync(string corpusId, string docId, string format, bool rmvRaw, bool changesOnly, string corpusTime) {
        this.GetDocAsync(corpusId, docId, format, rmvRaw, changesOnly, corpusTime, null);
    }
    
    /// <remarks/>
    public void GetDocAsync(string corpusId, string docId, string format, bool rmvRaw, bool changesOnly, string corpusTime, object userState) {
        if ((this.GetDocOperationCompleted == null)) {
            this.GetDocOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetDocOperationCompleted);
        }
        this.InvokeAsync("GetDoc", new object[] {
                    corpusId,
                    docId,
                    format,
                    rmvRaw,
                    changesOnly,
                    corpusTime}, this.GetDocOperationCompleted, userState);
    }
    
    private void OnGetDocOperationCompleted(object arg) {
        if ((this.GetDocCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.GetDocCompleted(this, new GetDocCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    public new void CancelAsync(object userState) {
        base.CancelAsync(userState);
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
public delegate void PingCompletedEventHandler(object sender, PingCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class PingCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal PingCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public string Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((string)(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
public delegate void GetDocRefsCompletedEventHandler(object sender, GetDocRefsCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class GetDocRefsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal GetDocRefsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public string[][] Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((string[][])(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
public delegate void GetDocCompletedEventHandler(object sender, GetDocCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.42")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class GetDocCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal GetDocCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public string Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((string)(this.results[0]));
        }
    }
}

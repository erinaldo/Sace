﻿//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:2.0.50727.5466
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2000/09/xmldsig#")]
[System.Xml.Serialization.XmlRootAttribute("Signature", Namespace="http://www.w3.org/2000/09/xmldsig#", IsNullable=false)]
public partial class SignatureType {
    
    private SignedInfoType signedInfoField;
    
    private SignatureValueType signatureValueField;
    
    private KeyInfoType keyInfoField;
    
    private string idField;
    
    /// <remarks/>
    public SignedInfoType SignedInfo {
        get {
            return this.signedInfoField;
        }
        set {
            this.signedInfoField = value;
        }
    }
    
    /// <remarks/>
    public SignatureValueType SignatureValue {
        get {
            return this.signatureValueField;
        }
        set {
            this.signatureValueField = value;
        }
    }
    
    /// <remarks/>
    public KeyInfoType KeyInfo {
        get {
            return this.keyInfoField;
        }
        set {
            this.keyInfoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
    public string Id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2000/09/xmldsig#")]
public partial class SignedInfoType {
    
    private SignedInfoTypeCanonicalizationMethod canonicalizationMethodField;
    
    private SignedInfoTypeSignatureMethod signatureMethodField;
    
    private ReferenceType referenceField;
    
    private string idField;
    
    /// <remarks/>
    public SignedInfoTypeCanonicalizationMethod CanonicalizationMethod {
        get {
            return this.canonicalizationMethodField;
        }
        set {
            this.canonicalizationMethodField = value;
        }
    }
    
    /// <remarks/>
    public SignedInfoTypeSignatureMethod SignatureMethod {
        get {
            return this.signatureMethodField;
        }
        set {
            this.signatureMethodField = value;
        }
    }
    
    /// <remarks/>
    public ReferenceType Reference {
        get {
            return this.referenceField;
        }
        set {
            this.referenceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
    public string Id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.w3.org/2000/09/xmldsig#")]
public partial class SignedInfoTypeCanonicalizationMethod {
    
    private string algorithmField;
    
    public SignedInfoTypeCanonicalizationMethod() {
        this.algorithmField = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
    public string Algorithm {
        get {
            return this.algorithmField;
        }
        set {
            this.algorithmField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2000/09/xmldsig#")]
public partial class X509DataType {
    
    private byte[] x509CertificateField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
    public byte[] X509Certificate {
        get {
            return this.x509CertificateField;
        }
        set {
            this.x509CertificateField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2000/09/xmldsig#")]
public partial class KeyInfoType {
    
    private X509DataType x509DataField;
    
    private string idField;
    
    /// <remarks/>
    public X509DataType X509Data {
        get {
            return this.x509DataField;
        }
        set {
            this.x509DataField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
    public string Id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2000/09/xmldsig#")]
public partial class SignatureValueType {
    
    private string idField;
    
    private byte[] valueField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
    public string Id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute(DataType="base64Binary")]
    public byte[] Value {
        get {
            return this.valueField;
        }
        set {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2000/09/xmldsig#")]
public partial class TransformType {
    
    private string[] xPathField;
    
    private TTransformURI algorithmField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("XPath")]
    public string[] XPath {
        get {
            return this.xPathField;
        }
        set {
            this.xPathField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public TTransformURI Algorithm {
        get {
            return this.algorithmField;
        }
        set {
            this.algorithmField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2000/09/xmldsig#")]
public enum TTransformURI {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("http://www.w3.org/2000/09/xmldsig#enveloped-signature")]
    httpwwww3org200009xmldsigenvelopedsignature,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("http://www.w3.org/TR/2001/REC-xml-c14n-20010315")]
    httpwwww3orgTR2001RECxmlc14n20010315,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2000/09/xmldsig#")]
public partial class ReferenceType {
    
    private TransformType[] transformsField;
    
    private ReferenceTypeDigestMethod digestMethodField;
    
    private byte[] digestValueField;
    
    private string idField;
    
    private string uRIField;
    
    private string typeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("Transform", IsNullable=false)]
    public TransformType[] Transforms {
        get {
            return this.transformsField;
        }
        set {
            this.transformsField = value;
        }
    }
    
    /// <remarks/>
    public ReferenceTypeDigestMethod DigestMethod {
        get {
            return this.digestMethodField;
        }
        set {
            this.digestMethodField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
    public byte[] DigestValue {
        get {
            return this.digestValueField;
        }
        set {
            this.digestValueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
    public string Id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
    public string URI {
        get {
            return this.uRIField;
        }
        set {
            this.uRIField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
    public string Type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.w3.org/2000/09/xmldsig#")]
public partial class ReferenceTypeDigestMethod {
    
    private string algorithmField;
    
    public ReferenceTypeDigestMethod() {
        this.algorithmField = "http://www.w3.org/2000/09/xmldsig#sha1";
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
    public string Algorithm {
        get {
            return this.algorithmField;
        }
        set {
            this.algorithmField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.w3.org/2000/09/xmldsig#")]
public partial class SignedInfoTypeSignatureMethod {
    
    private string algorithmField;
    
    public SignedInfoTypeSignatureMethod() {
        this.algorithmField = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
    public string Algorithm {
        get {
            return this.algorithmField;
        }
        set {
            this.algorithmField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe")]
[System.Xml.Serialization.XmlRootAttribute("consSitNFe", Namespace="http://www.portalfiscal.inf.br/nfe", IsNullable=false)]
public partial class TConsSitNFe {
    
    private TAmb tpAmbField;
    
    private TConsSitNFeXServ xServField;
    
    private string chNFeField;
    
    private TVerConsSitNFe versaoField;
    
    /// <remarks/>
    public TAmb tpAmb {
        get {
            return this.tpAmbField;
        }
        set {
            this.tpAmbField = value;
        }
    }
    
    /// <remarks/>
    public TConsSitNFeXServ xServ {
        get {
            return this.xServField;
        }
        set {
            this.xServField = value;
        }
    }
    
    /// <remarks/>
    public string chNFe {
        get {
            return this.chNFeField;
        }
        set {
            this.chNFeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public TVerConsSitNFe versao {
        get {
            return this.versaoField;
        }
        set {
            this.versaoField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TAmb {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("1")]
    Item1,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("2")]
    Item2,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TConsSitNFeXServ {
    
    /// <remarks/>
    CONSULTAR,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TVerConsSitNFe {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("2.01")]
    Item201,
}
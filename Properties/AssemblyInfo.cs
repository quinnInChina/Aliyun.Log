using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Aliyun.Log")]
[assembly: AssemblyDescription("1. 移除对YGOP.ESB.Log的依赖，日志不再记录Mongo；" +
    "2. 默认最大处理数改为100000，默认每批上传记录数改为200；" +
    "3. 依赖Jns.BasicService.Log版本1.3.3.5")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("houkui")]
[assembly: AssemblyProduct("Aliyun.Log")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("79eb22d3-a571-491d-89ba-ebb71bdbe376")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2.0.2.0")]
[assembly: AssemblyFileVersion("2.0.2.0")]

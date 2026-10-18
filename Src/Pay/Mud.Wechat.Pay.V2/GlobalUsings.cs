// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 注：根 Directory.Build.props 已开 ImplicitUsings（System / Collections.Generic / Linq / IO /
// Net.Http / Threading / Threading.Tasks），此处仅补**非隐式**的组件面、密码学/XML 面与本产品线命名空间。
// System.Text + System.Text.Json.* + System.Xml.Linq 是 XML 编解码与签名的必备面；
// System.Globalization 供不变文化数值解析（应答数值字段还原）。
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Mud.HttpUtils;
global using Mud.HttpUtils.Attributes;
global using Mud.Wechat.Pay.Abstractions.Configuration;
global using Mud.Wechat.Pay.Abstractions.Credential;
global using Mud.Wechat.Pay.Abstractions.Transport;
global using Mud.Wechat.Pay.V2.DataModels.PayV2;
global using Mud.Wechat.Pay.V2.Security;
global using Mud.Wechat.Pay.V2.Transport;
global using System.Globalization;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.Json.Serialization.Metadata;
global using System.Xml.Linq;

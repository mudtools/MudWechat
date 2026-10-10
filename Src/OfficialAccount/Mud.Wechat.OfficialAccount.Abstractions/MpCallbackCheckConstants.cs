// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions;

/// <summary>
/// 网络通信检测（<c>callbackCheck</c>）的 <c>action</c> 取值常量。
/// </summary>
/// <remarks>官方取值枚举仅此 3 项（请求体字段为字符串，非法值返回 <c>40202</c>）。</remarks>
public static class MpCallbackCheckActions
{
    /// <summary>域名解析（dns）。</summary>
    public const string Dns = "dns";

    /// <summary>ping 检测。</summary>
    public const string Ping = "ping";

    /// <summary>全部（dns + ping）。</summary>
    public const string All = "all";
}

/// <summary>
/// 网络通信检测（<c>callbackCheck</c>）的 <c>check_operator</c> 取值常量。
/// </summary>
/// <remarks>官方取值枚举仅此 4 项（请求体字段为字符串，非法值返回 <c>40203</c>）。</remarks>
public static class MpCallbackCheckOperators
{
    /// <summary>电信。</summary>
    public const string ChinaNet = "CHINANET";

    /// <summary>联通。</summary>
    public const string UniCom = "UNICOM";

    /// <summary>腾讯。</summary>
    public const string Cap = "CAP";

    /// <summary>自动（默认）。</summary>
    public const string Default = "DEFAULT";
}

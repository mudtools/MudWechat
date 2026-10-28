// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 「统一下单」响应的小程序调起信息（官方 <c>miniprogram</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxMiniProgramLaunch
{
    /// <summary>获取或设置小程序 <c>appid</c>（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置小程序页面路径（官方 <c>pagepath</c>）。</summary>
    [JsonPropertyName("pagepath")]
    public string? PagePath { get; set; }

    /// <summary>获取或设置小程序查询字符串（官方 <c>query_string</c>）。</summary>
    [JsonPropertyName("query_string")]
    public string? QueryString { get; set; }

    /// <summary>获取或设置业务类型（官方 <c>business_type</c>）。</summary>
    [JsonPropertyName("business_type")]
    public string? BusinessType { get; set; }
}

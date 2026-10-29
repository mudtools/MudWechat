// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.BusinessCapability;

/// <summary>
/// 向门店下发模板消息请求体（<c>POST /wxa/business/retailnotifybusiness</c>）。
/// </summary>
/// <remarks>
/// <para><b>模板消息列表及下发</b>：<c>type</c> 选择官方模板类型，<c>content</c> 为与模板匹配的 JSON 字符串。</para>
/// <para><b>接收人上限 200</b>：<c>to_user_list</c> 单次最多 200 个 openid；openid 属敏感信息，禁入日志。</para>
/// <para><b>业务错误族</b>：<c>9403000</c>~<c>9403003</c> 为门店助手侧拒绝；<c>40003</c> 为非法 openid。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BusinessCapability")]
public class WxaBusinessRetailNotifyBusinessRequest
{
    /// <summary>模板类型（必填，官方未发布枚举）。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>接收用户 openid 列表（必填，≤200）。</summary>
    [JsonPropertyName("to_user_list")]
    public List<string>? ToUserList { get; set; }

    /// <summary>模板内容（选填，JSON 字符串）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

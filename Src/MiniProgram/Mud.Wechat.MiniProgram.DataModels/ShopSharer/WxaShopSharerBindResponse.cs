// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopSharer;

/// <summary>
/// 批量邀请分享员应答（<c>POST /shop/sharer/bind</c>）。
/// </summary>
/// <remarks>
/// <para><b>三列表与 <c>result_list</c> 并存</b>：<c>success_list</c> / <c>fail_list</c> / <c>refuse_list</c>
/// 为旧口径按结果三分；<c>result_list</c> 为新口径逐人明细（<c>result_code</c> + <c>reason_code</c>）。
/// 官方回包示例两者同时出现 ⇒ 两套字段都在册，不得只留一种。</para>
/// <para><b><c>success_list</c> 语义</b>（官方原文）：成功给用户下发邀请模版消息、处于邀请中状态，
/// 用户同意绑定后通过 callback 推送结果 ⇒ <c>errcode=0</c> 不代表绑定完成。</para>
/// <para><b>敏感面</b>：列表元素为 openid（个人敏感信息），禁止写入日志、遥测或异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerBindResponse : WxaResponse
{
    /// <summary>成功列表（<c>success_list</c>，string array）：邀请模版消息下发成功、处于邀请中。禁落日志。</summary>
    [JsonPropertyName("success_list")]
    public List<string>? SuccessList { get; set; }

    /// <summary>失败列表（<c>fail_list</c>，string array）：可重试。禁落日志。</summary>
    [JsonPropertyName("fail_list")]
    public List<string>? FailList { get; set; }

    /// <summary>拒绝列表（<c>refuse_list</c>，string array）：不可重试（openid 错误、用户拒绝接收邀请等）。禁落日志。</summary>
    [JsonPropertyName("refuse_list")]
    public List<string>? RefuseList { get; set; }

    /// <summary>逐人结果列表（<c>result_list</c>，object array）。</summary>
    [JsonPropertyName("result_list")]
    public List<WxaShopSharerBindResult>? ResultList { get; set; }
}

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
/// 批量解绑分享员应答（<c>POST /shop/sharer/unbind</c>，只有三列表、<b>无</b> <c>result_list</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>success_list</c> 语义</b>（官方原文）：解除绑定后通过 callback 推送结果 ⇒ 本列表是「解绑动作受理」
/// 而非终态回执。</para>
/// <para><b><c>refuse_list</c> 官方例举</b>：openid 错误、<b>未到解绑时间</b>等 ⇒ 官方存在解绑时间窗约束，
/// 被拒条目不可重试，宿主不得放进自动重试管道。</para>
/// <para><b>与 <see cref="WxaShopSharerBindResponse"/> 不共用类型</b>：bind 应答多 <c>result_list</c> 一支，
/// 共用会让本支凭空多字段（守卫 SHOPSH5 锁定）。</para>
/// <para><b>敏感面</b>：列表元素为 openid，禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerUnbindResponse : WxaResponse
{
    /// <summary>成功列表（<c>success_list</c>，string array）。禁落日志。</summary>
    [JsonPropertyName("success_list")]
    public List<string>? SuccessList { get; set; }

    /// <summary>失败列表（<c>fail_list</c>，string array）：可重试。禁落日志。</summary>
    [JsonPropertyName("fail_list")]
    public List<string>? FailList { get; set; }

    /// <summary>拒绝列表（<c>refuse_list</c>，string array）：不可重试（openid 错误、未到解绑时间等）。禁落日志。</summary>
    [JsonPropertyName("refuse_list")]
    public List<string>? RefuseList { get; set; }
}

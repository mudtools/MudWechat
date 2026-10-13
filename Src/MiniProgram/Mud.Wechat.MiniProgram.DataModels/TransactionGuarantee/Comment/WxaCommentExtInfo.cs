// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 评价额外信息（<c>extInfo</c>，<b>仅列表侧参数表携带</b>，详情侧无该字段）。
/// </summary>
/// <remarks>
/// <para>
/// <b>业务用途</b>：官方 <c>confirmcompromise</c> 的错误码 <c>10000</c> 成因为「不是差评 / <b>发送过模板消息</b>」
/// ⇒ 本字段即和解前置条件的可读来源，宿主在发起和解前应先看 <see cref="IsAlreadySendTmpl"/>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentExtInfo
{
    /// <summary>是否已发过「差评客服会话」（<c>isAlreadySendTmpl</c>，官方类型 boolean）。</summary>
    [JsonPropertyName("isAlreadySendTmpl")]
    public bool? IsAlreadySendTmpl { get; set; }
}

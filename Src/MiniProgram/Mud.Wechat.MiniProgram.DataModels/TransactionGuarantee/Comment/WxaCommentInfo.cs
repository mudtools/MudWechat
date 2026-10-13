// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 评价详情外壳（<c>info</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方形态照录</b>：官方详情参数表把评价主体<b>再包一层</b>为 <c>info.content</c>
/// （<c>info</c> 对象只有一个 <c>content</c> 字段），与列表侧 <c>commentList[]</c> 直接是评价主体不同
/// ⇒ 本类型仅一层包装，主体字段见 <see cref="WxaCommentDetail"/>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentInfo
{
    /// <summary>评价（<c>content</c>，官方原文如此命名评价主体），见 <see cref="WxaCommentDetail"/>。</summary>
    [JsonPropertyName("content")]
    public WxaCommentDetail? Content { get; set; }
}

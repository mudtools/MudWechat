// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 评价域写操作应答（<c>errcode</c> / <c>errmsg</c> ＋ <c>success</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用裁决</b>：官方 6 个评价域写端点（<c>reply/add</c>、<c>reply/delete</c>、<c>commentreply/add</c>、
/// <c>commentreply/delete</c>、<c>confirmcompromise</c>、<c>apikfquota/reset</c>）的返回体参数表
/// <b>完全一致</b>（<c>errcode</c> + <c>success</c> + <c>errmsg</c>）⇒ 共用本类型，不做 6 份同形 DTO。
/// </para>
/// <para>
/// <b>成功判据</b>：官方示例为 <c>{ errcode: 0, success: true }</c> ⇒ 与 <c>IsSuccess</c>（<c>errcode == 0</c>）
/// <b>双条件</b>一致使用；<c>success</c> 为官方 boolean，缺省时按 <c>null</c> 处理（不得当作 <c>false</c> 判定业务失败）。
/// </para>
/// <para>
/// <b>本域错误码为负数形态</b>（<c>-10403</c> 无权限操作该评价/评论/回复、<c>-10008</c> 服务异常、
/// <c>-10007</c> 无权限调用接口、<c>-10003</c> 参数不匹配、<c>-10001</c> 参数异常、<c>10000</c> 见各端点说明）
/// ——与通用正数错误码不同，令牌的 errcode 自愈恢复集不含这些值，故出现 <c>-10007</c> 时应查权限而非令牌。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentOpResponse : WxaResponse
{
    /// <summary>请求是否成功状态（<c>success</c>，官方类型 boolean）。</summary>
    [JsonPropertyName("success")]
    public bool? Success { get; set; }
}

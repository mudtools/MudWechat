// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 撤回商品审核请求体（<c>POST /wxaapi/broadcast/goods/resetaudit</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_resetaudit.html"/>。
/// </para>
/// <para>
/// <b>两键必填且不可由其他端点推出</b>：<c>goodsId</c> 与 <c>auditId</c> 均来自「添加并提审商品」的应答
/// （官方注意事项「必须保存商品 ID 与审核单 ID，如果丢失，则无法调用其他相关接口」）⇒
/// 宿主须在添加时落库，否则本端点与重提审均不可用。
/// </para>
/// <para>调用额度：500 次/天。官方错误码：<c>300009</c>（商品审核撤回失败）/ <c>300017</c>（商品未提审）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveResetAuditRequest
{
    /// <summary>商品 ID（<c>goodsId</c>，必填）。</summary>
    [JsonPropertyName("goodsId")]
    public long? GoodsId { get; set; }

    /// <summary>审核单 ID（<c>auditId</c>，必填；来自添加或重提审应答）。</summary>
    [JsonPropertyName("auditId")]
    public long? AuditId { get; set; }
}

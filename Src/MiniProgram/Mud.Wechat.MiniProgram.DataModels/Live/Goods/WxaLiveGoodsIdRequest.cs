// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 「仅商品 ID」请求体（<c>POST /wxaapi/broadcast/goods/{audit,delete}</c>，官方请求体均只有 <c>goodsId</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用裁决</b>：重新提交商品审核与删除商品两端的官方请求体字段集完全一致（<c>goodsId</c>，必填，number）⇒
/// 共用本类型。两者<b>应答不同</b>：重提审回 <c>auditId</c>（见 <see cref="WxaLiveResubmitAuditResponse"/>），
/// 删除只回 <c>errcode</c> / <c>errmsg</c> ⇒ 返回值类型分开。
/// </para>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_resubmitaudit.html"/>。
/// </para>
/// <para>
/// <b>路由与页面名不一致（照录路由）</b>：重提审页文件名为 <c>api_resubmitaudit</c>、接口英文名
/// <c>resubmitAudit</c>，而路由是 <c>goods/audit</c> ⇒ 类型名按路由语义取 <c>Audit</c> 族。
/// </para>
/// <para>调用额度：重提审 500 次/天（与「添加并提审商品」共用）；删除商品 1000 次/天。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveGoodsIdRequest
{
    /// <summary>商品 ID（<c>goodsId</c>，必填；官方类型 number）。</summary>
    [JsonPropertyName("goodsId")]
    public long? GoodsId { get; set; }
}

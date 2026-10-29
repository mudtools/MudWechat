// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 添加并提审商品请求体（<c>POST /wxaapi/broadcast/goods/add</c>，官方只有一层包装 <c>goodsInfo</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_addgoods.html"/>。
/// </para>
/// <para>
/// <b>官方注意事项原文</b>：「开发者<b>必须保存商品 ID 与审核单 ID</b>，如果丢失，则无法调用其他相关接口」
/// ⇒ 两者来自 <see cref="WxaLiveAddGoodsResponse"/>，本域的重提审 / 撤回审核 / 更新均以之为键。
/// </para>
/// <para>调用额度：<b>500 次/天</b>，且与「重新提交商品审核」<b>共用</b>该 500 次限制（官方原文）。</para>
/// <para>官方错误码：300002 / 300003 / 300004 / 300006 / 300007 / 300018 / 300021（商品添加成功，审核失败）等 9 条。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveAddGoodsRequest
{
    /// <summary>商品信息（<c>goodsInfo</c>，必填；官方类型 object），见 <see cref="WxaLiveGoodsInfo"/>。</summary>
    [JsonPropertyName("goodsInfo")]
    public WxaLiveGoodsInfo? GoodsInfo { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

// ---------------------------------------------------------------- 创建卡券（POST /card/create）
//  请求体为「card 包装 + card_type 判别 + 11 个券型分支」。分支之间**互斥**（官方按 card_type 取其一），
//  故每个分支都是独立对象、各带自己的 base_info/advanced_info 引用 —— 不得为「省类型」而合并成
//  一个万能对象（那会让 cash 分支能传 discount 字段，官方该页未定义）。

/// <summary>
/// 创建卡券（<c>POST /card/create</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>顶层只有一个 <c>card</c> 对象</b>（官方形态，非扁平）；<c>access_token</c> 由 Query 注入、不入请求体。
/// </para>
/// <para>
/// <b>对齐基准</b>：SKIT <c>CardCreateRequest</c>（2026-10-10）。官方页面（现文档树
/// <c>doc/subscription/guide/product/card/Create_a_Coupon_Voucher_or_Card</c>）正文为 SPA、本次不可达，
/// 字段表与长度上限<b>待逐页核验</b>；守卫 CD1~CD4 只锁路由、分支归属与官方字段名。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCreateRequest
{
    /// <summary>获取或设置卡券对象（官方 <c>card</c>，必填）。</summary>
    [JsonPropertyName("card")]
    public MpCardCreateBody? Card { get; set; }
}

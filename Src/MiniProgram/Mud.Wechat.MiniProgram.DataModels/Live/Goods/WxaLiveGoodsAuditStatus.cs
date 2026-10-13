// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 商品审核状态取值（<c>status</c> 查询参数与 <c>audit_status</c> 应答字段，官方枚举）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/commodity-management/api_getgoodsinfo.html"/>。
/// </para>
/// <para>
/// <b>同一枚举在两处类型不同（照录不统一）</b>：「获取商品列表」的 <c>status</c> 为 <b>number 必填查询参数</b>，
/// 「获取商品的信息与审核状态」应答里的 <c>audit_status</c> 官方类型列为 <b>string</b>
/// （<see cref="WxaLiveWarehouseGoodsItem.AuditStatus"/>）⇒ 本常量类供查询参数侧使用，
/// 应答侧仍按官方类型建为字符串。
/// </para>
/// </remarks>
public static class WxaLiveGoodsAuditStatus
{
    /// <summary>未审核（<c>0</c>）。</summary>
    public const int NotAudited = 0;

    /// <summary>审核中（<c>1</c>）。</summary>
    public const int Auditing = 1;

    /// <summary>审核通过（<c>2</c>）——导入直播间的必要条件。</summary>
    public const int Passed = 2;

    /// <summary>审核失败 / 审核驳回（<c>3</c>；两页官方措辞分别为「审核失败」「审核驳回」，值同为 3）。</summary>
    public const int Failed = 3;
}

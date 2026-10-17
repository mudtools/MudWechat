// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>
/// 联盟带货机构资金流水的资金类型（官方 <c>headsupplier/funds/flow{list,detail}/get</c> 的 <c>funds_type</c> 枚举）。
/// </summary>
/// <remarks>
/// <para>
/// 与小店商家侧 Funds 域的 <c>funds_type</c> <b>不是同一套枚举</b>：机构侧含「联盟技术服务费」（<c>3</c>，
/// 官方 2026-03-04 变更日志新增）与「分账回退」（<c>4</c>，官方 2026-06-01 变更日志新增）两类联盟专属流水，
/// 商家侧另有 <c>funds_type_desc</c> 描述字段与 <c>related_info_list</c> 关联信息 —— 两套值互不混用。
/// </para>
/// </remarks>
public static class ChannelsLeagueHeadSupplierFundsTypes
{
    /// <summary>提现（官方值 <c>1</c>）。</summary>
    public const int Withdraw = 1;

    /// <summary>分账（官方值 <c>2</c>）。</summary>
    public const int Allocate = 2;

    /// <summary>联盟技术服务费（官方值 <c>3</c>，2026-03-04 变更日志新增）。</summary>
    public const int LeagueTechnologyServiceFee = 3;

    /// <summary>分账回退（官方值 <c>4</c>，2026-06-01 变更日志新增）。</summary>
    public const int AllocateRollback = 4;
}

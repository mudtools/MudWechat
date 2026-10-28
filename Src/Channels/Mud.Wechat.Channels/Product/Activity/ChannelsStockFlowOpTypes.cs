// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 商品管理（Product）域「营销子域」（8 端点：买赠活动 activity/add、activity/del、activity/stop +
// 限时抢购 limiteddiscounttask/add、update、delete、stop、list/get）DTO。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Product（功能族子目录仅作组织，不入命名空间）。
//
// 官方文档矛盾照录：activity/del 的 activity_id 为 number、activity/stop 的 activity_id 为 string
// （原文即如此），SDK 分别建模为 long / string，不做统一。

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>库存事件类型常量（官方 <c>op_type</c> 枚举值，<c>stock/getflow</c>）。</summary>
public static class ChannelsStockFlowOpTypes
{
    /// <summary>未知。</summary>
    public const int Unknown = 0;

    /// <summary>增加（API 操作）。</summary>
    public const int IncreaseOther = 1;

    /// <summary>减少。</summary>
    public const int DecreaseOther = 2;

    /// <summary>设置。</summary>
    public const int Set = 3;

    /// <summary>下单占用。</summary>
    public const int OrderLock = 4;

    /// <summary>退款/取消释放。</summary>
    public const int OrderUnlock = 5;

    /// <summary>分配（转出）。</summary>
    public const int MoveOut = 6;

    /// <summary>归还（转入）。</summary>
    public const int MoveIn = 7;
}

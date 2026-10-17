// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------
namespace Mud.Wechat.Channels.DataModels.Talent;

/// <summary>橱窗商品来源类型（官方 <c>products[].product_source</c> 枚举表）。</summary>
public static class ChannelsTalentWindowProductSources
{
    /// <summary>获取自营商品（官方原文「1 来源店铺的自营商品（此时返回 appid 字段）」）的值。</summary>
    public const int SelfOperated = 1;

    /// <summary>获取带货商品（官方原文「2 来源选品中心的带货商品（此时返回 out_product_id 字段）」）的值。</summary>
    public const int Promoted = 2;

}

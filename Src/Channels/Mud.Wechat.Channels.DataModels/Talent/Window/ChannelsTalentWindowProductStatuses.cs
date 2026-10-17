// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------
namespace Mud.Wechat.Channels.DataModels.Talent;

/// <summary>橱窗商品状态（官方 <c>product.status</c> 枚举表）。</summary>
public static class ChannelsTalentWindowProductStatuses
{
    /// <summary>获取生效中（官方原文「1 生效中」）的值。</summary>
    public const int Effective = 1;

    /// <summary>获取被禁止售卖（官方原文「2 被禁止售卖」）的值。</summary>
    public const int Forbidden = 2;

}

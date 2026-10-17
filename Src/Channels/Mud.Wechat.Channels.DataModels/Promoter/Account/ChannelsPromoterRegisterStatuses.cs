// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>推客注册状态常量（官方 <c>register_status</c> 枚举值，<c>promoter/get_promoter_register_and_bind_status</c>）。</summary>
public static class ChannelsPromoterRegisterStatuses
{
    /// <summary>未注册。</summary>
    public const int NotRegistered = 0;

    /// <summary>注册中，还未完成。</summary>
    public const int Registering = 1;

    /// <summary>已完成注册。</summary>
    public const int Registered = 2;

    /// <summary>用户未支付实名或已实名但未绑定银行卡，需完成微信支付实名并绑定银行卡后才能继续注册。</summary>
    public const int NeedRealNameAndBankCard = 3;
}

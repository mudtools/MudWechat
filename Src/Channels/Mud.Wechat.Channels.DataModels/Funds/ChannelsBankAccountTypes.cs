// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>结算账户类型常量（官方 <c>bank_account_type</c> 枚举值）。</summary>
public static class ChannelsBankAccountTypes
{
    /// <summary>对公银行账户。</summary>
    public const string Business = "ACCOUNT_TYPE_BUSINESS";

    /// <summary>经营者个人银行卡。</summary>
    public const string Private = "ACCOUNT_TYPE_PRIVATE";
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopFunds;

/// <summary>
/// 结算账户类型官方枚举值（<c>bank_account_type</c>，即官方 <c>BankAccountType</c> 表）。
/// </summary>
/// <remarks>
/// 官方原文（「修改结算账户」页 BankAccountType 枚举表）只公布这两个取值：
/// <c>ACCOUNT_TYPE_BUSINESS</c> 对公银行账户、<c>ACCOUNT_TYPE_PRIVATE</c> 经营者个人银行卡 ⇒ 本类<b>不得扩充</b>。
/// 注意与小店线的同名枚举是<b>两套独立类型</b>（令牌面不同、DTO 不复用，见 AGENTS §4 各线包边界）。
/// </remarks>
public static class WxaShopFundsBankAccountTypes
{
    /// <summary>对公银行账户（<c>ACCOUNT_TYPE_BUSINESS</c>）。</summary>
    public const string Business = "ACCOUNT_TYPE_BUSINESS";

    /// <summary>经营者个人银行卡（<c>ACCOUNT_TYPE_PRIVATE</c>）。</summary>
    public const string Private = "ACCOUNT_TYPE_PRIVATE";
}

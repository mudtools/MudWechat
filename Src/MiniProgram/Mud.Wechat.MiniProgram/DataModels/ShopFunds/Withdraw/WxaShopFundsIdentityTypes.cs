// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopFunds;

/// <summary>
/// 提款二维码「需要验证的身份」官方枚举值（<c>identity_type</c>）。
/// </summary>
/// <remarks>
/// 官方「生成二维码」页的「枚举值-identity_type」表<b>只有一行</b>：<c>1</c> 管理员 ⇒ 本类不得扩充。
/// 次管理员等身份只出现在「查询扫码状态」页的 <c>scan_user_type</c>（0 非管理员 / 1 管理员 / 2 次管理员），
/// 两者是<b>不同字段</b>，不得合并（守卫 SHOPF5 锁定本枚举只 1 值）。
/// </remarks>
public static class WxaShopFundsIdentityTypes
{
    /// <summary>管理员（<c>1</c>，官方 identity_type 枚举表唯一取值）。</summary>
    public const int Administrator = 1;
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductMedia;

/// <summary>扫码者身份（<c>qrcode/check</c> 回包 <c>scan_user_type</c>）。</summary>
/// <remarks>
/// <para>官方该段枚举表头误写为「枚举值-status」（与扫码状态表重复）⇒ 本节按字段语义整理。</para>
/// </remarks>
public static class WxaProductQrCodeScanUserTypes
{
    /// <summary>0 非管理员。</summary>
    public const int NonAdmin = 0;

    /// <summary>1 管理员。</summary>
    public const int Admin = 1;

    /// <summary>2 次管理员。</summary>
    public const int SubAdmin = 2;
}

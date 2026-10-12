// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>同城配送「扣费主体」取值（<c>pay_mode</c>，官方固定值）。</summary>
/// <remarks>
/// 官方原文：<c>PAY_MODE_STORE</c> 门店、<c>PAY_MODE_APP</c> 小程序、<c>PAY_MODE_COMPONENT</c> 服务商。
/// 该组常量由 <c>intracity_setpaymode</c> 入参与 <c>intracity_getpaymode</c> 出参共用。
/// </remarks>
public static class WxaExpressPayModes
{
    /// <summary>门店扣费（<c>PAY_MODE_STORE</c>）。</summary>
    public const string Store = "PAY_MODE_STORE";

    /// <summary>小程序扣费（<c>PAY_MODE_APP</c>）。</summary>
    public const string App = "PAY_MODE_APP";

    /// <summary>服务商扣费（<c>PAY_MODE_COMPONENT</c>）。</summary>
    public const string Component = "PAY_MODE_COMPONENT";
}

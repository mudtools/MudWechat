// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 版权验证方式取值（<c>copyright.copyright_verification</c>）。
/// </summary>
/// <remarks>
/// <para><b>常量类型是 <c>string</c> 而非 <c>int</c></b>（与 <see cref="WxaVodCopyright"/> 的属性类型同源）：官方类型列作 <c>string</c>、值域写作数字，照录原文 ⇒ 使用时须以字符串送值，勿「顺手」写成数字字面量。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_auditdrama.html"/></para>
/// </remarks>
public static class WxaVodCopyrightVerifications
{
    /// <summary>基于版权证明材料（<c>1</c>）。</summary>
    public const string ByMaterial = "1";

    /// <summary>基于版权授权关系（<c>2</c>，此时无需填 <c>proof_of_production</c>）。</summary>
    public const string ByAuthorization = "2";
}

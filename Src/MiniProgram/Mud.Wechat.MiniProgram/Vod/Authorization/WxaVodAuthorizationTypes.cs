// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 版权授权类型取值（<c>authorization_type</c>）。
/// </summary>
/// <remarks>
/// <para><b>查询侧的「不填」默认值是 <c>0</c>（表示全部），本枚举只有 1/2</b> ⇒ 不得把 0 当枚举成员传写端点。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/authorizecopyright/api_authorizecopyright.html"/></para>
/// </remarks>
public static class WxaVodAuthorizationTypes
{
    /// <summary>授权给主体（须给统一社会信用代码）。</summary>
    public const int Subject = 1;

    /// <summary>授权给小程序（须给 <c>authorized_appid</c>）。</summary>
    public const int MiniProgram = 2;
}

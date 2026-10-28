// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 流量包状态取值（<c>listpackages</c> 的 <c>status</c> 过滤与返回侧 <c>package_list.status</c> 同源）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/usagedata/api_listpackages.html"/></para>
/// </remarks>
public static class WxaVodPackageStatuses
{
    /// <summary>不过滤（<c>0</c>，仅请求侧有意义）。</summary>
    public const int NoFilter = 0;

    /// <summary>有效的（<c>1</c>）。</summary>
    public const int Valid = 1;

    /// <summary>无效的（<c>2</c>）。</summary>
    public const int Invalid = 2;

    /// <summary>过期的（<c>3</c>）。</summary>
    public const int Expired = 3;
}

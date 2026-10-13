// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 分片所属资源类型取值（<c>uploadpart</c> 的 <c>resource_type</c>，官方：1 视频 / 2 图片）。
/// </summary>
/// <remarks>
/// <para><b>封面分片须重新从 1 编号</b>（官方原文）：视频与封面各自是一条 <c>part_number</c> 序列，靠本枚举区分归属；混编会触发官方错误码 <c>10090005</c>（resource_type 无效）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_uploadpart.html"/></para>
/// </remarks>
public static class WxaVodResourceTypes
{
    /// <summary>视频分片（<c>1</c>）。</summary>
    public const int Video = 1;

    /// <summary>图片（封面）分片（<c>2</c>）。</summary>
    public const int Image = 2;
}

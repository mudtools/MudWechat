// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 上传任务类型取值（官方 <c>Res.task_info.task_type</c> 枚举表，当前只登记一个值）。
/// </summary>
/// <remarks>
/// <para><b>照录官方枚举表</b>：官方 2026-10 只给出 <c>1 = 拉取上传任务</c>；分片上传是同步路径、不产生任务，故本域只有这一支。<b>不得</b>因「看起来该有 2」而补值。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_fileupload/api_gettask.html"/></para>
/// </remarks>
public static class WxaVodTaskTypes
{
    /// <summary>拉取上传任务（<c>1</c>）</summary>
    public const int PullUpload = 1;
}
